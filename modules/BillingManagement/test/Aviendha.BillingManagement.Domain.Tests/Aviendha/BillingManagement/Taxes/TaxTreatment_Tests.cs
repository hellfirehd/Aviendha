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

namespace Aviendha.BillingManagement.Taxes;

public class TaxTreatment_Tests
{
    [Fact]
    public void TaxTreatment_ShouldHaveCorrectEnumValues()
    {
        // Test all tax treatments
        Assert.True(Enum.IsDefined(TaxTreatment.Standard));
        Assert.True(Enum.IsDefined(TaxTreatment.ZeroRated));
        Assert.True(Enum.IsDefined(TaxTreatment.Exempt));
        Assert.True(Enum.IsDefined(TaxTreatment.OutOfScope));

        // Verify expected numeric values
        Assert.Equal(0, (Int32)TaxTreatment.Standard);
        Assert.Equal(1, (Int32)TaxTreatment.ZeroRated);
        Assert.Equal(2, (Int32)TaxTreatment.Exempt);
        Assert.Equal(3, (Int32)TaxTreatment.OutOfScope);
    }
}
