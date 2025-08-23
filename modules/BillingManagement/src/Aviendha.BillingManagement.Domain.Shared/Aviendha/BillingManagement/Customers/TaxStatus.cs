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

namespace Aviendha.BillingManagement.Customers;

/// <summary>
/// Recipient status affecting tax treatment
/// </summary>
public enum TaxStatus
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    None = 0,

    [Description("Regular Consumer")]
    Regular = 1,

    [Description("GST/HST Registrant")]
    Registrant = 2,

    [Description("Tax-Exempt Organization")]
    TaxExemptOrganization = 3,

    [Description("First Nations")]
    FirstNations = 4,

    [Description("Non-Resident")]
    NonResident = 5,

    [Description("Government Entity")]
    Government = 6
}
