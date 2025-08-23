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

namespace Aviendha.BillingManagement;

public static class BillingManagementConsts
{
    public const Int32 MaxInvoiceNumberLength = 10;
    public const Int32 MaxReferenceNumberLength = 20;
    public const Int32 MaxEmailLength = 256;
    public const Int32 MaxNameLength = 128;
    public const Int32 MaxDescriptionLength = 1024;

    public const Int32 MaxReasonLength = 1024;
    public const Int32 MaxUnitTypeLength = 16;
    public const Int32 MaxProvinceCodeLength = 2;
    public const Int32 MaxProvinceNameLength = 32;
    public const Int32 MaxTaxCodeLength = 8;
    public const Int32 MaxTaxNameLength = 64;
}
