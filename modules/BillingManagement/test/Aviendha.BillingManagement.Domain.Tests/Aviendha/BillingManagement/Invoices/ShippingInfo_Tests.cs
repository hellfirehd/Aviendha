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

using Aviendha.BillingManagement.Shipping;
using System.Text.Json;

namespace Aviendha.BillingManagement.Invoices;

public class ShippingInfo_Tests
{
    [Fact]
    public void ShippingInfo_CanBe_Serialized_and_Deserialized()
    {
        // Arrange
        var shippingInfo = new ShippingInfo("John Doe", "UPS", "1Z999AA10123456784", 25.00m, IsRefundable: true);

        // Act
        var serialized = JsonSerializer.Serialize(shippingInfo);
        var deserialized = JsonSerializer.Deserialize<ShippingInfo>(serialized);

        // Assert
        deserialized.ShouldBe(shippingInfo);
    }
}
