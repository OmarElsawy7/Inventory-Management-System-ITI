using System.ComponentModel.DataAnnotations;
namespace InventorySystem.ViewModels
{
    public class SaleCreateViewModel
    {
        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        [EmailAddress] public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public List<SaleItemInputViewModel> Items { get; set; } = new();
    }
    public class SaleItemInputViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
        public int ProductID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}
