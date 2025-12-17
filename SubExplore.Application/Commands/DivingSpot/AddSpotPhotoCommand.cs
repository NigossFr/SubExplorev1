using MediatR;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Command to add a photo to a diving spot.
/// </summary>
/// <param name="SpotId">The ID of the diving spot.</param>
/// <param name="Url">The URL of the photo.</param>
/// <param name="Description">The description of the photo (optional).</param>
/// <param name="UserId">The ID of the user adding the photo.</param>
public record AddSpotPhotoCommand(
    Guid SpotId,
    string Url,
    string? Description,
    Guid UserId) : IRequest<AddSpotPhotoResult>;

/// <summary>
/// Result of adding a photo to a diving spot.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="PhotoId">The ID of the added photo.</param>
public record AddSpotPhotoResult(
    bool Success,
    Guid PhotoId);
