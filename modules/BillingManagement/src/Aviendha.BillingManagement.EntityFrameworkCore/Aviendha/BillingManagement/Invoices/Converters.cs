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

using Aviendha.BillingManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aviendha.BillingManagement.Invoices;

public class AppliedSurchargeListConverter : JsonValueConverter<IReadOnlyCollection<AppliedSurcharge>>;

public class AppliedSurchargeValueComparer : ValueComparer<IReadOnlyCollection<AppliedSurcharge>>
{
    public AppliedSurchargeValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    {
    }
}

public class AppliedTaxValueConverter : JsonValueConverter<AppliedTax>;

public class AppliedTaxListConverter : JsonValueConverter<IReadOnlyCollection<AppliedTax>>;

public class AppliedTaxListValueComparer : ValueComparer<IReadOnlyCollection<AppliedTax>>
{
    public AppliedTaxListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class ItemSnapshotConverter : JsonValueConverter<ItemSnapshot>;

public class LineItemListValueConverter : JsonValueConverter<IReadOnlyCollection<LineItem>>;

public class LineItemListValueComparer : ValueComparer<IReadOnlyCollection<LineItem>>
{
    public LineItemListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class LineItemDiscountListConverter : JsonValueConverter<IReadOnlyCollection<ItemDiscount>>;

public class LineItemDiscountListValueComparer : ValueComparer<IReadOnlyCollection<ItemDiscount>>
{
    public LineItemDiscountListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class OrderDiscountListConverter : JsonValueConverter<IReadOnlyCollection<AppliedDiscount>>;

public class OrderDiscountListValueComparer : ValueComparer<IReadOnlyCollection<AppliedDiscount>>
{
    public OrderDiscountListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}

public class RefundListConverter : JsonValueConverter<IReadOnlyCollection<Refund>>;
public class RefundListValueComparer : ValueComparer<IReadOnlyCollection<Refund>>
{
    public RefundListValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList().AsReadOnly()
        )
    { }
}
