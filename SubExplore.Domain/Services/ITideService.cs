using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Services;

/// <summary>
/// Service interface for tide information retrieval.
/// </summary>
public interface ITideService
{
    /// <summary>
    /// Gets tide data for specified coordinates and date.
    /// </summary>
    /// <param name="coordinates">Location coordinates.</param>
    /// <param name="date">Date for tide information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tide data for the specified date.</returns>
    Task<TideData> GetTideDataAsync(
        Coordinates coordinates,
        DateTime date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next high tide from current time.
    /// </summary>
    /// <param name="coordinates">Location coordinates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>DateTime of the next high tide.</returns>
    Task<DateTime> GetNextHighTideAsync(
        Coordinates coordinates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next low tide from current time.
    /// </summary>
    /// <param name="coordinates">Location coordinates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>DateTime of the next low tide.</returns>
    Task<DateTime> GetNextLowTideAsync(
        Coordinates coordinates,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents tide information for a specific location and date.
/// </summary>
public record TideData
{
    /// <summary>
    /// Gets the coordinates of the location.
    /// </summary>
    public required Coordinates Coordinates { get; init; }

    /// <summary>
    /// Gets the date for this tide data.
    /// </summary>
    public required DateTime Date { get; init; }

    /// <summary>
    /// Gets the collection of tide events for the day.
    /// </summary>
    public required IReadOnlyList<TideEvent> TideEvents { get; init; }

    /// <summary>
    /// Gets the current tide height in meters (if available).
    /// </summary>
    public double? CurrentHeightMeters { get; init; }

    /// <summary>
    /// Gets the current tide state.
    /// </summary>
    public TideState? CurrentState { get; init; }
}

/// <summary>
/// Represents a single tide event (high or low tide).
/// </summary>
public record TideEvent
{
    /// <summary>
    /// Gets the time of the tide event.
    /// </summary>
    public required DateTime Time { get; init; }

    /// <summary>
    /// Gets the tide type (high or low).
    /// </summary>
    public required TideType Type { get; init; }

    /// <summary>
    /// Gets the tide height in meters.
    /// </summary>
    public required double HeightMeters { get; init; }
}

/// <summary>
/// Tide type enumeration.
/// </summary>
public enum TideType
{
    /// <summary>
    /// High tide.
    /// </summary>
    High,

    /// <summary>
    /// Low tide.
    /// </summary>
    Low
}

/// <summary>
/// Current tide state enumeration.
/// </summary>
public enum TideState
{
    /// <summary>
    /// Tide is rising (incoming).
    /// </summary>
    Rising,

    /// <summary>
    /// Tide is falling (outgoing).
    /// </summary>
    Falling,

    /// <summary>
    /// At high tide.
    /// </summary>
    HighTide,

    /// <summary>
    /// At low tide.
    /// </summary>
    LowTide
}
