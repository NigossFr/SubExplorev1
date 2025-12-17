// <copyright file="GetUpcomingEventsHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUpcomingEventsHandler"/>.
/// </summary>
public class GetUpcomingEventsHandlerTests
{
    private readonly Mock<ILogger<GetUpcomingEventsHandler>> loggerMock;
    private readonly GetUpcomingEventsHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUpcomingEventsHandlerTests"/> class.
    /// </summary>
    public GetUpcomingEventsHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetUpcomingEventsHandler>>();
        this.handler = new GetUpcomingEventsHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithMaxDistance_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m,
            MaxDistanceKm: 50m);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithCustomDaysAhead_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m,
            DaysAhead: 60);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithCustomMaxResults_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m,
            MaxResults: 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }
}
