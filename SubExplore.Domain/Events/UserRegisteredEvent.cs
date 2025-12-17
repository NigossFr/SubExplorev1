namespace SubExplore.Domain.Events;

/// <summary>
/// Domain event raised when a new user registers in the system.
/// </summary>
/// <param name="UserId">The unique identifier of the newly registered user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="OccurredOn">The date and time when the event occurred.</param>
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    DateTime OccurredOn) : IDomainEvent;
