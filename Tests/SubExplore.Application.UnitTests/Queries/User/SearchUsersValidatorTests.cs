// <copyright file="SearchUsersValidatorTests.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.UnitTests.Queries.User;

using FluentAssertions;
using FluentValidation.TestHelper;
using SubExplore.Application.Queries.User;
using Xunit;

/// <summary>
/// Unit tests for <see cref="SearchUsersValidator"/>.
/// </summary>
public class SearchUsersValidatorTests
{
    private readonly SearchUsersValidator validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchUsersValidatorTests"/> class.
    /// </summary>
    public SearchUsersValidatorTests()
    {
        this.validator = new SearchUsersValidator();
    }

    [Fact]
    public void Validate_WithValidQuery_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers();

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidSearchTerm_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(SearchTerm: "john");

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithSearchTermExceeding100Characters_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(SearchTerm: new string('a', 101));

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Search term cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithNegativeMinTotalDives_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(MinTotalDives: -1);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinTotalDives)
            .WithErrorMessage("Minimum total dives must be a non-negative number.");
    }

    [Fact]
    public void Validate_WithZeroMinTotalDives_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(MinTotalDives: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidMinTotalDives_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(MinTotalDives: 50);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithCertificationLevelExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(CertificationLevel: new string('a', 51));

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CertificationLevel)
            .WithErrorMessage("Certification level cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(PageNumber: 0);

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
        var query = new SearchUsers(PageNumber: -1);

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
        var query = new SearchUsers(PageSize: 0);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 100.");
    }

    [Fact]
    public void Validate_WithPageSizeGreaterThan100_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(PageSize: 101);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 100.");
    }

    [Fact]
    public void Validate_WithValidPageSize_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(PageSize: 50);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidSortField_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(SortBy: (UserSortField)999);

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
        foreach (UserSortField sortField in Enum.GetValues(typeof(UserSortField)))
        {
            var query = new SearchUsers(SortBy: sortField);
            var result = this.validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Fact]
    public void Validate_WithAllValidFilters_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchUsers(
            SearchTerm: "john",
            IsPremium: true,
            MinTotalDives: 50,
            CertificationLevel: "Advanced Open Water",
            PageNumber: 2,
            PageSize: 25,
            SortBy: UserSortField.TotalDives,
            SortDescending: true);

        // Act
        var result = this.validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
