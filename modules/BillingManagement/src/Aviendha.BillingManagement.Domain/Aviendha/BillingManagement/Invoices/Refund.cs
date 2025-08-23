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

using Volo.Abp.Domain.Entities.Auditing;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Represents a refund processed against payments
/// </summary>
public class Refund : FullAuditedEntity<Guid>
{
    protected Refund()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public Refund(Guid paymentId, Decimal amount, String reason)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException("Payment ID cannot be empty", nameof(paymentId));
        }

        if (amount <= Decimal.Zero)
        {
            throw new ArgumentException("Refund amount must be positive", nameof(amount));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }

    public virtual Decimal Amount { get; protected set; }
    public virtual DateOnly RefundDate { get; protected set; }
    public virtual String Reason { get; protected set; } = String.Empty;
    public virtual String ReferenceNumber { get; protected set; } = String.Empty;
    public virtual String Notes { get; protected set; } = String.Empty;

    // Link to original payment
    public virtual Guid PaymentId { get; protected set; }

    // Payment gateway reference details
    public virtual String GatewayName { get; protected set; } = String.Empty;
    public virtual String GatewayTransactionId { get; protected set; } = String.Empty;

    // Note: Currently, surcharges are not refundable

    // Breakdown of what was refunded (proportional tax/discount adjustment)
    public virtual Decimal SubtotalRefund { get; protected set; }
    public virtual Decimal TaxRefund { get; protected set; }
    public virtual Boolean IsShippingRefunded { get; protected set; }
    public virtual Decimal ShippingRefund { get; protected set; }

    internal void SetAmounts(Decimal subtotalRefund, Decimal taxRefund, Decimal shippingRefund)
    {
        SubtotalRefund = subtotalRefund;
        TaxRefund = taxRefund;
        ShippingRefund = shippingRefund;
    }
}
