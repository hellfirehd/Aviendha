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

using Aviendha.BillingManagement.Customers;
using Aviendha.Domain.Values;

namespace Aviendha.BillingManagement;

public record Address(
    String Name,
    String Line1,
    String Line2,
    String City,
    String Province,
    String PostalCode,
    String Country,
    PhoneNumber PhoneNumber,
    Boolean IsDefault = false,
    Boolean IsShippingAddress = false,
    Boolean IsBillingAddress = false)
    : ValueObject<Address>
{
    public static readonly Address Empty = new();

    // Required for EF Core / JSON serialization
    protected Address()
        : this(
            String.Empty,
            String.Empty,
            String.Empty,
            String.Empty,
            String.Empty,
            String.Empty,
            String.Empty,
            PhoneNumber.Empty
        )
    {
    }

    public virtual String Name { get; private set; } = Name;
    public virtual String Line1 { get; private set; } = Line1;
    public virtual String Line2 { get; private set; } = Line2;
    public virtual String City { get; private set; } = City;
    public virtual String Province { get; private set; } = Province;
    public virtual String PostalCode { get; private set; } = PostalCode;
    public virtual String Country { get; private set; } = Country;
    public virtual PhoneNumber PhoneNumber { get; private set; } = PhoneNumber ?? PhoneNumber.Empty;
    public virtual Boolean IsDefault { get; private set; } = IsDefault;
    public virtual Boolean IsShippingAddress { get; private set; } = IsShippingAddress;
    public virtual Boolean IsBillingAddress { get; private set; } = IsBillingAddress;
}
