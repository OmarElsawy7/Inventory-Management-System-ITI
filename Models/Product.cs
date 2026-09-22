using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }


        // =========================
        // SKU
        // =========================

        [Required(ErrorMessage = "SKU is required.")]
        public string SKU { get; set; } = string.Empty;


        // =========================
        // Product Name
        // =========================

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; } = string.Empty;


        // =========================
        // Category
        // =========================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a category."
        )]
        public int CategoryID { get; set; }


        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }


        // =========================
        // Unit Price
        // =========================

        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "Price must be greater than 0."
        )]
        public decimal UnitPrice { get; set; }


        // =========================
        // Stock Quantity
        // =========================

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Stock cannot be negative."
        )]
        public int StockQuantity { get; set; }


        // =========================
        // Low Stock Threshold
        // =========================

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Low stock threshold cannot be negative."
        )]
        public int LowStockThreshold { get; set; } = 5;
    }
}