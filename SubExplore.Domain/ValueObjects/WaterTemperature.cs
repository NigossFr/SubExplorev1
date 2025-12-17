namespace SubExplore.Domain.ValueObjects;

/// <summary>
/// Unit of measurement for water temperature.
/// </summary>
public enum TemperatureUnit
{
    /// <summary>
    /// Temperature measured in degrees Celsius (metric system).
    /// </summary>
    Celsius,

    /// <summary>
    /// Temperature measured in degrees Fahrenheit (imperial system).
    /// </summary>
    Fahrenheit
}

/// <summary>
/// Value object representing water temperature in diving.
/// Supports conversion between Celsius and Fahrenheit.
/// </summary>
/// <param name="Value">The temperature value.</param>
/// <param name="Unit">The unit of measurement (Celsius or Fahrenheit).</param>
public readonly record struct WaterTemperature(decimal Value, TemperatureUnit Unit)
{
    /// <summary>
    /// Absolute zero in Celsius (-273.15°C).
    /// </summary>
    private const decimal AbsoluteZeroCelsius = -273.15m;

    /// <summary>
    /// Absolute zero in Fahrenheit (-459.67°F).
    /// </summary>
    private const decimal AbsoluteZeroFahrenheit = -459.67m;

    /// <summary>
    /// Gets the temperature value.
    /// </summary>
    public decimal Value { get; init; } = ValidateValue(Value, Unit);

    /// <summary>
    /// Gets the unit of measurement.
    /// </summary>
    public TemperatureUnit Unit { get; init; } = Unit;

    /// <summary>
    /// Creates a water temperature in Celsius.
    /// </summary>
    /// <param name="celsius">The temperature in Celsius.</param>
    /// <returns>A WaterTemperature value object in Celsius.</returns>
    public static WaterTemperature FromCelsius(decimal celsius) => new(celsius, TemperatureUnit.Celsius);

    /// <summary>
    /// Creates a water temperature in Fahrenheit.
    /// </summary>
    /// <param name="fahrenheit">The temperature in Fahrenheit.</param>
    /// <returns>A WaterTemperature value object in Fahrenheit.</returns>
    public static WaterTemperature FromFahrenheit(decimal fahrenheit) => new(fahrenheit, TemperatureUnit.Fahrenheit);

    /// <summary>
    /// Converts the temperature to Celsius.
    /// </summary>
    /// <returns>The temperature value in Celsius.</returns>
    public decimal ToCelsius()
    {
        return Unit == TemperatureUnit.Celsius
            ? Value
            : (Value - 32m) * 5m / 9m;
    }

    /// <summary>
    /// Converts the temperature to Fahrenheit.
    /// </summary>
    /// <returns>The temperature value in Fahrenheit.</returns>
    public decimal ToFahrenheit()
    {
        return Unit == TemperatureUnit.Fahrenheit
            ? Value
            : (Value * 9m / 5m) + 32m;
    }

    /// <summary>
    /// Converts the temperature to the specified unit.
    /// </summary>
    /// <param name="targetUnit">The target unit of measurement.</param>
    /// <returns>A new WaterTemperature value object in the target unit.</returns>
    public WaterTemperature ConvertTo(TemperatureUnit targetUnit)
    {
        if (Unit == targetUnit)
        {
            return this;
        }

        return targetUnit == TemperatureUnit.Celsius
            ? FromCelsius(ToCelsius())
            : FromFahrenheit(ToFahrenheit());
    }

    /// <summary>
    /// Validates that the temperature is above absolute zero.
    /// </summary>
    /// <param name="value">The temperature value to validate.</param>
    /// <param name="unit">The unit of measurement.</param>
    /// <returns>The validated temperature value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when temperature is below absolute zero.</exception>
    private static decimal ValidateValue(decimal value, TemperatureUnit unit)
    {
        var absoluteZero = unit == TemperatureUnit.Celsius
            ? AbsoluteZeroCelsius
            : AbsoluteZeroFahrenheit;

        if (value < absoluteZero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Temperature cannot be below absolute zero ({absoluteZero}°{(unit == TemperatureUnit.Celsius ? "C" : "F")}).");
        }

        return value;
    }

    /// <summary>
    /// Returns a string representation of the water temperature.
    /// </summary>
    /// <returns>A string in the format "{value}°{unit}".</returns>
    public override string ToString()
    {
        var unitSymbol = Unit == TemperatureUnit.Celsius ? "C" : "F";
        return $"{Value}°{unitSymbol}";
    }
}
