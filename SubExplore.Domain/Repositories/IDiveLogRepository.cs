using SubExplore.Domain.Entities;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Repositories;

/// <summary>
/// Repository interface for DiveLog entity operations.
/// </summary>
public interface IDiveLogRepository : IRepository<DiveLog>
{
    /// <summary>
    /// Gets all dive logs for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of dive logs for the user.</returns>
    Task<IReadOnlyList<DiveLog>> GetByUserAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all dive logs for a specific diving spot.
    /// </summary>
    /// <param name="divingSpotId">The diving spot ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of dive logs for the spot.</returns>
    Task<IReadOnlyList<DiveLog>> GetBySpotAsync(
        Guid divingSpotId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets dive logs within a specific date range for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of dive logs within the date range.</returns>
    Task<IReadOnlyList<DiveLog>> GetByDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets dive logs where a specific user was a buddy.
    /// </summary>
    /// <param name="buddyUserId">The buddy user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of dive logs with the user as buddy.</returns>
    Task<IReadOnlyList<DiveLog>> GetByBuddyAsync(
        Guid buddyUserId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets diving statistics for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User diving statistics.</returns>
    Task<UserDivingStatistics> GetStatisticsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents diving statistics for a user.
/// </summary>
public record UserDivingStatistics
{
    /// <summary>
    /// Gets the total number of dives.
    /// </summary>
    public int TotalDives { get; init; }

    /// <summary>
    /// Gets the total dive time in minutes.
    /// </summary>
    public int TotalDiveTimeMinutes { get; init; }

    /// <summary>
    /// Gets the maximum depth reached.
    /// </summary>
    public Depth? MaxDepthReached { get; init; }

    /// <summary>
    /// Gets the average dive duration in minutes.
    /// </summary>
    public double AverageDiveDurationMinutes { get; init; }

    /// <summary>
    /// Gets the number of unique diving spots visited.
    /// </summary>
    public int UniqueSpotsVisited { get; init; }

    /// <summary>
    /// Gets the date of the first dive.
    /// </summary>
    public DateTime? FirstDiveDate { get; init; }

    /// <summary>
    /// Gets the date of the most recent dive.
    /// </summary>
    public DateTime? LastDiveDate { get; init; }
}
