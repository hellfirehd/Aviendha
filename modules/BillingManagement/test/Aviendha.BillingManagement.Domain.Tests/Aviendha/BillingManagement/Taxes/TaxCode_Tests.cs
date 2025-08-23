using Aviendha.BillingManagement.Items;

namespace Aviendha.BillingManagement.Taxes;

public class TaxCode_Tests
{
    [Fact]
    public void TaxCode_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var taxCode = new TaxCode
        {
            Code = "ZR-GROCERY",
            Description = "Basic Groceries",
            TaxTreatment = TaxTreatment.ZeroRated,
            ItemCategory = ItemCategory.BasicGroceries,
            CraReference = "Schedule VI, Part III",
            Notes = "Basic food items"
        };

        // Assert
        Assert.Equal("ZR-GROCERY", taxCode.Code);
        Assert.Equal("Basic Groceries", taxCode.Description);
        Assert.Equal(TaxTreatment.ZeroRated, taxCode.TaxTreatment);
        Assert.Equal(ItemCategory.BasicGroceries, taxCode.ItemCategory);
        Assert.False(taxCode.IsTaxable); // Zero-rated is not taxable
        Assert.True(taxCode.IsZeroRated);
        Assert.False(taxCode.IsExempt);
        Assert.True(taxCode.IsValidOn(DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [Theory]
    [InlineData(TaxTreatment.Standard, true, false, false)]
    [InlineData(TaxTreatment.ZeroRated, false, true, false)]
    [InlineData(TaxTreatment.Exempt, false, false, true)]
    [InlineData(TaxTreatment.OutOfScope, false, false, false)]
    public void TaxCode_ShouldHaveCorrectTreatmentFlags(TaxTreatment treatment, Boolean expectedTaxable, Boolean expectedZeroRated, Boolean expectedExempt)
    {
        // Arrange
        var taxCode = new TaxCode
        {
            TaxTreatment = treatment
        };

        // Act & Assert
        Assert.Equal(expectedTaxable, taxCode.IsTaxable);
        Assert.Equal(expectedZeroRated, taxCode.IsZeroRated);
        Assert.Equal(expectedExempt, taxCode.IsExempt);
    }

    [Fact]
    public void TaxCode_ShouldValidateDateRanges()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var taxCode = new TaxCode
        {
            EffectiveDate = currentDate,
            ExpirationDate = futureDate
        };

        // Act & Assert
        Assert.False(taxCode.IsValidOn(pastDate)); // Before effective date
        Assert.True(taxCode.IsValidOn(currentDate)); // On effective date
        Assert.True(taxCode.IsValidOn(futureDate)); // On expiration date
        Assert.False(taxCode.IsValidOn(futureDate.AddDays(1))); // After expiration
    }
}
