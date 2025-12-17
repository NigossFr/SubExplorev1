using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a user of the SubExplore application.
/// </summary>
public class User
{
    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the user's email address (unique).
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the user's username (unique).
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// Gets the user's profile information.
    /// </summary>
    public UserProfile Profile { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user has a premium subscription.
    /// </summary>
    public bool IsPremium { get; private set; }

    /// <summary>
    /// Gets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the premium subscription started (null if not premium).
    /// </summary>
    public DateTime? PremiumSince { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private User()
    {
        Email = string.Empty;
        Username = string.Empty;
        Profile = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="username">The user's username.</param>
    /// <param name="profile">The user's profile information.</param>
    private User(string email, string username, UserProfile profile)
    {
        Id = Guid.NewGuid();
        Email = ValidateEmail(email);
        Username = ValidateUsername(username);
        Profile = profile;
        IsPremium = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        PremiumSince = null;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="username">The user's username.</param>
    /// <param name="profile">The user's profile information.</param>
    /// <returns>A new <see cref="User"/> instance.</returns>
    public static User Create(string email, string username, UserProfile profile)
    {
        return new User(email, username, profile);
    }

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    /// <param name="newProfile">The new profile information.</param>
    /// <exception cref="ArgumentNullException">Thrown when newProfile is null.</exception>
    public void UpdateProfile(UserProfile newProfile)
    {
        if (newProfile.Equals(default(UserProfile)))
        {
            throw new ArgumentNullException(nameof(newProfile), "Profile cannot be null.");
        }

        Profile = newProfile;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Upgrades the user to premium subscription.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is already premium.</exception>
    public void UpgradeToPremium()
    {
        if (IsPremium)
        {
            throw new InvalidOperationException("User is already premium.");
        }

        IsPremium = true;
        PremiumSince = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Downgrades the user from premium subscription.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is not premium.</exception>
    public void DowngradeToPremium()
    {
        if (!IsPremium)
        {
            throw new InvalidOperationException("User is not premium.");
        }

        IsPremium = false;
        PremiumSince = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's email address.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    public void UpdateEmail(string newEmail)
    {
        Email = ValidateEmail(newEmail);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's username.
    /// </summary>
    /// <param name="newUsername">The new username.</param>
    public void UpdateUsername(string newUsername)
    {
        Username = ValidateUsername(newUsername);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates an email address.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>The validated email.</returns>
    /// <exception cref="ArgumentException">Thrown when email is invalid.</exception>
    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        email = email.Trim().ToLowerInvariant();

        if (!email.Contains('@') || !email.Contains('.'))
        {
            throw new ArgumentException("Email must be in valid format.", nameof(email));
        }

        if (email.Length > 100)
        {
            throw new ArgumentException("Email cannot exceed 100 characters.", nameof(email));
        }

        return email;
    }

    /// <summary>
    /// Validates a username.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <returns>The validated username.</returns>
    /// <exception cref="ArgumentException">Thrown when username is invalid.</exception>
    private static string ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }

        username = username.Trim();

        if (username.Length < 3)
        {
            throw new ArgumentException("Username must be at least 3 characters.", nameof(username));
        }

        if (username.Length > 30)
        {
            throw new ArgumentException("Username cannot exceed 30 characters.", nameof(username));
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
        {
            throw new ArgumentException(
                "Username can only contain letters, numbers, underscores, and hyphens.",
                nameof(username));
        }

        return username;
    }
}
