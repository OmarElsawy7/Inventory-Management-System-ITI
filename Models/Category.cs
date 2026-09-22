using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Category
    {
        public int CategoryID { get; set; }


        // =========================
        // Category Name
        // =========================

        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; } = string.Empty;


        // =========================
        // Description
        // =========================

        public string Description { get; set; } = string.Empty;


        // =========================
        // Products
        // One Category → Many Products
        // =========================

        public virtual ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}