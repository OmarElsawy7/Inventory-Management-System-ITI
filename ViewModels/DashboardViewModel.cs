using InventorySystem.Models; // اتأكد إن الـ Namespace مطابق لمشروعك

namespace InventorySystem.ViewModels
{
    public class DashboardViewModel
    {
        // KPI Cards Data
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalStockQuantity { get; set; }
        public int LowStockProductsCount { get; set; }

        public int TotalPurchasesCount { get; set; }
        public decimal TotalPurchasesValue { get; set; }

        public int TotalSalesCount { get; set; }
        public decimal TotalSalesRevenue { get; set; }

        // Lists & Tables
        public List<Product> LowStockProducts { get; set; } = new();
        public List<Sale> RecentSales { get; set; } = new();
        public List<Purchase> RecentPurchases { get; set; } = new();
        public List<MostSoldProductViewModel> MostSoldProducts { get; set; } = new();
    }

    public class MostSoldProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}