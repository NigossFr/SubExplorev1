using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a diving spot location.
/// This is an aggregate root that manages its photos and ratings.
/// </summary>
public class DivingSpot
{
    private readonly List<DivingSpotPhoto> _photos = new();
    private readonly List<DivingSpotRating> _ratings = new();

    /// <summary>
    /// Gets the unique identifier for the diving spot.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the name of the diving spot.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the description of the diving spot.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the geographic coordinates of the diving spot.
    /// </summary>
    public Coordinates Location { get; private set; }

    /// <summary>
    /// Gets the current water temperature (can be updated from weather API).
    /// </summary>
    public WaterTemperature? CurrentTemperature { get; private set; }

    /// <summary>
    /// Gets the current visibility (can be updated from weather API).
    /// </summary>
    public Visibility? CurrentVisibility { get; private set; }

    /// <summary>
    /// Gets the maximum depth at this diving spot.
    /// </summary>
    public Depth? MaximumDepth { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who created this diving spot.
    /// </summary>
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the diving spot was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the diving spot was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the photos of the diving spot (read-only collection).
    /// </summary>
    public IReadOnlyCollection<DivingSpotPhoto> Photos => _photos.AsReadOnly();

    /// <summary>
    /// Gets the ratings of the diving spot (read-only collection).
    /// </summary>
    public IReadOnlyCollection<DivingSpotRating> Ratings => _ratings.AsReadOnly();

    /// <summary>
    /// Gets the average rating score.
    /// </summary>
    public double AverageRating => _ratings.Any() ? _ratings.Average(r => r.Score) : 0.0;

    /// <summary>
    /// Gets the total number of ratings.
    /// </summary>
    public int TotalRatings => _ratings.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpot"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private DivingSpot()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpot"/> class.
    /// </summary>
    /// <param name="name">The name of the diving spot.</param>
    /// <param name="description">The description of the diving spot.</param>
    /// <param name="location">The geographic coordinates.</param>
    /// <param name="createdBy">The user who created this spot.</param>
    /// <param name="maximumDepth">The maximum depth (optional).</param>
    private DivingSpot(
        string name,
        string description,
        Coordinates location,
        Guid createdBy,
        Depth? maximumDepth = null)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
        Description = ValidateDescription(description);
        Location = location;
        CreatedBy = createdBy;
        MaximumDepth = maximumDepth;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new diving spot.
    /// </summary>
    /// <param name="name">The name of the diving spot.</param>
    /// <param name="description">The description of the diving spot.</param>
    /// <param name="location">The geographic coordinates.</param>
    /// <param name="createdBy">The user who created this spot.</param>
    /// <param name="maximumDepth">The maximum depth (optional).</param>
    /// <returns>A new <see cref="DivingSpot"/> instance.</returns>
    public static DivingSpot Create(
        string name,
        string description,
        Coordinates location,
        Guid createdBy,
        Depth? maximumDepth = null)
    {
        return new DivingSpot(name, description, location, createdBy, maximumDepth);
    }

    /// <summary>
    /// Updates the diving spot information.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    public void UpdateInformation(string name, string description)
    {
        Name = ValidateName(name);
        Description = ValidateDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the current diving conditions (temperature and visibility).
    /// Typically called when receiving data from weather API.
    /// </summary>
    /// <param name="temperature">The current water temperature.</param>
    /// <param name="visibility">The current visibility.</param>
    public void UpdateConditions(WaterTemperature temperature, Visibility visibility)
    {
        CurrentTemperature = temperature;
        CurrentVisibility = visibility;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the maximum depth of the diving spot.
    /// </summary>
    /// <param name="depth">The new maximum depth.</param>
    public void UpdateMaximumDepth(Depth depth)
    {
        MaximumDepth = depth;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a photo to the diving spot.
    /// </summary>
    /// <param name="url">The photo URL.</param>
    /// <param name="caption">The photo caption.</param>
    /// <param name="uploadedBy">The user uploading the photo.</param>
    /// <returns>The newly created photo.</returns>
    public DivingSpotPhoto AddPhoto(string url, string? caption, Guid uploadedBy)
    {
        var photo = DivingSpotPhoto.Create(Id, url, caption, uploadedBy);
        _photos.Add(photo);
        UpdatedAt = DateTime.UtcNow;
        return photo;
    }

    /// <summary>
    /// Removes a photo from the diving spot.
    /// </summary>
    /// <param name="photoId">The photo identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when photo is not found.</exception>
    public void RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null)
        {
            throw new InvalidOperationException($"Photo with ID {photoId} not found.");
        }

        _photos.Remove(photo);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds or updates a rating for the diving spot.
    /// If the user has already rated this spot, their rating is updated.
    /// </summary>
    /// <param name="userId">The user submitting the rating.</param>
    /// <param name="score">The rating score (1-5).</param>
    /// <param name="comment">The review comment.</param>
    /// <returns>The rating (new or updated).</returns>
    public DivingSpotRating Rate(Guid userId, int score, string? comment)
    {
        var existingRating = _ratings.FirstOrDefault(r => r.UserId == userId);

        if (existingRating != null)
        {
            existingRating.Update(score, comment);
            UpdatedAt = DateTime.UtcNow;
            return existingRating;
        }

        var newRating = DivingSpotRating.Create(Id, userId, score, comment);
        _ratings.Add(newRating);
        UpdatedAt = DateTime.UtcNow;
        return newRating;
    }

    /// <summary>
    /// Removes a rating from the diving spot.
    /// </summary>
    /// <param name="userId">The user whose rating to remove.</param>
    /// <exception cref="InvalidOperationException">Thrown when rating is not found.</exception>
    public void RemoveRating(Guid userId)
    {
        var rating = _ratings.FirstOrDefault(r => r.UserId == userId);
        if (rating == null)
        {
            throw new InvalidOperationException($"Rating by user {userId} not found.");
        }

        _ratings.Remove(rating);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the diving spot name.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>The validated name.</returns>
    /// <exception cref="ArgumentException">Thrown when name is invalid.</exception>
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Diving spot name cannot be null or empty.", nameof(name));
        }

        name = name.Trim();

        if (name.Length < 3)
        {
            throw new ArgumentException("Diving spot name must be at least 3 characters.", nameof(name));
        }

        if (name.Length > 100)
        {
            throw new ArgumentException("Diving spot name cannot exceed 100 characters.", nameof(name));
        }

        return name;
    }

    /// <summary>
    /// Validates the diving spot description.
    /// </summary>
    /// <param name="description">The description to validate.</param>
    /// <returns>The validated description.</returns>
    /// <exception cref="ArgumentException">Thrown when description is invalid.</exception>
    private static string ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Diving spot description cannot be null or empty.", nameof(description));
        }

        description = description.Trim();

        if (description.Length < 10)
        {
            throw new ArgumentException("Diving spot description must be at least 10 characters.", nameof(description));
        }

        if (description.Length > 2000)
        {
            throw new ArgumentException("Diving spot description cannot exceed 2000 characters.", nameof(description));
        }

        return description;
    }
}
