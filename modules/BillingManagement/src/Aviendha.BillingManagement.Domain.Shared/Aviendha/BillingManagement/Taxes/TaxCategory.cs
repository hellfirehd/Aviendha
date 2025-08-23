using System.ComponentModel;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Legacy tax category enumeration - maintained for backward compatibility
/// Use TaxTreatment and ItemCategory for new implementations
/// </summary>
[Obsolete("Use TaxTreatment and ItemCategory for new implementations. This enum is maintained for backward compatibility only.")]
public enum TaxCategory
{
    [Description("Taxable Product")]
    TaxableProduct,

    [Description("Non-Taxable Product")]
    NonTaxableProduct,

    [Description("Taxable Service")]
    TaxableService,

    [Description("Non-Taxable Service")]
    NonTaxableService,

    [Description("Digital Product")]
    DigitalProduct,

    [Description("Shipping")]
    Shipping
}
