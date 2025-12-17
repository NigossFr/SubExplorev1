// <copyright file="SearchEvents.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using MediatR;

/// <summary>
/// Query to search for events based on various criteria.
/// </summary>
/// <param name="SearchTerm">Optional search term to filter by title or description.</param>
/// <param name="StartDate">Optional start date for filtering events.</param>
/// <param name="EndDate">Optional end date for filtering events.</param>
/// <param name="DivingSpotId">Optional filter for events at a specific diving spot.</param>
/// <param name="MinParticipants">Optional minimum number of participants.</param>
/// <param name="MaxParticipants">Optional maximum number of participants.</param>
/// <param name="OnlyAvailable">Whether to show only events with available spots.</param>
/// <param name="PageNumber">The page number for pagination (1-based).</param>
/// <param name="PageSize">The number of results per page (1-50).</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDescending">Whether to sort in descending order.</param>
public record SearchEvents(
    string? SearchTerm = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? DivingSpotId = null,
    int? MinParticipants = null,
    int? MaxParticipants = null,
    bool OnlyAvailable = false,
    int PageNumber = 1,
    int PageSize = 20,
    EventSortField SortBy = EventSortField.EventDate,
    bool SortDescending = false) : IRequest<SearchEventsResult>;

/// <summary>
/// Field to sort events by.
/// </summary>
public enum EventSortField
{
    /// <summary>
    /// Sort by event date.
    /// </summary>
    EventDate = 0,

    /// <summary>
    /// Sort by title.
    /// </summary>
    Title = 1,

    /// <summary>
    /// Sort by number of participants.
    /// </summary>
    ParticipantCount = 2,

    /// <summary>
    /// Sort by creation date.
    /// </summary>
    CreatedAt = 3,
}

/// <summary>
/// Result of the SearchEvents query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Events">List of events matching the search criteria.</param>
/// <param name="TotalCount">Total number of events matching the criteria.</param>
/// <param name="PageNumber">The current page number.</param>
/// <param name="PageSize">The page size.</param>
/// <param name="TotalPages">The total number of pages.</param>
public record SearchEventsResult(
    bool Success,
    List<EventSearchResultDto> Events,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

/// <summary>
/// Event search result data transfer object.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="Title">The event title.</param>
/// <param name="Description">The event description (truncated to 150 characters).</param>
/// <param name="EventDate">The date and time of the event.</param>
/// <param name="Location">The event location name.</param>
/// <param name="OrganizerUsername">The username of the organizer.</param>
/// <param name="ParticipantCount">Current number of participants.</param>
/// <param name="MaxParticipants">Maximum number of participants allowed.</param>
/// <param name="AvailableSpots">Number of available spots remaining.</param>
/// <param name="IsFull">Indicates whether the event has reached max capacity.</param>
/// <param name="DivingSpotId">Optional ID of associated diving spot.</param>
/// <param name="DivingSpotName">Optional name of associated diving spot.</param>
/// <param name="Cost">Optional cost to participate in the event.</param>
/// <param name="Currency">Currency code for the cost.</param>
public record EventSearchResultDto(
    Guid EventId,
    string Title,
    string Description,
    DateTime EventDate,
    string Location,
    string OrganizerUsername,
    int ParticipantCount,
    int MaxParticipants,
    int AvailableSpots,
    bool IsFull,
    Guid? DivingSpotId,
    string? DivingSpotName,
    decimal? Cost,
    string? Currency);
