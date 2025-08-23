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

using Aviendha.BillingManagement.Taxes;
using Aviendha.Domain.Values;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Value object representing an invoice line item (products and services)
/// </summary>
public record class LineItem : ValueObject<LineItem>
{
    private readonly List<AppliedTax> _taxes = [];
    private readonly List<ItemDiscount> _discounts = [];

    // Parameterless constructor for EF Core
    protected LineItem()
    {
        Item = default!;
    }

    public LineItem(ItemSnapshot item, Decimal quantity = 1.0m, Int32 sortOrder = 0)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Quantity = quantity;
        SortOrder = sortOrder;
    }

    public virtual ItemSnapshot Item { get; init; } = default!;
    public virtual Decimal Quantity { get; init; } = 1.0m;
    public virtual Int32 SortOrder { get; init; }
    public virtual IReadOnlyCollection<AppliedTax> Taxes => _taxes;
    public virtual IReadOnlyCollection<ItemDiscount> Discounts => _discounts;
    public virtual Decimal UnitPrice => Item.UnitPrice;

    public LineItem SetQuantity(Decimal newQuantity)
    {
        if (newQuantity < Decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");
        }

        return this with { Quantity = newQuantity };
    }

    public LineItem SetSortOrder(Int32 newSortOrder)
    {
        return this with { SortOrder = newSortOrder };
    }

    /// <summary>
    /// Applies taxes to this line item
    /// </summary>
    public void ApplyTaxes(IEnumerable<ApplicableTax> taxRates)
    {
        _taxes.Clear();

        foreach (var taxRate in taxRates)
        {
            _taxes.Add(new AppliedTax
            {
                Name = taxRate.Name,
                Rate = taxRate.Rate
            });
        }
    }

    /// <summary>
    /// Applies discounts to this line item
    /// </summary>
    public void AddDiscount(Discount discount)
    {
        ArgumentNullException.ThrowIfNull(discount);
        if (discount.Scope is not DiscountScope.PerLineItem and not DiscountScope.PerUnit)
        {
            throw new ArgumentException("Discount scope must be PerLineItem or PerUnit.", nameof(discount));
        }

        _discounts.Add(new ItemDiscount(discount.Id, discount.Name, discount.Scope, discount.Percentage, discount.FixedAmount));
    }

    /// <summary>
    /// Gets the subtotal before taxes and discounts
    /// </summary>
    public virtual Decimal GetSubtotal()
        => Item.UnitPrice * Quantity;

    /// <summary>
    /// Gets the total discount amount for this line item
    /// </summary>
    public virtual Decimal GetDiscount()
        => _discounts.Sum(d => d.GetAmount(this));

    /// <summary>
    /// Gets the total tax amount for this line item
    /// </summary>
    public virtual Decimal GetTaxAmount()
    {
        var taxableAmount = GetSubtotal() - GetDiscount();

        return _taxes.Sum(t => t.GetAmount(taxableAmount));
    }

    /// <summary>
    /// Gets the final total after applying discounts and taxes
    /// </summary>
    public virtual Decimal GetTotal() => GetSubtotal() - GetDiscount() + GetTaxAmount();
}
