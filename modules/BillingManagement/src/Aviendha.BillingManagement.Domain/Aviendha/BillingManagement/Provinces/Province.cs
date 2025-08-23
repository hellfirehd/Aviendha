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

using Aviendha.BillingManagement.Taxes;
using System.Diagnostics;
using Volo.Abp.Domain.Entities;

namespace Aviendha.BillingManagement.Provinces;

[DebuggerDisplay("{Code}:{Id}")]
public class Province : Entity<ProvinceId>, IEquatable<Province>, IComparable<Province>
{
    public static readonly Province Empty = new();
    private readonly List<Tax> _taxes = [];

    protected Province()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    public Province(ProvinceId id, String name, String code, IEnumerable<Tax>? taxes = null) : base(id)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();

        if (taxes != null)
        {
            _taxes.AddRange(taxes);
        }
    }

    public virtual String Name { get; private set; } = String.Empty;
    public virtual String Code { get; private set; } = String.Empty;
    public virtual Boolean HasHST { get; private set; }

    public virtual IReadOnlyCollection<Tax> Taxes => _taxes.AsReadOnly();

    public override String ToString() => Code;

    // IEquatable<Province> implementation
    public override Int32 GetHashCode() => HashCode.Combine(Code.ToUpperInvariant(), Name.ToUpperInvariant());

    public Boolean Equals(Province? other)
        => other is not null && String.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);

    public override Boolean Equals(Object? obj)
    {
        if (obj is Province otherProvince)
        {
            return Equals(otherProvince);
        }

        return false;
    }

    public static Boolean operator ==(Province left, Province right)
    {
        if (left is Province leftProvince && right is Province rightProvince)
        {
            return leftProvince.Equals(rightProvince);
        }

        return false;
    }

    public static Boolean operator !=(Province left, Province right) => !(left == right);

    // IComparable<Province> implementation
    public Int32 CompareTo(Province? other)
    {
        if (other is null)
        {
            return 1;
        }

        // Compare by Name first, then by Code if Codes are equal
        var nameComparison = String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        return nameComparison != 0 ? nameComparison : String.Compare(Code, other.Code, StringComparison.OrdinalIgnoreCase);
    }

    // Comparison operators
    public static Boolean operator <(Province left, Province right) => left.CompareTo(right) < 0;

    public static Boolean operator >(Province left, Province right) => left.CompareTo(right) > 0;

    public static Boolean operator <=(Province left, Province right) => left.CompareTo(right) <= 0;

    public static Boolean operator >=(Province left, Province right) => left.CompareTo(right) >= 0;

    public static implicit operator String(Province province) => province.ToString();
}

