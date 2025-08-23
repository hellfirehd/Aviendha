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
using Aviendha.BillingManagement.Invoices;
using Aviendha.BillingManagement.Items;
using Aviendha.BillingManagement.Provinces;
using Aviendha.BillingManagement.Shipping;
using Aviendha.BillingManagement.Taxes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aviendha.BillingManagement.EntityFrameworkCore;

public class CustomerIdValueConverter() : ValueConverter<CustomerId, Guid>(c => c.Value, c => CustomerId.FromGuid(c));
public class InvoiceIdValueConverter() : ValueConverter<InvoiceId, Guid>(c => c.Value, c => InvoiceId.FromGuid(c));
public class ItemIdValueConverter() : ValueConverter<ItemId, Guid>(c => c.Value, c => ItemId.FromGuid(c));
public class ProvinceIdValueConverter() : ValueConverter<ProvinceId, Guid>(c => c.Value, c => ProvinceId.FromGuid(c));
public class TaxIdValueConverter() : ValueConverter<TaxId, Guid>(c => c.Value, c => TaxId.FromGuid(c));

public class AddressConverter : JsonValueConverter<Address>;

public class AddressListValueConverter : JsonValueConverter<IReadOnlyCollection<Address>>;

public class AddressListValueComparer : ValueComparer<IReadOnlyCollection<Address>>
{
    public AddressListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d))
    { }
}

public class EmailConverter : JsonValueConverter<Email>;

public class ItemSnapshotValueConverter : JsonValueConverter<ItemSnapshot>;

public class ItemUnitPriceConverter : JsonValueConverter<ItemUnitPrice>;

public class ItemUnitPriceListConverter : JsonValueConverter<IReadOnlyCollection<ItemUnitPrice>>;

public class ItemUnitPriceListValueComparer : ValueComparer<IReadOnlyCollection<ItemUnitPrice>>
{
    public ItemUnitPriceListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class ShippingInfoConverter : JsonValueConverter<ShippingInfo>;

public class TaxRateValueConverter : JsonValueConverter<TaxRate>;
public class TaxRateListValueConverter : JsonValueConverter<IReadOnlyCollection<TaxRate>>;
public class TaxRateListValueComparer : ValueComparer<IReadOnlyCollection<TaxRate>>
{
    public TaxRateListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}
