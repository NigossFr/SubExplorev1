// <copyright file="GetUserStatisticsHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserStatisticsHandler"/>.
/// </summary>
public class GetUserStatisticsHandlerTests
{
    private readonly Mock<ILogger<GetUserStatisticsHandler>> loggerMock;
    private readonly GetUserStatisticsHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserStatisticsHandlerTests"/> class.
    /// </summary>
    public GetUserStatisticsHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetUserStatisticsHandler>>();
        this.handler = new GetUserStatisticsHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Statistics.Should().NotBeNull();
        result.Statistics!.UserId.Should().Be(query.UserId);
    }

    [Fact]
    public async Task Handle_WithIncludeByYear_ShouldIncludeYearlyStatistics()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeByYear: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Statistics.Should().NotBeNull();
        result.Statistics!.StatisticsByYear.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithIncludeBySpot_ShouldIncludeSpotStatistics()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeBySpot: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Statistics.Should().NotBeNull();
        result.Statistics!.StatisticsBySpot.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithoutOptionalIncludes_ShouldReturnNullForOptionalFields()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeByYear: false,
            IncludeBySpot: false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Statistics.Should().NotBeNull();
        result.Statistics!.StatisticsByYear.Should().BeNull();
        result.Statistics!.StatisticsBySpot.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithAllIncludes_ShouldIncludeAllStatistics()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeByYear: true,
            IncludeBySpot: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Statistics.Should().NotBeNull();
        result.Statistics!.StatisticsByYear.Should().NotBeNull();
        result.Statistics!.StatisticsBySpot.Should().NotBeNull();
        result.Statistics!.DivesByDiveType.Should().NotBeNull();
    }
}
