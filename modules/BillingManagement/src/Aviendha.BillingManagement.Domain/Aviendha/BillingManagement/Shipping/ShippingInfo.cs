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

using Aviendha.Domain.Values;

namespace Aviendha.BillingManagement.Shipping;

public record ShippingInfo(String Name, String Carrier, String TrackingNumber, Decimal ShippingCost, Boolean IsRefundable = false)
    : ValueObject<ShippingInfo>
{
    public static readonly ShippingInfo Empty = new();

    protected ShippingInfo() : this(
        String.Empty,
        String.Empty,
        String.Empty,
        0.0m,
        false)
    {
    }

    public static ShippingInfo Create(String name, String carrier, String trackingNumber, Decimal shippingCost, Boolean isRefundable)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(carrier);
        ArgumentException.ThrowIfNullOrWhiteSpace(trackingNumber);

        return new ShippingInfo(name, carrier, trackingNumber, shippingCost, isRefundable);
    }
}
