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

namespace Aviendha.BillingManagement.Items;

public class ItemCategory_Tests
{
    [Fact]
    public void ItemCategory_ShouldHaveCorrectEnumValues()
    {
        // Test key item categories
        Assert.True(Enum.IsDefined(ItemCategory.GeneralGoods));
        Assert.True(Enum.IsDefined(ItemCategory.BasicGroceries));
        Assert.True(Enum.IsDefined(ItemCategory.PrescriptionDrugs));
        Assert.True(Enum.IsDefined(ItemCategory.HealthcareServices));
        Assert.True(Enum.IsDefined(ItemCategory.EducationalServices));
        Assert.True(Enum.IsDefined(ItemCategory.FinancialServices));

        // Verify expected numeric values
        Assert.Equal(100, (Int32)ItemCategory.GeneralGoods); // Goods start at 100
        Assert.Equal(200, (Int32)ItemCategory.GeneralServices); // Services start at 200
        Assert.Equal(300, (Int32)ItemCategory.Exports); // Special categories start at 300
    }
}
