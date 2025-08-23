namespace Aviendha.BillingManagement.Taxes;

public enum TaxJurisdiction
{
    None = 0,
    Federal = 1, // GST
    Provincial = 2, // PST, QST, RST
    Harmonized = 3, // HST
}
