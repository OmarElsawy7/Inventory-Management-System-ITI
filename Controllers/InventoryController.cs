using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
namespace InventorySystem.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryContext context = new InventoryContext();
        public IActionResult Index(string? search, bool? lowStock)
        {
            var q=context.Products.AsQueryable();
            if(!string.IsNullOrWhiteSpace(search)) q=q.Where(p=>p.ProductName.Contains(search) || p.SKU.Contains(search));
            if(lowStock==true) q=q.Where(p=>p.StockQuantity<=p.LowStockThreshold);
            ViewBag.Search=search; ViewBag.LowStock=lowStock;
            return View(q.OrderBy(p=>p.StockQuantity).ThenBy(p=>p.ProductName).ToList());
        }
    }
}
