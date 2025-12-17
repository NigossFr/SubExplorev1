// <copyright file="SearchEventsValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.Event;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.Event;
using Xunit;

/// <summary>
/// Unit tests for <see cref="SearchEventsValidator"/>.
/// </summary>
public class SearchEventsValidatorTests
{
    private readonly SearchEventsValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchEventsValidatorTests"/> class.
    /// </summary>
    public SearchEventsValidatorTests()
    {
        this.validator = new SearchEventsValidator();
    }

    [Fact]
    public void Validate_WithValidQuery_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents();

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidSearchTerm_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(SearchTerm: "diving");

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithSearchTermExceeding100Characters_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(SearchTerm: new string('a', 101));

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Search term cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithValidDateRange_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(30));

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithStartDateAfterEndDate_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(
            StartDate: DateTime.UtcNow.AddDays(30),
            EndDate: DateTime.UtcNow);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Start date must be before or equal to end date.");
    }

    [Fact]
    public void Validate_WithNegativeMinParticipants_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(MinParticipants: -1);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinParticipants)
            .WithErrorMessage("Minimum participants must be a non-negative number.");
    }

    [Fact]
    public void Validate_WithValidMinParticipants_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(MinParticipants: 5);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNegativeMaxParticipants_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(MaxParticipants: -1);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxParticipants)
            .WithErrorMessage("Maximum participants must be a non-negative number.");
    }

    [Fact]
    public void Validate_WithMinGreaterThanMax_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(
            MinParticipants: 20,
            MaxParticipants: 10);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum participants must be less than or equal to maximum participants.");
    }

    [Fact]
    public void Validate_WithMinEqualToMax_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(
            MinParticipants: 10,
            MaxParticipants: 10);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(PageNumber: 0);

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
        var query = new SearchEvents(PageSize: 0);

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
        var query = new SearchEvents(PageSize: 51);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 50.");
    }

    [Fact]
    public void Validate_WithInvalidSortField_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(SortBy: (EventSortField)999);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("Invalid sort field.");
    }

    [Fact]
    public void Validate_WithValidSortFields_ShouldNotHaveValidationError()
    {
        // Arrange & Act & Assert
        foreach (EventSortField sortField in Enum.GetValues(typeof(EventSortField)))
        {
            var query = new SearchEvents(SortBy: sortField);
            var result = this.validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Fact]
    public void Validate_WithAllValidParameters_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchEvents(
            SearchTerm: "diving",
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(30),
            DivingSpotId: Guid.NewGuid(),
            MinParticipants: 5,
            MaxParticipants: 20,
            OnlyAvailable: true,
            PageNumber: 2,
            PageSize: 25,
            SortBy: EventSortField.EventDate,
            SortDescending: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
