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

using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Invoices.LineItems;

public abstract class LineItemFactory_IntegrationTests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule, LineItemFactory>
    where TStartupModule : IAbpModule
{
    private readonly DateOnly _testDate = new(2023, 01, 01);

    [Fact]
    public async Task Create_WithValidItem_ReturnsLineItem()
    {
        // Arrange
        var product = await ItemRepository.GetAsync(TestData.TaxableProductId, includeDetails: true);

        // Act
        var lineItem = await SUT.CreateAsync(product, _testDate);

        // Assert
        Assert.NotNull(lineItem);
        Assert.Equal(TestData.TaxableProductId, lineItem.Item.ItemId); // Use ItemSnapshot.ItemId instead of LineItem.Id
        Assert.Equal(100.00m, lineItem.UnitPrice);
    }

    [Fact]
    public async Task LineItem_ShouldSetTheItemType_TheSameAsTheOriginalItem()
    {
        // Arrange
        var product = await ItemRepository.GetAsync(TestData.TaxableProductId, includeDetails: true);

        // Act
        var lineItem = await SUT.CreateAsync(product, _testDate);

        // Assert
        Assert.Equal(product.ItemType, lineItem.Item.ItemType);
    }
}
