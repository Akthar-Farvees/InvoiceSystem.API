using InvoiceSystem.API.Data;
using InvoiceSystem.API.DTO;
using InvoiceSystem.API.Models;
using InvoiceSystem.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceResponseDto> CreateInvoiceAsync(InvoiceCreateDto invoiceCreateDto)
        {
            var invoice = new Invoice
            {
                TransactionDate = invoiceCreateDto.TransactionDate,
                CustomerName = invoiceCreateDto.CustomerName,
                CustomerEmail = invoiceCreateDto.CustomerEmail,
                CustomerPhone = invoiceCreateDto.CustomerPhone,
                Discount = invoiceCreateDto.Discount,
                CreatedAt = DateTime.UtcNow
            };

            // Calculate totals
            decimal subtotal = 0;
            var invoiceItems = new List<InvoiceItem>();

            foreach (var itemDto in invoiceCreateDto.Items)
            {
                var totalPrice = itemDto.UnitPrice * itemDto.Quantity;
                subtotal += totalPrice;

                var invoiceItem = new InvoiceItem
                {
                    ProductName = itemDto.ProductName,
                    ProductDescription = itemDto.ProductDescription,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };

                invoiceItems.Add(invoiceItem);
            }

            invoice.TotalAmount = subtotal - invoiceCreateDto.Discount;
            invoice.BalanceAmount = invoice.TotalAmount; // Assuming full balance initially
            invoice.InvoiceItems = invoiceItems;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return MapToResponseDto(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            return invoice != null ? MapToResponseDto(invoice) : null;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invoices.Select(MapToResponseDto);
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        private static InvoiceResponseDto MapToResponseDto(Invoice invoice)
        {
            return new InvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                TransactionDate = invoice.TransactionDate,
                CustomerName = invoice.CustomerName,
                CustomerEmail = invoice.CustomerEmail,
                CustomerPhone = invoice.CustomerPhone,
                Discount = invoice.Discount,
                TotalAmount = invoice.TotalAmount,
                BalanceAmount = invoice.BalanceAmount,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.InvoiceItems.Select(item => new InvoiceItemResponseDto
                {
                    InvoiceItemId = item.InvoiceItemId,
                    ProductName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }
    }
}
