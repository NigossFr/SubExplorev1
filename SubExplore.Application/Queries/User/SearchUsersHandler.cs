// <copyright file="SearchUsersHandler.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for the <see cref="SearchUsers"/> query.
/// </summary>
public class SearchUsersHandler : IRequestHandler<SearchUsers, SearchUsersResult>
{
    private readonly ILogger<SearchUsersHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchUsersHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SearchUsersHandler(ILogger<SearchUsersHandler> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Handles the SearchUsers query.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the query result.</returns>
    public async Task<SearchUsersResult> Handle(SearchUsers request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Searching users with term: {SearchTerm}, Page: {PageNumber}, PageSize: {PageSize}",
            request.SearchTerm ?? "all",
            request.PageNumber,
            request.PageSize);

        // TODO: Implement actual logic:
        // 1. Build query using IUserRepository with filters:
        //    - If SearchTerm is not null/empty: Filter by Username, FirstName, or LastName contains SearchTerm (case-insensitive)
        //    - If IsPremium has value: Filter by IsPremium == request.IsPremium.Value
        //    - If MinTotalDives has value: Join with dive logs, filter users with Count >= request.MinTotalDives
        //    - If CertificationLevel is not null: Filter users having certification with matching level
        // 2. Get total count of matching users (before pagination) for TotalCount and TotalPages calculation
        // 3. Apply sorting based on request.SortBy:
        //    - UserSortField.Username: Order by Username
        //    - UserSortField.TotalDives: Order by dive log count (requires join/subquery)
        //    - UserSortField.CreatedAt: Order by CreatedAt
        //    - UserSortField.LastDiveDate: Order by max DiveDate from dive logs
        //    - Apply request.SortDescending for sort direction
        // 4. Apply pagination:
        //    - Skip: (request.PageNumber - 1) * request.PageSize
        //    - Take: request.PageSize
        // 5. For each user, get additional data:
        //    - TotalDives: Count from IDiveLogRepository.GetUserDiveCountAsync(userId)
        //    - HighestCertificationLevel: Get from certifications or user entity
        //    - LastDiveDate: Get from IDiveLogRepository.GetUserLastDiveDateAsync(userId)
        // 6. Map each user to UserSearchResultDto (truncate Bio to 100 characters if longer)
        // 7. Calculate TotalPages: (int)Math.Ceiling(totalCount / (double)request.PageSize)
        // 8. Return SearchUsersResult with all data

        await Task.Delay(1, cancellationToken);

        // Placeholder response
        var users = new List<UserSearchResultDto>();
        var totalCount = 0;
        var totalPages = 0;

        this.logger.LogInformation(
            "Search completed. Found {TotalCount} users, returning page {PageNumber} of {TotalPages}",
            totalCount,
            request.PageNumber,
            totalPages);

        return new SearchUsersResult(
            Success: true,
            Users: users,
            TotalCount: totalCount,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            TotalPages: totalPages);
    }
}
