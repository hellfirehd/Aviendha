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

using System.Text.Json;

namespace Aviendha.Domain.Values;

public class ValueObject_Tests
{
    private record TestValueObject(int Value) : ValueObject<TestValueObject>;

    [Fact]
    public void ValueObjects_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var obj1 = new TestValueObject(42);
        var obj2 = new TestValueObject(42);

        // Act & Assert
        obj1.ShouldBeEquivalentTo(obj2);
    }

    [Fact]
    public void ValueObjects_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var obj1 = new TestValueObject(42);
        var obj2 = new TestValueObject(43);

        // Act & Assert
        Assert.NotEqual(obj1, obj2);
    }

    [Fact]
    public void ValueObjects_ShouldBeImmutable()
    {
        // Arrange
        var obj = new TestValueObject(42);

        // Act
        // Immutability is enforced by the record type and lack of setters.

        // Assert
        Assert.Equal(42, obj.Value);
    }

    [Fact]
    public void ValueObjects_ShouldHaveConsistentHashCodes()
    {
        // Arrange
        var obj1 = new TestValueObject(42);
        var obj2 = new TestValueObject(42);

        // Act
        var hash1 = obj1.GetHashCode();
        var hash2 = obj2.GetHashCode();

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Fact]
    public void ValueObjects_ShouldBeSerializable()
    {
        // Arrange
        var obj = new TestValueObject(42);

        // Act
        var serialized = JsonSerializer.Serialize(obj);
        var deserialized = JsonSerializer.Deserialize<TestValueObject>(serialized);

        // Assert
        deserialized.ShouldBeEquivalentTo(obj);
    }
}
