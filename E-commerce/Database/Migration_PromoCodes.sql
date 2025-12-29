-- =============================================
-- Database Migration for Promo Codes Feature
-- Version: 2.1
-- Date: 2025-01-28
-- =============================================

-- =============================================
-- 1. ALTER ORDERS TABLE (Add promo code columns)
-- =============================================

-- Add DiscountAmount column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'DiscountAmount')
BEGIN
    ALTER TABLE Orders ADD DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0;
    PRINT 'Colonne DiscountAmount ajoutée à la table Orders';
END
GO

-- Add CouponCode column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'CouponCode')
BEGIN
    ALTER TABLE Orders ADD CouponCode NVARCHAR(50) NULL;
    PRINT 'Colonne CouponCode ajoutée à la table Orders';
END
GO

-- Create index on CouponCode for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_CouponCode' AND object_id = OBJECT_ID(N'[dbo].[Orders]'))
BEGIN
    CREATE INDEX IX_Orders_CouponCode ON Orders(CouponCode);
    PRINT 'Index IX_Orders_CouponCode créé';
END
GO

-- =============================================
-- 2. CREATE COUPON USAGE TRACKING TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CouponUsage]') AND type in (N'U'))
BEGIN
    CREATE TABLE CouponUsage (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CouponId INT NOT NULL FOREIGN KEY REFERENCES Coupons(Id),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
        DiscountAmount DECIMAL(18, 2) NOT NULL,
        UsedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- Create indexes for better performance
    CREATE INDEX IX_CouponUsage_CouponId ON CouponUsage(CouponId);
    CREATE INDEX IX_CouponUsage_UserId ON CouponUsage(UserId);
    CREATE INDEX IX_CouponUsage_OrderId ON CouponUsage(OrderId);
    
    PRINT 'Table CouponUsage créée avec succès';
END
GO

PRINT '';
PRINT '=============================================';
PRINT 'Migration Promo Codes terminée avec succès!';
PRINT '=============================================';
PRINT 'Colonnes ajoutées à Orders:';
PRINT '  - DiscountAmount';
PRINT '  - CouponCode';
PRINT '';
PRINT 'Tables créées:';
PRINT '  - CouponUsage (Suivi de l''utilisation des codes promo)';
PRINT '';
PRINT 'Index créés:';
PRINT '  - IX_Orders_CouponCode';
PRINT '  - IX_CouponUsage_CouponId';
PRINT '  - IX_CouponUsage_UserId';
PRINT '  - IX_CouponUsage_OrderId';
PRINT '=============================================';
GO

