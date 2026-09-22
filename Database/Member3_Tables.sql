USE InventoryManagementDB;
GO

-- MEMBER 3 prerequisite tables.
-- Run this only if Suppliers, Purchases and PurchaseItems do not already exist.

IF OBJECT_ID('dbo.Suppliers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Suppliers
    (
        SupplierID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Suppliers PRIMARY KEY,
        SupplierName NVARCHAR(150) NOT NULL,
        ContactName NVARCHAR(150) NULL,
        Phone NVARCHAR(50) NULL,
        Email NVARCHAR(150) NULL,
        Address NVARCHAR(300) NULL
    );
END;
GO

IF OBJECT_ID('dbo.Purchases', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Purchases
    (
        PurchaseID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Purchases PRIMARY KEY,
        SupplierID INT NOT NULL,
        PurchaseDate DATETIME2 NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_Purchases_Suppliers_SupplierID
            FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(SupplierID)
    );

    CREATE INDEX IX_Purchases_SupplierID ON dbo.Purchases(SupplierID);
END;
GO

IF OBJECT_ID('dbo.PurchaseItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseItems
    (
        PurchaseItemID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseItems PRIMARY KEY,
        PurchaseID INT NOT NULL,
        ProductID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitCost DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_PurchaseItems_Purchases_PurchaseID
            FOREIGN KEY (PurchaseID) REFERENCES dbo.Purchases(PurchaseID) ON DELETE CASCADE,
        CONSTRAINT FK_PurchaseItems_Products_ProductID
            FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
    );

    CREATE INDEX IX_PurchaseItems_PurchaseID ON dbo.PurchaseItems(PurchaseID);
    CREATE INDEX IX_PurchaseItems_ProductID ON dbo.PurchaseItems(ProductID);
END;
GO
