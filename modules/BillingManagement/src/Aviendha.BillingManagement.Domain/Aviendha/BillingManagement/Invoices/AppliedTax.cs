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
using Volo.Abp.Domain.Values;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Represents a calculated tax amount for an invoice item
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class AppliedTax : ValueObject
{
    public String Name { get; set; } = String.Empty;
    public Decimal Rate { get; set; }
    public Decimal GetAmount(Decimal taxableAmount) => Math.Round(taxableAmount * Rate, 2);

    public override String ToString() => $"{Name} @ {Rate:P}";

    protected override IEnumerable<Object> GetAtomicValues()
    {
        yield return Name;
        yield return Rate;
    }
}
