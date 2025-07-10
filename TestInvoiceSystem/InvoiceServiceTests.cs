using InvoiceSystem.API.Data;
using InvoiceSystem.API.DTO;
using InvoiceSystem.API.Models;
using InvoiceSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInvoiceSystem
{
    public class InvoiceServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new InvoiceService(_context);
        }


        [Fact]
        public async Task CreateInvoiceAsync_ValidData_CreatesInvoice()
        {
            // Arrange
            var invoiceCreateDto = new InvoiceCreateDto
            {
                TransactionDate = DateTime.Now,
                CustomerName = "John Doe",
                CustomerEmail = "john@example.com",
                CustomerPhone = "1234567890",
                Discount = 50.00m,
                Items = new List<InvoiceItemCreateDto>
                {
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Laptop",
                        ProductDescription = "Gaming Laptop",
                        Quantity = 1,
                        UnitPrice = 1000.00m
                    },
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Mouse",
                        ProductDescription = "Wireless Mouse",
                        Quantity = 2,
                        UnitPrice = 25.00m
                    }
                }
            };

            // Act
            var result = await _service.CreateInvoiceAsync(invoiceCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.CustomerName);
            Assert.Equal("john@example.com", result.CustomerEmail);
            Assert.Equal(50.00m, result.Discount);
            Assert.Equal(1000.00m, result.TotalAmount); // 1050 - 50 discount
            Assert.Equal(2, result.Items.Count);

            // Verify in database
            var invoiceInDb = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == result.InvoiceId);

            Assert.NotNull(invoiceInDb);
            Assert.Equal(2, invoiceInDb.InvoiceItems.Count);
        }

        [Fact]
        public async Task GetInvoiceByIdAsync_ExistingId_ReturnsInvoice()
        {
            // Arrange
            var invoice = new Invoice
            {
                TransactionDate = DateTime.Now,
                CustomerName = "Jane Smith",
                CustomerEmail = "jane@example.com",
                Discount = 0,
                TotalAmount = 500.00m,
                BalanceAmount = 500.00m,
                InvoiceItems = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        ProductName = "Keyboard",
                        Quantity = 1,
                        UnitPrice = 500.00m,
                        TotalPrice = 500.00m
                    }
                }
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetInvoiceByIdAsync(invoice.InvoiceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane Smith", result.CustomerName);
            Assert.Equal(500.00m, result.TotalAmount);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetInvoiceByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetInvoiceByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllInvoicesAsync_ReturnsAllInvoices()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice
                {
                    TransactionDate = DateTime.Now,
                    CustomerName = "Customer 1",
                    TotalAmount = 100.00m,
                    BalanceAmount = 100.00m
                },
                new Invoice
                {
                    TransactionDate = DateTime.Now,
                    CustomerName = "Customer 2",
                    TotalAmount = 200.00m,
                    BalanceAmount = 200.00m
                }
            };

            _context.Invoices.AddRange(invoices);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllInvoicesAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteInvoiceAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var invoice = new Invoice
            {
                TransactionDate = DateTime.Now,
                CustomerName = "Test Customer",
                TotalAmount = 100.00m,
                BalanceAmount = 100.00m
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteInvoiceAsync(invoice.InvoiceId);

            // Assert
            Assert.True(result);

            var deletedInvoice = await _context.Invoices.FindAsync(invoice.InvoiceId);
            Assert.Null(deletedInvoice);
        }

        [Fact]
        public async Task DeleteInvoiceAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteInvoiceAsync(999);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task CreateInvoiceAsync_CalculatesCorrectTotals()
        {
            // Arrange
            var invoiceCreateDto = new InvoiceCreateDto
            {
                TransactionDate = DateTime.Now,
                CustomerName = "Test Customer",
                Discount = 100.00m,
                Items = new List<InvoiceItemCreateDto>
                {
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Product 1",
                        Quantity = 2,
                        UnitPrice = 300.00m
                    },
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Product 2",
                        Quantity = 1,
                        UnitPrice = 400.00m
                    }
                }
            };

            // Act
            var result = await _service.CreateInvoiceAsync(invoiceCreateDto);

            // Assert
            // Subtotal: (2 * 300) + (1 * 400) = 600 + 400 = 1000
            // Total: 1000 - 100 (discount) = 900
            Assert.Equal(900.00m, result.TotalAmount);
            Assert.Equal(900.00m, result.BalanceAmount);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

    

