namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a photo of a diving spot.
/// This is a child entity that belongs to a DivingSpot aggregate.
/// </summary>
public class DivingSpotPhoto
{
    /// <summary>
    /// Gets the unique identifier for the photo.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the diving spot this photo belongs to.
    /// </summary>
    public Guid DivingSpotId { get; private set; }

    /// <summary>
    /// Gets the URL of the photo.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Gets the caption/description of the photo.
    /// </summary>
    public string? Caption { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who uploaded the photo.
    /// </summary>
    public Guid UploadedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the photo was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpotPhoto"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private DivingSpotPhoto()
    {
        Url = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpotPhoto"/> class.
    /// </summary>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="url">The photo URL.</param>
    /// <param name="caption">The photo caption.</param>
    /// <param name="uploadedBy">The user who uploaded the photo.</param>
    private DivingSpotPhoto(Guid divingSpotId, string url, string? caption, Guid uploadedBy)
    {
        Id = Guid.NewGuid();
        DivingSpotId = divingSpotId;
        Url = ValidateUrl(url);
        Caption = ValidateCaption(caption);
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new diving spot photo.
    /// </summary>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="url">The photo URL.</param>
    /// <param name="caption">The photo caption.</param>
    /// <param name="uploadedBy">The user who uploaded the photo.</param>
    /// <returns>A new <see cref="DivingSpotPhoto"/> instance.</returns>
    public static DivingSpotPhoto Create(Guid divingSpotId, string url, string? caption, Guid uploadedBy)
    {
        return new DivingSpotPhoto(divingSpotId, url, caption, uploadedBy);
    }

    /// <summary>
    /// Updates the photo caption.
    /// </summary>
    /// <param name="newCaption">The new caption.</param>
    public void UpdateCaption(string? newCaption)
    {
        Caption = ValidateCaption(newCaption);
    }

    /// <summary>
    /// Validates the photo URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>The validated URL.</returns>
    /// <exception cref="ArgumentException">Thrown when URL is invalid.</exception>
    private static string ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Photo URL cannot be null or empty.", nameof(url));
        }

        url = url.Trim();

        if (url.Length > 500)
        {
            throw new ArgumentException("Photo URL cannot exceed 500 characters.", nameof(url));
        }

        return url;
    }

    /// <summary>
    /// Validates the photo caption.
    /// </summary>
    /// <param name="caption">The caption to validate.</param>
    /// <returns>The validated caption.</returns>
    /// <exception cref="ArgumentException">Thrown when caption exceeds maximum length.</exception>
    private static string? ValidateCaption(string? caption)
    {
        if (string.IsNullOrWhiteSpace(caption))
        {
            return null;
        }

        caption = caption.Trim();

        if (caption.Length > 200)
        {
            throw new ArgumentException("Photo caption cannot exceed 200 characters.", nameof(caption));
        }

        return caption;
    }
}
