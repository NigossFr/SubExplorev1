using MediatR;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Query to get dive logs for a specific user with filtering and pagination.
/// </summary>
/// <param name="UserId">The ID of the user.</param>
/// <param name="StartDate">Filter by dive date from this date (optional).</param>
/// <param name="EndDate">Filter by dive date to this date (optional).</param>
/// <param name="DivingSpotId">Filter by specific diving spot (optional).</param>
/// <param name="MinDepthMeters">Minimum depth filter (optional).</param>
/// <param name="MaxDepthMeters">Maximum depth filter (optional).</param>
/// <param name="DiveType">Filter by dive type (0-7, optional).</param>
/// <param name="SortBy">Sort field (DiveDate, MaxDepth, Duration, default DiveDate).</param>
/// <param name="SortDescending">Sort in descending order (default true).</param>
/// <param name="PageNumber">Page number for pagination (default 1).</param>
/// <param name="PageSize">Page size for pagination (default 20, max 100).</param>
public record GetUserDiveLogsQuery(
    Guid UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? DivingSpotId = null,
    double? MinDepthMeters = null,
    double? MaxDepthMeters = null,
    int? DiveType = null,
    string SortBy = "DiveDate",
    bool SortDescending = true,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<GetUserDiveLogsResult>;

/// <summary>
/// Result of GetUserDiveLogsQuery containing paginated list of dive logs.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="DiveLogs">List of dive logs matching the criteria.</param>
/// <param name="TotalCount">Total number of dive logs matching the criteria.</param>
/// <param name="PageNumber">Current page number.</param>
/// <param name="PageSize">Current page size.</param>
/// <param name="TotalPages">Total number of pages.</param>
public record GetUserDiveLogsResult(
    bool Success,
    List<DiveLogDto> DiveLogs,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

/// <summary>
/// DTO for dive log information in query results.
/// </summary>
/// <param name="Id">The dive log ID.</param>
/// <param name="UserId">The user ID.</param>
/// <param name="DivingSpotId">The diving spot ID.</param>
/// <param name="DivingSpotName">The diving spot name.</param>
/// <param name="DiveDate">The dive date.</param>
/// <param name="EntryTime">The entry time.</param>
/// <param name="ExitTime">The exit time.</param>
/// <param name="DurationMinutes">The dive duration in minutes.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="AverageDepthMeters">The average depth in meters (optional).</param>
/// <param name="WaterTemperatureCelsius">The water temperature (optional).</param>
/// <param name="VisibilityMeters">The visibility (optional).</param>
/// <param name="DiveType">The dive type (0-7).</param>
/// <param name="BuddyUserId">The buddy user ID (optional).</param>
/// <param name="Equipment">The equipment used (optional).</param>
/// <param name="Notes">The dive notes (optional).</param>
/// <param name="CreatedAt">The creation date.</param>
public record DiveLogDto(
    Guid Id,
    Guid UserId,
    Guid DivingSpotId,
    string DivingSpotName,
    DateTime DiveDate,
    TimeSpan EntryTime,
    TimeSpan ExitTime,
    int DurationMinutes,
    double MaxDepthMeters,
    double? AverageDepthMeters,
    double? WaterTemperatureCelsius,
    double? VisibilityMeters,
    int DiveType,
    Guid? BuddyUserId,
    string? Equipment,
    string? Notes,
    DateTime CreatedAt);
