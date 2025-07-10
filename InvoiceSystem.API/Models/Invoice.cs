using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.API.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BalanceAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
