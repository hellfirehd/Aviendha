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

namespace Aviendha.BillingManagement.Customers;

public class TaxStatus_Tests
{
    [Fact]
    public void TaxStatus_ShouldHaveCorrectEnumValues()
    {
        // Test that the recipient status enum has all expected values
        Assert.True(Enum.IsDefined(TaxStatus.Regular));
        Assert.True(Enum.IsDefined(TaxStatus.Registrant));
        Assert.True(Enum.IsDefined(TaxStatus.TaxExemptOrganization));
        Assert.True(Enum.IsDefined(TaxStatus.FirstNations));
        Assert.True(Enum.IsDefined(TaxStatus.NonResident));
        Assert.True(Enum.IsDefined(TaxStatus.Government));

        // Verify expected numeric values
        Assert.Equal(1, (Int32)TaxStatus.Regular);
        Assert.Equal(2, (Int32)TaxStatus.Registrant);
        Assert.Equal(3, (Int32)TaxStatus.TaxExemptOrganization);
        Assert.Equal(4, (Int32)TaxStatus.FirstNations);
        Assert.Equal(5, (Int32)TaxStatus.NonResident);
        Assert.Equal(6, (Int32)TaxStatus.Government);
    }
}
