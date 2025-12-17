using MediatR;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Query to get dive logs for a specific diving spot.
/// </summary>
/// <param name="DivingSpotId">The ID of the diving spot.</param>
/// <param name="StartDate">Filter by dive date from this date (optional).</param>
/// <param name="EndDate">Filter by dive date to this date (optional).</param>
/// <param name="MinDepthMeters">Minimum depth filter (optional).</param>
/// <param name="MaxDepthMeters">Maximum depth filter (optional).</param>
/// <param name="SortBy">Sort field (DiveDate, MaxDepth, Duration, default DiveDate).</param>
/// <param name="SortDescending">Sort in descending order (default true).</param>
/// <param name="PageNumber">Page number for pagination (default 1).</param>
/// <param name="PageSize">Page size for pagination (default 20, max 100).</param>
public record GetDiveLogsBySpotQuery(
    Guid DivingSpotId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    double? MinDepthMeters = null,
    double? MaxDepthMeters = null,
    string SortBy = "DiveDate",
    bool SortDescending = true,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<GetDiveLogsBySpotResult>;

/// <summary>
/// Result of GetDiveLogsBySpotQuery containing paginated list of dive logs.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="DivingSpotId">The diving spot ID.</param>
/// <param name="DivingSpotName">The diving spot name.</param>
/// <param name="DiveLogs">List of dive logs at this spot.</param>
/// <param name="TotalCount">Total number of dive logs at this spot.</param>
/// <param name="PageNumber">Current page number.</param>
/// <param name="PageSize">Current page size.</param>
/// <param name="TotalPages">Total number of pages.</param>
/// <param name="SpotStatistics">Summary statistics for this spot.</param>
public record GetDiveLogsBySpotResult(
    bool Success,
    Guid DivingSpotId,
    string DivingSpotName,
    List<SpotDiveLogDto> DiveLogs,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    SpotDiveStatisticsDto SpotStatistics);

/// <summary>
/// DTO for dive log at a specific spot (includes diver information).
/// </summary>
/// <param name="Id">The dive log ID.</param>
/// <param name="UserId">The user ID.</param>
/// <param name="UserName">The user name.</param>
/// <param name="DiveDate">The dive date.</param>
/// <param name="DurationMinutes">The dive duration in minutes.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="AverageDepthMeters">The average depth in meters (optional).</param>
/// <param name="WaterTemperatureCelsius">The water temperature (optional).</param>
/// <param name="VisibilityMeters">The visibility (optional).</param>
/// <param name="DiveType">The dive type (0-7).</param>
/// <param name="DiveTypeName">The dive type name.</param>
/// <param name="Notes">The dive notes (optional).</param>
public record SpotDiveLogDto(
    Guid Id,
    Guid UserId,
    string UserName,
    DateTime DiveDate,
    int DurationMinutes,
    double MaxDepthMeters,
    double? AverageDepthMeters,
    double? WaterTemperatureCelsius,
    double? VisibilityMeters,
    int DiveType,
    string DiveTypeName,
    string? Notes);

/// <summary>
/// DTO for diving spot statistics based on all dive logs.
/// </summary>
/// <param name="TotalDives">Total number of dives at this spot.</param>
/// <param name="UniqueDivers">Number of unique divers.</param>
/// <param name="AverageDepthMeters">Average maximum depth across all dives.</param>
/// <param name="AverageDurationMinutes">Average dive duration in minutes.</param>
/// <param name="AverageTemperatureCelsius">Average water temperature (optional).</param>
/// <param name="AverageVisibilityMeters">Average visibility (optional).</param>
/// <param name="LastDiveDate">Date of the most recent dive (optional).</param>
public record SpotDiveStatisticsDto(
    int TotalDives,
    int UniqueDivers,
    double AverageDepthMeters,
    int AverageDurationMinutes,
    double? AverageTemperatureCelsius,
    double? AverageVisibilityMeters,
    DateTime? LastDiveDate);
