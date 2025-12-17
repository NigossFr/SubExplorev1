// <copyright file="SearchEventsHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="SearchEventsHandler"/>.
/// </summary>
public class SearchEventsHandlerTests
{
    private readonly Mock<ILogger<SearchEventsHandler>> loggerMock;
    private readonly SearchEventsHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchEventsHandlerTests"/> class.
    /// </summary>
    public SearchEventsHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<SearchEventsHandler>>();
        this.handler = new SearchEventsHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new SearchEvents();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldReturnFilteredResults()
    {
        // Arrange
        var query = new SearchEvents(SearchTerm: "diving");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithDateRange_ShouldReturnFilteredResults()
    {
        // Arrange
        var query = new SearchEvents(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(30));

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithDivingSpotFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var query = new SearchEvents(DivingSpotId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithParticipantFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var query = new SearchEvents(
            MinParticipants: 5,
            MaxParticipants: 20);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithOnlyAvailable_ShouldReturnOnlyAvailableEvents()
    {
        // Arrange
        var query = new SearchEvents(OnlyAvailable: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldRespectPageParameters()
    {
        // Arrange
        var query = new SearchEvents(PageNumber: 2, PageSize: 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithSortParameters_ShouldReturnCorrectlySortedResults()
    {
        // Arrange
        var query = new SearchEvents(
            SortBy: EventSortField.ParticipantCount,
            SortDescending: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }
}
