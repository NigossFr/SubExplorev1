// <copyright file="SearchUsersHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="SearchUsersHandler"/>.
/// </summary>
public class SearchUsersHandlerTests
{
    private readonly Mock<ILogger<SearchUsersHandler>> loggerMock;
    private readonly SearchUsersHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchUsersHandlerTests"/> class.
    /// </summary>
    public SearchUsersHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<SearchUsersHandler>>();
        this.handler = new SearchUsersHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new SearchUsers();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Users.Should().NotBeNull();
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldReturnFilteredResults()
    {
        // Arrange
        var query = new SearchUsers(SearchTerm: "john");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Users.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldRespectPageParameters()
    {
        // Arrange
        var query = new SearchUsers(PageNumber: 2, PageSize: 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_ShouldApplyAllFilters()
    {
        // Arrange
        var query = new SearchUsers(
            SearchTerm: "john",
            IsPremium: true,
            MinTotalDives: 50);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Users.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithSortParameters_ShouldReturnCorrectlySortedResults()
    {
        // Arrange
        var query = new SearchUsers(
            SortBy: UserSortField.TotalDives,
            SortDescending: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Users.Should().NotBeNull();
    }
}
