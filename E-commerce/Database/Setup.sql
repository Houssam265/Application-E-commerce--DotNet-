-- Create Database (if not exists, usually run manually or via Master)
-- CREATE DATABASE EcommerceDB;
-- USE EcommerceDB;

-- Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Role NVARCHAR(20) DEFAULT 'Customer', -- 'Customer', 'Admin'
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- Addresses Table
CREATE TABLE Addresses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Street NVARCHAR(255) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) DEFAULT 'France',
    IsDefault BIT DEFAULT 0
);

-- Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500),
    IsActive BIT DEFAULT 1
);

-- Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    StockQuantity INT DEFAULT 0,
    ImageUrl NVARCHAR(500), -- Main image
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- Product Images (Additional images)
CREATE TABLE ProductImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    ImageUrl NVARCHAR(500) NOT NULL,
    IsPrimary BIT DEFAULT 0
);

-- Product Variants (Size, Color)
CREATE TABLE ProductVariants (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    Size NVARCHAR(50),
    Color NVARCHAR(50),
    StockQuantity INT DEFAULT 0,
    PriceAdjustment DECIMAL(18, 2) DEFAULT 0
);

-- Orders Table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Processing, Shipped, Delivered, Cancelled
    ShippingAddressId INT FOREIGN KEY REFERENCES Addresses(Id)
);

-- Order Items Table
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT FOREIGN KEY REFERENCES Orders(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    VariantId INT FOREIGN KEY REFERENCES ProductVariants(Id), 
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);

-- Reviews Table
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE()
);

-- Seed Data (Admin)
INSERT INTO Users (Email, PasswordHash, FullName, Role)
VALUES ('admin@ecommerce.com', 'hashed_secret', 'System Administrator', 'Admin');

-- Seed Categories
INSERT INTO Categories (Name, ImageUrl) VALUES ('Electronics', '/Assets/Images/Categories/electronics.jpg');
INSERT INTO Categories (Name, ImageUrl) VALUES ('Fashion', '/Assets/Images/Categories/fashion.jpg');
INSERT INTO Categories (Name, ImageUrl) VALUES ('Home', '/Assets/Images/Categories/home.jpg');
