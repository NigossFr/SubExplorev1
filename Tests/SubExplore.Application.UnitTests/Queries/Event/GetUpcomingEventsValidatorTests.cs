// <copyright file="GetUpcomingEventsValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="GetUpcomingEventsValidator"/>.
/// </summary>
public class GetUpcomingEventsValidatorTests
{
    private readonly GetUpcomingEventsValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUpcomingEventsValidatorTests"/> class.
    /// </summary>
    public GetUpcomingEventsValidatorTests()
    {
        this.validator = new GetUpcomingEventsValidator();
    }

    [Fact]
    public void Validate_WithValidCoordinates_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithLatitudeTooHigh_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 91m,
            Longitude: 0m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90 degrees.");
    }

    [Fact]
    public void Validate_WithLatitudeTooLow_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: -91m,
            Longitude: 0m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90 degrees.");
    }

    [Fact]
    public void Validate_WithLongitudeTooHigh_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 181m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180 degrees.");
    }

    [Fact]
    public void Validate_WithLongitudeTooLow_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: -181m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180 degrees.");
    }

    [Fact]
    public void Validate_WithValidMaxDistance_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxDistanceKm: 50m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithZeroMaxDistance_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxDistanceKm: 0m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxDistanceKm)
            .WithErrorMessage("Maximum distance must be greater than 0.");
    }

    [Fact]
    public void Validate_WithNegativeMaxDistance_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxDistanceKm: -10m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxDistanceKm)
            .WithErrorMessage("Maximum distance must be greater than 0.");
    }

    [Fact]
    public void Validate_WithMaxDistanceExceedingLimit_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxDistanceKm: 10001m);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxDistanceKm)
            .WithErrorMessage("Maximum distance cannot exceed 10,000 km.");
    }

    [Fact]
    public void Validate_WithDaysAheadZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            DaysAhead: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DaysAhead)
            .WithErrorMessage("Days ahead must be between 1 and 365.");
    }

    [Fact]
    public void Validate_WithDaysAheadTooHigh_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            DaysAhead: 366);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DaysAhead)
            .WithErrorMessage("Days ahead must be between 1 and 365.");
    }

    [Fact]
    public void Validate_WithMaxResultsZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxResults: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxResults)
            .WithErrorMessage("Max results must be between 1 and 100.");
    }

    [Fact]
    public void Validate_WithMaxResultsTooHigh_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 0m,
            Longitude: 0m,
            MaxResults: 101);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxResults)
            .WithErrorMessage("Max results must be between 1 and 100.");
    }

    [Fact]
    public void Validate_WithAllParametersValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetUpcomingEvents(
            Latitude: 48.8566m,
            Longitude: 2.3522m,
            MaxDistanceKm: 100m,
            DaysAhead: 60,
            MaxResults: 50);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
