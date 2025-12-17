using MediatR;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Command to rate a diving spot.
/// </summary>
/// <param name="SpotId">The ID of the diving spot to rate.</param>
/// <param name="UserId">The ID of the user rating the spot.</param>
/// <param name="Rating">The rating value (1-5).</param>
/// <param name="Comment">The optional comment for the rating.</param>
public record RateSpotCommand(
    Guid SpotId,
    Guid UserId,
    int Rating,
    string? Comment) : IRequest<RateSpotResult>;

/// <summary>
/// Result of rating a diving spot.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="RatingId">The ID of the created rating.</param>
/// <param name="AverageRating">The new average rating of the spot.</param>
public record RateSpotResult(
    bool Success,
    Guid RatingId,
    double AverageRating);
