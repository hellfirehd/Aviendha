// Aviendha Billing Management
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

using System.Diagnostics;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Represents a discount that can be applied to items or orders
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class Discount : FullAuditedEntity<Guid>
{
    public String Name { get; set; } = String.Empty;
    public String Description { get; set; } = String.Empty;
    public required DiscountScope Scope { get; set; }
    public Decimal Percentage { get; set; }
    public Decimal FixedAmount { get; set; }
    public Decimal? MinimumAmount { get; set; } // Minimum order amount for discount to apply
    public Decimal? MaximumDiscount { get; set; } // Maximum discount amount
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public override String ToString() => $"{Name} @ {Percentage:P} + {FixedAmount:C}";
}
