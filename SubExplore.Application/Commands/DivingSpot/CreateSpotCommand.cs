using MediatR;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Command to create a new diving spot.
/// </summary>
/// <param name="Name">The name of the diving spot.</param>
/// <param name="Description">The description of the diving spot.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="Difficulty">The difficulty level (0=Beginner, 1=Intermediate, 2=Advanced, 3=Expert).</param>
/// <param name="CreatedBy">The ID of the user creating the spot.</param>
public record CreateSpotCommand(
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    double MaxDepthMeters,
    int Difficulty,
    Guid CreatedBy) : IRequest<CreateSpotResult>;

/// <summary>
/// Result of creating a diving spot.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="SpotId">The ID of the created diving spot.</param>
public record CreateSpotResult(
    bool Success,
    Guid SpotId);
