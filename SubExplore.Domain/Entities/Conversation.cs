namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a conversation between users (private or group).
/// </summary>
public class Conversation
{
    private readonly List<Guid> _participantIds = new();
    private readonly List<Message> _messages = new();

    /// <summary>
    /// Gets the unique identifier for the conversation.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the list of participant user IDs.
    /// </summary>
    public IReadOnlyCollection<Guid> ParticipantIds => _participantIds.AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether this is a group conversation.
    /// </summary>
    public bool IsGroupConversation { get; private set; }

    /// <summary>
    /// Gets the title of the conversation (required for group conversations).
    /// </summary>
    public string? Title { get; private set; }

    /// <summary>
    /// Gets the date and time of the last message.
    /// </summary>
    public DateTime? LastMessageAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the conversation was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the collection of messages in this conversation.
    /// </summary>
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    // Private constructor for EF Core
    private Conversation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Conversation"/> class.
    /// </summary>
    /// <param name="id">The conversation ID.</param>
    /// <param name="participantIds">The participant user IDs.</param>
    /// <param name="isGroupConversation">Whether this is a group conversation.</param>
    /// <param name="title">The conversation title (required for groups).</param>
    /// <param name="createdAt">The creation date and time.</param>
    private Conversation(
        Guid id,
        IEnumerable<Guid> participantIds,
        bool isGroupConversation,
        string? title,
        DateTime createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Conversation ID cannot be empty.", nameof(id));
        }

        if (participantIds == null || !participantIds.Any())
        {
            throw new ArgumentException("Conversation must have at least one participant.", nameof(participantIds));
        }

        var participantList = participantIds.ToList();

        if (participantList.Any(id => id == Guid.Empty))
        {
            throw new ArgumentException("Participant IDs cannot be empty.", nameof(participantIds));
        }

        if (participantList.Distinct().Count() != participantList.Count)
        {
            throw new ArgumentException("Participant IDs must be unique.", nameof(participantIds));
        }

        if (!isGroupConversation && participantList.Count != 2)
        {
            throw new ArgumentException("Private conversation must have exactly 2 participants.", nameof(participantIds));
        }

        if (isGroupConversation && participantList.Count < 2)
        {
            throw new ArgumentException("Group conversation must have at least 2 participants.", nameof(participantIds));
        }

        if (isGroupConversation && string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Group conversation must have a title.", nameof(title));
        }

        if (isGroupConversation && title != null && title.Length > 100)
        {
            throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
        }

        if (createdAt > DateTime.UtcNow)
        {
            throw new ArgumentException("CreatedAt cannot be in the future.", nameof(createdAt));
        }

        Id = id;
        _participantIds.AddRange(participantList);
        IsGroupConversation = isGroupConversation;
        Title = isGroupConversation ? title?.Trim() : null;
        CreatedAt = createdAt;
        LastMessageAt = null;
    }

    /// <summary>
    /// Creates a new private conversation between two users.
    /// </summary>
    /// <param name="user1Id">The first user ID.</param>
    /// <param name="user2Id">The second user ID.</param>
    /// <returns>A new private conversation instance.</returns>
    public static Conversation CreatePrivate(Guid user1Id, Guid user2Id)
    {
        if (user1Id == user2Id)
        {
            throw new ArgumentException("Cannot create conversation with the same user.");
        }

        return new Conversation(
            Guid.NewGuid(),
            new[] { user1Id, user2Id },
            false,
            null,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Creates a new group conversation.
    /// </summary>
    /// <param name="title">The group title.</param>
    /// <param name="participantIds">The participant user IDs.</param>
    /// <returns>A new group conversation instance.</returns>
    public static Conversation CreateGroup(string title, IEnumerable<Guid> participantIds)
    {
        return new Conversation(
            Guid.NewGuid(),
            participantIds,
            true,
            title,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Adds a participant to the conversation (group only).
    /// </summary>
    /// <param name="userId">The user ID to add.</param>
    public void AddParticipant(Guid userId)
    {
        if (!IsGroupConversation)
        {
            throw new InvalidOperationException("Cannot add participants to a private conversation.");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (_participantIds.Contains(userId))
        {
            throw new InvalidOperationException("User is already a participant in this conversation.");
        }

        _participantIds.Add(userId);
    }

    /// <summary>
    /// Removes a participant from the conversation (group only).
    /// </summary>
    /// <param name="userId">The user ID to remove.</param>
    public void RemoveParticipant(Guid userId)
    {
        if (!IsGroupConversation)
        {
            throw new InvalidOperationException("Cannot remove participants from a private conversation.");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (!_participantIds.Contains(userId))
        {
            throw new InvalidOperationException("User is not a participant in this conversation.");
        }

        if (_participantIds.Count <= 2)
        {
            throw new InvalidOperationException("Group conversation must have at least 2 participants.");
        }

        _participantIds.Remove(userId);
    }

    /// <summary>
    /// Updates the title of the conversation (group only).
    /// </summary>
    /// <param name="newTitle">The new title.</param>
    public void UpdateTitle(string newTitle)
    {
        if (!IsGroupConversation)
        {
            throw new InvalidOperationException("Cannot update title of a private conversation.");
        }

        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(newTitle));
        }

        if (newTitle.Length > 100)
        {
            throw new ArgumentException("Title cannot exceed 100 characters.", nameof(newTitle));
        }

        Title = newTitle.Trim();
    }

    /// <summary>
    /// Adds a message to the conversation.
    /// </summary>
    /// <param name="message">The message to add.</param>
    internal void AddMessage(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (message.ConversationId != Id)
        {
            throw new ArgumentException("Message does not belong to this conversation.", nameof(message));
        }

        if (!_participantIds.Contains(message.SenderId))
        {
            throw new InvalidOperationException("Sender is not a participant in this conversation.");
        }

        _messages.Add(message);
        LastMessageAt = message.SentAt;
    }

    /// <summary>
    /// Checks if a user is a participant in the conversation.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if the user is a participant, false otherwise.</returns>
    public bool IsParticipant(Guid userId)
    {
        return _participantIds.Contains(userId);
    }
}
