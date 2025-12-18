using Errand.Core;
using FluentAssertions;
using Xunit;

namespace Errand.UnitTests.Core;

public class UnitTests
{
    [Fact]
    public void Value_ShouldReturnSameInstance()
    {
        // Act
        var value1 = Unit.Value;
        var value2 = Unit.Value;

        // Assert
        value1.Should().Be(value2);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForAnyUnitInstance()
    {
        // Arrange
        var unit1 = new Unit();
        var unit2 = new Unit();
        var unit3 = Unit.Value;

        // Assert
        unit1.Equals(unit2).Should().BeTrue();
        unit1.Equals(unit3).Should().BeTrue();
        unit2.Equals(unit3).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingWithObject()
    {
        // Arrange
        var unit = Unit.Value;
        object unitAsObject = new Unit();

        // Assert
        unit.Equals(unitAsObject).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNonUnitObject()
    {
        // Arrange
        var unit = Unit.Value;
        object notUnit = "not a unit";

        // Assert
        unit.Equals(notUnit).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnZero()
    {
        // Arrange
        var unit = Unit.Value;

        // Assert
        unit.GetHashCode().Should().Be(0);
    }

    [Fact]
    public void CompareTo_ShouldReturnZero()
    {
        // Arrange
        var unit1 = new Unit();
        var unit2 = new Unit();

        // Assert
        unit1.CompareTo(unit2).Should().Be(0);
    }

    [Fact]
    public void CompareTo_Object_ShouldReturnZero()
    {
        // Arrange
        var unit = Unit.Value;
        IComparable comparable = unit;

        // Assert
        comparable.CompareTo(new Unit()).Should().Be(0);
        comparable.CompareTo(null).Should().Be(0);
    }

    [Fact]
    public void ToString_ShouldReturnEmptyParentheses()
    {
        // Arrange
        var unit = Unit.Value;

        // Assert
        unit.ToString().Should().Be("()");
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue()
    {
        // Arrange
        var unit1 = new Unit();
        var unit2 = new Unit();

        // Assert
        (unit1 == unit2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnFalse()
    {
        // Arrange
        var unit1 = new Unit();
        var unit2 = new Unit();

        // Assert
        (unit1 != unit2).Should().BeFalse();
    }

    [Fact]
    public void Unit_ShouldBeValueType()
    {
        // Assert
        typeof(Unit).IsValueType.Should().BeTrue();
    }

    [Fact]
    public void Unit_ShouldBeReadonly()
    {
        // Assert
        typeof(Unit).IsValueType.Should().BeTrue();
        typeof(Unit).GetFields().Should().BeEmpty();
    }
}