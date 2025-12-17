namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a message in a conversation.
/// </summary>
public class Message
{
    private readonly List<Guid> _readByUserIds = new();

    /// <summary>
    /// Gets the unique identifier for the message.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the ID of the conversation this message belongs to.
    /// </summary>
    public Guid ConversationId { get; private set; }

    /// <summary>
    /// Gets the ID of the user who sent the message.
    /// </summary>
    public Guid SenderId { get; private set; }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Gets the date and time when the message was sent.
    /// </summary>
    public DateTime SentAt { get; private set; }

    /// <summary>
    /// Gets the collection of user IDs who have read this message.
    /// </summary>
    public IReadOnlyCollection<Guid> ReadByUserIds => _readByUserIds.AsReadOnly();

    // Private constructor for EF Core
    private Message()
    {
        Content = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="id">The message ID.</param>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="senderId">The sender user ID.</param>
    /// <param name="content">The message content.</param>
    /// <param name="sentAt">The sent date and time.</param>
    private Message(
        Guid id,
        Guid conversationId,
        Guid senderId,
        string content,
        DateTime sentAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Message ID cannot be empty.", nameof(id));
        }

        if (conversationId == Guid.Empty)
        {
            throw new ArgumentException("Conversation ID cannot be empty.", nameof(conversationId));
        }

        if (senderId == Guid.Empty)
        {
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty.", nameof(content));
        }

        if (content.Length > 2000)
        {
            throw new ArgumentException("Message content cannot exceed 2000 characters.", nameof(content));
        }

        if (sentAt > DateTime.UtcNow)
        {
            throw new ArgumentException("SentAt cannot be in the future.", nameof(sentAt));
        }

        Id = id;
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content.Trim();
        SentAt = sentAt;
        _readByUserIds.Add(senderId); // Sender automatically reads their own message
    }

    /// <summary>
    /// Creates a new message.
    /// </summary>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="senderId">The sender user ID.</param>
    /// <param name="content">The message content.</param>
    /// <returns>A new message instance.</returns>
    public static Message Create(Guid conversationId, Guid senderId, string content)
    {
        return new Message(
            Guid.NewGuid(),
            conversationId,
            senderId,
            content,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Marks the message as read by a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    public void MarkAsReadBy(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (_readByUserIds.Contains(userId))
        {
            return; // Already read, no error
        }

        _readByUserIds.Add(userId);
    }

    /// <summary>
    /// Checks if the message has been read by a specific user.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if the message has been read by the user, false otherwise.</returns>
    public bool IsReadBy(Guid userId)
    {
        return _readByUserIds.Contains(userId);
    }

    /// <summary>
    /// Updates the content of the message.
    /// </summary>
    /// <param name="newContent">The new content.</param>
    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
        {
            throw new ArgumentException("Message content cannot be empty.", nameof(newContent));
        }

        if (newContent.Length > 2000)
        {
            throw new ArgumentException("Message content cannot exceed 2000 characters.", nameof(newContent));
        }

        Content = newContent.Trim();
    }
}
