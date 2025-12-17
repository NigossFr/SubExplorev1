using FluentAssertions;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.ValueObjects;

public class CoordinatesTests
{
    [Fact]
    public void Coordinates_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        const double latitude = 48.8566;
        const double longitude = 2.3522;

        // Act
        var coordinates = new Coordinates(latitude, longitude);

        // Assert
        coordinates.Latitude.Should().Be(latitude);
        coordinates.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData(-90, 0)]    // South Pole
    [InlineData(90, 0)]     // North Pole
    [InlineData(0, -180)]   // International Date Line West
    [InlineData(0, 180)]    // International Date Line East
    [InlineData(0, 0)]      // Null Island
    public void Coordinates_Should_Accept_Edge_Case_Values(double latitude, double longitude)
    {
        // Act
        var coordinates = new Coordinates(latitude, longitude);

        // Assert
        coordinates.Latitude.Should().Be(latitude);
        coordinates.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void Coordinates_Should_Throw_When_Latitude_Is_Invalid(double invalidLatitude)
    {
        // Act
        Action act = () => new Coordinates(invalidLatitude, 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("latitude")
            .WithMessage("*must be between -90 and 90 degrees*");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void Coordinates_Should_Throw_When_Longitude_Is_Invalid(double invalidLongitude)
    {
        // Act
        Action act = () => new Coordinates(0, invalidLongitude);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("longitude")
            .WithMessage("*must be between -180 and 180 degrees*");
    }

    [Fact]
    public void Coordinates_Should_Be_Equal_When_Values_Are_Same()
    {
        // Arrange
        var coordinates1 = new Coordinates(48.8566, 2.3522);
        var coordinates2 = new Coordinates(48.8566, 2.3522);

        // Act & Assert
        coordinates1.Should().Be(coordinates2);
        (coordinates1 == coordinates2).Should().BeTrue();
        coordinates1.Equals(coordinates2).Should().BeTrue();
    }

    [Fact]
    public void Coordinates_Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        var paris = new Coordinates(48.8566, 2.3522);
        var london = new Coordinates(51.5074, -0.1278);

        // Act & Assert
        paris.Should().NotBe(london);
        (paris == london).Should().BeFalse();
        paris.Equals(london).Should().BeFalse();
    }

    [Fact]
    public void Coordinates_Should_Have_Consistent_HashCode()
    {
        // Arrange
        var coordinates1 = new Coordinates(48.8566, 2.3522);
        var coordinates2 = new Coordinates(48.8566, 2.3522);

        // Act & Assert
        coordinates1.GetHashCode().Should().Be(coordinates2.GetHashCode());
    }

    [Fact]
    public void Coordinates_ToString_Should_Return_Formatted_String()
    {
        // Arrange
        var coordinates = new Coordinates(48.8566, 2.3522);

        // Act
        var result = coordinates.ToString();

        // Assert
        result.Should().Contain("Latitude");
        result.Should().Contain("Longitude");
        result.Should().Contain("48");
        result.Should().Contain("8566");
        result.Should().Contain("2");
        result.Should().Contain("3522");
    }
}
