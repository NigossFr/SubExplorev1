// <copyright file="GetUserAchievementsValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserAchievementsValidator"/>.
/// </summary>
public class GetUserAchievementsValidatorTests
{
    private readonly GetUserAchievementsValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserAchievementsValidatorTests"/> class.
    /// </summary>
    public GetUserAchievementsValidatorTests()
    {
        this.validator = new GetUserAchievementsValidator();
    }

    [Fact]
    public void Validate_WithValidUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
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
        var query = new GetUserAchievements(
            UserId: Guid.Empty);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithIncludeLockedAchievementsTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithIncludeLockedAchievementsFalse_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: false);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidCategoryFilter_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            CategoryFilter: "Depth");

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithCategoryFilterExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            CategoryFilter: new string('a', 51));

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryFilter)
            .WithErrorMessage("Category filter cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithAllParametersValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserAchievements(
            UserId: Guid.NewGuid(),
            IncludeLockedAchievements: true,
            CategoryFilter: "Exploration");

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
