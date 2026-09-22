# Member 3 - Suppliers & Purchases

## Implemented
- Supplier CRUD: create, list, details, edit, delete.
- Supplier contact fields: name, contact name, phone, email, address.
- Supplier details show distinct products previously purchased from that supplier.
- Purchase history and purchase details.
- Purchase creation with supplier selection and multiple product rows.
- Quantity and unit cost per product.
- Client-side line total and purchase total preview.
- Server-side total calculation (authoritative).
- PurchaseItems creation.
- Product stock automatically increases when a purchase is completed.
- Database transaction protects purchase + stock update as one operation.
- Supplier deletion is blocked when purchase history exists.

## Database note
The received project contains only `Category` and `Product` entities/DbSets and has no Migrations folder. Member 3 therefore adds the missing `Supplier`, `Purchase`, and `PurchaseItem` entities required by the project specification.

If your local SQL Server database already exists with only Categories/Products, run:

`Database/Member3_Tables.sql`

Alternatively, if your team is using EF Core migrations, create/apply a migration from Visual Studio after coordinating with Member 1.

## Test flow
1. Run the project and open Suppliers.
2. Create at least one supplier.
3. Note the current stock of one or more products.
4. Open Purchases > Create Purchase.
5. Select supplier, add one or more products, enter quantity and unit cost.
6. Confirm the displayed total.
7. Complete Purchase.
8. Open Purchase Details and verify every line and total.
9. Open Products and verify stock increased by the purchased quantities.
10. Open Supplier Details and verify supplied products and purchase history appear.
