namespace SubExplore.Domain.Events;

/// <summary>
/// Domain event raised when a new diving event is created.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="CreatedBy">The unique identifier of the user who created the event.</param>
/// <param name="OccurredOn">The date and time when the event occurred.</param>
public record EventCreatedEvent(
    Guid EventId,
    Guid CreatedBy,
    DateTime OccurredOn) : IDomainEvent;
