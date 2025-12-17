// <copyright file="GetUserProfileValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserProfileValidator"/>.
/// </summary>
public class GetUserProfileValidatorTests
{
    private readonly GetUserProfileValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfileValidatorTests"/> class.
    /// </summary>
    public GetUserProfileValidatorTests()
    {
        this.validator = new GetUserProfileValidator();
    }

    [Fact]
    public void Validate_WithValidUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid());

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.Empty);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithIncludeAchievementsTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeAchievements: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithIncludeCertificationsTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeCertifications: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithIncludeStatisticsTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeStatistics: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithAllIncludesTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserProfile(
            UserId: Guid.NewGuid(),
            IncludeAchievements: true,
            IncludeCertifications: true,
            IncludeStatistics: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
