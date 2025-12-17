// <copyright file="GetUserAchievements.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using MediatR;

/// <summary>
/// Query to retrieve all achievements for a user, including unlocked and locked achievements.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="IncludeLockedAchievements">Whether to include achievements not yet unlocked.</param>
/// <param name="CategoryFilter">Optional filter by achievement category.</param>
public record GetUserAchievements(
    Guid UserId,
    bool IncludeLockedAchievements = true,
    string? CategoryFilter = null) : IRequest<GetUserAchievementsResult>;

/// <summary>
/// Result of the GetUserAchievements query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="TotalUnlocked">Total number of achievements unlocked by the user.</param>
/// <param name="TotalAvailable">Total number of achievements available in the system.</param>
/// <param name="CompletionPercentage">Percentage of achievements unlocked (0-100).</param>
/// <param name="Achievements">List of achievements (unlocked and/or locked based on request).</param>
public record GetUserAchievementsResult(
    bool Success,
    int TotalUnlocked,
    int TotalAvailable,
    decimal CompletionPercentage,
    List<DetailedAchievementDto> Achievements);

/// <summary>
/// Detailed achievement data transfer object.
/// </summary>
/// <param name="AchievementId">The unique identifier of the achievement.</param>
/// <param name="Title">The achievement title.</param>
/// <param name="Description">The achievement description.</param>
/// <param name="Category">The achievement category (e.g., Depth, Dives, Exploration, Social).</param>
/// <param name="IconUrl">URL to the achievement icon.</param>
/// <param name="Points">Points awarded for unlocking this achievement.</param>
/// <param name="Rarity">Rarity level (Common, Uncommon, Rare, Epic, Legendary).</param>
/// <param name="IsUnlocked">Indicates whether the user has unlocked this achievement.</param>
/// <param name="UnlockedAt">The date and time when the achievement was unlocked (null if locked).</param>
/// <param name="Progress">Current progress towards unlocking (0-100, only for locked achievements with trackable progress).</param>
/// <param name="ProgressDescription">Description of current progress (e.g., "15/50 dives completed").</param>
public record DetailedAchievementDto(
    Guid AchievementId,
    string Title,
    string Description,
    string Category,
    string IconUrl,
    int Points,
    string Rarity,
    bool IsUnlocked,
    DateTime? UnlockedAt,
    decimal? Progress,
    string? ProgressDescription);
