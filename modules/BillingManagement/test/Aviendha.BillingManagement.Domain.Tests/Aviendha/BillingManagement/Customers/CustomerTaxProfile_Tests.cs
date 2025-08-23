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

namespace Aviendha.BillingManagement.Customers;

public class CustomerTaxProfile_Tests
{
    [Fact]
    public void CustomerTaxProfile_ShouldHandleExemptions()
    {
        // Arrange
        var exemptProfile = new CustomerTaxProfile
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.UnixEpoch),
            TaxStatus = TaxStatus.FirstNations
        };

        var regularProfile = new CustomerTaxProfile
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.UnixEpoch),
            TaxStatus = TaxStatus.Regular
        };

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act & Assert
        Assert.True(exemptProfile.QualifiesForExemption(currentDate));
        Assert.False(regularProfile.QualifiesForExemption(currentDate));
    }
}
