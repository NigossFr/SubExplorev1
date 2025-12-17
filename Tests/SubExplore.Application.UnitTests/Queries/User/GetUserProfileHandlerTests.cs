// <copyright file="GetUserProfileHandlerTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserProfileHandler"/>.
/// </summary>
public class GetUserProfileHandlerTests
{
    private readonly Mock<ILogger<GetUserProfileHandler>> loggerMock;
    private readonly GetUserProfileHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfileHandlerTests"/> class.
    /// </summary>
    public GetUserProfileHandlerTests()
    {
        this.loggerMock = new Mock<ILogger<GetUserProfileHandler>>();
        this.handler = new GetUserProfileHandler(this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserProfile.Should().NotBeNull();
        result.UserProfile!.UserId.Should().Be(query.UserId);
    }

    [Fact]
    public async Task Handle_WithIncludeAchievements_ShouldIncludeAchievementsList()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeAchievements: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserProfile.Should().NotBeNull();
        result.UserProfile!.Achievements.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithIncludeCertifications_ShouldIncludeCertificationsList()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeCertifications: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserProfile.Should().NotBeNull();
        result.UserProfile!.Certifications.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithIncludeStatistics_ShouldIncludeStatistics()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeStatistics: true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserProfile.Should().NotBeNull();
        result.UserProfile!.Statistics.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithoutOptionalIncludes_ShouldReturnNullForOptionalFields()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeAchievements: false,
            IncludeCertifications: false,
            IncludeStatistics: false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserProfile.Should().NotBeNull();
        result.UserProfile!.Achievements.Should().BeNull();
        result.UserProfile!.Certifications.Should().BeNull();
        result.UserProfile!.Statistics.Should().BeNull();
    }
}
