using InventorySystem.Models;
using InventorySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly InventoryContext context =
            new InventoryContext();


        // =====================================================
        // View All Suppliers
        // =====================================================

        public IActionResult Index()
        {
            List<Supplier> suppliers = context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToList();

            return View(suppliers);
        }



        // =====================================================
        // Supplier Details
        // Products + Statistics + Purchase History
        // =====================================================

        public IActionResult Details(int id)
        {
            Supplier? supplier = context.Suppliers
                .Include(s => s.Purchases)
                    .ThenInclude(p => p.PurchaseItems)
                        .ThenInclude(pi => pi.Product)
                .FirstOrDefault(s => s.SupplierID == id);


            if (supplier == null)
            {
                return NotFound();
            }


            var allPurchaseItems = supplier.Purchases
                .SelectMany(p => p.PurchaseItems)
                .ToList();


            // ==========================================
            // Products supplied by this supplier
            // ==========================================

            List<SupplierProductSummaryViewModel> products =
                allPurchaseItems
                .Where(pi => pi.Product != null)
                .GroupBy(pi => new
                {
                    pi.ProductID,
                    pi.Product.SKU,
                    pi.Product.ProductName,
                    pi.Product.StockQuantity
                })
                .Select(group =>
                    new SupplierProductSummaryViewModel
                    {
                        ProductID =
                            group.Key.ProductID,

                        SKU =
                            group.Key.SKU,

                        ProductName =
                            group.Key.ProductName,

                        CurrentStock =
                            group.Key.StockQuantity,

                        TotalQuantitySupplied =
                            group.Sum(x => x.Quantity),

                        TotalPurchasedValue =
                            group.Sum(
                                x => x.Quantity * x.UnitCost
                            )
                    })
                .OrderBy(p => p.ProductName)
                .ToList();



            // ==========================================
            // Create Supplier Details ViewModel
            // ==========================================

            SupplierDetailsViewModel viewModel =
                new SupplierDetailsViewModel
                {
                    Supplier = supplier,

                    TotalPurchases =
                        supplier.Purchases.Count,

                    TotalPurchaseValue =
                        supplier.Purchases
                            .Sum(p => p.TotalAmount),

                    UniqueProductsSupplied =
                        products.Count,

                    LastPurchaseDate =
                        supplier.Purchases.Any()
                        ? supplier.Purchases
                            .Max(p => p.PurchaseDate)
                        : null,

                    Products = products
                };


            return View(viewModel);
        }



        // =====================================================
        // Create Supplier
        // =====================================================

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }


            context.Suppliers.Add(supplier);

            context.SaveChanges();


            TempData["Success"] =
                "Supplier created successfully.";


            return RedirectToAction("Index");
        }



        // =====================================================
        // Edit Supplier
        // =====================================================

        public IActionResult Edit(int id)
        {
            Supplier? supplier =
                context.Suppliers
                .FirstOrDefault(
                    s => s.SupplierID == id
                );


            if (supplier == null)
            {
                return NotFound();
            }


            return View(supplier);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }


            Supplier? existing =
                context.Suppliers
                .FirstOrDefault(
                    s => s.SupplierID
                         == supplier.SupplierID
                );


            if (existing == null)
            {
                return NotFound();
            }


            existing.SupplierName =
                supplier.SupplierName;

            existing.ContactName =
                supplier.ContactName;

            existing.Phone =
                supplier.Phone;

            existing.Email =
                supplier.Email;

            existing.Address =
                supplier.Address;


            context.SaveChanges();


            TempData["Success"] =
                "Supplier updated successfully.";


            return RedirectToAction("Index");
        }



        // =====================================================
        // Delete Supplier
        // =====================================================

        public IActionResult Delete(int id)
        {
            Supplier? supplier =
                context.Suppliers
                .FirstOrDefault(
                    s => s.SupplierID == id
                );


            if (supplier == null)
            {
                return RedirectToAction("Index");
            }


            bool hasPurchases =
                context.Purchases
                .Any(
                    p => p.SupplierID == id
                );


            if (hasPurchases)
            {
                TempData["Error"] =
                    "This supplier cannot be deleted because it has purchase history.";

                return RedirectToAction("Index");
            }


            context.Suppliers.Remove(supplier);

            context.SaveChanges();


            TempData["Success"] =
                "Supplier deleted successfully.";


            return RedirectToAction("Index");
        }
    }
}