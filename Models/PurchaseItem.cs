using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemID { get; set; }

        public int PurchaseID { get; set; }

        [ForeignKey("PurchaseID")]
        public virtual Purchase Purchase { get; set; } = null!;

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }
    }
}
