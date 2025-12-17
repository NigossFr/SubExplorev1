// <copyright file="GetEventByIdHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetEventByIdHandler"/>.
/// </summary>
public class GetEventByIdHandlerTests
{
    private readonly Mock<ILogger<GetEventByIdHandler>> loggerMock;
    private readonly GetEventByIdHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEventByIdHandlerTests"/> class.
    /// </summary>
    public GetEventByIdHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetEventByIdHandler>>();
        this.handler = new GetEventByIdHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidEventId_ShouldReturnResult()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithRequestingUserId_ShouldReturnResult()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid(),
            RequestingUserId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithoutRequestingUserId_ShouldReturnResult()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid(),
            RequestingUserId: null);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
