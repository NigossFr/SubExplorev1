using MediatR;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Command to create a new dive log.
/// </summary>
/// <param name="UserId">The ID of the user creating the dive log.</param>
/// <param name="DivingSpotId">The ID of the diving spot.</param>
/// <param name="DiveDate">The date of the dive.</param>
/// <param name="EntryTime">The entry time of the dive.</param>
/// <param name="ExitTime">The exit time of the dive.</param>
/// <param name="MaxDepthMeters">The maximum depth reached in meters.</param>
/// <param name="AverageDepthMeters">The average depth in meters (optional).</param>
/// <param name="WaterTemperatureCelsius">The water temperature in Celsius (optional).</param>
/// <param name="VisibilityMeters">The visibility in meters (optional).</param>
/// <param name="DiveType">The type of dive (0=Recreational, 1=Training, 2=Technical, 3=FreeDiving, 4=Night, 5=Wreck, 6=Cave, 7=Deep).</param>
/// <param name="BuddyUserId">The ID of the buddy diver (optional).</param>
/// <param name="Equipment">The equipment used (optional).</param>
/// <param name="Notes">Additional notes about the dive (optional).</param>
public record CreateDiveLogCommand(
    Guid UserId,
    Guid DivingSpotId,
    DateTime DiveDate,
    TimeSpan EntryTime,
    TimeSpan ExitTime,
    double MaxDepthMeters,
    double? AverageDepthMeters,
    double? WaterTemperatureCelsius,
    double? VisibilityMeters,
    int DiveType,
    Guid? BuddyUserId,
    string? Equipment,
    string? Notes) : IRequest<CreateDiveLogResult>;

/// <summary>
/// Result of creating a dive log.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="DiveLogId">The ID of the created dive log.</param>
/// <param name="DurationMinutes">The calculated duration of the dive in minutes.</param>
public record CreateDiveLogResult(
    bool Success,
    Guid DiveLogId,
    int DurationMinutes);
