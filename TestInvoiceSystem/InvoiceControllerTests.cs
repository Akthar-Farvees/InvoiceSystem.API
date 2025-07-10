using InvoiceSystem.API.Controllers;
using InvoiceSystem.API.DTO;
using InvoiceSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace TestInvoiceSystem
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _mockInvoiceService;
        private readonly Mock<ILogger<InvoiceController>> _mockLogger;
        private readonly InvoiceController _controller;

        public InvoiceControllerTests()
        {
            _mockInvoiceService = new Mock<IInvoiceService>();
            _mockLogger = new Mock<ILogger<InvoiceController>>();
            _controller = new InvoiceController(_mockInvoiceService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateInvoice_ValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var invoiceCreateDto = new InvoiceCreateDto
            {
                TransactionDate = DateTime.Now,
                CustomerName = "John Doe",
                CustomerEmail = "john@example.com",
                CustomerPhone = "1234567890",
                Discount = 10.00m,
                Items = new List<InvoiceItemCreateDto>
                {
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Laptop",
                        ProductDescription = "Gaming Laptop",
                        Quantity = 1,
                        UnitPrice = 1000.00m
                    }
                }
            };

            var invoiceResponse = new InvoiceResponseDto
            {
                InvoiceId = 1,
                TransactionDate = invoiceCreateDto.TransactionDate,
                CustomerName = invoiceCreateDto.CustomerName,
                CustomerEmail = invoiceCreateDto.CustomerEmail,
                CustomerPhone = invoiceCreateDto.CustomerPhone,
                Discount = invoiceCreateDto.Discount,
                TotalAmount = 990.00m,
                BalanceAmount = 990.00m,
                CreatedAt = DateTime.UtcNow
            };

            _mockInvoiceService.Setup(x => x.CreateInvoiceAsync(It.IsAny<InvoiceCreateDto>()))
                .ReturnsAsync(invoiceResponse);

            // Act
            var result = await _controller.CreateInvoice(invoiceCreateDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<InvoiceResponseDto>(createdResult.Value);
            Assert.Equal(1, returnValue.InvoiceId);
            Assert.Equal("John Doe", returnValue.CustomerName);
        }

        [Fact]
        public async Task CreateInvoice_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var invoiceCreateDto = new InvoiceCreateDto
            {
                TransactionDate = DateTime.Now,
                CustomerName = "John Doe",
                Items = new List<InvoiceItemCreateDto>
                {
                    new InvoiceItemCreateDto
                    {
                        ProductName = "Laptop",
                        Quantity = 1,
                        UnitPrice = 1000.00m
                    }
                }
            };

            _mockInvoiceService.Setup(x => x.CreateInvoiceAsync(It.IsAny<InvoiceCreateDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateInvoice(invoiceCreateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetInvoiceById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var invoiceId = 1;
            var invoiceResponse = new InvoiceResponseDto
            {
                InvoiceId = invoiceId,
                CustomerName = "John Doe",
                TotalAmount = 1000.00m
            };

            _mockInvoiceService.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
                .ReturnsAsync(invoiceResponse);

            // Act
            var result = await _controller.GetInvoiceById(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<InvoiceResponseDto>(okResult.Value);
            Assert.Equal(invoiceId, returnValue.InvoiceId);
        }

        [Fact]
        public async Task GetInvoiceById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var invoiceId = 999;
            _mockInvoiceService.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
                .ReturnsAsync((InvoiceResponseDto?)null);

            // Act
            var result = await _controller.GetInvoiceById(invoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Invoice with ID {invoiceId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllInvoices_ReturnsOkResult()
        {
            // Arrange
            var invoices = new List<InvoiceResponseDto>
            {
                new InvoiceResponseDto { InvoiceId = 1, CustomerName = "John Doe" },
                new InvoiceResponseDto { InvoiceId = 2, CustomerName = "Jane Smith" }
            };

            _mockInvoiceService.Setup(x => x.GetAllInvoicesAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _controller.GetAllInvoices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<InvoiceResponseDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task DeleteInvoice_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var invoiceId = 1;
            _mockInvoiceService.Setup(x => x.DeleteInvoiceAsync(invoiceId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteInvoice(invoiceId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteInvoice_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var invoiceId = 999;
            _mockInvoiceService.Setup(x => x.DeleteInvoiceAsync(invoiceId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteInvoice(invoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Invoice with ID {invoiceId} not found", notFoundResult.Value);
        }
    }
}