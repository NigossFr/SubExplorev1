using MediatR;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Query to get detailed information about a specific dive log.
/// </summary>
/// <param name="DiveLogId">The ID of the dive log.</param>
/// <param name="UserId">The ID of the requesting user (for permission check).</param>
public record GetDiveLogByIdQuery(
    Guid DiveLogId,
    Guid UserId) : IRequest<GetDiveLogByIdResult>;

/// <summary>
/// Result of GetDiveLogByIdQuery containing detailed dive log information.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="DiveLog">The detailed dive log information (null if not found or no permission).</param>
public record GetDiveLogByIdResult(
    bool Success,
    DetailedDiveLogDto? DiveLog);

/// <summary>
/// Detailed DTO for dive log information including all details.
/// </summary>
/// <param name="Id">The dive log ID.</param>
/// <param name="UserId">The user ID.</param>
/// <param name="UserName">The user name.</param>
/// <param name="DivingSpotId">The diving spot ID.</param>
/// <param name="DivingSpotName">The diving spot name.</param>
/// <param name="DivingSpotLatitude">The diving spot latitude.</param>
/// <param name="DivingSpotLongitude">The diving spot longitude.</param>
/// <param name="DiveDate">The dive date.</param>
/// <param name="EntryTime">The entry time.</param>
/// <param name="ExitTime">The exit time.</param>
/// <param name="DurationMinutes">The dive duration in minutes.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="AverageDepthMeters">The average depth in meters (optional).</param>
/// <param name="WaterTemperatureCelsius">The water temperature (optional).</param>
/// <param name="VisibilityMeters">The visibility (optional).</param>
/// <param name="DiveType">The dive type (0-7).</param>
/// <param name="DiveTypeName">The dive type name.</param>
/// <param name="BuddyUserId">The buddy user ID (optional).</param>
/// <param name="BuddyUserName">The buddy user name (optional).</param>
/// <param name="Equipment">The equipment used (optional).</param>
/// <param name="Notes">The dive notes (optional).</param>
/// <param name="IsShared">Whether the dive log is shared with the requesting user.</param>
/// <param name="SharedBy">The user ID who shared (if applicable).</param>
/// <param name="CreatedAt">The creation date.</param>
/// <param name="UpdatedAt">The last update date.</param>
public record DetailedDiveLogDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid DivingSpotId,
    string DivingSpotName,
    double DivingSpotLatitude,
    double DivingSpotLongitude,
    DateTime DiveDate,
    TimeSpan EntryTime,
    TimeSpan ExitTime,
    int DurationMinutes,
    double MaxDepthMeters,
    double? AverageDepthMeters,
    double? WaterTemperatureCelsius,
    double? VisibilityMeters,
    int DiveType,
    string DiveTypeName,
    Guid? BuddyUserId,
    string? BuddyUserName,
    string? Equipment,
    string? Notes,
    bool IsShared,
    Guid? SharedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt);
