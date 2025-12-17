using SubExplore.Domain.Entities;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Repositories;

/// <summary>
/// Repository interface for DivingSpot entity operations.
/// </summary>
public interface IDivingSpotRepository : IRepository<DivingSpot>
{
    /// <summary>
    /// Gets diving spots near a specific location within a given radius.
    /// </summary>
    /// <param name="coordinates">The center coordinates.</param>
    /// <param name="radiusInKilometers">The search radius in kilometers.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of nearby diving spots.</returns>
    Task<IReadOnlyList<DivingSpot>> GetNearbyAsync(
        Coordinates coordinates,
        double radiusInKilometers,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for diving spots by name or description.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching diving spots.</returns>
    Task<IReadOnlyList<DivingSpot>> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets diving spots owned by a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of diving spots owned by the user.</returns>
    Task<IReadOnlyList<DivingSpot>> GetByOwnerAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most popular diving spots based on ratings.
    /// </summary>
    /// <param name="count">Number of spots to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of popular diving spots.</returns>
    Task<IReadOnlyList<DivingSpot>> GetPopularAsync(
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets diving spots filtered by minimum average rating.
    /// </summary>
    /// <param name="minRating">Minimum average rating (1-5).</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of highly-rated diving spots.</returns>
    Task<IReadOnlyList<DivingSpot>> GetByMinimumRatingAsync(
        double minRating,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
