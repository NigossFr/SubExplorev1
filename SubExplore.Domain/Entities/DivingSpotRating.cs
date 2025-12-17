namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a rating/review of a diving spot.
/// This is a child entity that belongs to a DivingSpot aggregate.
/// </summary>
public class DivingSpotRating
{
    /// <summary>
    /// Gets the unique identifier for the rating.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the diving spot this rating belongs to.
    /// </summary>
    public Guid DivingSpotId { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who submitted the rating.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the rating score (1-5 stars).
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// Gets the review comment.
    /// </summary>
    public string? Comment { get; private set; }

    /// <summary>
    /// Gets the date and time when the rating was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the rating was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpotRating"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private DivingSpotRating()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivingSpotRating"/> class.
    /// </summary>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="score">The rating score.</param>
    /// <param name="comment">The review comment.</param>
    private DivingSpotRating(Guid divingSpotId, Guid userId, int score, string? comment)
    {
        Id = Guid.NewGuid();
        DivingSpotId = divingSpotId;
        UserId = userId;
        Score = ValidateScore(score);
        Comment = ValidateComment(comment);
        SubmittedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new diving spot rating.
    /// </summary>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="score">The rating score (1-5).</param>
    /// <param name="comment">The review comment.</param>
    /// <returns>A new <see cref="DivingSpotRating"/> instance.</returns>
    public static DivingSpotRating Create(Guid divingSpotId, Guid userId, int score, string? comment)
    {
        return new DivingSpotRating(divingSpotId, userId, score, comment);
    }

    /// <summary>
    /// Updates the rating score and comment.
    /// </summary>
    /// <param name="newScore">The new rating score.</param>
    /// <param name="newComment">The new review comment.</param>
    public void Update(int newScore, string? newComment)
    {
        Score = ValidateScore(newScore);
        Comment = ValidateComment(newComment);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the rating score.
    /// </summary>
    /// <param name="score">The score to validate.</param>
    /// <returns>The validated score.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when score is not between 1 and 5.</exception>
    private static int ValidateScore(int score)
    {
        if (score < 1 || score > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(score),
                score,
                "Rating score must be between 1 and 5.");
        }

        return score;
    }

    /// <summary>
    /// Validates the review comment.
    /// </summary>
    /// <param name="comment">The comment to validate.</param>
    /// <returns>The validated comment.</returns>
    /// <exception cref="ArgumentException">Thrown when comment exceeds maximum length.</exception>
    private static string? ValidateComment(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return null;
        }

        comment = comment.Trim();

        if (comment.Length > 1000)
        {
            throw new ArgumentException("Rating comment cannot exceed 1000 characters.", nameof(comment));
        }

        return comment;
    }
}
