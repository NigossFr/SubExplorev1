using SubExplore.Domain.Enums;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a community diving event (group dives, training sessions, social gatherings).
/// This is an aggregate root that manages participant registrations.
/// </summary>
public class Event
{
    private readonly List<EventParticipant> _participants = new();

    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the title of the event.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the detailed description of the event.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the date and time when the event takes place.
    /// </summary>
    public DateTime EventDate { get; private set; }

    /// <summary>
    /// Gets the location coordinates of the event (optional).
    /// </summary>
    public Coordinates? Location { get; private set; }

    /// <summary>
    /// Gets the location name/address of the event.
    /// </summary>
    public string LocationName { get; private set; }

    /// <summary>
    /// Gets the identifier of the diving spot associated with this event (optional).
    /// </summary>
    public Guid? DivingSpotId { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who organized the event.
    /// </summary>
    public Guid OrganizerId { get; private set; }

    /// <summary>
    /// Gets the maximum number of participants allowed.
    /// Null means unlimited participants.
    /// </summary>
    public int? MaxParticipants { get; private set; }

    /// <summary>
    /// Gets the current status of the event.
    /// </summary>
    public EventStatus Status { get; private set; }

    /// <summary>
    /// Gets the date and time when the event was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the event was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the list of participants (read-only collection).
    /// </summary>
    public IReadOnlyCollection<EventParticipant> Participants => _participants.AsReadOnly();

    /// <summary>
    /// Gets the current number of registered participants.
    /// </summary>
    public int ParticipantCount => _participants.Count;

    /// <summary>
    /// Gets a value indicating whether the event is full (max participants reached).
    /// </summary>
    public bool IsFull => MaxParticipants.HasValue && _participants.Count >= MaxParticipants.Value;

    /// <summary>
    /// Gets the number of available spots remaining.
    /// Returns null if there's no participant limit.
    /// </summary>
    public int? AvailableSpots => MaxParticipants.HasValue ? MaxParticipants.Value - _participants.Count : null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private Event()
    {
        Title = string.Empty;
        Description = string.Empty;
        LocationName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.
    /// </summary>
    /// <param name="title">The event title.</param>
    /// <param name="description">The event description.</param>
    /// <param name="eventDate">The event date and time.</param>
    /// <param name="locationName">The location name.</param>
    /// <param name="organizerId">The organizer user identifier.</param>
    /// <param name="maxParticipants">The maximum number of participants (optional).</param>
    private Event(
        string title,
        string description,
        DateTime eventDate,
        string locationName,
        Guid organizerId,
        int? maxParticipants = null)
    {
        Id = Guid.NewGuid();
        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        EventDate = ValidateEventDate(eventDate);
        LocationName = ValidateLocationName(locationName);
        OrganizerId = organizerId;
        MaxParticipants = ValidateMaxParticipants(maxParticipants);
        Status = EventStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="title">The event title.</param>
    /// <param name="description">The event description.</param>
    /// <param name="eventDate">The event date and time.</param>
    /// <param name="locationName">The location name.</param>
    /// <param name="organizerId">The organizer user identifier.</param>
    /// <param name="maxParticipants">The maximum number of participants (optional).</param>
    /// <returns>A new <see cref="Event"/> instance.</returns>
    public static Event Create(
        string title,
        string description,
        DateTime eventDate,
        string locationName,
        Guid organizerId,
        int? maxParticipants = null)
    {
        return new Event(title, description, eventDate, locationName, organizerId, maxParticipants);
    }

    /// <summary>
    /// Updates the event details.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="eventDate">The new event date.</param>
    /// <param name="locationName">The new location name.</param>
    public void UpdateDetails(string title, string description, DateTime eventDate, string locationName)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot update a cancelled event.");
        }

        if (Status == EventStatus.Completed)
        {
            throw new InvalidOperationException("Cannot update a completed event.");
        }

        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        EventDate = ValidateEventDate(eventDate);
        LocationName = ValidateLocationName(locationName);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the location coordinates for the event.
    /// </summary>
    /// <param name="location">The location coordinates.</param>
    public void SetLocation(Coordinates location)
    {
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Associates the event with a diving spot.
    /// </summary>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    public void SetDivingSpot(Guid divingSpotId)
    {
        DivingSpotId = divingSpotId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the maximum number of participants.
    /// </summary>
    /// <param name="maxParticipants">The new maximum (null for unlimited).</param>
    public void UpdateMaxParticipants(int? maxParticipants)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot update a cancelled event.");
        }

        var newMax = ValidateMaxParticipants(maxParticipants);

        if (newMax.HasValue && _participants.Count > newMax.Value)
        {
            throw new InvalidOperationException(
                $"Cannot set max participants to {newMax} when {_participants.Count} are already registered.");
        }

        MaxParticipants = newMax;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Registers a participant for the event.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="comment">The optional comment.</param>
    /// <returns>The participant registration.</returns>
    public EventParticipant RegisterParticipant(Guid userId, string? comment = null)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot register for a cancelled event.");
        }

        if (Status == EventStatus.Completed)
        {
            throw new InvalidOperationException("Cannot register for a completed event.");
        }

        if (userId == OrganizerId)
        {
            throw new InvalidOperationException("Organizer is automatically a participant.");
        }

        if (_participants.Any(p => p.UserId == userId))
        {
            throw new InvalidOperationException("User is already registered for this event.");
        }

        if (IsFull)
        {
            throw new InvalidOperationException("Event is full. Maximum participants reached.");
        }

        var participant = EventParticipant.Create(Id, userId, comment);
        _participants.Add(participant);
        UpdatedAt = DateTime.UtcNow;

        return participant;
    }

    /// <summary>
    /// Unregisters a participant from the event.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    public void UnregisterParticipant(Guid userId)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot unregister from a cancelled event.");
        }

        if (Status == EventStatus.Completed)
        {
            throw new InvalidOperationException("Cannot unregister from a completed event.");
        }

        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
        {
            throw new InvalidOperationException("User is not registered for this event.");
        }

        _participants.Remove(participant);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if a user is registered for the event.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>True if the user is registered, false otherwise.</returns>
    public bool IsUserRegistered(Guid userId)
    {
        return _participants.Any(p => p.UserId == userId);
    }

    /// <summary>
    /// Cancels the event.
    /// </summary>
    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Event is already cancelled.");
        }

        if (Status == EventStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed event.");
        }

        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the event as completed.
    /// </summary>
    public void Complete()
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot complete a cancelled event.");
        }

        if (Status == EventStatus.Completed)
        {
            throw new InvalidOperationException("Event is already completed.");
        }

        if (EventDate > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot complete an event that hasn't occurred yet.");
        }

        Status = EventStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the event title.
    /// </summary>
    /// <param name="title">The title to validate.</param>
    /// <returns>The validated title.</returns>
    /// <exception cref="ArgumentException">Thrown when title is invalid.</exception>
    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Event title cannot be null or empty.", nameof(title));
        }

        title = title.Trim();

        if (title.Length < 3)
        {
            throw new ArgumentException("Event title must be at least 3 characters.", nameof(title));
        }

        if (title.Length > 100)
        {
            throw new ArgumentException("Event title cannot exceed 100 characters.", nameof(title));
        }

        return title;
    }

    /// <summary>
    /// Validates the event description.
    /// </summary>
    /// <param name="description">The description to validate.</param>
    /// <returns>The validated description.</returns>
    /// <exception cref="ArgumentException">Thrown when description is invalid.</exception>
    private static string ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Event description cannot be null or empty.", nameof(description));
        }

        description = description.Trim();

        if (description.Length < 10)
        {
            throw new ArgumentException("Event description must be at least 10 characters.", nameof(description));
        }

        if (description.Length > 2000)
        {
            throw new ArgumentException("Event description cannot exceed 2000 characters.", nameof(description));
        }

        return description;
    }

    /// <summary>
    /// Validates the event date.
    /// </summary>
    /// <param name="eventDate">The event date to validate.</param>
    /// <returns>The validated event date.</returns>
    private static DateTime ValidateEventDate(DateTime eventDate)
    {
        // Allow creating past events for historical purposes
        // Business logic prevents operations on past events through status
        return eventDate;
    }

    /// <summary>
    /// Validates the location name.
    /// </summary>
    /// <param name="locationName">The location name to validate.</param>
    /// <returns>The validated location name.</returns>
    /// <exception cref="ArgumentException">Thrown when location name is invalid.</exception>
    private static string ValidateLocationName(string locationName)
    {
        if (string.IsNullOrWhiteSpace(locationName))
        {
            throw new ArgumentException("Location name cannot be null or empty.", nameof(locationName));
        }

        locationName = locationName.Trim();

        if (locationName.Length < 3)
        {
            throw new ArgumentException("Location name must be at least 3 characters.", nameof(locationName));
        }

        if (locationName.Length > 200)
        {
            throw new ArgumentException("Location name cannot exceed 200 characters.", nameof(locationName));
        }

        return locationName;
    }

    /// <summary>
    /// Validates the maximum participants value.
    /// </summary>
    /// <param name="maxParticipants">The max participants to validate.</param>
    /// <returns>The validated max participants.</returns>
    /// <exception cref="ArgumentException">Thrown when max participants is invalid.</exception>
    private static int? ValidateMaxParticipants(int? maxParticipants)
    {
        if (!maxParticipants.HasValue)
        {
            return null;
        }

        if (maxParticipants.Value < 1)
        {
            throw new ArgumentException("Maximum participants must be at least 1.", nameof(maxParticipants));
        }

        if (maxParticipants.Value > 1000)
        {
            throw new ArgumentException("Maximum participants cannot exceed 1000.", nameof(maxParticipants));
        }

        return maxParticipants;
    }
}
