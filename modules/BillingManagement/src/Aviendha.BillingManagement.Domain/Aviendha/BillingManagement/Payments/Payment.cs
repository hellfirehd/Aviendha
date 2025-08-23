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

namespace Aviendha.BillingManagement.Payments;

/// <summary>
/// Represents a payment made against an invoice
/// </summary>
public class Payment : FullAuditedEntity<Guid>
{
    protected Payment()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public Payment(Decimal amount, DateOnly paymentDate, String referenceNumber)
    {
        Amount = amount;
        PaymentDate = paymentDate;
        ReferenceNumber = referenceNumber ?? throw new ArgumentNullException(nameof(referenceNumber));
    }

    public virtual Decimal Amount { get; set; }
    public virtual DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public virtual PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public virtual String ReferenceNumber { get; set; } = String.Empty;
    public virtual String Notes { get; set; } = String.Empty;

    // For tracking gateway information
    public virtual String GatewayName { get; set; } = String.Empty;
    public virtual String GatewayTransactionId { get; set; } = String.Empty;

    /// <summary>
    /// Marks the payment as completed
    /// </summary>
    public virtual void MarkAsCompleted(String? transactionId = null)
    {
        Status = PaymentStatus.Completed;
        if (!String.IsNullOrEmpty(transactionId))
        {
            GatewayTransactionId = transactionId;
        }
    }

    /// <summary>
    /// Marks the payment as failed
    /// </summary>
    public virtual void MarkAsFailed(String reason = "")
    {
        Status = PaymentStatus.Failed;
        if (!String.IsNullOrEmpty(reason))
        {
            Notes = reason;
        }
    }

    /// <summary>
    /// Cancels the payment
    /// </summary>
    public virtual void Cancel(String reason = "")
    {
        if (Status == PaymentStatus.Pending)
        {
            Status = PaymentStatus.Cancelled;
            if (!String.IsNullOrEmpty(reason))
            {
                Notes = reason;
            }
        }
        else
        {
            throw new InvalidOperationException($"Cannot cancel payment with status: {Status}");
        }
    }
}
