namespace SubExplore.Domain.Services;

/// <summary>
/// Service interface for sending notifications through various channels.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a push notification to a user's device.
    /// </summary>
    /// <param name="userId">Target user ID.</param>
    /// <param name="title">Notification title.</param>
    /// <param name="message">Notification message.</param>
    /// <param name="data">Optional additional data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if sent successfully; otherwise, false.</returns>
    Task<bool> SendPushNotificationAsync(
        Guid userId,
        string title,
        string message,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email notification.
    /// </summary>
    /// <param name="email">Target email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body (HTML or plain text).</param>
    /// <param name="isHtml">Whether the body is HTML.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if sent successfully; otherwise, false.</returns>
    Task<bool> SendEmailAsync(
        string email,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an in-app notification for a user.
    /// </summary>
    /// <param name="userId">Target user ID.</param>
    /// <param name="notificationType">Type of notification.</param>
    /// <param name="title">Notification title.</param>
    /// <param name="message">Notification message.</param>
    /// <param name="relatedEntityId">Optional related entity ID.</param>
    /// <param name="actionUrl">Optional action URL or deep link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created notification ID.</returns>
    Task<Guid> CreateInAppNotificationAsync(
        Guid userId,
        NotificationType notificationType,
        string title,
        string message,
        Guid? relatedEntityId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to multiple users.
    /// </summary>
    /// <param name="userIds">Target user IDs.</param>
    /// <param name="title">Notification title.</param>
    /// <param name="message">Notification message.</param>
    /// <param name="notificationType">Type of notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of successfully sent notifications.</returns>
    Task<int> SendBulkNotificationAsync(
        IEnumerable<Guid> userIds,
        string title,
        string message,
        NotificationType notificationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an in-app notification as read.
    /// </summary>
    /// <param name="notificationId">Notification ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if marked successfully; otherwise, false.</returns>
    Task<bool> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unread notification count for a user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of unread notifications.</returns>
    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification type enumeration.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// System notification.
    /// </summary>
    System,

    /// <summary>
    /// New dive log from buddy.
    /// </summary>
    DiveLogShared,

    /// <summary>
    /// Event invitation.
    /// </summary>
    EventInvitation,

    /// <summary>
    /// Event reminder.
    /// </summary>
    EventReminder,

    /// <summary>
    /// Event cancellation.
    /// </summary>
    EventCancelled,

    /// <summary>
    /// New message in conversation.
    /// </summary>
    NewMessage,

    /// <summary>
    /// Achievement unlocked.
    /// </summary>
    AchievementUnlocked,

    /// <summary>
    /// New diving spot nearby.
    /// </summary>
    NewSpotNearby,

    /// <summary>
    /// Weather alert for planned dive.
    /// </summary>
    WeatherAlert,

    /// <summary>
    /// Buddy request received.
    /// </summary>
    BuddyRequest,

    /// <summary>
    /// Certification expiring soon.
    /// </summary>
    CertificationExpiring,

    /// <summary>
    /// Premium subscription.
    /// </summary>
    PremiumUpdate
}
