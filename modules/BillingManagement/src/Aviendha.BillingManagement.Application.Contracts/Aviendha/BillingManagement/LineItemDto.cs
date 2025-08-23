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

using Aviendha.BillingManagement.Items;

namespace Aviendha.BillingManagement;

public class LineItemDto
{
    public Guid Id { get; set; }
    public String Description { get; set; } = String.Empty;
    public Decimal UnitPrice { get; set; }
    public Decimal Quantity { get; set; }
    public Decimal Subtotal { get; set; }
    public Decimal TotalTax { get; set; }
    public Decimal TotalDiscount { get; set; }
    public Decimal Total { get; set; }
    public ItemType ItemType { get; set; }

    public ItemId ItemId { get; init; }
    public String SKU { get; init; } = String.Empty;
    public String Name { get; init; } = String.Empty;
    public String UnitType { get; init; } = "Each";
    public ItemCategory ItemCategory { get; init; } = ItemCategory.GeneralGoods;
    public String TaxCode { get; init; } = String.Empty;

}

public record LineItemSnapshotDto(
    ItemId ItemId,
    String SKU,
    String Name,
    String Description,
    Decimal UnitPrice,
    String UnitType,
    ItemType ItemType,
    ItemCategory ItemCategory,
    String TaxCode
);
