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

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Tax classification linking products/services to their appropriate tax codes
/// </summary>
public record TaxClassification : ValueObject<TaxClassification>
{
    /// <summary>
    /// Reference to the item (Product or Service)
    /// </summary>
    public ItemId ItemId { get; set; }

    /// <summary>
    /// The tax code assigned to this item
    /// </summary>
    public String TaxCode { get; set; } = String.Empty;

    /// <summary>
    /// When this classification was assigned
    /// </summary>
    public DateOnly AssignedDate { get; set; }

    /// <summary>
    /// When this classification expires (if applicable)
    /// </summary>
    public DateOnly? ExpirationDate { get; set; }

    /// <summary>
    /// Who assigned this classification
    /// </summary>
    public String AssignedBy { get; set; } = String.Empty;

    /// <summary>
    /// Additional notes about why this classification was chosen
    /// </summary>
    public String Notes { get; set; } = String.Empty;

    /// <summary>
    /// Checks if this classification is valid for a specific date
    /// </summary>
    public Boolean IsValidOn(DateOnly date)
    {
        return date >= AssignedDate
            && (ExpirationDate == null || date <= ExpirationDate.Value);
    }
}
