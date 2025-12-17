namespace SubExplore.Domain.ValueObjects;

/// <summary>
/// Unit of measurement for depth.
/// </summary>
public enum DepthUnit
{
    /// <summary>
    /// Depth measured in meters (metric system).
    /// </summary>
    Meters,

    /// <summary>
    /// Depth measured in feet (imperial system).
    /// </summary>
    Feet
}

/// <summary>
/// Value object representing a depth measurement in diving.
/// Supports conversion between meters and feet.
/// </summary>
/// <param name="Value">The depth value (must be non-negative).</param>
/// <param name="Unit">The unit of measurement (Meters or Feet).</param>
public readonly record struct Depth(decimal Value, DepthUnit Unit)
{
    /// <summary>
    /// Conversion factor from meters to feet.
    /// </summary>
    private const decimal MetersToFeetFactor = 3.28084m;

    /// <summary>
    /// Gets the depth value.
    /// </summary>
    public decimal Value { get; init; } = ValidateValue(Value);

    /// <summary>
    /// Gets the unit of measurement.
    /// </summary>
    public DepthUnit Unit { get; init; } = Unit;

    /// <summary>
    /// Creates a depth measurement in meters.
    /// </summary>
    /// <param name="meters">The depth in meters.</param>
    /// <returns>A Depth value object in meters.</returns>
    public static Depth FromMeters(decimal meters) => new(meters, DepthUnit.Meters);

    /// <summary>
    /// Creates a depth measurement in feet.
    /// </summary>
    /// <param name="feet">The depth in feet.</param>
    /// <returns>A Depth value object in feet.</returns>
    public static Depth FromFeet(decimal feet) => new(feet, DepthUnit.Feet);

    /// <summary>
    /// Converts the depth to meters.
    /// </summary>
    /// <returns>The depth value in meters.</returns>
    public decimal ToMeters()
    {
        return Unit == DepthUnit.Meters
            ? Value
            : Value / MetersToFeetFactor;
    }

    /// <summary>
    /// Converts the depth to feet.
    /// </summary>
    /// <returns>The depth value in feet.</returns>
    public decimal ToFeet()
    {
        return Unit == DepthUnit.Feet
            ? Value
            : Value * MetersToFeetFactor;
    }

    /// <summary>
    /// Converts the depth to the specified unit.
    /// </summary>
    /// <param name="targetUnit">The target unit of measurement.</param>
    /// <returns>A new Depth value object in the target unit.</returns>
    public Depth ConvertTo(DepthUnit targetUnit)
    {
        if (Unit == targetUnit)
        {
            return this;
        }

        return targetUnit == DepthUnit.Meters
            ? FromMeters(ToMeters())
            : FromFeet(ToFeet());
    }

    /// <summary>
    /// Validates that the depth value is non-negative.
    /// </summary>
    /// <param name="value">The depth value to validate.</param>
    /// <returns>The validated depth value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when depth is negative.</exception>
    private static decimal ValidateValue(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Depth cannot be negative.");
        }

        return value;
    }

    /// <summary>
    /// Returns a string representation of the depth.
    /// </summary>
    /// <returns>A string in the format "{value} {unit}".</returns>
    public override string ToString()
    {
        var unitAbbreviation = Unit == DepthUnit.Meters ? "m" : "ft";
        return $"{Value} {unitAbbreviation}";
    }
}
