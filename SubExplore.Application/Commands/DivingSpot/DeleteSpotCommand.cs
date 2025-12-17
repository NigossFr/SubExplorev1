using MediatR;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Command to delete a diving spot.
/// </summary>
/// <param name="SpotId">The ID of the diving spot to delete.</param>
/// <param name="UserId">The ID of the user requesting the deletion.</param>
public record DeleteSpotCommand(
    Guid SpotId,
    Guid UserId) : IRequest<DeleteSpotResult>;

/// <summary>
/// Result of deleting a diving spot.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="SpotId">The ID of the deleted diving spot.</param>
public record DeleteSpotResult(
    bool Success,
    Guid SpotId);
