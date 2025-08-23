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
using Aviendha.Domain.Values;

namespace Aviendha.BillingManagement.Invoices;

public record ItemSnapshot : ValueObject<ItemSnapshot>
{
    /// <summary>
    /// Gets or sets the unique identifier for the original item.
    /// </summary>
    public virtual required ItemId ItemId { get; set; }
    public virtual required String SKU { get; set; } = String.Empty;
    public virtual required String Name { get; set; } = String.Empty;
    public virtual String Description { get; set; } = String.Empty;
    public virtual required Decimal UnitPrice { get; set; }
    public virtual String UnitType { get; set; } = "Each";
    public virtual ItemType ItemType { get; set; }
    public virtual ItemCategory ItemCategory { get; set; } = ItemCategory.GeneralGoods;
    public virtual String TaxCode { get; set; } = String.Empty;
}
