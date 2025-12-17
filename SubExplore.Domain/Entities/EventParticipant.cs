namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a participant registration for an event.
/// This is a child entity that belongs to an Event aggregate.
/// </summary>
public class EventParticipant
{
    /// <summary>
    /// Gets the unique identifier for the participant registration.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the event.
    /// </summary>
    public Guid EventId { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who registered.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the date and time when the user registered for the event.
    /// </summary>
    public DateTime RegisteredAt { get; private set; }

    /// <summary>
    /// Gets the optional comment from the participant.
    /// </summary>
    public string? Comment { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventParticipant"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private EventParticipant()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventParticipant"/> class.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="comment">The optional comment.</param>
    private EventParticipant(Guid eventId, Guid userId, string? comment)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        UserId = userId;
        Comment = ValidateComment(comment);
        RegisteredAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new event participant registration.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="comment">The optional comment.</param>
    /// <returns>A new <see cref="EventParticipant"/> instance.</returns>
    public static EventParticipant Create(Guid eventId, Guid userId, string? comment)
    {
        return new EventParticipant(eventId, userId, comment);
    }

    /// <summary>
    /// Updates the participant's comment.
    /// </summary>
    /// <param name="newComment">The new comment.</param>
    public void UpdateComment(string? newComment)
    {
        Comment = ValidateComment(newComment);
    }

    /// <summary>
    /// Validates the comment.
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

        if (comment.Length > 500)
        {
            throw new ArgumentException("Participant comment cannot exceed 500 characters.", nameof(comment));
        }

        return comment;
    }
}
