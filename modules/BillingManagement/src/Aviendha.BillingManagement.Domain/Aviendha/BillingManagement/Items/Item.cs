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

namespace Aviendha.BillingManagement.Items;

/// <summary>
/// Represents an item with properties for identification, classification, pricing, and tax information.
/// </summary>
/// <remarks>
/// This class serves as a base type for items in an inventory or catalog system. It includes essential
/// properties  such as ID, name, description, classification, tax code, unit price, and unit type. The <see
/// cref="IsActive"/>  property indicates whether the item is currently active or available.
/// </remarks>
public abstract class Item : FullAuditedEntity<ItemId>
{
    private readonly List<ItemUnitPrice> _pricing = [];

    protected Item()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public Item(
        ItemId id,
        String code,
        String name,
        String description,
        ItemType itemType,
        ItemCategory itemCategory,
        String taxCode)
    {
        Id = id == ItemId.Empty
            ? throw new ArgumentNullException(nameof(id), "Id cannot be empty.")
            : id;
        SKU = code;
        Name = name
            ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");

        Description = description
            ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");

        ItemCategory = itemCategory;

        TaxCode = taxCode
            ?? throw new ArgumentNullException(nameof(taxCode), "Tax Code cannot be null.");

        UnitType = "Each";

        IsActive = true;
    }

    public virtual String SKU { get; init; } = String.Empty;
    public virtual String Name { get; init; } = String.Empty;
    public virtual String Description { get; init; } = String.Empty;
    public virtual String UnitType { get; init; } = "Each";
    public virtual ItemType ItemType { get; init; }
    public virtual ItemCategory ItemCategory { get; init; } = ItemCategory.GeneralGoods;
    public virtual String TaxCode { get; init; } = "STD-GOODS";
    public virtual Boolean IsActive { get; init; } = true;
    public virtual IReadOnlyCollection<ItemUnitPrice> Pricing => _pricing.AsReadOnly();

    public void AddPrice(Decimal unitPrice, DateOnly effectiveDate, DateOnly? expirationDate = null)
    {
        if (unitPrice < Decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice),
                "Unit Price cannot be less than $0.00. If you need a negative amount, "
                + "apply a discount to the Invoice Line Item.");
        }

        AddPrice(new ItemUnitPrice(unitPrice, effectiveDate, expirationDate));
    }

    public void AddPrice(ItemUnitPrice itemPrice)
    {
        if (itemPrice.UnitPrice < Decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(itemPrice),
                "THe Unit Price of the item must not be less than $0.00. "
                + "If you need a negative amount, apply a discount to the "
                + "Invoice Line Item.");
        }

        _pricing.Add(itemPrice);
    }

    public Boolean IsAvailableOn(DateOnly date)
    {
        if (_pricing is null || _pricing.Count == 0)
        {
            return false; // No pricing information available
        }

        return _pricing.Any(p => p.InEffect(date));
    }

    public Decimal GetUnitPrice(DateOnly effectiveDate)
    {
        if (!IsAvailableOn(effectiveDate))
        {
            throw new InvalidOperationException("This item has not been priced.");
        }

        // Find the most recent price that is effective on or before the given date
        return _pricing
            .Where(p => p.InEffect(effectiveDate))
            .OrderByDescending(p => p.EffectiveDate)
            .Select(p => p.UnitPrice)
            .FirstOrDefault();
    }

    public Boolean IsValid()
    {
        return !String.IsNullOrWhiteSpace(SKU)
            && !String.IsNullOrWhiteSpace(Name)
            && Description != null
            && ItemType != ItemType.None
            && ItemCategory != ItemCategory.None
            && TaxCode != null
            && _pricing.Count > 0;
    }
}
