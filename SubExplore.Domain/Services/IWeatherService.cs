using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Services;

/// <summary>
/// Service interface for weather data retrieval.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Gets current weather data for specified coordinates.
    /// </summary>
    /// <param name="coordinates">Location coordinates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Current weather data.</returns>
    Task<WeatherData> GetCurrentWeatherAsync(
        Coordinates coordinates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets weather forecast for specified coordinates.
    /// </summary>
    /// <param name="coordinates">Location coordinates.</param>
    /// <param name="days">Number of days to forecast (1-7).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Weather forecast data.</returns>
    Task<IReadOnlyList<WeatherData>> GetForecastAsync(
        Coordinates coordinates,
        int days,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents weather data for a specific location and time.
/// </summary>
public record WeatherData
{
    /// <summary>
    /// Gets the coordinates of the location.
    /// </summary>
    public required Coordinates Coordinates { get; init; }

    /// <summary>
    /// Gets the timestamp of the weather data.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the temperature in Celsius.
    /// </summary>
    public required double TemperatureCelsius { get; init; }

    /// <summary>
    /// Gets the "feels like" temperature in Celsius.
    /// </summary>
    public double? FeelsLikeCelsius { get; init; }

    /// <summary>
    /// Gets the atmospheric pressure in hPa.
    /// </summary>
    public double? PressureHpa { get; init; }

    /// <summary>
    /// Gets the humidity percentage (0-100).
    /// </summary>
    public int? HumidityPercent { get; init; }

    /// <summary>
    /// Gets the visibility in meters.
    /// </summary>
    public double? VisibilityMeters { get; init; }

    /// <summary>
    /// Gets the wind speed in m/s.
    /// </summary>
    public double? WindSpeedMps { get; init; }

    /// <summary>
    /// Gets the wind direction in degrees (0-360).
    /// </summary>
    public int? WindDirectionDegrees { get; init; }

    /// <summary>
    /// Gets the cloud coverage percentage (0-100).
    /// </summary>
    public int? CloudsPercent { get; init; }

    /// <summary>
    /// Gets the weather condition (e.g., "Clear", "Clouds", "Rain").
    /// </summary>
    public string? Condition { get; init; }

    /// <summary>
    /// Gets the detailed weather description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the precipitation volume in mm (if applicable).
    /// </summary>
    public double? PrecipitationMm { get; init; }

    /// <summary>
    /// Gets the UV index.
    /// </summary>
    public double? UvIndex { get; init; }
}
