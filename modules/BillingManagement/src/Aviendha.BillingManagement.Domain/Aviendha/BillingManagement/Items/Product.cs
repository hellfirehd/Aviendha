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

namespace Aviendha.BillingManagement.Items;

/// <summary>
/// Represents a physical product that can be sold
/// </summary>
public class Product : Item
{
    public static Product Create(ItemId id, String sku, String name, String taxCode, ItemCategory itemCategory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(taxCode);

        return new Product
        {
            Id = id,
            SKU = sku,
            Name = name,
            ItemCategory = itemCategory,
            TaxCode = taxCode
        };
    }

    public override ItemType ItemType { get => ItemType.Product; init { } }
    public Decimal Weight { get; set; }
    public String Manufacturer { get; set; } = String.Empty;
    public Boolean RequiresShipping { get; set; } = true;
}
