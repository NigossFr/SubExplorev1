using MediatR;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Query to get diving statistics for a user.
/// </summary>
/// <param name="UserId">The ID of the user.</param>
/// <param name="StartDate">Calculate statistics from this date (optional, default: all time).</param>
/// <param name="EndDate">Calculate statistics to this date (optional, default: now).</param>
public record GetDiveStatisticsQuery(
    Guid UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IRequest<GetDiveStatisticsResult>;

/// <summary>
/// Result of GetDiveStatisticsQuery containing comprehensive diving statistics.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="Statistics">The diving statistics.</param>
public record GetDiveStatisticsResult(
    bool Success,
    DiveStatisticsDto Statistics);

/// <summary>
/// DTO for diving statistics.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="TotalDives">Total number of dives.</param>
/// <param name="TotalDiveTimeMinutes">Total dive time in minutes.</param>
/// <param name="TotalDiveTimeHours">Total dive time in hours.</param>
/// <param name="MaxDepthMeters">Maximum depth ever reached.</param>
/// <param name="AverageDepthMeters">Average maximum depth across all dives.</param>
/// <param name="AverageDiveTimeMinutes">Average dive duration in minutes.</param>
/// <param name="DeepestDiveId">ID of the deepest dive.</param>
/// <param name="DeepestDiveSpotName">Name of the spot for the deepest dive.</param>
/// <param name="LongestDiveId">ID of the longest dive.</param>
/// <param name="LongestDiveDurationMinutes">Duration of the longest dive in minutes.</param>
/// <param name="FavoriteSpotId">ID of the most visited diving spot.</param>
/// <param name="FavoriteSpotName">Name of the most visited spot.</param>
/// <param name="FavoriteSpotDiveCount">Number of dives at favorite spot.</param>
/// <param name="DivesByType">Distribution of dives by type.</param>
/// <param name="DivesByMonth">Distribution of dives by month (last 12 months).</param>
/// <param name="UniqueSpots">Number of unique diving spots visited.</param>
/// <param name="DiveBuddiesCount">Number of unique dive buddies.</param>
/// <param name="FirstDiveDate">Date of the first dive (optional).</param>
/// <param name="LastDiveDate">Date of the last dive (optional).</param>
/// <param name="PeriodStartDate">Start date of the statistics period.</param>
/// <param name="PeriodEndDate">End date of the statistics period.</param>
public record DiveStatisticsDto(
    Guid UserId,
    int TotalDives,
    int TotalDiveTimeMinutes,
    double TotalDiveTimeHours,
    double MaxDepthMeters,
    double AverageDepthMeters,
    int AverageDiveTimeMinutes,
    Guid? DeepestDiveId,
    string? DeepestDiveSpotName,
    Guid? LongestDiveId,
    int LongestDiveDurationMinutes,
    Guid? FavoriteSpotId,
    string? FavoriteSpotName,
    int FavoriteSpotDiveCount,
    Dictionary<string, int> DivesByType,
    Dictionary<string, int> DivesByMonth,
    int UniqueSpots,
    int DiveBuddiesCount,
    DateTime? FirstDiveDate,
    DateTime? LastDiveDate,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate);
