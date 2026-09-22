using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryContext context =
            new InventoryContext();


        // =====================================================
        // Products List
        // Search + Filter + Pagination
        // =====================================================

        public IActionResult Index(
            string? search,
            int? categoryId,
            string? stockStatus,
            int page = 1)
        {
            const int pageSize = 5;


            // =========================
            // Initial Query
            // =========================

            var products =
                context.Products.AsQueryable();



            // =========================
            // Search
            // Product Name OR SKU
            // =========================

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();


                products = products.Where(
                    p =>
                        p.ProductName.Contains(search)
                        ||
                        p.SKU.Contains(search)
                );
            }



            // =========================
            // Filter by Category
            // =========================

            if (
                categoryId.HasValue
                &&
                categoryId.Value > 0
            )
            {
                products = products.Where(
                    p => p.CategoryID == categoryId.Value
                );
            }



            // =========================
            // Filter by Stock Status
            // =========================

            if (!string.IsNullOrWhiteSpace(stockStatus))
            {
                switch (stockStatus)
                {
                    case "OutOfStock":

                        products = products.Where(
                            p => p.StockQuantity == 0
                        );

                        break;


                    case "LowStock":

                        products = products.Where(
                            p =>
                                p.StockQuantity > 0
                                &&
                                p.StockQuantity <=
                                p.LowStockThreshold
                        );

                        break;


                    case "InStock":

                        products = products.Where(
                            p =>
                                p.StockQuantity >
                                p.LowStockThreshold
                        );

                        break;
                }
            }



            // =========================
            // Pagination
            // =========================

            int totalItems =
                products.Count();


            int totalPages =
                totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(
                        (double)totalItems / pageSize
                    );


            if (page < 1)
            {
                page = 1;
            }


            if (
                totalPages > 0
                &&
                page > totalPages
            )
            {
                page = totalPages;
            }



            List<Product> pagedProducts =
                products
                    .OrderBy(p => p.ProductName)
                    .ThenBy(p => p.ProductID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();



            // =========================
            // View Data
            // =========================

            ViewBag.Categories =
                context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToList();


            ViewBag.CurrentPage = page;

            ViewBag.TotalPages = totalPages;

            ViewBag.TotalItems = totalItems;

            ViewBag.Search = search;

            ViewBag.CategoryId = categoryId;

            ViewBag.StockStatus = stockStatus;



            return View(
                "ProductsIndex",
                pagedProducts
            );
        }



        // =====================================================
        // Product Details
        // =====================================================

        public IActionResult Details(int id)
        {
            Product? product =
                context.Products
                    .FirstOrDefault(
                        p => p.ProductID == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            product.Category =
                context.Categories
                    .FirstOrDefault(
                        c =>
                            c.CategoryID
                            ==
                            product.CategoryID
                    );


            return View(
                "ProductsDetails",
                product
            );
        }



        // =====================================================
        // Create Product
        // =====================================================

        public IActionResult Create()
        {
            LoadCategories();


            return View(
                "ProductsCreate"
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            // =========================
            // Normalize SKU
            // =========================

            product.SKU =
                product.SKU?.Trim().ToUpper()
                ?? string.Empty;


            product.ProductName =
                product.ProductName?.Trim()
                ?? string.Empty;



            // =========================
            // Duplicate SKU Check
            // =========================

            bool skuExists =
                context.Products.Any(
                    p =>
                        p.SKU.ToUpper()
                        ==
                        product.SKU
                );


            if (skuExists)
            {
                ModelState.AddModelError(
                    nameof(Product.SKU),
                    "This SKU already exists. Please use a unique SKU."
                );
            }



            // =========================
            // Validation Failed
            // =========================

            if (!ModelState.IsValid)
            {
                LoadCategories();


                return View(
                    "ProductsCreate",
                    product
                );
            }



            // =========================
            // Save Product
            // =========================

            context.Products.Add(product);

            context.SaveChanges();


            TempData["Success"] =
                "Product created successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }



        // =====================================================
        // Edit Product
        // =====================================================

        public IActionResult Edit(int id)
        {
            Product? product =
                context.Products
                    .FirstOrDefault(
                        p => p.ProductID == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            LoadCategories();


            return View(
                "ProductsEdit",
                product
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            // =========================
            // Normalize
            // =========================

            product.SKU =
                product.SKU?.Trim().ToUpper()
                ?? string.Empty;


            product.ProductName =
                product.ProductName?.Trim()
                ?? string.Empty;



            // =========================
            // Duplicate SKU
            // Ignore Current Product
            // =========================

            bool skuExists =
                context.Products.Any(
                    p =>
                        p.ProductID
                        !=
                        product.ProductID

                        &&

                        p.SKU.ToUpper()
                        ==
                        product.SKU
                );


            if (skuExists)
            {
                ModelState.AddModelError(
                    nameof(Product.SKU),
                    "This SKU is already used by another product."
                );
            }



            if (!ModelState.IsValid)
            {
                LoadCategories();


                return View(
                    "ProductsEdit",
                    product
                );
            }



            Product? existing =
                context.Products
                    .FirstOrDefault(
                        p =>
                            p.ProductID
                            ==
                            product.ProductID
                    );


            if (existing == null)
            {
                return NotFound();
            }



            // =========================
            // Update Data
            // =========================

            existing.SKU =
                product.SKU;


            existing.ProductName =
                product.ProductName;


            existing.CategoryID =
                product.CategoryID;


            existing.UnitPrice =
                product.UnitPrice;


            existing.StockQuantity =
                product.StockQuantity;


            existing.LowStockThreshold =
                product.LowStockThreshold;



            context.SaveChanges();


            TempData["Success"] =
                "Product updated successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }



        // =====================================================
        // Delete Product
        // =====================================================

        public IActionResult Delete(int id)
        {
            Product? product =
                context.Products
                    .FirstOrDefault(
                        p => p.ProductID == id
                    );


            if (product == null)
            {
                return RedirectToAction(
                    nameof(Index)
                );
            }



            // =========================
            // Protect Purchase History
            // =========================

            bool usedInPurchases =
                context.PurchaseItems.Any(
                    pi =>
                        pi.ProductID == id
                );


            if (usedInPurchases)
            {
                TempData["Error"] =
                    "This product cannot be deleted because it exists in purchase history.";


                return RedirectToAction(
                    nameof(Index)
                );
            }



            context.Products.Remove(product);

            context.SaveChanges();


            TempData["Success"] =
                "Product deleted successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }



        // =====================================================
        // Helper
        // Load Categories
        // =====================================================

        private void LoadCategories()
        {
            ViewBag.Categories =
                context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToList();
        }
    }
}