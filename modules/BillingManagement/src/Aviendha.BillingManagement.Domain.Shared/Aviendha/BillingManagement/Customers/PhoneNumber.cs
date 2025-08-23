namespace Aviendha.BillingManagement.Customers;

public record PhoneNumber(String Number) // Value Object
{
    public static readonly PhoneNumber Empty = new(String.Empty);

    public override String ToString() => Number;
}
