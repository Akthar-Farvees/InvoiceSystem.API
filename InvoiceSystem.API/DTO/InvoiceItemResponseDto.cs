namespace InvoiceSystem.API.DTO
{
    public class InvoiceItemResponseDto
    {
        public int InvoiceItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
