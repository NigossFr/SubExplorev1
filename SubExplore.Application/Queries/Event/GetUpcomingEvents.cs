// <copyright file="GetUpcomingEvents.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using MediatR;

/// <summary>
/// Query to retrieve upcoming diving events within a specified date range.
/// </summary>
/// <param name="Latitude">The user's current latitude for distance calculation.</param>
/// <param name="Longitude">The user's current longitude for distance calculation.</param>
/// <param name="MaxDistanceKm">Optional maximum distance in kilometers for filtering events.</param>
/// <param name="DaysAhead">Number of days to look ahead (default 30, max 365).</param>
/// <param name="MaxResults">Maximum number of events to return (default 20, max 100).</param>
public record GetUpcomingEvents(
    decimal Latitude,
    decimal Longitude,
    decimal? MaxDistanceKm = null,
    int DaysAhead = 30,
    int MaxResults = 20) : IRequest<GetUpcomingEventsResult>;

/// <summary>
/// Result of the GetUpcomingEvents query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Events">List of upcoming events ordered by date.</param>
public record GetUpcomingEventsResult(
    bool Success,
    List<UpcomingEventDto> Events);

/// <summary>
/// Upcoming event data transfer object.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="Title">The event title.</param>
/// <param name="Description">The event description (truncated to 200 characters).</param>
/// <param name="EventDate">The date and time of the event.</param>
/// <param name="Location">The event location name.</param>
/// <param name="Latitude">The event location latitude.</param>
/// <param name="Longitude">The event location longitude.</param>
/// <param name="DistanceKm">Distance from user's location in kilometers.</param>
/// <param name="OrganizerUserId">The ID of the user who organized the event.</param>
/// <param name="OrganizerUsername">The username of the organizer.</param>
/// <param name="MaxParticipants">Maximum number of participants allowed.</param>
/// <param name="CurrentParticipants">Current number of registered participants.</param>
/// <param name="IsFull">Indicates whether the event has reached max capacity.</param>
/// <param name="IsUserRegistered">Indicates whether the requesting user is registered.</param>
/// <param name="DivingSpotId">Optional ID of associated diving spot.</param>
/// <param name="DivingSpotName">Optional name of associated diving spot.</param>
public record UpcomingEventDto(
    Guid EventId,
    string Title,
    string Description,
    DateTime EventDate,
    string Location,
    decimal Latitude,
    decimal Longitude,
    decimal DistanceKm,
    Guid OrganizerUserId,
    string OrganizerUsername,
    int MaxParticipants,
    int CurrentParticipants,
    bool IsFull,
    bool IsUserRegistered,
    Guid? DivingSpotId,
    string? DivingSpotName);
