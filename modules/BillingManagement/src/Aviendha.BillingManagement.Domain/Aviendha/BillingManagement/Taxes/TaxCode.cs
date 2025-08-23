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

using Aviendha.BillingManagement.Items;
using Volo.Abp.Domain.Entities;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Canadian Tax Code master table entry
/// Defines how specific items should be taxed according to CRA guidelines
/// </summary>
public class TaxCode : Entity<Guid>
{
    public static readonly TaxCode Empty = new();

    /// <summary>
    /// Unique tax code identifier (e.g., "STD-GOODS", "ZR-GROCERY", "EX-HEALTH")
    /// </summary>
    public String Code { get; set; } = String.Empty;

    /// <summary>
    /// Human-readable description of the tax code
    /// </summary>
    public String Description { get; set; } = String.Empty;

    /// <summary>
    /// Tax treatment under Canadian GST/HST rules
    /// </summary>
    public TaxTreatment TaxTreatment { get; set; } = TaxTreatment.Standard;

    /// <summary>
    /// Category of item this tax code applies to
    /// </summary>
    public ItemCategory ItemCategory { get; set; } = ItemCategory.GeneralGoods;

    /// <summary>
    /// Effective date when this tax code becomes active
    /// </summary>
    public DateOnly EffectiveDate { get; set; } = DateOnly.FromDateTime(DateTime.UnixEpoch);

    /// <summary>
    /// Expiration date when this tax code is no longer valid
    /// </summary>
    public DateOnly? ExpirationDate { get; set; }

    /// <summary>
    /// Whether this tax code is active independent of its effective/expiration dates
    /// </summary>
    public Boolean IsActive { get; set; } = true;

    /// <summary>
    /// Additional notes or CRA reference information
    /// </summary>
    public String Notes { get; set; } = String.Empty;

    /// <summary>
    /// Reference to CRA documentation or ruling
    /// </summary>
    public String CraReference { get; set; } = String.Empty;

    /// <summary>
    /// Determines if GST/HST should be calculated for this tax code
    /// </summary>
    public Boolean IsTaxable => TaxTreatment == TaxTreatment.Standard;

    /// <summary>
    /// Determines if this is zero-rated (0% but still in GST system)
    /// </summary>
    public Boolean IsZeroRated => TaxTreatment == TaxTreatment.ZeroRated;

    /// <summary>
    /// Determines if this is exempt from GST/HST
    /// </summary>
    public Boolean IsExempt => TaxTreatment == TaxTreatment.Exempt;

    /// <summary>
    /// Checks if tax code is valid for a specific date
    /// </summary>
    public Boolean IsValidOn(DateOnly date)
    {
        return date >= EffectiveDate
            && (ExpirationDate == null || date <= ExpirationDate.Value);
    }
}
