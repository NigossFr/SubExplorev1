// <copyright file="GetUserEvents.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using MediatR;

/// <summary>
/// Query to retrieve events for a specific user (organized by or registered to).
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="IncludeOrganized">Whether to include events organized by the user.</param>
/// <param name="IncludeRegistered">Whether to include events the user is registered to.</param>
/// <param name="IncludePastEvents">Whether to include past events (default false).</param>
/// <param name="PageNumber">The page number for pagination (1-based).</param>
/// <param name="PageSize">The number of results per page (1-50).</param>
public record GetUserEvents(
    Guid UserId,
    bool IncludeOrganized = true,
    bool IncludeRegistered = true,
    bool IncludePastEvents = false,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<GetUserEventsResult>;

/// <summary>
/// Result of the GetUserEvents query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Events">List of user events.</param>
/// <param name="TotalCount">Total number of events matching the criteria.</param>
/// <param name="PageNumber">The current page number.</param>
/// <param name="PageSize">The page size.</param>
/// <param name="TotalPages">The total number of pages.</param>
public record GetUserEventsResult(
    bool Success,
    List<UserEventDto> Events,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

/// <summary>
/// User event data transfer object.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="Title">The event title.</param>
/// <param name="Description">The event description (truncated to 200 characters).</param>
/// <param name="EventDate">The date and time of the event.</param>
/// <param name="Location">The event location name.</param>
/// <param name="IsOrganizer">Indicates whether the user is the organizer.</param>
/// <param name="IsParticipant">Indicates whether the user is a participant.</param>
/// <param name="ParticipantCount">Current number of participants.</param>
/// <param name="MaxParticipants">Maximum number of participants allowed.</param>
/// <param name="IsFull">Indicates whether the event has reached max capacity.</param>
/// <param name="IsCancelled">Indicates whether the event has been cancelled.</param>
/// <param name="DivingSpotId">Optional ID of associated diving spot.</param>
/// <param name="DivingSpotName">Optional name of associated diving spot.</param>
public record UserEventDto(
    Guid EventId,
    string Title,
    string Description,
    DateTime EventDate,
    string Location,
    bool IsOrganizer,
    bool IsParticipant,
    int ParticipantCount,
    int MaxParticipants,
    bool IsFull,
    bool IsCancelled,
    Guid? DivingSpotId,
    string? DivingSpotName);
