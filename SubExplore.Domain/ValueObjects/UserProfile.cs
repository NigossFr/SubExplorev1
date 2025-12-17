namespace SubExplore.Domain.ValueObjects;

/// <summary>
/// Value object representing a user's profile information.
/// </summary>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Bio">User's biography (optional).</param>
/// <param name="ProfilePictureUrl">URL to user's profile picture (optional).</param>
public readonly record struct UserProfile(
    string FirstName,
    string LastName,
    string? Bio,
    string? ProfilePictureUrl)
{
    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; init; } = ValidateFirstName(FirstName);

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName { get; init; } = ValidateLastName(LastName);

    /// <summary>
    /// Gets the user's biography (optional).
    /// </summary>
    public string? Bio { get; init; } = ValidateBio(Bio);

    /// <summary>
    /// Gets the URL to the user's profile picture (optional).
    /// </summary>
    public string? ProfilePictureUrl { get; init; } = ProfilePictureUrl;

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Creates a new UserProfile with updated values.
    /// </summary>
    /// <param name="firstName">New first name (null to keep current).</param>
    /// <param name="lastName">New last name (null to keep current).</param>
    /// <param name="bio">New bio (null to keep current).</param>
    /// <param name="profilePictureUrl">New profile picture URL (null to keep current).</param>
    /// <returns>A new UserProfile with updated values.</returns>
    public UserProfile With(
        string? firstName = null,
        string? lastName = null,
        string? bio = null,
        string? profilePictureUrl = null)
    {
        return new UserProfile(
            firstName ?? FirstName,
            lastName ?? LastName,
            bio ?? Bio,
            profilePictureUrl ?? ProfilePictureUrl
        );
    }

    /// <summary>
    /// Validates the first name.
    /// </summary>
    /// <param name="firstName">The first name to validate.</param>
    /// <returns>The validated first name.</returns>
    /// <exception cref="ArgumentException">Thrown when first name is invalid.</exception>
    private static string ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
        }

        if (firstName.Length > 50)
        {
            throw new ArgumentException("First name cannot exceed 50 characters.", nameof(firstName));
        }

        return firstName.Trim();
    }

    /// <summary>
    /// Validates the last name.
    /// </summary>
    /// <param name="lastName">The last name to validate.</param>
    /// <returns>The validated last name.</returns>
    /// <exception cref="ArgumentException">Thrown when last name is invalid.</exception>
    private static string ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        }

        if (lastName.Length > 50)
        {
            throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(lastName));
        }

        return lastName.Trim();
    }

    /// <summary>
    /// Validates the biography.
    /// </summary>
    /// <param name="bio">The biography to validate.</param>
    /// <returns>The validated biography.</returns>
    /// <exception cref="ArgumentException">Thrown when bio exceeds maximum length.</exception>
    private static string? ValidateBio(string? bio)
    {
        if (string.IsNullOrWhiteSpace(bio))
        {
            return null;
        }

        if (bio.Length > 500)
        {
            throw new ArgumentException("Biography cannot exceed 500 characters.", nameof(bio));
        }

        return bio.Trim();
    }

    /// <summary>
    /// Returns a string representation of the user profile.
    /// </summary>
    /// <returns>The user's full name.</returns>
    public override string ToString() => FullName;
}
