using InvoiceSystem.API.DTO;
using MediatR;

namespace InvoiceSystem.API.Commands
{
    public class CreateInvoiceCommand : IRequest<InvoiceResponseDto>
    {
        public InvoiceCreateDto InvoiceDto { get; set; }

        public CreateInvoiceCommand(InvoiceCreateDto invoiceDto)
        {
            InvoiceDto = invoiceDto;
        }
    }
}
