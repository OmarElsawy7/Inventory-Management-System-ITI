using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(150)]
        public string? ContactName { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(50)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
