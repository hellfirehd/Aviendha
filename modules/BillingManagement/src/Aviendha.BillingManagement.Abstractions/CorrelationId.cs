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

namespace Aviendha.BillingManagement;

public readonly struct CorrelationId(Guid correlationId) : IEquatable<CorrelationId>, IComparable<CorrelationId>, IComparable<Guid>
{
    public static readonly CorrelationId Empty = new(Guid.Empty);

    private readonly Guid _id = correlationId != Guid.Empty ? correlationId : throw new ArgumentNullException(nameof(correlationId));

    public static implicit operator Guid(CorrelationId correlationId) => correlationId._id;

    public static implicit operator CorrelationId(Guid correlationId) => new(correlationId);

    public override String ToString() => _id.ToString();

    public override Boolean Equals(Object? obj) => obj is CorrelationId other && Equals(other);

    public Boolean Equals(CorrelationId other) => _id == other._id;

    public override Int32 GetHashCode() => _id.GetHashCode();

    public Int32 CompareTo(CorrelationId other) => throw new NotImplementedException();

    public Int32 CompareTo(Guid other) => throw new NotImplementedException();

    public static Boolean operator ==(CorrelationId left, CorrelationId right) => left.Equals(right);

    public static Boolean operator !=(CorrelationId left, CorrelationId right) => !left.Equals(right);

    public static Boolean operator <(CorrelationId left, CorrelationId right) => left.CompareTo(right) < 0;

    public static Boolean operator >(CorrelationId left, CorrelationId right) => left.CompareTo(right) > 0;

    public static Boolean operator <=(CorrelationId left, CorrelationId right) => left.CompareTo(right) <= 0;

    public static Boolean operator >=(CorrelationId left, CorrelationId right) => left.CompareTo(right) >= 0;
}
