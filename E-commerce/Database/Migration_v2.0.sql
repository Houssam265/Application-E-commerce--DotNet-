-- =============================================
-- Database Enhancements for E-commerce Platform
-- Version: 2.0
-- Date: 2025-12-28
-- =============================================

-- =============================================
-- 1. CREATE COMPLAINTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Complaints]') AND type in (N'U'))
BEGIN
    CREATE TABLE Complaints (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        OrderId INT NULL FOREIGN KEY REFERENCES Orders(Id),
        Subject NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Resolved, Closed
        Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium', -- Low, Medium, High
        Category NVARCHAR(100) NULL, -- Product Quality, Shipping, Payment, Other
        AdminResponse NVARCHAR(MAX) NULL,
        AdminId INT NULL FOREIGN KEY REFERENCES Users(Id),
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,
        ResolvedAt DATETIME NULL
    );
    
    -- Create indexes for better performance
    CREATE INDEX IX_Complaints_UserId ON Complaints(UserId);
    CREATE INDEX IX_Complaints_OrderId ON Complaints(OrderId);
    CREATE INDEX IX_Complaints_Status ON Complaints(Status);
    CREATE INDEX IX_Complaints_Priority ON Complaints(Priority);
    
    PRINT 'Table Complaints créée avec succès';
END
GO

-- =============================================
-- 2. CREATE ORDER HISTORY TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE OrderHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        OrderNumber NVARCHAR(50) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        OrderDate DATETIME NOT NULL,
        CompletedDate DATETIME NOT NULL DEFAULT GETDATE(),
        Notes NVARCHAR(MAX) NULL
    );
    
    -- Create indexes
    CREATE INDEX IX_OrderHistory_OrderId ON OrderHistory(OrderId);
    CREATE INDEX IX_OrderHistory_UserId ON OrderHistory(UserId);
    CREATE INDEX IX_OrderHistory_Status ON OrderHistory(Status);
    
    PRINT 'Table OrderHistory créée avec succès';
END
GO

-- =============================================
-- 3. ALTER ORDERS TABLE (Add archiving columns)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'IsArchived')
BEGIN
    ALTER TABLE Orders ADD IsArchived BIT NOT NULL DEFAULT 0;
    PRINT 'Colonne IsArchived ajoutée à la table Orders';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'ArchivedAt')
BEGIN
    ALTER TABLE Orders ADD ArchivedAt DATETIME NULL;
    PRINT 'Colonne ArchivedAt ajoutée à la table Orders';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'StatusLockedAt')
BEGIN
    ALTER TABLE Orders ADD StatusLockedAt DATETIME NULL;
    PRINT 'Colonne StatusLockedAt ajoutée à la table Orders';
END
GO

-- =============================================
-- 4. CREATE STORED PROCEDURES
-- =============================================

-- Procedure to archive an order
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ArchiveOrder]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ArchiveOrder]
GO

CREATE PROCEDURE sp_ArchiveOrder
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT, @OrderNumber NVARCHAR(50), @TotalAmount DECIMAL(18,2), 
            @Status NVARCHAR(50), @OrderDate DATETIME, @Notes NVARCHAR(MAX);
    
    -- Get order details
    SELECT @UserId = UserId, @OrderNumber = OrderNumber, @TotalAmount = TotalAmount,
           @Status = Status, @OrderDate = OrderDate, @Notes = Notes
    FROM Orders
    WHERE Id = @OrderId;
    
    -- Check if order is already archived
    IF NOT EXISTS (SELECT 1 FROM OrderHistory WHERE OrderId = @OrderId)
    BEGIN
        -- Insert into OrderHistory
        INSERT INTO OrderHistory (OrderId, UserId, OrderNumber, TotalAmount, Status, OrderDate, Notes)
        VALUES (@OrderId, @UserId, @OrderNumber, @TotalAmount, @Status, @OrderDate, @Notes);
        
        -- Update Orders table
        UPDATE Orders 
        SET IsArchived = 1, ArchivedAt = GETDATE()
        WHERE Id = @OrderId;
        
        RETURN 1;
    END
    
    RETURN 0;
END
GO

PRINT 'Procédure sp_ArchiveOrder créée avec succès';
GO

-- =============================================
-- 5. CREATE VIEWS FOR STATISTICS
-- =============================================

-- View for active orders (not archived)
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_ActiveOrders]'))
    DROP VIEW [dbo].[vw_ActiveOrders]
GO

CREATE VIEW vw_ActiveOrders
AS
SELECT 
    O.Id, O.OrderNumber, O.UserId, O.OrderDate, O.TotalAmount, O.Status,
    U.FullName, U.Email
FROM Orders O
INNER JOIN Users U ON O.UserId = U.Id
WHERE O.IsArchived = 0
GO

PRINT 'Vue vw_ActiveOrders créée avec succès';
GO

-- View for archived orders
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_ArchivedOrders]'))
    DROP VIEW [dbo].[vw_ArchivedOrders]
GO

CREATE VIEW vw_ArchivedOrders
AS
SELECT 
    OH.Id, OH.OrderId, OH.OrderNumber, OH.UserId, OH.OrderDate, 
    OH.CompletedDate, OH.TotalAmount, OH.Status, OH.Notes,
    U.FullName, U.Email
FROM OrderHistory OH
INNER JOIN Users U ON OH.UserId = U.Id
GO

PRINT 'Vue vw_ArchivedOrders créée avec succès';
GO

-- View for pending complaints
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_PendingComplaints]'))
    DROP VIEW [dbo].[vw_PendingComplaints]
GO

CREATE VIEW vw_PendingComplaints
AS
SELECT 
    C.Id, C.Subject, C.Description, C.Status, C.Priority, C.Category,
    C.CreatedAt, C.UserId, U.FullName, U.Email,
    C.OrderId, O.OrderNumber
FROM Complaints C
INNER JOIN Users U ON C.UserId = U.Id
LEFT JOIN Orders O ON C.OrderId = O.Id
WHERE C.Status IN ('Pending', 'InProgress')
GO

PRINT 'Vue vw_PendingComplaints créée avec succès';
GO

-- =============================================
-- 6. CREATE TRIGGERS
-- =============================================

-- Trigger to lock order status when marked as Delivered or Cancelled
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[tr_LockOrderStatus]'))
    DROP TRIGGER [dbo].[tr_LockOrderStatus]
GO

CREATE TRIGGER tr_LockOrderStatus
ON Orders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- If status changed to Delivered or Cancelled, lock it
    UPDATE Orders
    SET StatusLockedAt = GETDATE()
    WHERE Id IN (
        SELECT i.Id 
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.Status IN ('Delivered', 'Cancelled')
          AND d.Status NOT IN ('Delivered', 'Cancelled')
          AND i.StatusLockedAt IS NULL
    );
END
GO

PRINT 'Trigger tr_LockOrderStatus créé avec succès';
GO

-- =============================================
-- 7. SAMPLE DATA FOR TESTING
-- =============================================

-- Insert sample complaint categories (optional)
PRINT '';
PRINT '=============================================';
PRINT 'Migration de la base de données terminée avec succès!';
PRINT '=============================================';
PRINT 'Tables créées:';
PRINT '  - Complaints (Réclamations)';
PRINT '  - OrderHistory (Historique des commandes)';
PRINT '';
PRINT 'Colonnes ajoutées à Orders:';
PRINT '  - IsArchived';
PRINT '  - ArchivedAt';
PRINT '  - StatusLockedAt';
PRINT '';
PRINT 'Procédures stockées créées:';
PRINT '  - sp_ArchiveOrder';
PRINT '';
PRINT 'Vues créées:';
PRINT '  - vw_ActiveOrders';
PRINT '  - vw_ArchivedOrders';
PRINT '  - vw_PendingComplaints';
PRINT '';
PRINT 'Triggers créés:';
PRINT '  - tr_LockOrderStatus';
PRINT '=============================================';
GO
