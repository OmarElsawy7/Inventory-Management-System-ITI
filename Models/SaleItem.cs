using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InventorySystem.Models
{
    public class SaleItem
    {
        public int SaleItemID { get; set; }
        public int SaleID { get; set; }
        [ForeignKey(nameof(SaleID))] public Sale Sale { get; set; } = null!;
        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))] public Product Product { get; set; } = null!;
        [Range(1, int.MaxValue)] public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal UnitPrice { get; set; }
        [NotMapped] public decimal LineTotal => Quantity * UnitPrice;
    }
}
