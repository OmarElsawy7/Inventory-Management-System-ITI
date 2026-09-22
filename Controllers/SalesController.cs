using InventorySystem.Models;
using InventorySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventorySystem.Controllers
{
    public class SalesController : Controller
    {
        private readonly InventoryContext context = new InventoryContext();
        public IActionResult Index(string? search)
        {
            var q = context.Sales.Include(s=>s.SaleItems).AsQueryable();
            if(!string.IsNullOrWhiteSpace(search))
            {
                search=search.Trim();
                if(int.TryParse(search.Replace("#", ""), out var id))
                    q=q.Where(s=>s.SaleID==id || (s.CustomerInfo!=null && s.CustomerInfo.Contains(search)));
                else q=q.Where(s=>s.CustomerInfo!=null && s.CustomerInfo.Contains(search));
            }
            ViewBag.Search=search;
            return View(q.OrderByDescending(s=>s.SaleDate).ThenByDescending(s=>s.SaleID).ToList());
        }
        public IActionResult Details(int id)
        {
            var sale=context.Sales.Include(s=>s.SaleItems).ThenInclude(i=>i.Product).FirstOrDefault(s=>s.SaleID==id);
            return sale==null ? NotFound() : View(sale);
        }
        public IActionResult Receipt(int id)
        {
            var sale=context.Sales.Include(s=>s.SaleItems).ThenInclude(i=>i.Product).FirstOrDefault(s=>s.SaleID==id);
            return sale==null ? NotFound() : View(sale);
        }
        public IActionResult Create()
        {
            LoadProducts();
            return View(new SaleCreateViewModel{SaleDate=DateTime.Now,Items=new(){new SaleItemInputViewModel{Quantity=1}}});
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(SaleCreateViewModel model)
        {
            if(model.Items==null || model.Items.Count==0) ModelState.AddModelError("Items","Add at least one product.");
            if(model.Items.Any(i=>i.ProductID<=0)) ModelState.AddModelError("Items","Please select a product for every row.");
            var duplicate=model.Items.Where(i=>i.ProductID>0).GroupBy(i=>i.ProductID).Any(g=>g.Count()>1);
            if(duplicate) ModelState.AddModelError("Items","The same product cannot be added more than once.");
            foreach(var item in model.Items.Select((x,i)=>(x,i)))
            {
                var product=context.Products.Find(item.x.ProductID);
                if(product==null) ModelState.AddModelError($"Items[{item.i}].ProductID","Please select a valid product.");
                else if(item.x.Quantity>product.StockQuantity) ModelState.AddModelError($"Items[{item.i}].Quantity",$"Only {product.StockQuantity} unit(s) of {product.ProductName} are available.");
            }
            if(!ModelState.IsValid){LoadProducts(); return View(model);}
            using var tx=context.Database.BeginTransaction();
            try
            {
                var customerInfo = string.Join(" | ", new[] { model.CustomerName?.Trim(), model.Phone?.Trim(), model.Email?.Trim(), model.Address?.Trim() }.Where(x=>!string.IsNullOrWhiteSpace(x)));
                var sale=new Sale{CustomerInfo=customerInfo,SaleDate=model.SaleDate};
                foreach(var item in model.Items)
                {
                    var product=context.Products.First(p=>p.ProductID==item.ProductID);
                    if(product.StockQuantity<item.Quantity) throw new InvalidOperationException($"Insufficient stock for {product.ProductName}.");
                    product.StockQuantity-=item.Quantity;
                    sale.SaleItems.Add(new SaleItem{ProductID=product.ProductID,Quantity=item.Quantity,UnitPrice=product.UnitPrice});
                    sale.TotalAmount+=item.Quantity*product.UnitPrice;
                }
                context.Sales.Add(sale); context.SaveChanges(); tx.Commit();
                TempData["Success"]=$"Sale #{sale.SaleID} completed successfully. Stock was updated.";
                return RedirectToAction(nameof(Details),new{id=sale.SaleID});
            }
            catch(Exception ex){tx.Rollback(); ModelState.AddModelError(string.Empty,ex.Message); LoadProducts(); return View(model);}
        }
        private void LoadProducts()=>ViewBag.Products=context.Products.Where(p=>p.StockQuantity>0).OrderBy(p=>p.ProductName).ToList();
    }
}
