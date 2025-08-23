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

using Aviendha.BillingManagement.Customers;
using Aviendha.BillingManagement.Provinces;
using Volo.Abp.Domain.Entities;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Customer tax status and exemption information
/// </summary>
public class CustomerTaxProfile : Entity<Guid>
{
    /// <summary>
    /// Customer identifier
    /// </summary>
    public virtual CustomerId CustomerId { get; set; }

    /// <summary>
    /// Customer's tax status
    /// </summary>
    public virtual TaxStatus TaxStatus { get; set; } = TaxStatus.Regular;

    /// <summary>
    /// GST/HST registration number (if applicable)
    /// </summary>
    public virtual String GstHstNumber { get; set; } = String.Empty;

    /// <summary>
    /// Tax exemption certificate number (if applicable)
    /// </summary>
    public virtual String ExemptionCertificateNumber { get; set; } = String.Empty;

    /// <summary>
    /// Province where customer is located for tax purposes
    /// </summary>
    public virtual ProvinceId PlaceOfSupplyId { get; set; }

    ///// <summary>
    ///// Whether customer is eligible for tax exemptions
    ///// </summary>
    //public virtual Boolean IsEligibleForExemption { get { return CustomerTaxType } }

    /// <summary>
    /// Date when tax profile becomes effective
    /// </summary>
    public virtual DateOnly EffectiveDate { get; set; }

    /// <summary>
    /// Date when tax profile expires
    /// </summary>
    public virtual DateOnly? ExpirationDate { get; set; }

    /// <summary>
    /// Additional notes about customer's tax status
    /// </summary>
    public virtual String Notes { get; set; } = String.Empty;

    /// <summary>
    /// Checks if customer qualifies for tax exemption on a specific date
    /// </summary>
    public virtual Boolean QualifiesForExemption(DateOnly date)
    {
        if (date >= EffectiveDate && (ExpirationDate == null || date <= ExpirationDate.Value))
        {
            return TaxStatus switch
            {
                TaxStatus.Registrant => true,
                TaxStatus.TaxExemptOrganization => true,
                TaxStatus.FirstNations => true,
                TaxStatus.NonResident => true,
                _ => false
            };
        }

        return false;
    }
}
