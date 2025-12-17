using MediatR;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Query to get popular diving spots based on ratings and activity.
/// </summary>
/// <param name="Limit">Maximum number of results to return (default 10, max 50).</param>
/// <param name="MinimumRatings">Minimum number of ratings required to be considered popular (default 5).</param>
/// <param name="DaysBack">Number of days to look back for recent activity (default 90).</param>
public record GetPopularSpotsQuery(
    int Limit = 10,
    int MinimumRatings = 5,
    int DaysBack = 90) : IRequest<GetPopularSpotsResult>;

/// <summary>
/// Result of GetPopularSpotsQuery containing list of popular spots.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="Spots">List of popular diving spots ordered by popularity.</param>
/// <param name="TotalCount">Total number of spots in the result.</param>
public record GetPopularSpotsResult(
    bool Success,
    List<PopularDivingSpotDto> Spots,
    int TotalCount);

/// <summary>
/// DTO for popular diving spot information with popularity metrics.
/// </summary>
/// <param name="Id">The spot ID.</param>
/// <param name="Name">The spot name.</param>
/// <param name="Description">The spot description.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="Difficulty">The difficulty level (0-3).</param>
/// <param name="AverageRating">The average rating (0-5).</param>
/// <param name="RatingCount">The total number of ratings.</param>
/// <param name="RecentDiveLogsCount">The number of dive logs in the recent period.</param>
/// <param name="PopularityScore">The calculated popularity score.</param>
/// <param name="CurrentTemperatureCelsius">The current water temperature (optional).</param>
/// <param name="CurrentVisibilityMeters">The current visibility (optional).</param>
public record PopularDivingSpotDto(
    Guid Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    double MaxDepthMeters,
    int Difficulty,
    double AverageRating,
    int RatingCount,
    int RecentDiveLogsCount,
    double PopularityScore,
    double? CurrentTemperatureCelsius = null,
    double? CurrentVisibilityMeters = null);
