using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InventorySystem.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(10,2)")] public decimal TotalAmount { get; set; }
        [StringLength(255)] public string? CustomerInfo { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
