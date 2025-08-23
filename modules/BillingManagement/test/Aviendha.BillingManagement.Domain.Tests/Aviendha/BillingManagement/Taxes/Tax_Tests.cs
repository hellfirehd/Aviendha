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

namespace Aviendha.BillingManagement.Taxes;

public class Tax_Tests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var tax = new Tax(TaxId.New(), "Goods and Services Tax", "GST", isGstHst: true);

        // Assert
        Assert.Equal("GST", tax.Code);
        Assert.Equal("Goods and Services Tax", tax.Name);
        Assert.Empty(tax.Rates);
    }

    [Fact]
    public void AddTaxRate_AddsRateToCollection()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        var effectiveDate = new DateOnly(2020, 1, 1);
        var expirationDate = new DateOnly(2020, 12, 31);

        // Act
        tax.AddTaxRate(0.1m, effectiveDate, expirationDate);

        // Assert
        Assert.Single(tax.Rates);
        var addedRate = tax.Rates.First();
        Assert.Equal(0.1m, addedRate.Rate);
        Assert.Equal(effectiveDate, addedRate.EffectiveDate);
        Assert.Equal(expirationDate, addedRate.ExpiryDate);
    }

    [Fact]
    public void AddTaxRate_ReturnsSameInstance_ForChaining()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);

        // Act
        var result = tax.AddTaxRate(0.1m, new DateOnly(2020, 1, 1));

        // Assert
        Assert.Same(tax, result);
    }

    [Fact]
    public void GetTaxRate_ReturnsNull_WhenNoRatesExist()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        var date = new DateOnly(2020, 6, 1);

        // Act
        var result = tax.GetTaxRate(date);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTaxRate_ReturnsRate_WhenDateIsWithinRange()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        var effectiveDate = new DateOnly(2020, 1, 1);
        var expirationDate = new DateOnly(2020, 12, 31);
        tax.AddTaxRate(0.1m, effectiveDate, expirationDate);
        var date = new DateOnly(2020, 6, 1);

        // Act
        var result = tax.GetTaxRate(date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.1m, result.Rate);
    }

    [Fact]
    public void GetTaxRate_ReturnsNull_WhenDateIsOutsideRange()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        tax.AddTaxRate(0.1m, new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31));
        var date = new DateOnly(2021, 6, 1);

        // Act
        var result = tax.GetTaxRate(date);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTaxRate_ReturnsMostRecentRate_WhenMultipleRatesMatch()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        tax.AddTaxRate(0.1m, new DateOnly(2019, 1, 1), new DateOnly(2022, 12, 31));
        tax.AddTaxRate(0.15m, new DateOnly(2020, 1, 1), new DateOnly(2021, 12, 31));
        var date = new DateOnly(2020, 6, 1);

        // Act
        var result = tax.GetTaxRate(date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.15m, result.Rate);
    }

    [Fact]
    public void GetTaxRate_HandlesNullExpirationDate()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);
        tax.AddTaxRate(0.1m, new DateOnly(2020, 1, 1));
        var date = new DateOnly(2023, 6, 1);

        // Act
        var result = tax.GetTaxRate(date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.1m, result.Rate);
    }

    [Fact]
    public void Name_CanBeModified()
    {
        // Arrange
        var tax = new Tax(TaxId.New(), "GST", "GST", isGstHst: true);

        // Act
        tax.Rename("Value Added Tax", "VAT");

        // Assert
        tax.Name.ShouldBe("Value Added Tax");
        tax.Code.ShouldBe("VAT");
    }
}
