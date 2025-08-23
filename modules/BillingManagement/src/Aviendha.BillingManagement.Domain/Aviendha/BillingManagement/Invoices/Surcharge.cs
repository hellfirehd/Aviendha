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

using Aviendha.BillingManagement.Taxes;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Represents a surcharge applied to an order
/// </summary>
/// <remarks>
/// <para>When you supply taxable goods or services in Canada, any additional fee you add to
/// the invoice—whether called a surcharge, convenience fee, or handling charge—is 
/// generally considered part of the “consideration” and is itself subject to GST/HST.</para>
/// <list type="bullet">
///     <item>Prior to October 2022, credit-card surcharges were treated as financial services and exempt from GST/HST.</item>
///     <item>As of March 29, 2023, the Excise Tax Act was amended to specifically include credit-card surcharges in the tax base. All such surcharges are now taxable supplies and subject to GST/HST.</item>
///     <item>Other financing charges (e.g., one-time installment financing) remain exempt as financial services.</item>
/// </list>
/// <para>NOTE: Quebec does not allow credit card surcharges.</para>
/// <para><b>Other Common Surcharges</b></para>
/// <list type="table">
///     <listheader>
///         <term>Surcharge Type</term>
///         <description>Tax Treatment</description>
///     </listheader>
///     <item>
///         <term>Fuel, shipping or delivery surcharges</term>
///         <description>Taxable if the underlying supply (transportation of goods/services) is taxable.</description>
///     </item>
///     <item>
///         <term>Convenience fees (e.g., online order processing)</term>
///         <description>Taxable as part of the service you’re providing.</description>
///     </item>
///     <item>
///        <term>Service or handling fees (e.g., baggage, administration)</term>
///        <description>Taxable whenever tied to a taxable supply of goods or services.</description>
///     </item>
/// </list>
/// <para><b>Practical takeaway</b></para>
/// <list type="bullet">
///     <item>Clearly identify the surcharge(s) on each invoice as a separate line item.</item>
///     <item>Charge GST/HST on that surcharge at the same rate you apply to the main supply.</item>
///     <item>For credit-card surcharges added on or after March 29, 2023, ensure you collect and remit GST/HST on the surcharge amount.</item>
/// </list> 
/// </remarks>
public class Surcharge : FullAuditedAggregateRoot<Guid>
{
    public virtual String Name { get; set; } = String.Empty;
    public virtual String Description { get; set; } = String.Empty;
    public virtual TaxTreatment TaxTreatment { get; set; }
    public virtual Decimal FixedAmount { get; set; }
    public virtual Decimal PercentageRate { get; set; } // As Decimal (e.g., 0.029 for 2.9%)
    public virtual Boolean IsActive { get; set; } = true;

    /// <summary>
    /// Calculates the surcharge amount based on the order total
    /// </summary>
    public Decimal CalculateSurchargeAmount(Decimal orderTotal)
    {
        var surchargeAmount = FixedAmount + orderTotal * PercentageRate;
        return Math.Round(surchargeAmount, 2);
    }
}
