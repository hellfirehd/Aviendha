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

using Aviendha.BillingManagement.Taxes;
using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Invoices.LineItems;

public abstract class LineItem_Tests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private static readonly DateOnly InvoiceDate = new(2025, 01, 01);

    [Fact]
    public async Task LineItem_GetTotal_MustIncludePerUnitDiscount()
    {
        // Arrange
        var lineItem = await NonTaxableProductAsync(InvoiceDate, quantity: 2); // 2 items at $50.00 each = $100.00
        lineItem.AddDiscount(new Discount() { Scope = DiscountScope.PerUnit, FixedAmount = 5.00m }); // $5.00 per unit discount = $10.00 total discount

        // Act
        var total = lineItem.GetTotal();

        // Assert
        Assert.Equal(90.00m, total);
    }

    [Fact]
    public async Task LineItem_GetTotal_MustIncludePerLineItemDiscount()
    {
        // Arrange
        var lineItem = await NonTaxableProductAsync(InvoiceDate, quantity: 2); // 2 items at $50.00 each = $100.00
        lineItem.AddDiscount(new Discount() { Scope = DiscountScope.PerLineItem, Percentage = 0.10m });

        // Act
        var total = lineItem.GetTotal();

        // Assert
        Assert.Equal(90.00m, total);
    }

    [Fact]
    public async Task LineItem_ShouldCalculateTotal_WithMultipleTaxes()
    {
        // Arrange
        var date = new DateOnly(2025, 01, 01);
        var lineItem = (await NonTaxableProductAsync(date)).SetQuantity(2);

        var gst = new Tax(TaxId.New(), "GST", "GST", isGstHst: true)
            .AddTaxRate(0.05M, new DateOnly(2008, 1, 1), DateOnly.MaxValue); // 5% GST

        var pst = new Tax(TaxId.New(), "PST", "PST", isGstHst: false)
            .AddTaxRate(0.07M, new DateOnly(2013, 4, 1), DateOnly.MaxValue); // 7% PST

        lineItem.ApplyTaxes([gst.GetTaxRate(date)!, pst.GetTaxRate(date)!]);

        // Act
        var total = lineItem.GetTotal();

        // Assert
        Assert.Equal(112.00m, total); // (2 * 50) + (2 * 50 * 0.05) + (2 * 50 * 0.07) == 
    }

    [Fact]
    public async Task LineItem_ShouldCalculateTotal_WithoutApplyingTaxes()
    {
        // Arrange
        var lineItem = (await NonTaxableProductAsync(InvoiceDate)).SetQuantity(2);

        // Act
        var total = lineItem.GetTotal();

        // Assert
        Assert.Equal(100.00m, total); // 2 * 50
    }

    [Fact]
    public async Task LineItem_ShouldCalculateTotal_WithTax()
    {
        // Arrange
        var gst = new Tax(TaxId.New(), "GST", "GST", isGstHst: true)
            .AddTaxRate(0.05M, new DateOnly(2008, 1, 1), DateOnly.MaxValue); // 5% GST

        var lineItem = (await TaxableProductAsync(InvoiceDate)).SetQuantity(2); // @ $100.00 ea
        lineItem.ApplyTaxes([gst.GetTaxRate(InvoiceDate)!]);

        // Act
        var total = lineItem.GetTotal();

        // Assert
        Assert.Equal(210.00m, total); // (2 * 100) + (2 * 100 * 0.05)
    }
}
