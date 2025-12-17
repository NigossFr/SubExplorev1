using MediatR;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Command to update an existing dive log.
/// </summary>
/// <param name="DiveLogId">The ID of the dive log to update.</param>
/// <param name="UserId">The ID of the user updating the dive log.</param>
/// <param name="MaxDepthMeters">The updated maximum depth in meters.</param>
/// <param name="AverageDepthMeters">The updated average depth in meters (optional).</param>
/// <param name="WaterTemperatureCelsius">The updated water temperature in Celsius (optional).</param>
/// <param name="VisibilityMeters">The updated visibility in meters (optional).</param>
/// <param name="Equipment">The updated equipment description (optional).</param>
/// <param name="Notes">The updated notes (optional).</param>
public record UpdateDiveLogCommand(
    Guid DiveLogId,
    Guid UserId,
    double MaxDepthMeters,
    double? AverageDepthMeters,
    double? WaterTemperatureCelsius,
    double? VisibilityMeters,
    string? Equipment,
    string? Notes) : IRequest<UpdateDiveLogResult>;

/// <summary>
/// Result of updating a dive log.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="DiveLogId">The ID of the updated dive log.</param>
public record UpdateDiveLogResult(
    bool Success,
    Guid DiveLogId);
