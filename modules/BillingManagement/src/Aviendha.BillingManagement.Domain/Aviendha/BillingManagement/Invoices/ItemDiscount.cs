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

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Represents a <see cref="Discount"/> that has been applied to an <see cref="Invoice"/> or <see cref="LineItem"/>.
/// </summary>
public record ItemDiscount(Guid Id, String Name, DiscountScope DiscountScope, Decimal Percentage, Decimal FixedAmount)
{
    public override String ToString() => $"{Name} @ {Percentage:P} + {FixedAmount:C}";

    /// <summary>
    /// Calculates the discount on the <paramref name="eligibleAmount"/>
    /// </summary>
    public Decimal GetAmount(LineItem lineItem)
    {
        var accumulatedDiscount = 0m;

        accumulatedDiscount += lineItem.UnitPrice * Percentage * lineItem.Quantity;

        if (DiscountScope is DiscountScope.PerUnit)
        {
            accumulatedDiscount += FixedAmount * lineItem.Quantity;
        }
        else
        {
            accumulatedDiscount += FixedAmount;
        }

        return Math.Round(accumulatedDiscount, 2);
    }
}
