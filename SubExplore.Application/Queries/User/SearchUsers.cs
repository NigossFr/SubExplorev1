// <copyright file="SearchUsers.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using MediatR;

/// <summary>
/// Query to search for users based on various criteria.
/// </summary>
/// <param name="SearchTerm">Optional search term to filter by username, first name, or last name.</param>
/// <param name="IsPremium">Optional filter for premium users only.</param>
/// <param name="MinTotalDives">Optional filter for minimum number of dives logged.</param>
/// <param name="CertificationLevel">Optional filter for specific certification level.</param>
/// <param name="PageNumber">The page number for pagination (1-based).</param>
/// <param name="PageSize">The number of results per page (1-100).</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDescending">Whether to sort in descending order.</param>
public record SearchUsers(
    string? SearchTerm = null,
    bool? IsPremium = null,
    int? MinTotalDives = null,
    string? CertificationLevel = null,
    int PageNumber = 1,
    int PageSize = 20,
    UserSortField SortBy = UserSortField.Username,
    bool SortDescending = false) : IRequest<SearchUsersResult>;

/// <summary>
/// Field to sort users by.
/// </summary>
public enum UserSortField
{
    /// <summary>
    /// Sort by username.
    /// </summary>
    Username = 0,

    /// <summary>
    /// Sort by total number of dives.
    /// </summary>
    TotalDives = 1,

    /// <summary>
    /// Sort by account creation date.
    /// </summary>
    CreatedAt = 2,

    /// <summary>
    /// Sort by last dive date.
    /// </summary>
    LastDiveDate = 3,
}

/// <summary>
/// Result of the SearchUsers query.
/// </summary>
/// <param name="Success">Indicates whether the query was successful.</param>
/// <param name="Users">The list of users matching the search criteria.</param>
/// <param name="TotalCount">The total number of users matching the criteria (before pagination).</param>
/// <param name="PageNumber">The current page number.</param>
/// <param name="PageSize">The page size.</param>
/// <param name="TotalPages">The total number of pages.</param>
public record SearchUsersResult(
    bool Success,
    List<UserSearchResultDto> Users,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

/// <summary>
/// User search result data transfer object.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Username">The username.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Bio">The user's biography (truncated to 100 characters).</param>
/// <param name="ProfilePictureUrl">URL to the profile picture.</param>
/// <param name="IsPremium">Indicates whether the user has premium status.</param>
/// <param name="TotalDives">Total number of dives logged by this user.</param>
/// <param name="HighestCertificationLevel">The user's highest certification level.</param>
/// <param name="LastDiveDate">The date of the user's most recent dive.</param>
/// <param name="CreatedAt">The date and time when the user account was created.</param>
public record UserSearchResultDto(
    Guid UserId,
    string Username,
    string? FirstName,
    string? LastName,
    string? Bio,
    string? ProfilePictureUrl,
    bool IsPremium,
    int TotalDives,
    string? HighestCertificationLevel,
    DateTime? LastDiveDate,
    DateTime CreatedAt);
