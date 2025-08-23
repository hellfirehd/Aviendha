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

using Volo.Abp.Domain.Values;

namespace Aviendha.BillingManagement.Items;

/// <summary>
/// Represents the per unit price of an item and the date range during which the price is in effect.
/// </summary>
/// <remarks>
/// This record is used to define the pricing details for an item. It must have a start date of when
/// the price goes into effect and an optional expiration date of when the price is no longer in effect.
/// The <see cref="InEffect(DateOnly)"/> method can be used to determine whether the price is valid for
/// a specific date.
/// </remarks>
/// <param name="UnitPrice"></param>
/// <param name="EffectiveDate"></param>
/// <param name="ExpirationDate"></param>
//public record ItemUnitPrice(Decimal UnitPrice, DateOnly EffectiveDate, DateOnly? ExpirationDate = null) : ValueObject<ItemUnitPrice>
//{
//    public Boolean InEffect(DateOnly date)
//        => EffectiveDate <= date
//        && (!ExpirationDate.HasValue || date <= ExpirationDate.Value);
//}

public class ItemUnitPrice : ValueObject
{
    protected ItemUnitPrice()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public Decimal UnitPrice { get; }
    public DateOnly EffectiveDate { get; }
    public DateOnly? ExpirationDate { get; }

    public ItemUnitPrice(Decimal unitPrice, DateOnly effectiveDate, DateOnly? expirationDate = null)
    {
        UnitPrice = unitPrice;
        EffectiveDate = effectiveDate;
        ExpirationDate = expirationDate;
    }

    public Boolean InEffect(DateOnly date)
        => EffectiveDate <= date
        && (!ExpirationDate.HasValue || date <= ExpirationDate.Value);
    protected override IEnumerable<Object> GetAtomicValues()
    {
        yield return UnitPrice;
        yield return EffectiveDate;
        yield return ExpirationDate!;
    }
}
