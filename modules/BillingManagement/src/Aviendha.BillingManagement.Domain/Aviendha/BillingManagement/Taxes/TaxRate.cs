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

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Represents a tax rate for a specific date range, 
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class TaxRate
{
    private TaxRate()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public TaxRate(String code, Decimal rate, DateOnly effectiveDate, DateOnly? expiryDate = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Code = code;

        Rate = rate < 0
        ? throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be non-negative.")
        : rate;

        EffectiveDate = effectiveDate < DateOnly.FromDateTime(DateTime.UnixEpoch)
            ? throw new ArgumentOutOfRangeException(nameof(effectiveDate), "Effective Date is too far in the past.")
            : effectiveDate;

        ExpiryDate = expiryDate;
    }

    public String Code { get; private set; } = String.Empty;
    public Decimal Rate { get; private set; } // Allow init to facilitate Zero-Rated tax rates.
    public Boolean IsActive { get; private set; } = true;
    public DateOnly EffectiveDate { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }

    public Boolean IsInEffectOn(DateOnly date)
        => EffectiveDate <= date
        && (!ExpiryDate.HasValue || date <= ExpiryDate.Value);

    public override String ToString()
        => $"Tax: {Code}, Rate: {Rate}, Active: {IsActive}, Effective: {EffectiveDate}, Expiration: {ExpiryDate?.ToString(CultureInfo.InvariantCulture) ?? "N/A"}";

    public static TaxRate Create(String code, Decimal rate, DateOnly effectiveDate, DateOnly? expirationDate = null)
        => new(code, rate, effectiveDate, expirationDate);
}
