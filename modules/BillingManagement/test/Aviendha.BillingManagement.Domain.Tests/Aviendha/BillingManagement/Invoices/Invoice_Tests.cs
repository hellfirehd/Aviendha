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

using Aviendha.BillingManagement.Items;
using Aviendha.BillingManagement.Payments;
using Aviendha.BillingManagement.Provinces;
using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Unit tests for the Invoice domain entity
/// </summary>
public abstract class Invoice_Tests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule>, IAsyncLifetime
    where TStartupModule : IAbpModule
{
    private static readonly DateOnly TestDate = new(2025, 01, 01);
    protected Province Ontario { get; set; } = default!;

    protected InvoiceId InvoiceId { get; set; }
    protected Invoice Invoice { get; set; } = default!;

    public async Task InitializeAsync()
    {
        Ontario = await ProvinceRepository.GetAsync("ON");
        InvoiceId = InvoiceId.FromGuid(GuidGenerator.Create());
        Invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void AddItem_ShouldAddItemToInvoice()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        var item = Product.Create(ItemId.New(), "SKU", "NAME", "STRING", ItemCategory.GeneralGoods);
        item.AddPrice(100.00m, TestDate);

        var itemSnapshot = new ItemSnapshot
        {
            ItemId = item.Id,
            SKU = item.SKU,
            Name = item.Name,
            Description = item.Description,
            UnitPrice = item.GetUnitPrice(TestDate),
            UnitType = item.UnitType,
            ItemType = item.ItemType,
            ItemCategory = item.ItemCategory,
            TaxCode = item.TaxCode
        };

        var invoiceItem = new LineItem(itemSnapshot, quantity: 1.0m, sortOrder: 0);

        // Act
        invoice.AddLineItem(invoiceItem);

        // Assert
        Assert.Single(invoice.LineItems);
        Assert.Equal(100.00m, invoice.GetSubtotal());
    }

    [Fact]
    public async Task CancelInvoice_Allowed_WhenNoPayments()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        // Act
        invoice.CancelInvoice();

        // Assert
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
    }

    [Fact]
    public async Task CancelInvoice_ShouldThrowException_WhenPaymentsExist()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var payment = new Payment(50.00m, TestDate, String.Empty);
        payment.MarkAsCompleted();
        invoice.ProcessPayment(payment);

        // Act & Assert
        var exception = Assert.Throws<BillingManagementException>(() => invoice.CancelInvoice());
        exception.ErrorCode.ShouldBe(ErrorCodes.CannotCancelInvoice);
    }

    [Fact]
    public async Task Invoice_ComplexScenario()
    {
        // Arrange - Complex scenario with multiple items, taxes, discounts, and surcharges
        var customer = await CustomerRepository.GetAsync(TestData.OntarioCustomerId);
        var invoice = Invoice.Create(InvoiceId, customer, Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate)); // $100 taxable product
        invoice.AddLineItem(await NonTaxableProductAsync(TestDate)); // $50 non-taxable product
        var taxableService = await TaxableServiceAsync(TestDate);
        taxableService.AddDiscount(new Discount
        {
            Name = "Service Discount",
            Scope = DiscountScope.PerLineItem,
            Percentage = 0.10m // 10% off
        });
        invoice.AddLineItem(taxableService); // $500 taxable service
        invoice.AddLineItem(await NonTaxableServiceAsync(TestDate)); // $75 non-taxable service

        // Apply order discount for large orders
        var orderDiscount = new Discount
        {
            Name = "Large Order Discount",
            Scope = DiscountScope.PerOrder,
            Percentage = 0.15m // 15% off
        };
        invoice.AddDiscount(orderDiscount);

        // Add credit card surcharge
        var surcharge = new Surcharge
        {
            Name = "Credit Card Processing",
            FixedAmount = 0.30m,
            PercentageRate = 0.029m
        };
        invoice.AddSurcharge(surcharge);

        // Apply taxes
        await invoice.ApplyTaxesAsync(TaxProvider);

        // Act
        var subtotal = invoice.GetSubtotal();
        var taxTotal = invoice.GetTaxAmount();
        var lineItemDiscount = invoice.GetLineItemDiscount();
        var orderDiscountTotal = invoice.GetOrderDiscount();
        var surchargeTotal = invoice.GetSurchargeAmount();
        var finalTotal = invoice.GetGrandTotal();

        // Assert
        // Verify calculations are reasonable
        subtotal.ShouldBeGreaterThan(Decimal.Zero);
        taxTotal.ShouldBeGreaterThan(Decimal.Zero);
        lineItemDiscount.ShouldBe(50.00m); // One line item discount applied
        orderDiscountTotal.ShouldBeGreaterThan(Decimal.Zero);
        surchargeTotal.ShouldBeGreaterThan(Decimal.Zero);
        finalTotal.ShouldBeGreaterThan(Decimal.Zero);

        // Verify the formula: Total = Subtotal - LineItemDiscounts - OrderDiscounts + Surcharges + Shipping + Taxes
        var expectedTotal = subtotal - lineItemDiscount - orderDiscountTotal + surchargeTotal + invoice.GetShippingAmount() + taxTotal;
        Assert.Equal(expectedTotal, finalTotal);
    }

    [Fact]
    public async Task Invoice_AddDiscount_ShouldApplyDiscountToTotal()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate)); // $100.00

        var discount = new Discount
        {
            Name = "Order Discount",
            Scope = DiscountScope.PerOrder,
            FixedAmount = 50.00m
        };

        // Act
        invoice.AddDiscount(discount);

        // Assert
        Assert.Single(invoice.Discounts);
        Assert.Equal(100, invoice.GetSubtotal());
        Assert.Equal(50.00m, invoice.GetOrderDiscount());
        Assert.Equal(50.00m, invoice.GetTotalAmount());
    }

    [Fact]
    public async Task Invoice_AddItem_ShouldAddItemAndUpdateSubtotal()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        var product = await TaxableProductAsync(TestDate);

        // Act
        invoice.AddLineItem(product);

        // Assert
        Assert.Single(invoice.LineItems);
        Assert.Equal(product, invoice.LineItems.Single());
        Assert.Equal(100.00m, invoice.GetSubtotal());
    }

    [Fact]
    public async Task Invoice_AddOrderDiscount_ShouldApplyDiscountToTotal()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var orderDiscount = new Discount
        {
            Name = "Order Discount",
            Scope = DiscountScope.PerOrder,
            FixedAmount = 50.00m
        };

        // Act
        invoice.AddDiscount(orderDiscount);

        // Assert
        Assert.Single(invoice.Discounts);
        Assert.Equal(50.00m, invoice.GetOrderDiscount());
    }

    [Fact]
    public async Task Invoice_AddPayment_ShouldUpdateStatusAndBalance()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var payment = new Payment(100.00m, TestDate, String.Empty)
        {
            Status = PaymentStatus.Completed
        };
        payment.MarkAsCompleted();

        // Act
        invoice.ProcessPayment(payment);

        // Assert
        Assert.Single(invoice.Payments);
        Assert.Equal(100.00m, invoice.GetTotalPaid());
        Assert.Equal(0m, invoice.GetBalance());
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public async Task Invoice_AddSurcharge_ShouldApplySurchargeToTotal()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var surcharge = new Surcharge
        {
            Name = "Credit Card Fee",
            FixedAmount = 0.30m, // $0.30 flat fee
            PercentageRate = 0.029m // 2.9%
        };

        // Act
        invoice.AddSurcharge(surcharge);

        // Assert
        Assert.Single(invoice.Surcharges);
        // Surcharge = 0.30 + (100 * 0.029) = 0.30 + 2.90 = 3.20
        Assert.Equal(3.20m, invoice.GetSurchargeAmount());
    }

    [Fact]
    public async Task Invoice_ProcessRefund_ShouldThrowException_WhenRefundExceedsNetPayments()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var payment = new Payment(50.00m, TestDate, String.Empty);
        payment.MarkAsCompleted();
        invoice.ProcessPayment(payment);

        var refund = new Refund(Guid.NewGuid(), 75.00m, "Invalid refund");

        // Act & Assert
        var exception = Assert.Throws<BillingManagementException>(() => invoice.ProcessRefund(refund));

        exception.ErrorCode.ShouldBe(ErrorCodes.InvalidRefundAmount);
    }

    [Fact]
    public async Task Invoice_ProcessRefund_ShouldUpdateStatusAndBalance()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        var payment = new Payment(100.00m, TestDate, String.Empty)
        {
            Status = PaymentStatus.Completed
        };
        payment.MarkAsCompleted();
        invoice.ProcessPayment(payment);

        var refund = new Refund(Guid.NewGuid(), 25.00m, "Partial return");

        // Act
        invoice.ProcessRefund(refund);

        // Assert
        Assert.Single(invoice.Refunds);
        Assert.Equal(25.00m, invoice.GetTotalRefunded());
        Assert.Equal(25.00m, invoice.GetBalance()); // 100 total - 100 paid + 25 refunded = 25
        Assert.Equal(InvoiceStatus.PartiallyRefunded, invoice.Status);
    }

    [Fact]
    public async Task Invoice_RemoveItem_ShouldRemoveItemAndUpdateSubtotal()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        var product = await TaxableProductAsync(TestDate);
        invoice.AddLineItem(product);

        // Act - Remove by ItemSnapshot.ItemId instead of LineItem.Id
        invoice.RemoveLineItem(product.Item.ItemId);

        // Assert
        Assert.Empty(invoice.LineItems);
        Assert.Equal(0m, invoice.GetSubtotal());
    }

    [Fact]
    public async Task Invoice_ShouldCalculateCorrectTotalTax_ForMixedItems()
    {
        // Arrange
        var customer = await CustomerRepository.GetAsync(TestData.BritishColumbiaCustomerId);
        var province = await ProvinceRepository.GetAsync("BC");
        var invoice = Invoice.Create(InvoiceId, customer, province, TestDate);

        var taxableProduct = await TaxableProductAsync(TestDate);
        invoice.AddLineItem(taxableProduct);

        var nonTaxableProduct = await NonTaxableProductAsync(TestDate);
        invoice.AddLineItem(nonTaxableProduct);

        // Act
        await invoice.ApplyTaxesAsync(TaxProvider);

        // Assert
        // Only the taxable product should have tax: $100 * 12% = $12
        Assert.Equal(12.00m, invoice.GetTaxAmount());
        Assert.Equal(2, taxableProduct.Taxes.Count); // GST + PST
        Assert.Equal(162.00m, invoice.GetGrandTotal()); // $150 subtotal + $12 tax
    }

    [Fact]
    public void PostInvoice_ShouldThrowException_WhenThereAreNoLineItems()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);

        // Act
        var exception = Assert.Throws<BillingManagementException>(invoice.PostInvoice);

        exception.ErrorCode.ShouldBe(ErrorCodes.CannotPostInvoice);
    }

    [Fact]
    public async Task PostInvoice_ShouldUpdateStatus_WhenInDraftOrPending()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        // Act
        invoice.PostInvoice();

        // Assert
        Assert.Equal(InvoiceStatus.Posted, invoice.Status);
    }

    [Fact]
    public async Task SetTaxRates_ShouldApplyCorrectTaxes_ByCategory()
    {
        // Arrange
        var province = await ProvinceRepository.GetAsync("BC");
        var customer = await CustomerRepository.GetAsync(TestData.BritishColumbiaCustomerId);
        var invoice = Invoice.Create(InvoiceId, customer, province, TestDate);

        var taxableProduct = await TaxableProductAsync(TestDate); // $100 taxable product
        invoice.AddLineItem(taxableProduct);

        var taxableService = await TaxableServiceAsync(TestDate); // $500 taxable service
        invoice.AddLineItem(taxableService);

        var nonTaxableProduct = await NonTaxableProductAsync(TestDate); // $50 non-taxable product
        invoice.AddLineItem(nonTaxableProduct);

        // Act
        await invoice.ApplyTaxesAsync(TaxProvider);

        // Assert
        invoice.GetSubtotal().ShouldBe(650.00m); // $100 + $500 + $50 = $650
        Assert.Equal(72.00m, invoice.GetTaxAmount()); // ($100 + $500) * 12% = $72
        Assert.Equal(2, taxableProduct.Taxes.Count); // GST + PST
        Assert.Equal(2, taxableService.Taxes.Count); // GST + PST
        nonTaxableProduct.Taxes.ShouldAllBe(t => t.Rate == 0m); // No taxes
    }

    [Fact]
    public async Task TaxCalculation_ShouldBeAccurate_ForComplexScenario()
    {
        // Arrange
        var customer = await CustomerRepository.GetAsync(TestData.OntarioCustomerId);
        var invoice = Invoice.Create(InvoiceId, customer, Ontario, TestDate);

        var product = await TaxableProductAsync(TestDate); // $100.00 

        var service = await TaxableServiceAsync(TestDate); // $500.00

        // Add line item discount
        var itemDiscount = new Discount
        {
            Name = "Volume Discount",
            Scope = DiscountScope.PerLineItem,
            Percentage = 0.10m // 10% off
        };

        product.AddDiscount(itemDiscount);

        invoice.AddLineItem(product);
        invoice.AddLineItem(service);

        // Add order discount
        var orderDiscount = new Discount
        {
            Name = "Early Bird Discount",
            Scope = DiscountScope.PerOrder,
            FixedAmount = 25.00m
        };
        invoice.AddDiscount(orderDiscount);

        // Add surcharge
        var surcharge = new Surcharge
        {
            Name = "Payment Processing",
            FixedAmount = 2.00m,
            PercentageRate = 0.029m // 2.9%
        };
        invoice.AddSurcharge(surcharge);

        // Apply taxes
        await invoice.ApplyTaxesAsync(TaxProvider);

        // Act & Assert
        var subtotal = invoice.GetSubtotal(); // $100.00 + $500.00 = $600.00
        var itemDiscounts = invoice.GetLineItemDiscount(); // $100 * 10% = $10.00
        var orderDiscounts = invoice.GetOrderDiscount(); // $25.00
        var totalAfterDiscounts = invoice.GetTotalAmount(); // $600 - $35.00 = $565.00
        var totalTax = invoice.GetTaxAmount(); // Should be calculated on discounted amounts
        var totalSurcharges = invoice.GetSurchargeAmount(); // $2.00 + ($600 * 2.9%) = $19.40
        var finalTotal = invoice.GetGrandTotal();

        // Verify the calculation chain
        subtotal.ShouldBe(600.00m);
        itemDiscounts.ShouldBe(10.00m);
        orderDiscounts.ShouldBe(25.00m);
        totalTax.ShouldBeGreaterThan(0.00m);
        totalSurcharges.ShouldBeGreaterThan(0.00m);
        finalTotal.ShouldBeGreaterThan(totalAfterDiscounts);
    }

    [Fact]
    public async Task UpdateStatus_ShouldSetOverdue_WhenPastDueDate()
    {
        // Arrange
        var invoice = Invoice.Create(InvoiceId, TestData.Customer(Ontario), Ontario, TestDate);
        invoice.AddLineItem(await TaxableProductAsync(TestDate));

        // Act
        invoice.UpdateStatus(TestDate.AddDays(60));

        // Assert
        Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
    }

    //[Fact]
    //public void LargeInvoice_ShouldCalculateEfficiently()
    //{
    //    // Test with 100+ line items
    //    throw new NotImplementedException();
    //}

    //[Theory]
    //[InlineData(-1, "Amount cannot be negative")]
    //[InlineData(0, "Amount must be greater than zero")]
    //public void Payment_ShouldValidateAmount(Decimal amount, String expectedError)
    //{
    //    // Input validation tests
    //    throw new NotImplementedException();
    //}
}
