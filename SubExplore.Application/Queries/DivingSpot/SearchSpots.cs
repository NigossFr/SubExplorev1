using MediatR;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Query to search diving spots with various filters.
/// </summary>
/// <param name="SearchText">Text to search in name and description (optional).</param>
/// <param name="MinDifficulty">Minimum difficulty level filter (0-3, optional).</param>
/// <param name="MaxDifficulty">Maximum difficulty level filter (0-3, optional).</param>
/// <param name="MinDepthMeters">Minimum depth filter (optional).</param>
/// <param name="MaxDepthMeters">Maximum depth filter (optional).</param>
/// <param name="MinRating">Minimum average rating filter (1-5, optional).</param>
/// <param name="MinTemperatureCelsius">Minimum water temperature filter (optional).</param>
/// <param name="MaxTemperatureCelsius">Maximum water temperature filter (optional).</param>
/// <param name="MinVisibilityMeters">Minimum visibility filter (optional).</param>
/// <param name="SortBy">Sort field (Name, Rating, Depth, CreatedAt, default Rating).</param>
/// <param name="SortDescending">Sort in descending order (default true).</param>
/// <param name="PageNumber">Page number for pagination (default 1).</param>
/// <param name="PageSize">Page size for pagination (default 20, max 100).</param>
public record SearchSpotsQuery(
    string? SearchText = null,
    int? MinDifficulty = null,
    int? MaxDifficulty = null,
    double? MinDepthMeters = null,
    double? MaxDepthMeters = null,
    double? MinRating = null,
    double? MinTemperatureCelsius = null,
    double? MaxTemperatureCelsius = null,
    double? MinVisibilityMeters = null,
    string SortBy = "Rating",
    bool SortDescending = true,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<SearchSpotsResult>;

/// <summary>
/// Result of SearchSpotsQuery containing paginated list of spots.
/// </summary>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="Spots">List of diving spots matching the search criteria.</param>
/// <param name="TotalCount">Total number of spots matching the criteria.</param>
/// <param name="PageNumber">Current page number.</param>
/// <param name="PageSize">Current page size.</param>
/// <param name="TotalPages">Total number of pages.</param>
public record SearchSpotsResult(
    bool Success,
    List<DivingSpotDto> Spots,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);
