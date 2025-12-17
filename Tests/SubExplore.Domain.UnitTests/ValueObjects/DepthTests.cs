using FluentAssertions;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.ValueObjects;

public class DepthTests
{
    [Fact]
    public void Depth_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        const decimal value = 30m;
        const DepthUnit unit = DepthUnit.Meters;

        // Act
        var depth = new Depth(value, unit);

        // Assert
        depth.Value.Should().Be(value);
        depth.Unit.Should().Be(unit);
    }

    [Fact]
    public void FromMeters_Should_Create_Depth_In_Meters()
    {
        // Arrange
        const decimal meters = 25m;

        // Act
        var depth = Depth.FromMeters(meters);

        // Assert
        depth.Value.Should().Be(meters);
        depth.Unit.Should().Be(DepthUnit.Meters);
    }

    [Fact]
    public void FromFeet_Should_Create_Depth_In_Feet()
    {
        // Arrange
        const decimal feet = 82m;

        // Act
        var depth = Depth.FromFeet(feet);

        // Assert
        depth.Value.Should().Be(feet);
        depth.Unit.Should().Be(DepthUnit.Feet);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100.5)]
    public void Depth_Should_Accept_Non_Negative_Values(decimal value)
    {
        // Act
        var depth = new Depth(value, DepthUnit.Meters);

        // Assert
        depth.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    [InlineData(-100)]
    public void Depth_Should_Throw_When_Value_Is_Negative(decimal negativeValue)
    {
        // Act
        Action act = () => new Depth(negativeValue, DepthUnit.Meters);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("value")
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void ToMeters_Should_Return_Same_Value_When_Already_In_Meters()
    {
        // Arrange
        var depth = Depth.FromMeters(30m);

        // Act
        var meters = depth.ToMeters();

        // Assert
        meters.Should().Be(30m);
    }

    [Fact]
    public void ToMeters_Should_Convert_From_Feet_To_Meters()
    {
        // Arrange
        var depth = Depth.FromFeet(98.425m); // ~30 meters

        // Act
        var meters = depth.ToMeters();

        // Assert
        meters.Should().BeApproximately(30m, 0.01m);
    }

    [Fact]
    public void ToFeet_Should_Return_Same_Value_When_Already_In_Feet()
    {
        // Arrange
        var depth = Depth.FromFeet(100m);

        // Act
        var feet = depth.ToFeet();

        // Assert
        feet.Should().Be(100m);
    }

    [Fact]
    public void ToFeet_Should_Convert_From_Meters_To_Feet()
    {
        // Arrange
        var depth = Depth.FromMeters(30m);

        // Act
        var feet = depth.ToFeet();

        // Assert
        feet.Should().BeApproximately(98.425m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Return_Same_Instance_When_Unit_Is_Same()
    {
        // Arrange
        var depth = Depth.FromMeters(30m);

        // Act
        var converted = depth.ConvertTo(DepthUnit.Meters);

        // Assert
        converted.Should().Be(depth);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Meters_To_Feet()
    {
        // Arrange
        var depth = Depth.FromMeters(30m);

        // Act
        var converted = depth.ConvertTo(DepthUnit.Feet);

        // Assert
        converted.Unit.Should().Be(DepthUnit.Feet);
        converted.Value.Should().BeApproximately(98.425m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Feet_To_Meters()
    {
        // Arrange
        var depth = Depth.FromFeet(98.425m);

        // Act
        var converted = depth.ConvertTo(DepthUnit.Meters);

        // Assert
        converted.Unit.Should().Be(DepthUnit.Meters);
        converted.Value.Should().BeApproximately(30m, 0.01m);
    }

    [Fact]
    public void Depth_Should_Be_Equal_When_Values_And_Units_Are_Same()
    {
        // Arrange
        var depth1 = Depth.FromMeters(30m);
        var depth2 = Depth.FromMeters(30m);

        // Act & Assert
        depth1.Should().Be(depth2);
        (depth1 == depth2).Should().BeTrue();
        depth1.Equals(depth2).Should().BeTrue();
    }

    [Fact]
    public void Depth_Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        var depth1 = Depth.FromMeters(30m);
        var depth2 = Depth.FromMeters(40m);

        // Act & Assert
        depth1.Should().NotBe(depth2);
        (depth1 == depth2).Should().BeFalse();
    }

    [Fact]
    public void Depth_Should_Not_Be_Equal_When_Units_Are_Different()
    {
        // Arrange
        var depth1 = new Depth(30m, DepthUnit.Meters);
        var depth2 = new Depth(30m, DepthUnit.Feet);

        // Act & Assert
        depth1.Should().NotBe(depth2);
        (depth1 == depth2).Should().BeFalse();
    }

    [Fact]
    public void Depth_Should_Have_Consistent_HashCode()
    {
        // Arrange
        var depth1 = Depth.FromMeters(30m);
        var depth2 = Depth.FromMeters(30m);

        // Act & Assert
        depth1.GetHashCode().Should().Be(depth2.GetHashCode());
    }

    [Theory]
    [InlineData(30, DepthUnit.Meters, "m")]
    [InlineData(100, DepthUnit.Feet, "ft")]
    public void Depth_ToString_Should_Return_Formatted_String(decimal value, DepthUnit unit, string expectedUnit)
    {
        // Arrange
        var depth = new Depth(value, unit);

        // Act
        var result = depth.ToString();

        // Assert
        result.Should().Contain(value.ToString());
        result.Should().Contain(expectedUnit);
    }
}
