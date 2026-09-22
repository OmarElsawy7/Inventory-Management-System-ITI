using InventorySystem.Models;
using InventorySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly InventoryContext context = new InventoryContext();

        // ============ Purchase History ============
        // ============ Purchase History + Search & Filter ============
        public IActionResult Index(
            string? search,
            int? supplierId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var purchases = context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .AsQueryable();


            // ================= Search =================
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                // If user entered a purchase number
                if (int.TryParse(search.Replace("#", ""), out int purchaseNumber))
                {
                    purchases = purchases.Where(p =>
                        p.PurchaseID == purchaseNumber ||
                        (p.Supplier != null &&
                         p.Supplier.SupplierName.Contains(search)));
                }
                else
                {
                    // Search by Supplier Name
                    purchases = purchases.Where(p =>
                        p.Supplier != null &&
                        p.Supplier.SupplierName.Contains(search));
                }
            }


            // ================= Supplier Filter =================
            if (supplierId.HasValue && supplierId.Value > 0)
            {
                purchases = purchases.Where(p =>
                    p.SupplierID == supplierId.Value);
            }


            // ================= From Date =================
            if (fromDate.HasValue)
            {
                purchases = purchases.Where(p =>
                    p.PurchaseDate >= fromDate.Value.Date);
            }


            // ================= To Date =================
            if (toDate.HasValue)
            {
                DateTime endDate =
                    toDate.Value.Date.AddDays(1);

                purchases = purchases.Where(p =>
                    p.PurchaseDate < endDate);
            }


            // ================= Dropdown Suppliers =================
            ViewBag.Suppliers = context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToList();


            // Keep Filter Values after Search
            ViewBag.Search = search;
            ViewBag.SupplierId = supplierId;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;


            List<Purchase> result = purchases
                .OrderByDescending(p => p.PurchaseDate)
                .ThenByDescending(p => p.PurchaseID)
                .ToList();


            return View(result);
        }

        // ============ Purchase Details ============
        public IActionResult Details(int id)
        {
            Purchase? purchase = context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefault(p => p.PurchaseID == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // ============ Create Purchase ============
        public IActionResult Create()
        {
            LoadCreateLists();

            PurchaseCreateViewModel model = new PurchaseCreateViewModel
            {
                PurchaseDate = DateTime.Now,
                Items = new List<PurchaseItemInputViewModel>
                {
                    new PurchaseItemInputViewModel { Quantity = 1 }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PurchaseCreateViewModel model)
        {
            if (!context.Suppliers.Any(s => s.SupplierID == model.SupplierID))
            {
                ModelState.AddModelError(nameof(model.SupplierID), "Please select a valid supplier.");
            }

            if (model.Items == null || model.Items.Count == 0)
            {
                ModelState.AddModelError("Items", "Add at least one product to the purchase.");
            }
            else
            {
                for (int i = 0; i < model.Items.Count; i++)
                {
                    PurchaseItemInputViewModel item = model.Items[i];

                    if (!context.Products.Any(p => p.ProductID == item.ProductID))
                        ModelState.AddModelError($"Items[{i}].ProductID", "Please select a valid product.");
                }

                List<int> duplicateProductIds = model.Items
                    .Where(i => i.ProductID > 0)
                    .GroupBy(i => i.ProductID)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateProductIds.Count > 0)
                {
                    ModelState.AddModelError("Items", "The same product cannot be added more than once. Increase its quantity instead.");
                }
            }

            if (!ModelState.IsValid)
            {
                LoadCreateLists();
                return View(model);
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                Purchase purchase = new Purchase
                {
                    SupplierID = model.SupplierID,
                    PurchaseDate = model.PurchaseDate,
                    TotalAmount = 0
                };

                foreach (PurchaseItemInputViewModel item in model.Items)
                {
                    Product product = context.Products.First(p => p.ProductID == item.ProductID);

                    purchase.PurchaseItems.Add(new PurchaseItem
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost
                    });

                    purchase.TotalAmount += item.Quantity * item.UnitCost;

                    // Member 3 stock logic: purchases increase the current product stock.
                    product.StockQuantity += item.Quantity;
                }

                context.Purchases.Add(purchase);
                context.SaveChanges();
                transaction.Commit();

                TempData["Success"] = $"Purchase #{purchase.PurchaseID} completed successfully. Stock was updated.";
                return RedirectToAction("Details", new { id = purchase.PurchaseID });
            }
            catch
            {
                transaction.Rollback();
                ModelState.AddModelError(string.Empty, "The purchase could not be completed. No stock changes were saved.");
                LoadCreateLists();
                return View(model);
            }
        }

        private void LoadCreateLists()
        {
            ViewBag.Suppliers = context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToList();

            ViewBag.Products = context.Products
                .OrderBy(p => p.ProductName)
                .ToList();
        }
    }
}
