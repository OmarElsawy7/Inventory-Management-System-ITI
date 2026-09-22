using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using InventorySystem.ViewModels;

namespace InventorySystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly InventoryContext _context;

        public DashboardController(InventoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productsCount = await _context.Products.CountAsync();
            var categoriesCount = await _context.Categories.CountAsync();
            var suppliersCount = await _context.Suppliers.CountAsync();
            var totalStock = await _context.Products.SumAsync(p => (int?)p.StockQuantity) ?? 0;
            var lowStockCount = await _context.Products.CountAsync(p => p.StockQuantity <= p.LowStockThreshold);

            var purchasesCount = await _context.Purchases.CountAsync();
            var purchasesValue = await _context.Purchases.SumAsync(pu => (decimal?)pu.TotalAmount) ?? 0;

            var salesCount = await _context.Sales.CountAsync();
            var salesRevenue = await _context.Sales.SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

            var lowStockList = await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .ToListAsync();

            var recentPurchases = await _context.Purchases
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(5)
                .ToListAsync();

            var recentSales = await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync();

            var allSaleItems = await _context.SaleItems
                .Include(si => si.Product)
                .ToListAsync();

            var mostSoldProducts = allSaleItems
                .Where(si => si.Product != null)
                .GroupBy(si => si.ProductID)
                .Select(g => new MostSoldProductViewModel
                {
                    ProductName = g.First().Product!.ProductName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalProducts = productsCount,
                TotalCategories = categoriesCount,
                TotalSuppliers = suppliersCount,
                TotalStockQuantity = totalStock,
                LowStockProductsCount = lowStockCount,
                TotalPurchasesCount = purchasesCount,
                TotalPurchasesValue = purchasesValue,
                TotalSalesCount = salesCount,
                TotalSalesRevenue = salesRevenue,
                LowStockProducts = lowStockList,
                RecentPurchases = recentPurchases,
                RecentSales = recentSales,
                MostSoldProducts = mostSoldProducts
            };

            return View(viewModel);
        }
    }
}