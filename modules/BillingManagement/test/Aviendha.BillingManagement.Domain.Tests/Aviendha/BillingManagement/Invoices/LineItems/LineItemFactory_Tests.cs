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

using Aviendha.BillingManagement.Items;
using Volo.Abp.Guids;

namespace Aviendha.BillingManagement.Invoices.LineItems;

public class LineItemFactory_Tests
{
    private readonly Mock<ILineItemFactoryProvider> _mockProvider;
    private readonly Mock<IInvoiceLineItemFactory> _mockItemFactory;

    private readonly ItemId _itemId = ItemId.New();
    private readonly Item _testItem;

    private readonly LineItem _lineItem;

    private readonly DateOnly _testDate = new(2023, 1, 1);
    private readonly Decimal _testQuantity = 5.0m;

    private LineItemFactory SUT { get; }

    public LineItemFactory_Tests()
    {
        // Setup test data
        _testItem = TestData.NewProduct(_itemId);

        var itemSnapshot = new ItemSnapshot
        {
            ItemId = _testItem.Id,
            SKU = _testItem.SKU,
            Name = _testItem.Name,
            Description = _testItem.Description,
            UnitPrice = _testItem.GetUnitPrice(_testDate),
            UnitType = _testItem.UnitType,
            ItemType = _testItem.ItemType,
            ItemCategory = _testItem.ItemCategory,
            TaxCode = _testItem.TaxCode
        };

        _lineItem = new LineItem(itemSnapshot, _testQuantity);

        // Factory setup
        _mockItemFactory = new Mock<IInvoiceLineItemFactory>();

        // Provider Setup
        _mockProvider = new Mock<ILineItemFactoryProvider>();
        _mockProvider.Setup(p => p.Factories).Returns([_mockItemFactory.Object]);

        // GuidGenerator setup (no longer needed but keeping for compatibility)
        var mockGuidGenerator = new Mock<IGuidGenerator>();
        mockGuidGenerator.Setup(g => g.Create()).Returns(Guid.NewGuid());

        // Create manager with mocks
        SUT = new LineItemFactory(_mockProvider.Object, mockGuidGenerator.Object);
    }

    [Fact]
    public async Task Create_WithValidItemAndFactory_ReturnsLineItem()
    {
        // Arrange
        _mockItemFactory.Setup(f => f.CanCreate(_testItem)).Returns(true);
        _mockItemFactory.Setup(f => f.CreateAsync(_testItem, _testDate, It.IsAny<CancellationToken>())).ReturnsAsync(_lineItem);

        // Act
        var result = await SUT.CreateAsync(_testItem, _testDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_itemId, result.Item.ItemId);
        Assert.Equal(_testQuantity, result.Quantity);

        // Verify factory was called
        _mockItemFactory.Verify(f => f.CreateAsync(_testItem, _testDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithNoMatchingFactory_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = ItemId.New();
        var item = TestData.NewProduct(itemId);

        _mockItemFactory.Setup(f => f.CanCreate(item)).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SUT.CreateAsync(item, _testDate));

        Assert.Contains("No factory found", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleFactories_SelectsCorrectFactory()
    {
        // Arrange
        var mockGuidGenerator = new Mock<IGuidGenerator>();
        mockGuidGenerator.Setup(g => g.Create()).Returns(Guid.NewGuid());

        var mockFactory1 = new Mock<IInvoiceLineItemFactory>();
        var mockFactory2 = new Mock<IInvoiceLineItemFactory>();
        var mockProvider = new Mock<ILineItemFactoryProvider>();

        mockFactory1.Setup(f => f.CanCreate(_testItem)).Returns(false);
        mockFactory2.Setup(f => f.CanCreate(_testItem)).Returns(true);
        mockFactory2.Setup(f => f.CreateAsync(_testItem, _testDate, It.IsAny<CancellationToken>())).ReturnsAsync(_lineItem);
        mockProvider.Setup(p => p.Factories).Returns([mockFactory1.Object, mockFactory2.Object]);

        var factory = new LineItemFactory(mockProvider.Object, mockGuidGenerator.Object);

        // Act
        var result = await factory.CreateAsync(_testItem, _testDate);

        // Assert
        Assert.NotNull(result);
        mockFactory1.Verify(f => f.CreateAsync(It.IsAny<Item>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        mockFactory2.Verify(f => f.CreateAsync(It.IsAny<Item>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
