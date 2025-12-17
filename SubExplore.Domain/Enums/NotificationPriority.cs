namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the priority level of a notification.
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Low priority notification (informational, can be read later).
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority notification (default priority).
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority notification (should be read soon).
    /// </summary>
    High = 2,

    /// <summary>
    /// Urgent priority notification (requires immediate attention).
    /// </summary>
    Urgent = 3
}
