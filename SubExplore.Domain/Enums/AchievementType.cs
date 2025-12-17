namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the type/category of an achievement.
/// </summary>
public enum AchievementType
{
    /// <summary>
    /// Achievements related to depth records (deepest dive, depth milestones).
    /// </summary>
    Depth = 0,

    /// <summary>
    /// Achievements related to number of dives (first dive, 10 dives, 100 dives, etc.).
    /// </summary>
    DiveCount = 1,

    /// <summary>
    /// Achievements related to total dive time or experience.
    /// </summary>
    Experience = 2,

    /// <summary>
    /// Achievements related to exploring different diving spots.
    /// </summary>
    Exploration = 3,

    /// <summary>
    /// Achievements related to social interactions (events, buddies, community).
    /// </summary>
    Social = 4,

    /// <summary>
    /// Achievements related to conservation and environmental actions.
    /// </summary>
    Conservation = 5,

    /// <summary>
    /// Achievements related to education, training, and certifications.
    /// </summary>
    Education = 6,

    /// <summary>
    /// Achievements related to safety records and practices.
    /// </summary>
    Safety = 7
}
