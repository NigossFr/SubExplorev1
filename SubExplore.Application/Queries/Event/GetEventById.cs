// <copyright file="GetEventById.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using MediatR;

/// <summary>
/// Query to retrieve detailed information about a specific event.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="RequestingUserId">Optional ID of the user making the request (for checking registration status).</param>
public record GetEventById(
    Guid EventId,
    Guid? RequestingUserId = null) : IRequest<GetEventByIdResult>;

/// <summary>
/// Result of the GetEventById query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Event">The detailed event information.</param>
public record GetEventByIdResult(
    bool Success,
    DetailedEventDto? Event);

/// <summary>
/// Detailed event data transfer object.
/// </summary>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="Title">The event title.</param>
/// <param name="Description">The full event description.</param>
/// <param name="EventDate">The date and time of the event.</param>
/// <param name="Location">The event location name.</param>
/// <param name="Latitude">The event location latitude.</param>
/// <param name="Longitude">The event location longitude.</param>
/// <param name="OrganizerUserId">The ID of the user who organized the event.</param>
/// <param name="OrganizerUsername">The username of the organizer.</param>
/// <param name="OrganizerProfilePictureUrl">The organizer's profile picture URL.</param>
/// <param name="MaxParticipants">Maximum number of participants allowed.</param>
/// <param name="CurrentParticipants">Current number of registered participants.</param>
/// <param name="IsFull">Indicates whether the event has reached max capacity.</param>
/// <param name="IsUserRegistered">Indicates whether the requesting user is registered.</param>
/// <param name="RegistrationDeadline">Optional deadline for registration.</param>
/// <param name="CancellationDeadline">Optional deadline for cancellation.</param>
/// <param name="DivingSpotId">Optional ID of associated diving spot.</param>
/// <param name="DivingSpotName">Optional name of associated diving spot.</param>
/// <param name="RequiredCertificationLevel">Optional required certification level for participation.</param>
/// <param name="MinimumExperienceDives">Optional minimum number of dives required.</param>
/// <param name="Cost">Optional cost to participate in the event.</param>
/// <param name="Currency">Currency code for the cost (e.g., EUR, USD).</param>
/// <param name="Participants">List of registered participants.</param>
/// <param name="CreatedAt">The date and time when the event was created.</param>
/// <param name="UpdatedAt">The date and time when the event was last updated.</param>
public record DetailedEventDto(
    Guid EventId,
    string Title,
    string Description,
    DateTime EventDate,
    string Location,
    decimal Latitude,
    decimal Longitude,
    Guid OrganizerUserId,
    string OrganizerUsername,
    string? OrganizerProfilePictureUrl,
    int MaxParticipants,
    int CurrentParticipants,
    bool IsFull,
    bool IsUserRegistered,
    DateTime? RegistrationDeadline,
    DateTime? CancellationDeadline,
    Guid? DivingSpotId,
    string? DivingSpotName,
    string? RequiredCertificationLevel,
    int? MinimumExperienceDives,
    decimal? Cost,
    string? Currency,
    List<EventParticipantDto> Participants,
    DateTime CreatedAt,
    DateTime UpdatedAt);

/// <summary>
/// Event participant data transfer object.
/// </summary>
/// <param name="UserId">The participant's user ID.</param>
/// <param name="Username">The participant's username.</param>
/// <param name="ProfilePictureUrl">The participant's profile picture URL.</param>
/// <param name="TotalDives">The participant's total number of dives.</param>
/// <param name="RegistrationDate">The date the participant registered.</param>
public record EventParticipantDto(
    Guid UserId,
    string Username,
    string? ProfilePictureUrl,
    int TotalDives,
    DateTime RegistrationDate);
