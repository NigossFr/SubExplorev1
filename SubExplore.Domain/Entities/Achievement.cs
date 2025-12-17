using SubExplore.Domain.Enums;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents an achievement template that can be unlocked by users.
/// This is a catalog/definition of achievements available in the system.
/// </summary>
public class Achievement
{
    /// <summary>
    /// Gets the unique identifier for the achievement.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the title of the achievement.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the description explaining how to unlock this achievement.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the type/category of the achievement.
    /// </summary>
    public AchievementType Type { get; private set; }

    /// <summary>
    /// Gets the rarity/difficulty tier of the achievement.
    /// </summary>
    public AchievementCategory Category { get; private set; }

    /// <summary>
    /// Gets the points awarded when this achievement is unlocked.
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    /// Gets the URL to the achievement icon (optional).
    /// </summary>
    public string? IconUrl { get; private set; }

    /// <summary>
    /// Gets the required value for progressive achievements (e.g., 100 for "100 Dives").
    /// Null for non-progressive achievements.
    /// </summary>
    public int? RequiredValue { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this achievement is secret/hidden until unlocked.
    /// </summary>
    public bool IsSecret { get; private set; }

    /// <summary>
    /// Gets the date and time when the achievement was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the achievement was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Achievement"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private Achievement()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Achievement"/> class.
    /// </summary>
    /// <param name="title">The achievement title.</param>
    /// <param name="description">The achievement description.</param>
    /// <param name="type">The achievement type.</param>
    /// <param name="category">The achievement category/tier.</param>
    /// <param name="points">The points awarded.</param>
    /// <param name="requiredValue">The required value for progressive achievements (optional).</param>
    /// <param name="isSecret">Whether the achievement is secret.</param>
    private Achievement(
        string title,
        string description,
        AchievementType type,
        AchievementCategory category,
        int points,
        int? requiredValue = null,
        bool isSecret = false)
    {
        Id = Guid.NewGuid();
        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        Type = type;
        Category = category;
        Points = ValidatePoints(points);
        RequiredValue = ValidateRequiredValue(requiredValue);
        IsSecret = isSecret;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new achievement.
    /// </summary>
    /// <param name="title">The achievement title.</param>
    /// <param name="description">The achievement description.</param>
    /// <param name="type">The achievement type.</param>
    /// <param name="category">The achievement category/tier.</param>
    /// <param name="points">The points awarded.</param>
    /// <param name="requiredValue">The required value for progressive achievements (optional).</param>
    /// <param name="isSecret">Whether the achievement is secret.</param>
    /// <returns>A new <see cref="Achievement"/> instance.</returns>
    public static Achievement Create(
        string title,
        string description,
        AchievementType type,
        AchievementCategory category,
        int points,
        int? requiredValue = null,
        bool isSecret = false)
    {
        return new Achievement(title, description, type, category, points, requiredValue, isSecret);
    }

    /// <summary>
    /// Updates the achievement details.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="points">The new points value.</param>
    public void UpdateDetails(string title, string description, int points)
    {
        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        Points = ValidatePoints(points);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the icon URL for the achievement.
    /// </summary>
    /// <param name="iconUrl">The icon URL.</param>
    public void SetIconUrl(string? iconUrl)
    {
        IconUrl = ValidateIconUrl(iconUrl);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the required value for progressive achievements.
    /// </summary>
    /// <param name="requiredValue">The new required value.</param>
    public void UpdateRequiredValue(int? requiredValue)
    {
        RequiredValue = ValidateRequiredValue(requiredValue);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Toggles the secret status of the achievement.
    /// </summary>
    public void ToggleSecret()
    {
        IsSecret = !IsSecret;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the achievement title.
    /// </summary>
    /// <param name="title">The title to validate.</param>
    /// <returns>The validated title.</returns>
    /// <exception cref="ArgumentException">Thrown when title is invalid.</exception>
    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Achievement title cannot be null or empty.", nameof(title));
        }

        title = title.Trim();

        if (title.Length < 3)
        {
            throw new ArgumentException("Achievement title must be at least 3 characters.", nameof(title));
        }

        if (title.Length > 100)
        {
            throw new ArgumentException("Achievement title cannot exceed 100 characters.", nameof(title));
        }

        return title;
    }

    /// <summary>
    /// Validates the achievement description.
    /// </summary>
    /// <param name="description">The description to validate.</param>
    /// <returns>The validated description.</returns>
    /// <exception cref="ArgumentException">Thrown when description is invalid.</exception>
    private static string ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Achievement description cannot be null or empty.", nameof(description));
        }

        description = description.Trim();

        if (description.Length < 10)
        {
            throw new ArgumentException("Achievement description must be at least 10 characters.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Achievement description cannot exceed 500 characters.", nameof(description));
        }

        return description;
    }

    /// <summary>
    /// Validates the points value.
    /// </summary>
    /// <param name="points">The points to validate.</param>
    /// <returns>The validated points.</returns>
    /// <exception cref="ArgumentException">Thrown when points is invalid.</exception>
    private static int ValidatePoints(int points)
    {
        if (points < 0)
        {
            throw new ArgumentException("Achievement points cannot be negative.", nameof(points));
        }

        if (points > 10000)
        {
            throw new ArgumentException("Achievement points cannot exceed 10000.", nameof(points));
        }

        return points;
    }

    /// <summary>
    /// Validates the icon URL.
    /// </summary>
    /// <param name="iconUrl">The icon URL to validate.</param>
    /// <returns>The validated icon URL.</returns>
    /// <exception cref="ArgumentException">Thrown when icon URL is invalid.</exception>
    private static string? ValidateIconUrl(string? iconUrl)
    {
        if (string.IsNullOrWhiteSpace(iconUrl))
        {
            return null;
        }

        iconUrl = iconUrl.Trim();

        if (iconUrl.Length > 500)
        {
            throw new ArgumentException("Icon URL cannot exceed 500 characters.", nameof(iconUrl));
        }

        return iconUrl;
    }

    /// <summary>
    /// Validates the required value.
    /// </summary>
    /// <param name="requiredValue">The required value to validate.</param>
    /// <returns>The validated required value.</returns>
    /// <exception cref="ArgumentException">Thrown when required value is invalid.</exception>
    private static int? ValidateRequiredValue(int? requiredValue)
    {
        if (!requiredValue.HasValue)
        {
            return null;
        }

        if (requiredValue.Value < 1)
        {
            throw new ArgumentException("Required value must be at least 1.", nameof(requiredValue));
        }

        if (requiredValue.Value > 1000000)
        {
            throw new ArgumentException("Required value cannot exceed 1000000.", nameof(requiredValue));
        }

        return requiredValue;
    }
}
