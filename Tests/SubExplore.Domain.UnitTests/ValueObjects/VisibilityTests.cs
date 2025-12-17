using FluentAssertions;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.ValueObjects;

public class VisibilityTests
{
    [Fact]
    public void Visibility_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        const decimal value = 15m;
        const VisibilityUnit unit = VisibilityUnit.Meters;

        // Act
        var visibility = new Visibility(value, unit);

        // Assert
        visibility.Value.Should().Be(value);
        visibility.Unit.Should().Be(unit);
    }

    [Fact]
    public void FromMeters_Should_Create_Visibility_In_Meters()
    {
        // Arrange
        const decimal meters = 20m;

        // Act
        var visibility = Visibility.FromMeters(meters);

        // Assert
        visibility.Value.Should().Be(meters);
        visibility.Unit.Should().Be(VisibilityUnit.Meters);
    }

    [Fact]
    public void FromFeet_Should_Create_Visibility_In_Feet()
    {
        // Arrange
        const decimal feet = 65m;

        // Act
        var visibility = Visibility.FromFeet(feet);

        // Assert
        visibility.Value.Should().Be(feet);
        visibility.Unit.Should().Be(VisibilityUnit.Feet);
    }

    [Theory]
    [InlineData(0)]       // No visibility
    [InlineData(10)]      // Poor visibility
    [InlineData(50)]      // Excellent visibility
    [InlineData(100.5)]   // Exceptional visibility
    public void Visibility_Should_Accept_Non_Negative_Values(decimal value)
    {
        // Act
        var visibility = new Visibility(value, VisibilityUnit.Meters);

        // Assert
        visibility.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    [InlineData(-100)]
    public void Visibility_Should_Throw_When_Value_Is_Negative(decimal negativeValue)
    {
        // Act
        Action act = () => new Visibility(negativeValue, VisibilityUnit.Meters);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("value")
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void ToMeters_Should_Return_Same_Value_When_Already_In_Meters()
    {
        // Arrange
        var visibility = Visibility.FromMeters(20m);

        // Act
        var meters = visibility.ToMeters();

        // Assert
        meters.Should().Be(20m);
    }

    [Fact]
    public void ToMeters_Should_Convert_From_Feet_To_Meters()
    {
        // Arrange
        var visibility = Visibility.FromFeet(65.617m); // ~20 meters

        // Act
        var meters = visibility.ToMeters();

        // Assert
        meters.Should().BeApproximately(20m, 0.01m);
    }

    [Fact]
    public void ToFeet_Should_Return_Same_Value_When_Already_In_Feet()
    {
        // Arrange
        var visibility = Visibility.FromFeet(65m);

        // Act
        var feet = visibility.ToFeet();

        // Assert
        feet.Should().Be(65m);
    }

    [Fact]
    public void ToFeet_Should_Convert_From_Meters_To_Feet()
    {
        // Arrange
        var visibility = Visibility.FromMeters(20m);

        // Act
        var feet = visibility.ToFeet();

        // Assert
        feet.Should().BeApproximately(65.617m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Return_Same_Instance_When_Unit_Is_Same()
    {
        // Arrange
        var visibility = Visibility.FromMeters(20m);

        // Act
        var converted = visibility.ConvertTo(VisibilityUnit.Meters);

        // Assert
        converted.Should().Be(visibility);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Meters_To_Feet()
    {
        // Arrange
        var visibility = Visibility.FromMeters(20m);

        // Act
        var converted = visibility.ConvertTo(VisibilityUnit.Feet);

        // Assert
        converted.Unit.Should().Be(VisibilityUnit.Feet);
        converted.Value.Should().BeApproximately(65.617m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Feet_To_Meters()
    {
        // Arrange
        var visibility = Visibility.FromFeet(65.617m);

        // Act
        var converted = visibility.ConvertTo(VisibilityUnit.Meters);

        // Assert
        converted.Unit.Should().Be(VisibilityUnit.Meters);
        converted.Value.Should().BeApproximately(20m, 0.01m);
    }

    [Fact]
    public void Visibility_Should_Be_Equal_When_Values_And_Units_Are_Same()
    {
        // Arrange
        var visibility1 = Visibility.FromMeters(20m);
        var visibility2 = Visibility.FromMeters(20m);

        // Act & Assert
        visibility1.Should().Be(visibility2);
        (visibility1 == visibility2).Should().BeTrue();
        visibility1.Equals(visibility2).Should().BeTrue();
    }

    [Fact]
    public void Visibility_Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        var visibility1 = Visibility.FromMeters(20m);
        var visibility2 = Visibility.FromMeters(30m);

        // Act & Assert
        visibility1.Should().NotBe(visibility2);
        (visibility1 == visibility2).Should().BeFalse();
    }

    [Fact]
    public void Visibility_Should_Not_Be_Equal_When_Units_Are_Different()
    {
        // Arrange
        var visibility1 = new Visibility(20m, VisibilityUnit.Meters);
        var visibility2 = new Visibility(20m, VisibilityUnit.Feet);

        // Act & Assert
        visibility1.Should().NotBe(visibility2);
        (visibility1 == visibility2).Should().BeFalse();
    }

    [Fact]
    public void Visibility_Should_Have_Consistent_HashCode()
    {
        // Arrange
        var visibility1 = Visibility.FromMeters(20m);
        var visibility2 = Visibility.FromMeters(20m);

        // Act & Assert
        visibility1.GetHashCode().Should().Be(visibility2.GetHashCode());
    }

    [Theory]
    [InlineData(20, VisibilityUnit.Meters, "m")]
    [InlineData(65, VisibilityUnit.Feet, "ft")]
    public void Visibility_ToString_Should_Return_Formatted_String(decimal value, VisibilityUnit unit, string expectedUnit)
    {
        // Arrange
        var visibility = new Visibility(value, unit);

        // Act
        var result = visibility.ToString();

        // Assert
        result.Should().Contain(value.ToString());
        result.Should().Contain(expectedUnit);
    }
}
