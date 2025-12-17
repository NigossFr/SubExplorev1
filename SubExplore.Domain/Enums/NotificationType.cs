namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the type of notification.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Notification related to an event (e.g., event reminder, participant joined).
    /// </summary>
    Event = 0,

    /// <summary>
    /// Notification related to a message (e.g., new message received).
    /// </summary>
    Message = 1,

    /// <summary>
    /// Notification related to an achievement (e.g., achievement unlocked).
    /// </summary>
    Achievement = 2,

    /// <summary>
    /// System notification (e.g., maintenance, updates, announcements).
    /// </summary>
    System = 3
}
