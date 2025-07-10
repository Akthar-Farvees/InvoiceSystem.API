using InvoiceSystem.API.DTO;

namespace InvoiceSystem.API.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateInvoiceAsync(InvoiceCreateDto invoiceCreateDto);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync();
        Task<bool> DeleteInvoiceAsync(int invoiceId);
    }
}
