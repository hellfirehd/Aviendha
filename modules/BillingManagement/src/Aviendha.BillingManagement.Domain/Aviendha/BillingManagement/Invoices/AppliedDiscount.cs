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

public record AppliedDiscount(Guid Id, String Name, Decimal FixedAmount, Decimal Percentage)
{
    /// <summary>
    /// Calculates the discount on the <paramref name="eligibleAmount"/>
    /// </summary>
    public Decimal GetAmount(Invoice invoice)
    {
        var eligibleAmount = invoice.GetSubtotal() - invoice.GetLineItemDiscount();

        var accumulatedDiscount = 0m;

        if (Percentage > Decimal.Zero)
        {
            accumulatedDiscount += eligibleAmount * Percentage;
        }

        if (FixedAmount > Decimal.Zero)
        {
            accumulatedDiscount += FixedAmount;
        }

        return Math.Round(accumulatedDiscount, 2);
    }
}
