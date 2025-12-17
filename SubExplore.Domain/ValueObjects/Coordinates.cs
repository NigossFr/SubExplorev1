namespace SubExplore.Domain.ValueObjects;

/// <summary>
/// Value object representing geographic coordinates (latitude and longitude).
/// Used for dive site locations and other geographic points.
/// </summary>
/// <param name="Latitude">Latitude in decimal degrees (-90 to 90).</param>
/// <param name="Longitude">Longitude in decimal degrees (-180 to 180).</param>
public readonly record struct Coordinates(double Latitude, double Longitude)
{
    /// <summary>
    /// Gets the latitude in decimal degrees.
    /// Valid range: -90 (South Pole) to 90 (North Pole).
    /// </summary>
    public double Latitude { get; init; } = ValidateLatitude(Latitude);

    /// <summary>
    /// Gets the longitude in decimal degrees.
    /// Valid range: -180 (West) to 180 (East).
    /// </summary>
    public double Longitude { get; init; } = ValidateLongitude(Longitude);

    /// <summary>
    /// Validates that the latitude is within the valid range.
    /// </summary>
    /// <param name="latitude">The latitude value to validate.</param>
    /// <returns>The validated latitude value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude is not between -90 and 90.</exception>
    private static double ValidateLatitude(double latitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentOutOfRangeException(
                nameof(latitude),
                latitude,
                "Latitude must be between -90 and 90 degrees.");
        }

        return latitude;
    }

    /// <summary>
    /// Validates that the longitude is within the valid range.
    /// </summary>
    /// <param name="longitude">The longitude value to validate.</param>
    /// <returns>The validated longitude value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when longitude is not between -180 and 180.</exception>
    private static double ValidateLongitude(double longitude)
    {
        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentOutOfRangeException(
                nameof(longitude),
                longitude,
                "Longitude must be between -180 and 180 degrees.");
        }

        return longitude;
    }

    /// <summary>
    /// Returns a string representation of the coordinates.
    /// </summary>
    /// <returns>A string in the format "Latitude: {lat}, Longitude: {lon}".</returns>
    public override string ToString()
    {
        return $"Latitude: {Latitude}, Longitude: {Longitude}";
    }
}
