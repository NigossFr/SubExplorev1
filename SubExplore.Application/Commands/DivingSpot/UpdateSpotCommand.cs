using MediatR;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Command to update an existing diving spot.
/// </summary>
/// <param name="SpotId">The ID of the diving spot to update.</param>
/// <param name="Name">The updated name of the diving spot.</param>
/// <param name="Description">The updated description of the diving spot.</param>
/// <param name="MaxDepthMeters">The updated maximum depth in meters.</param>
/// <param name="Difficulty">The updated difficulty level (0=Beginner, 1=Intermediate, 2=Advanced, 3=Expert).</param>
/// <param name="CurrentTemperatureCelsius">The current water temperature in Celsius (optional).</param>
/// <param name="CurrentVisibilityMeters">The current visibility in meters (optional).</param>
/// <param name="UserId">The ID of the user updating the spot.</param>
public record UpdateSpotCommand(
    Guid SpotId,
    string Name,
    string Description,
    double MaxDepthMeters,
    int Difficulty,
    double? CurrentTemperatureCelsius,
    double? CurrentVisibilityMeters,
    Guid UserId) : IRequest<UpdateSpotResult>;

/// <summary>
/// Result of updating a diving spot.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="SpotId">The ID of the updated diving spot.</param>
public record UpdateSpotResult(
    bool Success,
    Guid SpotId);
