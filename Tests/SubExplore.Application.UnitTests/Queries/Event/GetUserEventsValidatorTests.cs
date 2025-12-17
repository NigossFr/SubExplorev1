// <copyright file="GetUserEventsValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUserEventsValidator"/>.
/// </summary>
public class GetUserEventsValidatorTests
{
    private readonly GetUserEventsValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserEventsValidatorTests"/> class.
    /// </summary>
    public GetUserEventsValidatorTests()
    {
        this.validator = new GetUserEventsValidator();
    }

    [Fact]
    public void Validate_WithValidUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
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
        var query = new GetUserEvents(
            UserId: Guid.Empty);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithBothIncludeFlagsTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: true,
            IncludeRegistered: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithOnlyOrganizedTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: true,
            IncludeRegistered: false);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithOnlyRegisteredTrue_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: false,
            IncludeRegistered: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithBothIncludeFlagsFalse_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: false,
            IncludeRegistered: false);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one of IncludeOrganized or IncludeRegistered must be true.");
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            PageNumber: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be at least 1.");
    }

    [Fact]
    public void Validate_WithNegativePageNumber_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            PageNumber: -1);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be at least 1.");
    }

    [Fact]
    public void Validate_WithPageSizeZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            PageSize: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 50.");
    }

    [Fact]
    public void Validate_WithPageSizeGreaterThan50_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            PageSize: 51);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 50.");
    }

    [Fact]
    public void Validate_WithAllParametersValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUserEvents(
            UserId: Guid.NewGuid(),
            IncludeOrganized: true,
            IncludeRegistered: true,
            IncludePastEvents: true,
            PageNumber: 2,
            PageSize: 25);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
