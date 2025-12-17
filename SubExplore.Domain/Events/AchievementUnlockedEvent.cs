namespace SubExplore.Domain.Events;

/// <summary>
/// Domain event raised when a user unlocks an achievement.
/// </summary>
/// <param name="UserId">The unique identifier of the user who unlocked the achievement.</param>
/// <param name="AchievementId">The unique identifier of the unlocked achievement.</param>
/// <param name="OccurredOn">The date and time when the event occurred.</param>
public record AchievementUnlockedEvent(
    Guid UserId,
    Guid AchievementId,
    DateTime OccurredOn) : IDomainEvent;
