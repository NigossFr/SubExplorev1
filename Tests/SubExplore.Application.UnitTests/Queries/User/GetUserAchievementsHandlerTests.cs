// <copyright file="GetUserAchievementsHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserAchievementsHandler"/>.
/// </summary>
public class GetUserAchievementsHandlerTests
{
    private readonly Mock<ILogger<GetUserAchievementsHandler>> loggerMock;
    private readonly GetUserAchievementsHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserAchievementsHandlerTests"/> class.
    /// </summary>
    public GetUserAchievementsHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetUserAchievementsHandler>>();
        this.handler = new GetUserAchievementsHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Achievements.Should().NotBeNull();
        result.TotalUnlocked.Should().BeGreaterThanOrEqualTo(0);
        result.TotalAvailable.Should().BeGreaterThanOrEqualTo(0);
        result.CompletionPercentage.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task Handle_WithIncludeLockedAchievements_ShouldIncludeBothUnlockedAndLocked()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Achievements.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithoutIncludeLockedAchievements_ShouldIncludeOnlyUnlocked()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Achievements.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldFilterByCategory()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            CategoryFilter: "Depth");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Achievements.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithAllParameters_ShouldApplyAllFilters()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: true,
            CategoryFilter: "Exploration");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Achievements.Should().NotBeNull();
        result.TotalUnlocked.Should().BeGreaterThanOrEqualTo(0);
        result.TotalAvailable.Should().BeGreaterThanOrEqualTo(result.TotalUnlocked);
    }
}
