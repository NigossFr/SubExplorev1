namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the status of an event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event is scheduled and open for registration.
    /// </summary>
    Scheduled = 0,

    /// <summary>
    /// Event has been cancelled.
    /// </summary>
    Cancelled = 1,

    /// <summary>
    /// Event has been completed.
    /// </summary>
    Completed = 2
}
