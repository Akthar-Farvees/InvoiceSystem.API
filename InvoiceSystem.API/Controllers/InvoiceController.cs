using InvoiceSystem.API.DTO;
using InvoiceSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCreateDto invoiceCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });

                }

                var invoice = await _invoiceService.CreateInvoiceAsync(invoiceCreateDto);
                return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.InvoiceId }, new
                {
                    success = true,
                    data = invoice,
                    message = "Invoice created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return StatusCode(500, "Internal server error occurred while creating invoice");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
                if (invoice == null)
                {
                    return NotFound($"Invoice with ID {id} not found");
                }

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with ID {InvoiceId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving invoice");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                return StatusCode(500, "Internal server error occurred while retrieving invoices");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var deleted = await _invoiceService.DeleteInvoiceAsync(id);
                if (!deleted)
                {
                    return NotFound($"Invoice with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice with ID {InvoiceId}", id);
                return StatusCode(500, "Internal server error occurred while deleting invoice");
            }
        }
    }
}

