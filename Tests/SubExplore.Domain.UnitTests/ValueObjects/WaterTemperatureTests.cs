using FluentAssertions;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.ValueObjects;

public class WaterTemperatureTests
{
    [Fact]
    public void WaterTemperature_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        const decimal value = 20m;
        const TemperatureUnit unit = TemperatureUnit.Celsius;

        // Act
        var temperature = new WaterTemperature(value, unit);

        // Assert
        temperature.Value.Should().Be(value);
        temperature.Unit.Should().Be(unit);
    }

    [Fact]
    public void FromCelsius_Should_Create_Temperature_In_Celsius()
    {
        // Arrange
        const decimal celsius = 25m;

        // Act
        var temperature = WaterTemperature.FromCelsius(celsius);

        // Assert
        temperature.Value.Should().Be(celsius);
        temperature.Unit.Should().Be(TemperatureUnit.Celsius);
    }

    [Fact]
    public void FromFahrenheit_Should_Create_Temperature_In_Fahrenheit()
    {
        // Arrange
        const decimal fahrenheit = 77m;

        // Act
        var temperature = WaterTemperature.FromFahrenheit(fahrenheit);

        // Assert
        temperature.Value.Should().Be(fahrenheit);
        temperature.Unit.Should().Be(TemperatureUnit.Fahrenheit);
    }

    [Theory]
    [InlineData(0)]      // Freezing point
    [InlineData(25)]     // Typical diving temp
    [InlineData(100)]    // Boiling point
    [InlineData(-273.15)] // Absolute zero Celsius
    public void WaterTemperature_Should_Accept_Valid_Celsius_Values(decimal value)
    {
        // Act
        var temperature = WaterTemperature.FromCelsius(value);

        // Assert
        temperature.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(32)]      // Freezing point
    [InlineData(77)]      // Typical diving temp
    [InlineData(212)]     // Boiling point
    [InlineData(-459.67)] // Absolute zero Fahrenheit
    public void WaterTemperature_Should_Accept_Valid_Fahrenheit_Values(decimal value)
    {
        // Act
        var temperature = WaterTemperature.FromFahrenheit(value);

        // Assert
        temperature.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-273.16)]
    [InlineData(-300)]
    [InlineData(-1000)]
    public void WaterTemperature_Should_Throw_When_Celsius_Below_Absolute_Zero(decimal invalidCelsius)
    {
        // Act
        Action act = () => WaterTemperature.FromCelsius(invalidCelsius);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("value")
            .WithMessage("*absolute zero*");
    }

    [Theory]
    [InlineData(-459.68)]
    [InlineData(-500)]
    [InlineData(-1000)]
    public void WaterTemperature_Should_Throw_When_Fahrenheit_Below_Absolute_Zero(decimal invalidFahrenheit)
    {
        // Act
        Action act = () => WaterTemperature.FromFahrenheit(invalidFahrenheit);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("value")
            .WithMessage("*absolute zero*");
    }

    [Fact]
    public void ToCelsius_Should_Return_Same_Value_When_Already_In_Celsius()
    {
        // Arrange
        var temperature = WaterTemperature.FromCelsius(20m);

        // Act
        var celsius = temperature.ToCelsius();

        // Assert
        celsius.Should().Be(20m);
    }

    [Fact]
    public void ToCelsius_Should_Convert_From_Fahrenheit_To_Celsius()
    {
        // Arrange
        var temperature = WaterTemperature.FromFahrenheit(68m);

        // Act
        var celsius = temperature.ToCelsius();

        // Assert
        celsius.Should().BeApproximately(20m, 0.01m);
    }

    [Fact]
    public void ToCelsius_Should_Convert_Freezing_Point_Correctly()
    {
        // Arrange
        var temperature = WaterTemperature.FromFahrenheit(32m);

        // Act
        var celsius = temperature.ToCelsius();

        // Assert
        celsius.Should().BeApproximately(0m, 0.01m);
    }

    [Fact]
    public void ToFahrenheit_Should_Return_Same_Value_When_Already_In_Fahrenheit()
    {
        // Arrange
        var temperature = WaterTemperature.FromFahrenheit(68m);

        // Act
        var fahrenheit = temperature.ToFahrenheit();

        // Assert
        fahrenheit.Should().Be(68m);
    }

    [Fact]
    public void ToFahrenheit_Should_Convert_From_Celsius_To_Fahrenheit()
    {
        // Arrange
        var temperature = WaterTemperature.FromCelsius(20m);

        // Act
        var fahrenheit = temperature.ToFahrenheit();

        // Assert
        fahrenheit.Should().BeApproximately(68m, 0.01m);
    }

    [Fact]
    public void ToFahrenheit_Should_Convert_Freezing_Point_Correctly()
    {
        // Arrange
        var temperature = WaterTemperature.FromCelsius(0m);

        // Act
        var fahrenheit = temperature.ToFahrenheit();

        // Assert
        fahrenheit.Should().BeApproximately(32m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Return_Same_Instance_When_Unit_Is_Same()
    {
        // Arrange
        var temperature = WaterTemperature.FromCelsius(20m);

        // Act
        var converted = temperature.ConvertTo(TemperatureUnit.Celsius);

        // Assert
        converted.Should().Be(temperature);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Celsius_To_Fahrenheit()
    {
        // Arrange
        var temperature = WaterTemperature.FromCelsius(20m);

        // Act
        var converted = temperature.ConvertTo(TemperatureUnit.Fahrenheit);

        // Assert
        converted.Unit.Should().Be(TemperatureUnit.Fahrenheit);
        converted.Value.Should().BeApproximately(68m, 0.01m);
    }

    [Fact]
    public void ConvertTo_Should_Convert_From_Fahrenheit_To_Celsius()
    {
        // Arrange
        var temperature = WaterTemperature.FromFahrenheit(68m);

        // Act
        var converted = temperature.ConvertTo(TemperatureUnit.Celsius);

        // Assert
        converted.Unit.Should().Be(TemperatureUnit.Celsius);
        converted.Value.Should().BeApproximately(20m, 0.01m);
    }

    [Fact]
    public void WaterTemperature_Should_Be_Equal_When_Values_And_Units_Are_Same()
    {
        // Arrange
        var temp1 = WaterTemperature.FromCelsius(20m);
        var temp2 = WaterTemperature.FromCelsius(20m);

        // Act & Assert
        temp1.Should().Be(temp2);
        (temp1 == temp2).Should().BeTrue();
        temp1.Equals(temp2).Should().BeTrue();
    }

    [Fact]
    public void WaterTemperature_Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        var temp1 = WaterTemperature.FromCelsius(20m);
        var temp2 = WaterTemperature.FromCelsius(25m);

        // Act & Assert
        temp1.Should().NotBe(temp2);
        (temp1 == temp2).Should().BeFalse();
    }

    [Fact]
    public void WaterTemperature_Should_Not_Be_Equal_When_Units_Are_Different()
    {
        // Arrange
        var temp1 = new WaterTemperature(20m, TemperatureUnit.Celsius);
        var temp2 = new WaterTemperature(20m, TemperatureUnit.Fahrenheit);

        // Act & Assert
        temp1.Should().NotBe(temp2);
        (temp1 == temp2).Should().BeFalse();
    }

    [Fact]
    public void WaterTemperature_Should_Have_Consistent_HashCode()
    {
        // Arrange
        var temp1 = WaterTemperature.FromCelsius(20m);
        var temp2 = WaterTemperature.FromCelsius(20m);

        // Act & Assert
        temp1.GetHashCode().Should().Be(temp2.GetHashCode());
    }

    [Theory]
    [InlineData(20, TemperatureUnit.Celsius, "C")]
    [InlineData(68, TemperatureUnit.Fahrenheit, "F")]
    public void WaterTemperature_ToString_Should_Return_Formatted_String(decimal value, TemperatureUnit unit, string expectedUnit)
    {
        // Arrange
        var temperature = new WaterTemperature(value, unit);

        // Act
        var result = temperature.ToString();

        // Assert
        result.Should().Contain("°");
        result.Should().Contain(expectedUnit);
    }
}
