// <copyright file="GetEventByIdValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetEventByIdValidator"/>.
/// </summary>
public class GetEventByIdValidatorTests
{
    private readonly GetEventByIdValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEventByIdValidatorTests"/> class.
    /// </summary>
    public GetEventByIdValidatorTests()
    {
        this.validator = new GetEventByIdValidator();
    }

    [Fact]
    public void Validate_WithValidEventId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid());

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyEventId_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.Empty);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EventId)
            .WithErrorMessage("Event ID is required.");
    }

    [Fact]
    public void Validate_WithRequestingUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid(),
            RequestingUserId: Guid.NewGuid());

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithoutRequestingUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetEventById(
            EventId: Guid.NewGuid(),
            RequestingUserId: null);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
