namespace SubExplore.Domain.Services;

/// <summary>
/// Service interface for achievement and badge management.
/// </summary>
public interface IAchievementService
{
    /// <summary>
    /// Checks and unlocks achievements for a user based on their activities.
    /// </summary>
    /// <param name="userId">User ID to check achievements for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of newly unlocked achievement IDs.</returns>
    Task<IReadOnlyList<Guid>> CheckAndUnlockAchievementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a specific achievement should be unlocked for a user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="achievementId">Achievement ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the achievement was unlocked; otherwise, false.</returns>
    Task<bool> TryUnlockAchievementAsync(
        Guid userId,
        Guid achievementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user's progress towards a specific achievement.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="achievementId">Achievement ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Achievement progress data.</returns>
    Task<AchievementProgress> GetProgressAsync(
        Guid userId,
        Guid achievementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all achievements with the user's progress.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of achievements with progress.</returns>
    Task<IReadOnlyList<AchievementProgress>> GetAllProgressAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements unlocked by a user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of unlocked achievement IDs with unlock dates.</returns>
    Task<IReadOnlyList<UnlockedAchievement>> GetUnlockedAchievementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the total points earned by a user from achievements.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total achievement points.</returns>
    Task<int> GetTotalPointsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a user's progress towards an achievement.
/// </summary>
public record AchievementProgress
{
    /// <summary>
    /// Gets the achievement ID.
    /// </summary>
    public required Guid AchievementId { get; init; }

    /// <summary>
    /// Gets the achievement name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the achievement description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the current progress value.
    /// </summary>
    public required int CurrentProgress { get; init; }

    /// <summary>
    /// Gets the target value to unlock the achievement.
    /// </summary>
    public required int TargetValue { get; init; }

    /// <summary>
    /// Gets whether the achievement is unlocked.
    /// </summary>
    public required bool IsUnlocked { get; init; }

    /// <summary>
    /// Gets the unlock date if the achievement is unlocked.
    /// </summary>
    public DateTime? UnlockedAt { get; init; }

    /// <summary>
    /// Gets the achievement points value.
    /// </summary>
    public required int Points { get; init; }

    /// <summary>
    /// Gets the achievement category.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the icon URL or identifier.
    /// </summary>
    public string? IconUrl { get; init; }

    /// <summary>
    /// Gets the progress percentage (0-100).
    /// </summary>
    public double ProgressPercentage => TargetValue > 0
        ? Math.Min(100, (CurrentProgress / (double)TargetValue) * 100)
        : 0;
}

/// <summary>
/// Represents an unlocked achievement.
/// </summary>
public record UnlockedAchievement
{
    /// <summary>
    /// Gets the achievement ID.
    /// </summary>
    public required Guid AchievementId { get; init; }

    /// <summary>
    /// Gets the achievement name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the unlock date.
    /// </summary>
    public required DateTime UnlockedAt { get; init; }

    /// <summary>
    /// Gets the achievement points.
    /// </summary>
    public required int Points { get; init; }
}
