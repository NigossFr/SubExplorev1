using MediatR;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Query to get nearby diving spots based on geolocation.
/// </summary>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="RadiusKm">The search radius in kilometers (default 10km, max 100km).</param>
/// <param name="MinDifficulty">Minimum difficulty level filter (0-3, optional).</param>
/// <param name="MaxDifficulty">Maximum difficulty level filter (0-3, optional).</param>
/// <param name="MinDepthMeters">Minimum depth filter (optional).</param>
/// <param name="MaxDepthMeters">Maximum depth filter (optional).</param>
/// <param name="Limit">Maximum number of results to return (default 20, max 100).</param>
public record GetNearbySpotsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm = 10.0,
    int? MinDifficulty = null,
    int? MaxDifficulty = null,
    double? MinDepthMeters = null,
    double? MaxDepthMeters = null,
    int Limit = 20) : IRequest<GetNearbySpotsResult>;

/// <summary>
/// Result of GetNearbySpotsQuery containing list of nearby spots with distances.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="Spots">List of diving spots with distance information.</param>
/// <param name="TotalCount">Total number of spots found.</param>
public record GetNearbySpotsResult(
    bool Success,
    List<DivingSpotDto> Spots,
    int TotalCount);

/// <summary>
/// DTO for diving spot information in query results.
/// </summary>
/// <param name="Id">The spot ID.</param>
/// <param name="Name">The spot name.</param>
/// <param name="Description">The spot description.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="Difficulty">The difficulty level (0-3).</param>
/// <param name="AverageRating">The average rating (0-5).</param>
/// <param name="RatingCount">The number of ratings.</param>
/// <param name="DistanceKm">The distance from search point in kilometers.</param>
/// <param name="CurrentTemperatureCelsius">The current water temperature (optional).</param>
/// <param name="CurrentVisibilityMeters">The current visibility (optional).</param>
public record DivingSpotDto(
    Guid Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    double MaxDepthMeters,
    int Difficulty,
    double AverageRating,
    int RatingCount,
    double DistanceKm,
    double? CurrentTemperatureCelsius = null,
    double? CurrentVisibilityMeters = null);
