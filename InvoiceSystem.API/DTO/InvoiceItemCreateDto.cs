using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.API.DTO
{
    public class InvoiceItemCreateDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Product description cannot exceed 500 characters")]
        public string ProductDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
