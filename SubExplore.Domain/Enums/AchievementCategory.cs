namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the rarity/difficulty tier of an achievement.
/// </summary>
public enum AchievementCategory
{
    /// <summary>
    /// Bronze tier - Common achievements for beginners.
    /// </summary>
    Bronze = 0,

    /// <summary>
    /// Silver tier - Intermediate achievements.
    /// </summary>
    Silver = 1,

    /// <summary>
    /// Gold tier - Advanced achievements.
    /// </summary>
    Gold = 2,

    /// <summary>
    /// Platinum tier - Expert achievements.
    /// </summary>
    Platinum = 3,

    /// <summary>
    /// Diamond tier - Legendary achievements for the most dedicated divers.
    /// </summary>
    Diamond = 4
}
