using SubExplore.Domain.Enums;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class Notification
{
    /// <summary>
    /// Gets the unique identifier for the notification.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the ID of the user who receives this notification.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the type of the notification.
    /// </summary>
    public NotificationType Type { get; private set; }

    /// <summary>
    /// Gets the title of the notification.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the message content of the notification.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the notification has been read.
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// Gets the priority level of the notification.
    /// </summary>
    public NotificationPriority Priority { get; private set; }

    /// <summary>
    /// Gets the date and time when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the notification was read (null if unread).
    /// </summary>
    public DateTime? ReadAt { get; private set; }

    /// <summary>
    /// Gets the optional reference ID (e.g., EventId, MessageId, AchievementId).
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    // Private constructor for EF Core
    private Notification()
    {
        Title = string.Empty;
        Message = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Notification"/> class.
    /// </summary>
    /// <param name="id">The notification ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="type">The notification type.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="priority">The notification priority.</param>
    /// <param name="referenceId">The optional reference ID.</param>
    /// <param name="createdAt">The creation date and time.</param>
    private Notification(
        Guid id,
        Guid userId,
        NotificationType type,
        string title,
        string message,
        NotificationPriority priority,
        Guid? referenceId,
        DateTime createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Notification ID cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be empty.", nameof(message));
        }

        if (message.Length > 1000)
        {
            throw new ArgumentException("Message cannot exceed 1000 characters.", nameof(message));
        }

        if (createdAt > DateTime.UtcNow)
        {
            throw new ArgumentException("CreatedAt cannot be in the future.", nameof(createdAt));
        }

        Id = id;
        UserId = userId;
        Type = type;
        Title = title.Trim();
        Message = message.Trim();
        Priority = priority;
        ReferenceId = referenceId;
        CreatedAt = createdAt;
        IsRead = false;
        ReadAt = null;
    }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="type">The notification type.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="priority">The notification priority (default: Normal).</param>
    /// <param name="referenceId">The optional reference ID.</param>
    /// <returns>A new notification instance.</returns>
    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        NotificationPriority priority = NotificationPriority.Normal,
        Guid? referenceId = null)
    {
        return new Notification(
            Guid.NewGuid(),
            userId,
            type,
            title,
            message,
            priority,
            referenceId,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Marks the notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (IsRead)
        {
            throw new InvalidOperationException("Notification is already marked as read.");
        }

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the notification as unread.
    /// </summary>
    public void MarkAsUnread()
    {
        if (!IsRead)
        {
            throw new InvalidOperationException("Notification is already marked as unread.");
        }

        IsRead = false;
        ReadAt = null;
    }

    /// <summary>
    /// Updates the priority of the notification.
    /// </summary>
    /// <param name="newPriority">The new priority level.</param>
    public void UpdatePriority(NotificationPriority newPriority)
    {
        if (IsRead)
        {
            throw new InvalidOperationException("Cannot update priority of a read notification.");
        }

        Priority = newPriority;
    }

    /// <summary>
    /// Updates the content (title and message) of the notification.
    /// </summary>
    /// <param name="newTitle">The new title.</param>
    /// <param name="newMessage">The new message.</param>
    public void UpdateContent(string newTitle, string newMessage)
    {
        if (IsRead)
        {
            throw new InvalidOperationException("Cannot update content of a read notification.");
        }

        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(newTitle));
        }

        if (newTitle.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(newTitle));
        }

        if (string.IsNullOrWhiteSpace(newMessage))
        {
            throw new ArgumentException("Message cannot be empty.", nameof(newMessage));
        }

        if (newMessage.Length > 1000)
        {
            throw new ArgumentException("Message cannot exceed 1000 characters.", nameof(newMessage));
        }

        Title = newTitle.Trim();
        Message = newMessage.Trim();
    }
}
