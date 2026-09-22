using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly InventoryContext context =
            new InventoryContext();


        // =====================================================
        // View All Categories
        // Search + Product Count
        // =====================================================

        public IActionResult Index(string? search)
        {
            var categories =
                context.Categories
                    .Include(c => c.Products)
                    .AsQueryable();


            // =========================
            // Search
            // =========================

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();


                categories = categories.Where(
                    c =>
                        c.CategoryName.Contains(search)
                        ||
                        c.Description.Contains(search)
                );
            }


            List<Category> result =
                categories
                    .OrderBy(c => c.CategoryName)
                    .ToList();


            ViewBag.Search = search;


            return View(
                "CategoriesIndex",
                result
            );
        }



        // =====================================================
        // Category Details
        // Products + Stock Overview
        // =====================================================

        public IActionResult Details(int id)
        {
            Category? category =
                context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefault(
                        c => c.CategoryID == id
                    );


            if (category == null)
            {
                return NotFound();
            }


            return View(
                "CategoriesDetails",
                category
            );
        }



        // =====================================================
        // Create Category
        // =====================================================

        public IActionResult Create()
        {
            return View(
                "CategoriesCreate"
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            // =========================
            // Normalize
            // =========================

            category.CategoryName =
                category.CategoryName?.Trim()
                ?? string.Empty;


            category.Description =
                category.Description?.Trim()
                ?? string.Empty;



            // =========================
            // Duplicate Name Check
            // =========================

            bool categoryExists =
                context.Categories.Any(
                    c =>
                        c.CategoryName.ToLower()
                        ==
                        category.CategoryName.ToLower()
                );


            if (categoryExists)
            {
                ModelState.AddModelError(
                    nameof(Category.CategoryName),
                    "This category name already exists."
                );
            }



            // =========================
            // Validation Failed
            // =========================

            if (!ModelState.IsValid)
            {
                return View(
                    "CategoriesCreate",
                    category
                );
            }



            // =========================
            // Save
            // =========================

            context.Categories.Add(category);

            context.SaveChanges();


            TempData["Success"] =
                "Category created successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }



        // =====================================================
        // Edit Category
        // =====================================================

        public IActionResult Edit(int id)
        {
            Category? category =
                context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefault(
                        c => c.CategoryID == id
                    );


            if (category == null)
            {
                return NotFound();
            }


            return View(
                "CategoriesEdit",
                category
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            // =========================
            // Normalize
            // =========================

            category.CategoryName =
                category.CategoryName?.Trim()
                ?? string.Empty;


            category.Description =
                category.Description?.Trim()
                ?? string.Empty;



            // =========================
            // Duplicate Name Check
            // Ignore Current Category
            // =========================

            bool categoryExists =
                context.Categories.Any(
                    c =>
                        c.CategoryID
                        !=
                        category.CategoryID

                        &&

                        c.CategoryName.ToLower()
                        ==
                        category.CategoryName.ToLower()
                );


            if (categoryExists)
            {
                ModelState.AddModelError(
                    nameof(Category.CategoryName),
                    "This category name is already used by another category."
                );
            }



            if (!ModelState.IsValid)
            {
                return View(
                    "CategoriesEdit",
                    category
                );
            }



            Category? existing =
                context.Categories
                    .FirstOrDefault(
                        c =>
                            c.CategoryID
                            ==
                            category.CategoryID
                    );


            if (existing == null)
            {
                return NotFound();
            }



            // =========================
            // Update
            // =========================

            existing.CategoryName =
                category.CategoryName;


            existing.Description =
                category.Description;


            context.SaveChanges();


            TempData["Success"] =
                "Category updated successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }



        // =====================================================
        // Delete Category
        // POST for safer deletion
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Category? category =
                context.Categories
                    .FirstOrDefault(
                        c => c.CategoryID == id
                    );


            if (category == null)
            {
                TempData["Error"] =
                    "Category was not found.";


                return RedirectToAction(
                    nameof(Index)
                );
            }



            // =========================
            // Protect Products
            // =========================

            bool hasProducts =
                context.Products.Any(
                    p => p.CategoryID == id
                );


            if (hasProducts)
            {
                TempData["Error"] =
                    "This category cannot be deleted because it contains products.";


                return RedirectToAction(
                    nameof(Index)
                );
            }



            context.Categories.Remove(category);

            context.SaveChanges();


            TempData["Success"] =
                "Category deleted successfully.";


            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}