// <copyright file="GetUserEventsHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserEventsHandler"/>.
/// </summary>
public class GetUserEventsHandlerTests
{
    private readonly Mock<ILogger<GetUserEventsHandler>> loggerMock;
    private readonly GetUserEventsHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserEventsHandlerTests"/> class.
    /// </summary>
    public GetUserEventsHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetUserEventsHandler>>();
        this.handler = new GetUserEventsHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid());

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
    public async Task Handle_WithIncludeOrganizedOnly_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: true,
            IncludeRegistered: false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithIncludeRegisteredOnly_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: false,
            IncludeRegistered: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Events.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithIncludePastEvents_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludePastEvents: true);

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
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            PageNumber: 2,
            PageSize: 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
    }
}
