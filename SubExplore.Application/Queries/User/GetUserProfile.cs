// <copyright file="GetUserProfile.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using MediatR;

/// <summary>
/// Query to retrieve a user's profile information.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="IncludeAchievements">Whether to include user achievements in the response.</param>
/// <param name="IncludeCertifications">Whether to include diving certifications in the response.</param>
/// <param name="IncludeStatistics">Whether to include dive statistics in the response.</param>
public record GetUserProfile(
    Guid UserId,
    bool IncludeAchievements = false,
    bool IncludeCertifications = false,
    bool IncludeStatistics = false) : IRequest<GetUserProfileResult>;

/// <summary>
/// Result of the GetUserProfile query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="UserProfile">The user profile data transfer object.</param>
public record GetUserProfileResult(
    bool Success,
    UserProfileDto? UserProfile);

/// <summary>
/// User profile data transfer object.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Username">The username.</param>
/// <param name="Email">The email address.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Bio">The user's biography.</param>
/// <param name="ProfilePictureUrl">URL to the profile picture.</param>
/// <param name="IsPremium">Indicates whether the user has premium status.</param>
/// <param name="CreatedAt">The date and time when the user account was created.</param>
/// <param name="Achievements">List of user achievements (if requested).</param>
/// <param name="Certifications">List of diving certifications (if requested).</param>
/// <param name="Statistics">User dive statistics (if requested).</param>
public record UserProfileDto(
    Guid UserId,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? Bio,
    string? ProfilePictureUrl,
    bool IsPremium,
    DateTime CreatedAt,
    List<AchievementDto>? Achievements = null,
    List<CertificationDto>? Certifications = null,
    UserStatisticsDto? Statistics = null);

/// <summary>
/// Achievement data transfer object.
/// </summary>
/// <param name="AchievementId">The unique identifier of the achievement.</param>
/// <param name="Title">The achievement title.</param>
/// <param name="Description">The achievement description.</param>
/// <param name="IconUrl">URL to the achievement icon.</param>
/// <param name="UnlockedAt">The date and time when the achievement was unlocked.</param>
public record AchievementDto(
    Guid AchievementId,
    string Title,
    string Description,
    string IconUrl,
    DateTime UnlockedAt);

/// <summary>
/// Certification data transfer object.
/// </summary>
/// <param name="Organization">The certification organization (e.g., PADI, SSI, CMAS).</param>
/// <param name="Level">The certification level (e.g., Open Water, Advanced, Rescue).</param>
/// <param name="CertificationNumber">The certification number.</param>
/// <param name="IssueDate">The date when the certification was issued.</param>
public record CertificationDto(
    string Organization,
    string Level,
    string? CertificationNumber,
    DateTime? IssueDate);

/// <summary>
/// User dive statistics data transfer object.
/// </summary>
/// <param name="TotalDives">Total number of dives logged.</param>
/// <param name="TotalDiveTimeMinutes">Total dive time in minutes.</param>
/// <param name="MaxDepthMeters">Maximum depth reached in meters.</param>
/// <param name="AverageDepthMeters">Average depth across all dives in meters.</param>
/// <param name="FavoriteDivingSpotId">The ID of the user's favorite diving spot.</param>
/// <param name="FavoriteDivingSpotName">The name of the user's favorite diving spot.</param>
public record UserStatisticsDto(
    int TotalDives,
    int TotalDiveTimeMinutes,
    decimal MaxDepthMeters,
    decimal AverageDepthMeters,
    Guid? FavoriteDivingSpotId,
    string? FavoriteDivingSpotName);
