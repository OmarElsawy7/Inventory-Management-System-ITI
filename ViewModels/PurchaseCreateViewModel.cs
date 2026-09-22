using System.ComponentModel.DataAnnotations;

namespace InventorySystem.ViewModels
{
    public class PurchaseCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a supplier.")]
        public int SupplierID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public List<PurchaseItemInputViewModel> Items { get; set; } = new List<PurchaseItemInputViewModel>();
    }

    public class PurchaseItemInputViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
        public decimal UnitCost { get; set; }
    }
}
