namespace SubExplore.Domain.ValueObjects;

/// <summary>
/// Unit of measurement for underwater visibility distance.
/// </summary>
public enum VisibilityUnit
{
    /// <summary>
    /// Visibility measured in meters (metric system).
    /// </summary>
    Meters,

    /// <summary>
    /// Visibility measured in feet (imperial system).
    /// </summary>
    Feet
}

/// <summary>
/// Value object representing underwater visibility distance in diving.
/// Supports conversion between meters and feet.
/// </summary>
/// <param name="Value">The visibility distance value (must be non-negative).</param>
/// <param name="Unit">The unit of measurement (Meters or Feet).</param>
public readonly record struct Visibility(decimal Value, VisibilityUnit Unit)
{
    /// <summary>
    /// Conversion factor from meters to feet.
    /// </summary>
    private const decimal MetersToFeetFactor = 3.28084m;

    /// <summary>
    /// Gets the visibility distance value.
    /// </summary>
    public decimal Value { get; init; } = ValidateValue(Value);

    /// <summary>
    /// Gets the unit of measurement.
    /// </summary>
    public VisibilityUnit Unit { get; init; } = Unit;

    /// <summary>
    /// Creates a visibility measurement in meters.
    /// </summary>
    /// <param name="meters">The visibility distance in meters.</param>
    /// <returns>A Visibility value object in meters.</returns>
    public static Visibility FromMeters(decimal meters) => new(meters, VisibilityUnit.Meters);

    /// <summary>
    /// Creates a visibility measurement in feet.
    /// </summary>
    /// <param name="feet">The visibility distance in feet.</param>
    /// <returns>A Visibility value object in feet.</returns>
    public static Visibility FromFeet(decimal feet) => new(feet, VisibilityUnit.Feet);

    /// <summary>
    /// Converts the visibility to meters.
    /// </summary>
    /// <returns>The visibility distance value in meters.</returns>
    public decimal ToMeters()
    {
        return Unit == VisibilityUnit.Meters
            ? Value
            : Value / MetersToFeetFactor;
    }

    /// <summary>
    /// Converts the visibility to feet.
    /// </summary>
    /// <returns>The visibility distance value in feet.</returns>
    public decimal ToFeet()
    {
        return Unit == VisibilityUnit.Feet
            ? Value
            : Value * MetersToFeetFactor;
    }

    /// <summary>
    /// Converts the visibility to the specified unit.
    /// </summary>
    /// <param name="targetUnit">The target unit of measurement.</param>
    /// <returns>A new Visibility value object in the target unit.</returns>
    public Visibility ConvertTo(VisibilityUnit targetUnit)
    {
        if (Unit == targetUnit)
        {
            return this;
        }

        return targetUnit == VisibilityUnit.Meters
            ? FromMeters(ToMeters())
            : FromFeet(ToFeet());
    }

    /// <summary>
    /// Validates that the visibility value is non-negative.
    /// </summary>
    /// <param name="value">The visibility value to validate.</param>
    /// <returns>The validated visibility value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when visibility is negative.</exception>
    private static decimal ValidateValue(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Visibility cannot be negative.");
        }

        return value;
    }

    /// <summary>
    /// Returns a string representation of the visibility.
    /// </summary>
    /// <returns>A string in the format "{value} {unit}".</returns>
    public override string ToString()
    {
        var unitAbbreviation = Unit == VisibilityUnit.Meters ? "m" : "ft";
        return $"{Value} {unitAbbreviation}";
    }
}
