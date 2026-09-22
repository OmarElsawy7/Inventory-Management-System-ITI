using InventorySystem.Models;

namespace InventorySystem.ViewModels
{
    public class SupplierDetailsViewModel
    {
        public Supplier Supplier { get; set; } = new Supplier();

        public int TotalPurchases { get; set; }

        public decimal TotalPurchaseValue { get; set; }

        public int UniqueProductsSupplied { get; set; }

        public DateTime? LastPurchaseDate { get; set; }

        public List<SupplierProductSummaryViewModel> Products { get; set; }
            = new List<SupplierProductSummaryViewModel>();
    }


    public class SupplierProductSummaryViewModel
    {
        public int ProductID { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public int TotalQuantitySupplied { get; set; }

        public decimal TotalPurchasedValue { get; set; }
    }
}