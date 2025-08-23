// Aviendha ABP Framework Extensions
// Copyright (C) 2025 Doug Wilson
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Aviendha.BillingManagement.Customers;
using Aviendha.BillingManagement.Payments;
using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Unit tests for the Invoice Application Service
/// </summary>
public abstract class InvoiceApplicationService_Tests<TStartupModule> : BillingManagementApplicationTestBase<TStartupModule, InvoiceApplicationService>
    where TStartupModule : IAbpModule
{
    private readonly DateOnly TestDate = new(2022, 1, 1);

    [Fact]
    public async Task CreateInvoiceAsync_ShouldCreateInvoice_WithValidRequest()
    {
        // Arrange
        var command = new CreateInvoice()
        {
            CustomerId = CustomerId.New(),
            InvoiceDate = TestDate,
            Items =
            [
                new CreateLineItem
                {
                    ItemId = TestData.TaxableProductId, // $100.00
                    Quantity = 2.0m
                }
            ],
        };

        // Act
        var result = await SUT.CreateInvoiceAsync(command);

        // Assert
        result.Value.ShouldNotBe(InvoiceId.Empty);

        var invoice = await SUT.GetInvoiceAsync(result.Value);
        Assert.NotNull(invoice);
        Assert.Single(invoice.LineItems);
        Assert.Equal(200.00m, invoice.Subtotal);
        Assert.Equal(15.00m, invoice.ShippingCost);
    }

    [Fact]
    public async Task GetInvoiceAsync_ShouldReturnNull_WhenInvoiceNotFound()
    {
        // Arrange
        var nonExistentId = InvoiceId.New();

        // Act
        var result = await SUT.GetInvoiceAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddLineItemAsync_ShouldAddItem_ToExistingInvoice()
    {
        // Arrange
        var invoiceId = await CreateTestInvoice();
        var command = new AddLineItem
        {
            InvoiceId = invoiceId,
            ItemId = TestData.TaxableServiceId,
            Quantity = 1,
        };

        // Act
        await SUT.AddLineItemAsync(command);

        // Assert
        var invoice = await SUT.GetInvoiceAsync(invoiceId);
        Assert.NotNull(invoice);
        Assert.Equal(2, invoice.LineItems.Count);
        Assert.Equal(600.00m, invoice.Subtotal); // Original 100 + new 500
    }

    [Fact]
    public async Task AddLineItemAsync_ShouldThrowException_WhenInvoiceNotFound()
    {
        // Arrange
        var nonExistentId = InvoiceId.New();
        var command = new AddLineItem
        {
            InvoiceId = nonExistentId,
            ItemId = TestData.TaxableProductId,
            Quantity = 1,
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            SUT.AddLineItemAsync(command));
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldAddPayment_AndUpdateInvoiceStatus()
    {
        // Arrange
        var invoiceId = await CreateTestInvoice();
        var invoice = await SUT.GetInvoiceAsync(invoiceId);

        var command = new ProcessPayment
        {
            InvoiceId = invoiceId,
            Amount = invoice!.Total, // Full payment
            PaymentMethodId = Guid.NewGuid(),
            ReferenceNumber = "CC-001",
            Notes = "Test payment",
            GatewayTransactionId = "TXN-12345"
        };

        // Act
        var result = await SUT.ProcessPaymentAsync(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var updatedInvoice = await SUT.GetInvoiceAsync(invoiceId);
        Assert.NotNull(updatedInvoice);
        Assert.Single(updatedInvoice.Payments);
        Assert.Equal(PaymentStatus.Completed, updatedInvoice.Payments[0].Status);
        Assert.Equal(0m, updatedInvoice.Balance);
        Assert.Equal(InvoiceStatus.Paid, updatedInvoice.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldThrowException_WhenInvoiceNotFound()
    {
        // Arrange
        var command = new ProcessPayment
        {
            InvoiceId = InvoiceId.New(),
            Amount = 100.00m,
            PaymentMethodId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            SUT.ProcessPaymentAsync(command));
    }

    [Fact]
    public async Task ProcessRefundAsync_ShouldCreateRefund_WithProportionalBreakdown()
    {
        // Arrange
        var invoiceId = await CreateTestInvoiceWithPayment();
        var invoice = await SUT.GetInvoiceAsync(invoiceId);

        var command = new ProcessRefund
        {
            InvoiceId = invoiceId,
            Amount = invoice!.Total * 0.5m, // 50% refund
            Reason = "Product defect",
            ReferenceNumber = "REF-001"
        };

        // Act
        var result = await SUT.ProcessRefundAsync(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var updatedInvoice = await SUT.GetInvoiceAsync(invoiceId);
        Assert.NotNull(updatedInvoice);
        Assert.Single(updatedInvoice.Refunds);
        Assert.Equal(InvoiceStatus.PartiallyRefunded, updatedInvoice.Status);
        Assert.True(updatedInvoice.Balance > 0);
    }

    [Fact]
    public async Task ProcessRefundAsync_ShouldThrowException_WhenRefundExceedsPayments()
    {
        // Arrange
        var invoiceId = await CreateTestInvoice(); // No payments
        var invoice = await SUT.GetInvoiceAsync(invoiceId);

        var command = new ProcessRefund
        {
            InvoiceId = invoiceId,
            Amount = 100.00m, // More than paid (which is 0)
            Reason = "Invalid refund"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            SUT.ProcessRefundAsync(command));
    }

    [Fact]
    public async Task CompleteWorkflow_ShouldHandleFullInvoiceLifecycle()
    {
        // Arrange & Act - Create invoice
        var invoiceId = await CreateTestInvoice();
        var invoice = await SUT.GetInvoiceAsync(invoiceId);
        Assert.NotNull(invoice);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);

        // Add additional item
        await SUT.AddLineItemAsync(new AddLineItem
        {
            InvoiceId = invoiceId,
            ItemId = TestData.TaxableServiceId,
            Quantity = 2.0m
        });

        // Make partial payment
        await SUT.ProcessPaymentAsync(new ProcessPayment
        {
            InvoiceId = invoiceId,
            Amount = 150.00m, // Partial payment
            PaymentMethodId = Guid.NewGuid(),
            ReferenceNumber = "BT-001"
        });

        // Make remaining payment
        invoice = await SUT.GetInvoiceAsync(invoiceId);
        var remainingBalance = invoice!.Balance;

        var finalPaymentId = await SUT.ProcessPaymentAsync(new ProcessPayment
        {
            InvoiceId = invoiceId,
            Amount = remainingBalance,
            PaymentMethodId = Guid.NewGuid(),
            ReferenceNumber = "CC-002"
        });

        // Process partial refund
        var refundId = await SUT.ProcessRefundAsync(new ProcessRefund
        {
            InvoiceId = invoiceId,
            Amount = 50.00m,
            Reason = "Customer satisfaction",
            ReferenceNumber = "REF-001"
        });

        // Assert final state
        var finalInvoice = await SUT.GetInvoiceAsync(invoiceId);
        Assert.NotNull(finalInvoice);
        Assert.Equal(2, finalInvoice.LineItems.Count);
        Assert.Equal(2, finalInvoice.Payments.Count);
        Assert.Single(finalInvoice.Refunds);
        Assert.Equal(InvoiceStatus.PartiallyRefunded, finalInvoice.Status);
        Assert.True(finalInvoice.TotalPaid > finalInvoice.TotalRefunded);
        Assert.Equal(50.00m, finalInvoice.Balance); // Should equal refund amount
    }

    /// <summary>
    /// Creates a test invoice with a single taxable item with a fixed price of $100.00.
    /// </summary>
    /// <returns></returns>
    private async Task<InvoiceId> CreateTestInvoice()
    {
        var manager = GetRequiredService<IInvoiceManager>();

        var lineItem = await manager.CreateLineItemAsync(TestData.TaxableProductId, TestDate);

        return await manager.CreateInvoiceAsync(TestData.OntarioCustomerId, [lineItem]);
    }

    private async Task<InvoiceId> CreateTestInvoiceWithPayment()
    {
        var invoiceId = await CreateTestInvoice();
        var invoice = await SUT.GetInvoiceAsync(invoiceId);

        await SUT.ProcessPaymentAsync(new ProcessPayment
        {
            InvoiceId = invoiceId,
            Amount = invoice!.Total,
            PaymentMethodId = Guid.NewGuid(),
            ReferenceNumber = "CC-SETUP"
        });

        return invoiceId;
    }
}
