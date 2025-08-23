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

using System.ComponentModel;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Canadian GST/HST tax treatment classifications
/// Based on CRA guidelines for goods and services taxation
/// </summary>
public enum TaxTreatment
{
    /// <summary>
    /// Standard rate applies (GST/HST at full rate)
    /// Most goods and services fall into this category
    /// </summary>
    [Description("Taxable at Standard Rate")]
    Standard = 0,

    /// <summary>
    /// Zero-rated supplies (0% GST/HST but input tax credits available)
    /// Examples: Basic groceries, prescription drugs, medical devices, exports
    /// </summary>
    [Description("Zero-Rated")]
    ZeroRated = 1,

    /// <summary>
    /// GST/HST exempt (no GST/HST charged, no input tax credits)
    /// Examples: Healthcare services, educational services, financial services
    /// </summary>
    [Description("Exempt")]
    Exempt = 2,

    /// <summary>
    /// Not subject to GST/HST (outside the tax system)
    /// Examples: Employee wages, gifts, used goods sold by individuals
    /// </summary>
    [Description("Out of Scope")]
    OutOfScope = 3
}
