-- =============================================
-- E-commerce Platform Database Schema
-- =============================================

-- Users Table (Clients et Administrateurs)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Role NVARCHAR(20) DEFAULT 'Customer', -- 'Customer', 'Admin'
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    EmailVerified BIT DEFAULT 0,
    EmailVerificationCode NVARCHAR(10) NULL,
    EmailVerificationExpiry DATETIME NULL,
    ResetPasswordToken NVARCHAR(255) NULL,
    ResetPasswordExpiry DATETIME NULL
);

-- Addresses Table (Adresses de livraison)
CREATE TABLE Addresses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    FullName NVARCHAR(100) NOT NULL,
    Street NVARCHAR(255) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) DEFAULT 'Maroc',
    Phone NVARCHAR(20),
    IsDefault BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500),
    ParentCategoryId INT FOREIGN KEY REFERENCES Categories(Id) NULL,
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ShortDescription NVARCHAR(500),
    Price DECIMAL(18, 2) NOT NULL,
    CompareAtPrice DECIMAL(18, 2) NULL,
    StockQuantity INT DEFAULT 0,
    SKU NVARCHAR(100),
    Weight DECIMAL(10, 2) NULL,
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsActive BIT DEFAULT 1,
    IsFeatured BIT DEFAULT 0,
    ViewCount INT DEFAULT 0
);

-- Product Images (Additional images)
CREATE TABLE ProductImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id) ON DELETE CASCADE,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsPrimary BIT DEFAULT 0,
    DisplayOrder INT DEFAULT 0
);

-- Product Variants (Size, Color, etc.)
CREATE TABLE ProductVariants (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id) ON DELETE CASCADE,
    VariantType NVARCHAR(50) NOT NULL, -- 'Size', 'Color', 'Weight', etc.
    VariantValue NVARCHAR(100) NOT NULL,
    StockQuantity INT DEFAULT 0,
    PriceAdjustment DECIMAL(18, 2) DEFAULT 0,
    SKU NVARCHAR(100) NULL
);

-- Orders Table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    SubTotal DECIMAL(18, 2) NOT NULL,
    ShippingCost DECIMAL(18, 2) DEFAULT 0,
    TaxAmount DECIMAL(18, 2) DEFAULT 0,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Processing, Shipped, Delivered, Cancelled
    ShippingAddressId INT FOREIGN KEY REFERENCES Addresses(Id),
    ShippingMethod NVARCHAR(100),
    TrackingNumber NVARCHAR(100) NULL,
    Notes NVARCHAR(MAX) NULL,
    UpdatedAt DATETIME NULL
);

-- Order Items Table
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT FOREIGN KEY REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    VariantId INT FOREIGN KEY REFERENCES ProductVariants(Id) NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    TotalPrice DECIMAL(18, 2) NOT NULL
);

-- Reviews Table (Avis clients)
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderId INT FOREIGN KEY REFERENCES Orders(Id) NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    IsApproved BIT DEFAULT 0,
    IsVerifiedPurchase BIT DEFAULT 0
);

-- Shopping Cart Table (Panier)
CREATE TABLE ShoppingCart (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id) NULL,
    SessionId NVARCHAR(255) NULL, -- Pour les utilisateurs non connectés
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    VariantId INT FOREIGN KEY REFERENCES ProductVariants(Id) NULL,
    Quantity INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Wishlist Table (Liste de souhaits)
CREATE TABLE Wishlist (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UNIQUE(UserId, ProductId)
);

-- Coupons Table (Codes promo)
CREATE TABLE Coupons (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL UNIQUE,
    DiscountType NVARCHAR(20) NOT NULL, -- 'Percentage', 'Fixed'
    DiscountValue DECIMAL(18, 2) NOT NULL,
    MinimumAmount DECIMAL(18, 2) NULL,
    MaximumDiscount DECIMAL(18, 2) NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    UsageLimit INT NULL,
    UsedCount INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Payment Methods Table
CREATE TABLE PaymentMethods (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0
);

-- Shipping Methods Table
CREATE TABLE ShippingMethods (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Cost DECIMAL(18, 2) NOT NULL,
    FreeShippingThreshold DECIMAL(18, 2) NULL,
    EstimatedDays INT NULL,
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0
);
CREATE TABLE Complaints (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    OrderId INT NULL FOREIGN KEY REFERENCES Orders(Id),
    Subject NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    Priority NVARCHAR(20) DEFAULT 'Medium',
    Category NVARCHAR(100) NULL,
    AdminResponse NVARCHAR(MAX) NULL,
    AdminId INT NULL FOREIGN KEY REFERENCES Users(Id),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    ResolvedAt DATETIME NULL
)
CREATE TABLE OrderHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
    UserId INT NOT NULL,
    OrderNumber NVARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    OrderDate DATETIME NOT NULL,
    CompletedDate DATETIME DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) NULL
)

-- Notifications Table
CREATE TABLE Notifications (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX),
    Type NVARCHAR(50), -- 'Order', 'Product', 'System'
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =============================================
-- Seed Data
-- =============================================

-- Admin User (Password: admin123)
INSERT INTO Users (Email, PasswordHash, FullName, Role, IsActive, EmailVerified)
VALUES ('admin@ecommerce.ma', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Administrateur', 'Admin', 1, 1);

-- Categories
INSERT INTO Categories (Name, Description, ImageUrl, DisplayOrder) VALUES 
('Agriculture', 'Produits agricoles biologiques du terroir marocain', '/Assets/Images/Categories/agriculture.jpg', 1),
('Artisanat', 'Produits artisanaux traditionnels marocains', '/Assets/Images/Categories/artisanat.jpg', 2),
('Produits du Terroir', 'Spécialités culinaires et produits locaux', '/Assets/Images/Categories/terroir.jpg', 3),
('Huile d''Olive', 'Huiles d''olive extra vierge', '/Assets/Images/Categories/huile.jpg', 4),
('Miel', 'Miel naturel et produits de la ruche', '/Assets/Images/Categories/miel.jpg', 5),
('Argan', 'Huile d''argan et produits à base d''argan', '/Assets/Images/Categories/argan.jpg', 6);

-- Payment Methods
INSERT INTO PaymentMethods (Name, Description, DisplayOrder) VALUES
('Paiement à la livraison', 'Payer en espèces à la réception', 1),
('Virement bancaire', 'Virement bancaire direct', 2),
('Carte bancaire', 'Paiement sécurisé par carte bancaire', 3);

-- Shipping Methods
INSERT INTO ShippingMethods (Name, Description, Cost, EstimatedDays, DisplayOrder) VALUES
('Livraison standard', 'Livraison en 3-5 jours ouvrés', 30.00, 4, 1),
('Livraison express', 'Livraison en 1-2 jours ouvrés', 60.00, 2, 2),
('Livraison gratuite', 'Livraison gratuite pour commandes > 500 MAD', 0.00, 5, 3);

-- Sample Products
DECLARE @CatAgricId INT = (SELECT Id FROM Categories WHERE Name = 'Agriculture');
DECLARE @CatTerroirId INT = (SELECT Id FROM Categories WHERE Name = 'Produits du Terroir');

INSERT INTO Products (CategoryId, Name, Description, ShortDescription, Price, StockQuantity, SKU, IsActive, IsFeatured)
VALUES 
(@CatAgricId, 'Tomates Bio', 'Tomates biologiques fraîches du terroir marocain', 'Tomates biologiques cultivées sans pesticides', 15.00, 100, 'TOM-BIO-001', 1, 1),
(@CatTerroirId, 'Huile d''Olive Extra Vierge', 'Huile d''olive pressée à froid, 500ml', 'Huile d''olive de première qualité', 85.00, 50, 'HUI-OLIVE-500', 1, 1),
(@CatAgricId, 'Amandes du Rif', 'Amandes fraîches du Rif marocain, 500g', 'Amandes de qualité supérieure', 120.00, 75, 'AMANDE-RIF-500', 1, 0);

-- Indexes pour améliorer les performances
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Products_IsFeatured ON Products(IsFeatured);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
CREATE INDEX IX_ShoppingCart_UserId ON ShoppingCart(UserId);
CREATE INDEX IX_ShoppingCart_SessionId ON ShoppingCart(SessionId);
CREATE INDEX IX_Reviews_ProductId ON Reviews(ProductId);
CREATE INDEX IX_Reviews_UserId ON Reviews(UserId);
