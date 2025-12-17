using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Services;

/// <summary>
/// Service interface for geolocation and distance calculations.
/// </summary>
public interface IGeolocationService
{
    /// <summary>
    /// Calculates the distance between two coordinates.
    /// </summary>
    /// <param name="coord1">First coordinate.</param>
    /// <param name="coord2">Second coordinate.</param>
    /// <param name="unit">Distance unit (default: kilometers).</param>
    /// <returns>The distance between the two points.</returns>
    double CalculateDistance(
        Coordinates coord1,
        Coordinates coord2,
        DistanceUnit unit = DistanceUnit.Kilometers);

    /// <summary>
    /// Gets nearby points within a specified radius from a center point.
    /// </summary>
    /// <param name="center">Center coordinate.</param>
    /// <param name="radiusInKilometers">Search radius in kilometers.</param>
    /// <param name="points">Collection of points to search.</param>
    /// <returns>List of points within the radius.</returns>
    IReadOnlyList<Coordinates> GetNearbyPoints(
        Coordinates center,
        double radiusInKilometers,
        IEnumerable<Coordinates> points);

    /// <summary>
    /// Converts a distance from one unit to another.
    /// </summary>
    /// <param name="distance">Distance value to convert.</param>
    /// <param name="fromUnit">Source unit.</param>
    /// <param name="toUnit">Target unit.</param>
    /// <returns>Converted distance value.</returns>
    double ConvertUnits(
        double distance,
        DistanceUnit fromUnit,
        DistanceUnit toUnit);
}

/// <summary>
/// Distance unit enumeration.
/// </summary>
public enum DistanceUnit
{
    /// <summary>
    /// Kilometers.
    /// </summary>
    Kilometers,

    /// <summary>
    /// Miles.
    /// </summary>
    Miles,

    /// <summary>
    /// Nautical miles.
    /// </summary>
    NauticalMiles,

    /// <summary>
    /// Meters.
    /// </summary>
    Meters,

    /// <summary>
    /// Feet.
    /// </summary>
    Feet
}
