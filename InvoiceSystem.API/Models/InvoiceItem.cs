using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.API.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public virtual Invoice Invoice { get; set; } = null!;
    }
}
