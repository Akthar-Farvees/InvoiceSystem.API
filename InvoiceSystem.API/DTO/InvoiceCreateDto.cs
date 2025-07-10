using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.API.DTO
{
    public class InvoiceCreateDto
    {
        [Required(ErrorMessage = "Transaction date is required")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be non-negative")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Invoice items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<InvoiceItemCreateDto> Items { get; set; } = new List<InvoiceItemCreateDto>();
    }
}
