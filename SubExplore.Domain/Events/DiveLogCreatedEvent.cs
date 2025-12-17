namespace SubExplore.Domain.Events;

/// <summary>
/// Domain event raised when a new dive log is created.
/// </summary>
/// <param name="DiveLogId">The unique identifier of the dive log.</param>
/// <param name="UserId">The unique identifier of the user who created the dive log.</param>
/// <param name="SpotId">The unique identifier of the diving spot.</param>
/// <param name="OccurredOn">The date and time when the event occurred.</param>
public record DiveLogCreatedEvent(
    Guid DiveLogId,
    Guid UserId,
    Guid SpotId,
    DateTime OccurredOn) : IDomainEvent;
