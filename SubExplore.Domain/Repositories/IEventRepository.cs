using SubExplore.Domain.Entities;
using SubExplore.Domain.Enums;

namespace SubExplore.Domain.Repositories;

/// <summary>
/// Repository interface for Event entity operations.
/// </summary>
public interface IEventRepository : IRepository<Event>
{
    /// <summary>
    /// Gets upcoming events (future events).
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of upcoming events ordered by date.</returns>
    Task<IReadOnlyList<Event>> GetUpcomingAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets past events (completed events).
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of past events ordered by date descending.</returns>
    Task<IReadOnlyList<Event>> GetPastAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events organized by a specific user.
    /// </summary>
    /// <param name="organizerId">The organizer user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of events organized by the user.</returns>
    Task<IReadOnlyList<Event>> GetByOrganizerAsync(
        Guid organizerId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events where a specific user is a participant.
    /// </summary>
    /// <param name="userId">The participant user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of events with the user as participant.</returns>
    Task<IReadOnlyList<Event>> GetByParticipantAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events filtered by status.
    /// </summary>
    /// <param name="status">The event status.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of events matching the status.</returns>
    Task<IReadOnlyList<Event>> GetByStatusAsync(
        EventStatus status,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events associated with a specific diving spot.
    /// </summary>
    /// <param name="divingSpotId">The diving spot ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of events at the diving spot.</returns>
    Task<IReadOnlyList<Event>> GetByDivingSpotAsync(
        Guid divingSpotId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for events by title or description.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching events.</returns>
    Task<IReadOnlyList<Event>> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events with available spots (not full).
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of events with available spots.</returns>
    Task<IReadOnlyList<Event>> GetWithAvailableSpotsAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
