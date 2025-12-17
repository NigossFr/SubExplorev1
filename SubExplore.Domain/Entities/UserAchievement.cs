namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents an achievement unlocked by a specific user.
/// This is the junction entity linking users to their unlocked achievements.
/// </summary>
public class UserAchievement
{
    /// <summary>
    /// Gets the unique identifier for this user achievement record.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who unlocked the achievement.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the identifier of the unlocked achievement.
    /// </summary>
    public Guid AchievementId { get; private set; }

    /// <summary>
    /// Gets the date and time when the achievement was unlocked.
    /// </summary>
    public DateTime UnlockedAt { get; private set; }

    /// <summary>
    /// Gets the current progress value for progressive achievements.
    /// For example, if the achievement requires 100 dives, this could be 50.
    /// Null for non-progressive achievements.
    /// </summary>
    public int? Progress { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAchievement"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private UserAchievement()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAchievement"/> class.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="achievementId">The achievement identifier.</param>
    /// <param name="progress">The initial progress (optional).</param>
    private UserAchievement(Guid userId, Guid achievementId, int? progress = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        AchievementId = achievementId;
        UnlockedAt = DateTime.UtcNow;
        Progress = ValidateProgress(progress);
    }

    /// <summary>
    /// Creates a new user achievement record when a user unlocks an achievement.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="achievementId">The achievement identifier.</param>
    /// <param name="progress">The initial progress (optional).</param>
    /// <returns>A new <see cref="UserAchievement"/> instance.</returns>
    public static UserAchievement Create(Guid userId, Guid achievementId, int? progress = null)
    {
        return new UserAchievement(userId, achievementId, progress);
    }

    /// <summary>
    /// Updates the progress for progressive achievements.
    /// </summary>
    /// <param name="newProgress">The new progress value.</param>
    public void UpdateProgress(int newProgress)
    {
        Progress = ValidateProgress(newProgress);
    }

    /// <summary>
    /// Validates the progress value.
    /// </summary>
    /// <param name="progress">The progress to validate.</param>
    /// <returns>The validated progress.</returns>
    /// <exception cref="ArgumentException">Thrown when progress is invalid.</exception>
    private static int? ValidateProgress(int? progress)
    {
        if (!progress.HasValue)
        {
            return null;
        }

        if (progress.Value < 0)
        {
            throw new ArgumentException("Progress cannot be negative.", nameof(progress));
        }

        if (progress.Value > 1000000)
        {
            throw new ArgumentException("Progress cannot exceed 1000000.", nameof(progress));
        }

        return progress;
    }
}
