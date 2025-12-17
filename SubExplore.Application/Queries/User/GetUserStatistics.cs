// <copyright file="GetUserStatistics.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using MediatR;

/// <summary>
/// Query to retrieve comprehensive diving statistics for a user.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="IncludeByYear">Whether to include statistics broken down by year.</param>
/// <param name="IncludeBySpot">Whether to include statistics grouped by diving spot.</param>
public record GetUserStatistics(
    Guid UserId,
    bool IncludeByYear = false,
    bool IncludeBySpot = false) : IRequest<GetUserStatisticsResult>;

/// <summary>
/// Result of the GetUserStatistics query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Statistics">The comprehensive user statistics.</param>
public record GetUserStatisticsResult(
    bool Success,
    ComprehensiveUserStatisticsDto? Statistics);

/// <summary>
/// Comprehensive user statistics data transfer object.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="TotalDives">Total number of dives logged.</param>
/// <param name="TotalDiveTimeMinutes">Total dive time in minutes.</param>
/// <param name="TotalDiveTimeFormatted">Total dive time formatted as "XXh YYm".</param>
/// <param name="MaxDepthMeters">Maximum depth reached in meters.</param>
/// <param name="AverageDepthMeters">Average depth across all dives in meters.</param>
/// <param name="MaxDiveTimeMinutes">Longest single dive duration in minutes.</param>
/// <param name="AverageDiveTimeMinutes">Average dive duration in minutes.</param>
/// <param name="TotalDistinctSpots">Number of distinct diving spots visited.</param>
/// <param name="FavoriteDivingSpotId">The ID of the most frequently visited diving spot.</param>
/// <param name="FavoriteDivingSpotName">The name of the most frequently visited diving spot.</param>
/// <param name="FirstDiveDate">The date of the user's first logged dive.</param>
/// <param name="LastDiveDate">The date of the user's most recent logged dive.</param>
/// <param name="DivesByDiveType">Breakdown of dives by type (Recreational, Training, Technical, etc.).</param>
/// <param name="StatisticsByYear">Statistics broken down by year (if requested).</param>
/// <param name="StatisticsBySpot">Statistics grouped by diving spot (if requested).</param>
public record ComprehensiveUserStatisticsDto(
    Guid UserId,
    int TotalDives,
    int TotalDiveTimeMinutes,
    string TotalDiveTimeFormatted,
    decimal MaxDepthMeters,
    decimal AverageDepthMeters,
    int MaxDiveTimeMinutes,
    decimal AverageDiveTimeMinutes,
    int TotalDistinctSpots,
    Guid? FavoriteDivingSpotId,
    string? FavoriteDivingSpotName,
    DateTime? FirstDiveDate,
    DateTime? LastDiveDate,
    Dictionary<string, int> DivesByDiveType,
    List<YearlyStatisticsDto>? StatisticsByYear = null,
    List<SpotStatisticsDto>? StatisticsBySpot = null);

/// <summary>
/// Yearly statistics data transfer object.
/// </summary>
/// <param name="Year">The year.</param>
/// <param name="TotalDives">Total number of dives in this year.</param>
/// <param name="TotalDiveTimeMinutes">Total dive time in minutes for this year.</param>
/// <param name="MaxDepthMeters">Maximum depth reached in this year.</param>
/// <param name="DistinctSpots">Number of distinct spots visited in this year.</param>
public record YearlyStatisticsDto(
    int Year,
    int TotalDives,
    int TotalDiveTimeMinutes,
    decimal MaxDepthMeters,
    int DistinctSpots);

/// <summary>
/// Spot-specific statistics data transfer object.
/// </summary>
/// <param name="DivingSpotId">The diving spot ID.</param>
/// <param name="SpotName">The diving spot name.</param>
/// <param name="TotalDives">Total number of dives at this spot.</param>
/// <param name="TotalDiveTimeMinutes">Total dive time at this spot in minutes.</param>
/// <param name="MaxDepthMeters">Maximum depth reached at this spot.</param>
/// <param name="AverageDepthMeters">Average depth at this spot.</param>
/// <param name="LastDiveDate">Date of the last dive at this spot.</param>
public record SpotStatisticsDto(
    Guid DivingSpotId,
    string SpotName,
    int TotalDives,
    int TotalDiveTimeMinutes,
    decimal MaxDepthMeters,
    decimal AverageDepthMeters,
    DateTime LastDiveDate);
