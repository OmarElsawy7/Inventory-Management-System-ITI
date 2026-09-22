using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }

        public int SupplierID { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; } = null!;

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
