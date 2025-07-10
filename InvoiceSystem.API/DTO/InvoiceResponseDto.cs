namespace InvoiceSystem.API.DTO
{
    public class InvoiceResponseDto
    {
        public int InvoiceId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new List<InvoiceItemResponseDto>();
    }
}
