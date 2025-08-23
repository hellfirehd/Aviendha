namespace Aviendha.BillingManagement;

public class RefundResponse
{
    public Guid Id { get; set; }
    public Decimal Amount { get; set; }
    public DateOnly RefundDate { get; set; }
    public String Reason { get; set; } = String.Empty;
    public String ReferenceNumber { get; set; } = String.Empty;
    public String? OriginalPaymentId { get; set; }
}
