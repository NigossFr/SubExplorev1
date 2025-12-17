// <copyright file="GetUserStatisticsValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserStatisticsValidator"/>.
/// </summary>
public class GetUserStatisticsValidatorTests
{
    private readonly GetUserStatisticsValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserStatisticsValidatorTests"/> class.
    /// </summary>
    public GetUserStatisticsValidatorTests()
    {
        this.validator = new GetUserStatisticsValidator();
    }

    [Fact]
    public void Validate_WithValidUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserStatistics(
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
        var query = new GetUserStatistics(
            UserId: Guid.Empty);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithIncludeByYearTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeByYear: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithIncludeBySpotTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeBySpot: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithAllIncludesTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserStatistics(
            UserId: Guid.NewGuid(),
            IncludeByYear: true,
            IncludeBySpot: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
