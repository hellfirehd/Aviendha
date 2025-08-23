using Aviendha.BillingManagement.Invoices;

namespace Aviendha.BillingManagement;

public class InvoiceFilter
{
    public InvoiceStatus? Status { get; set; }
    public String? CustomerName { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Decimal? MinAmount { get; set; }
    public Decimal? MaxAmount { get; set; }
}
