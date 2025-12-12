-- Run this script AFTER Setup.sql to populate with sample data
USE EcommerceDB;
GO

-- Clear existing data (except admin user)
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Addresses WHERE UserId != 1;
DELETE FROM ProductVariants;
DELETE FROM ProductImages;
DELETE FROM Reviews;
DELETE FROM Products;
DELETE FROM Categories WHERE Id != 1;

-- Insert Categories
INSERT INTO Categories (Name, Description) VALUES 
('Montres de Luxe', 'Collection de montres haut de gamme'),
('Sacs Premium', 'Sacs en cuir authentique'),
('Bijoux', 'Bijoux raffinés et élégants'),
('Accessoires', 'Accessoires de mode premium');

-- Get Category IDs
DECLARE @WatchCat INT = (SELECT Id FROM Categories WHERE Name = 'Montres de Luxe');
DECLARE @BagCat INT = (SELECT Id FROM Categories WHERE Name = 'Sacs Premium');
DECLARE @JewelryCat INT = (SELECT Id FROM Categories WHERE Name = 'Bijoux');
DECLARE @AccessoryCat INT = (SELECT Id FROM Categories WHERE Name = 'Accessoires');

-- Insert Products - Watches
INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES
(@WatchCat, 'Montre Chronographe Or', 'Montre suisse automatique en or 18 carats avec bracelet cuir véritable. Mouvement automatique de précision.', 2499.00, 5, 'https://images.unsplash.com/photo-1587836374828-4dbafa94cf0e?w=400'),
(@WatchCat, 'Montre Élégance Diamant', 'Montre femme sertie de diamants avec cadran nacre. Boîtier acier inoxydable et bracelet milanais.', 1899.00, 8, 'https://images.unsplash.com/photo-1523170335258-f5ed11844a49?w=400'),
(@WatchCat, 'Montre Sport Premium', 'Chronographe sportif étanche 200m. Cadran noir avec détails dorés. Parfait pour le quotidien.', 899.00, 15, 'https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?w=400'),
(@WatchCat, 'Montre Classique Argent', 'Design intemporel avec cadran minimaliste. Bracelet en cuir italien et boîtier argenté.', 649.00, 12, 'https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=400');

-- Insert Products - Bags
INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES
(@BagCat, 'Sac à Main Cuir Premium', 'Sac en cuir italien véritable couleur camel. Spacieux avec multiples compartiments et fermeture dorée.', 799.00, 10, 'https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400'),
(@BagCat, 'Pochette Soirée Élégante', 'Pochette luxueuse noire avec chaîne dorée. Parfaite pour vos soirées chic.', 299.00, 20, 'https://images.unsplash.com/photo-1566150905458-1bf1fc113f0d?w=400'),
(@BagCat, 'Cabas Travail Prestige', 'Grand cabas professionnel en cuir noir. Compartiment ordinateur et finitions premium.', 549.00, 7, 'https://images.unsplash.com/photo-1590874103328-eac38a683ce7?w=400'),
(@BagCat, 'Sac Bandoulière Chic', 'Petit sac crossbody en cuir bordeaux. Design moderne et pratique pour tous les jours.', 399.00, 15, 'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400');

-- Insert Products - Jewelry
INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES
(@JewelryCat, 'Collier Diamant Solitaire', 'Collier or blanc 18k avec diamant certifié 0.5 carat. Chaîne fine et élégante.', 1299.00, 5, 'https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?w=400'),
(@JewelryCat, 'Bracelet Perles Tahiti', 'Bracelet perles de Tahiti authentiques avec fermoir or. Pièce unique et raffinée.', 899.00, 3, 'https://images.unsplash.com/photo-1611591437281-460bfbe1220a?w=400'),
(@JewelryCat, 'Boucles Oreilles Or Rose', 'Boucles en or rose 14k avec petits diamants. Design moderne et féminin.', 449.00, 12, 'https://images.unsplash.com/photo-1535632066927-ab7c9ab60908?w=400'),
(@JewelryCat, 'Bague Émeraude Classique', 'Bague or jaune avec émeraude naturelle entourée de diamants. Élégance intemporelle.', 1599.00, 4, 'https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=400');

-- Insert Products - Accessories
INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES
(@AccessoryCat, 'Écharpe Cachemire Luxe', 'Écharpe 100% cachemire de Mongolie. Douce et chaleureuse, disponible en plusieurs couleurs.', 199.00, 25, 'https://images.unsplash.com/photo-1520903920243-00d872a2d1c9?w=400'),
(@AccessoryCat, 'Lunettes Soleil Premium', 'Lunettes de soleil polarisées avec monture acétate italienne. Protection UV 400.', 349.00, 18, 'https://images.unsplash.com/photo-1511499767150-a48a237f0083?w=400'),
(@AccessoryCat, 'Ceinture Cuir Designer', 'Ceinture en cuir pleine fleur avec boucle argentée. Fabriquée en Italie.', 159.00, 30, 'https://images.unsplash.com/photo-1624222247344-550fb60583f1?w=400'),
(@AccessoryCat, 'Portefeuille Luxe Homme', 'Portefeuille compact en cuir noir avec protection RFID. Multiple compartements cartes.', 129.00, 40, 'https://images.unsplash.com/photo-1627123424574-724758594e93?w=400');

PRINT 'Sample data inserted successfully!';
