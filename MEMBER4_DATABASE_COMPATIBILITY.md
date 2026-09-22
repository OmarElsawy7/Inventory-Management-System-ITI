# Member 4 - Database Compatibility

This version is aligned with `DBprojITI.sql`.

## Existing database tables used
- `Products` (`ProductID`, `SKU`, `ProductName`, `UnitPrice`, `StockQuantity`, `LowStockThreshold`)
- `Sales` (`SaleID`, `SaleDate`, `TotalAmount`, `CustomerInfo`)
- `SalesItems` (`SaleItemID`, `SaleID`, `ProductID`, `Quantity`, `UnitPrice`)

## Important change
The original Member 4 implementation introduced a `Customers` table and `CustomerID` relationship. The supplied database does not contain that table. Customer data is now stored in `Sales.CustomerInfo`, exactly as defined in the supplied schema.

## Database setup
Run `DBprojITI.sql` to create the database, or restore `InventoryManagementDB.bak`. Do not apply the old Member 4 migrations that create `Customers` or map sale items to `SaleItems`.
