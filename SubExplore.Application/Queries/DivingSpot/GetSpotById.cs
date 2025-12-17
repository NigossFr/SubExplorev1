using MediatR;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Query to get detailed information about a specific diving spot.
/// </summary>
/// <param name="SpotId">The ID of the diving spot.</param>
/// <param name="IncludePhotos">Whether to include photos in the result (default true).</param>
/// <param name="IncludeRatings">Whether to include ratings in the result (default true).</param>
public record GetSpotByIdQuery(
    Guid SpotId,
    bool IncludePhotos = true,
    bool IncludeRatings = true) : IRequest<GetSpotByIdResult>;

/// <summary>
/// Result of GetSpotByIdQuery containing detailed spot information.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="Spot">The detailed spot information (null if not found).</param>
public record GetSpotByIdResult(
    bool Success,
    DetailedDivingSpotDto? Spot);

/// <summary>
/// Detailed DTO for diving spot information including photos and ratings.
/// </summary>
/// <param name="Id">The spot ID.</param>
/// <param name="Name">The spot name.</param>
/// <param name="Description">The spot description.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="MaxDepthMeters">The maximum depth in meters.</param>
/// <param name="Difficulty">The difficulty level (0-3).</param>
/// <param name="AverageRating">The average rating (0-5).</param>
/// <param name="RatingCount">The number of ratings.</param>
/// <param name="CurrentTemperatureCelsius">The current water temperature (optional).</param>
/// <param name="CurrentVisibilityMeters">The current visibility (optional).</param>
/// <param name="CreatedBy">The user ID who created the spot.</param>
/// <param name="CreatedAt">The creation date.</param>
/// <param name="UpdatedAt">The last update date.</param>
/// <param name="Photos">List of spot photos.</param>
/// <param name="Ratings">List of spot ratings.</param>
public record DetailedDivingSpotDto(
    Guid Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    double MaxDepthMeters,
    int Difficulty,
    double AverageRating,
    int RatingCount,
    double? CurrentTemperatureCelsius,
    double? CurrentVisibilityMeters,
    Guid CreatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<SpotPhotoDto> Photos,
    List<SpotRatingDto> Ratings);

/// <summary>
/// DTO for diving spot photo.
/// </summary>
/// <param name="Id">The photo ID.</param>
/// <param name="Url">The photo URL.</param>
/// <param name="Description">The photo description.</param>
/// <param name="UploadedBy">The user ID who uploaded the photo.</param>
/// <param name="UploadedAt">The upload date.</param>
public record SpotPhotoDto(
    Guid Id,
    string Url,
    string? Description,
    Guid UploadedBy,
    DateTime UploadedAt);

/// <summary>
/// DTO for diving spot rating.
/// </summary>
/// <param name="Id">The rating ID.</param>
/// <param name="UserId">The user ID who rated.</param>
/// <param name="Rating">The rating value (1-5).</param>
/// <param name="Comment">The rating comment.</param>
/// <param name="CreatedAt">The rating date.</param>
public record SpotRatingDto(
    Guid Id,
    Guid UserId,
    int Rating,
    string? Comment,
    DateTime CreatedAt);
