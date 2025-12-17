using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for SearchSpotsValidator.
/// </summary>
public class SearchSpotsValidatorTests
{
    private readonly SearchSpotsValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSpotsValidatorTests"/> class.
    /// </summary>
    public SearchSpotsValidatorTests()
    {
        _validator = new SearchSpotsValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SearchText_Exceeds_MaxLength()
    {
        var query = new SearchSpotsQuery(new string('a', 101));

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.SearchText);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Should_Have_Error_When_MinDifficulty_Is_Out_Of_Range(int difficulty)
    {
        var query = new SearchSpotsQuery(MinDifficulty: difficulty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinDifficulty);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Should_Have_Error_When_MaxDifficulty_Is_Out_Of_Range(int difficulty)
    {
        var query = new SearchSpotsQuery(MaxDifficulty: difficulty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MaxDifficulty);
    }

    [Fact]
    public void Should_Have_Error_When_MinDifficulty_Greater_Than_MaxDifficulty()
    {
        var query = new SearchSpotsQuery(MinDifficulty: 3, MaxDifficulty: 1);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Have_Error_When_MinDepthMeters_Exceeds_Limit()
    {
        var query = new SearchSpotsQuery(MinDepthMeters: 501);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepthMeters_Exceeds_Limit()
    {
        var query = new SearchSpotsQuery(MaxDepthMeters: 501);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MinDepthMeters_Greater_Than_MaxDepthMeters()
    {
        var query = new SearchSpotsQuery(MinDepthMeters: 50, MaxDepthMeters: 30);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Should_Have_Error_When_MinRating_Is_Out_Of_Range(double rating)
    {
        var query = new SearchSpotsQuery(MinRating: rating);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinRating);
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(51)]
    public void Should_Have_Error_When_MinTemperatureCelsius_Is_Out_Of_Range(double temp)
    {
        var query = new SearchSpotsQuery(MinTemperatureCelsius: temp);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinTemperatureCelsius);
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(51)]
    public void Should_Have_Error_When_MaxTemperatureCelsius_Is_Out_Of_Range(double temp)
    {
        var query = new SearchSpotsQuery(MaxTemperatureCelsius: temp);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MaxTemperatureCelsius);
    }

    [Fact]
    public void Should_Have_Error_When_MinTemperature_Greater_Than_MaxTemperature()
    {
        var query = new SearchSpotsQuery(MinTemperatureCelsius: 25, MaxTemperatureCelsius: 15);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Have_Error_When_MinVisibilityMeters_Exceeds_Limit()
    {
        var query = new SearchSpotsQuery(MinVisibilityMeters: 101);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinVisibilityMeters);
    }

    [Fact]
    public void Should_Have_Error_When_SortBy_Is_Invalid()
    {
        var query = new SearchSpotsQuery(SortBy: "InvalidField");

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_PageNumber_Is_Zero_Or_Negative(int pageNumber)
    {
        var query = new SearchSpotsQuery(PageNumber: pageNumber);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_PageSize_Is_Zero_Or_Negative(int pageSize)
    {
        var query = new SearchSpotsQuery(PageSize: pageSize);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Exceeds_Maximum()
    {
        var query = new SearchSpotsQuery(PageSize: 101);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var query = new SearchSpotsQuery(
            "diving",
            1,
            3,
            10,
            50,
            4.0,
            15,
            25,
            10,
            "Rating",
            true,
            1,
            20);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Using_Default_Values()
    {
        var query = new SearchSpotsQuery();

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Rating")]
    [InlineData("Depth")]
    [InlineData("CreatedAt")]
    public void Should_Not_Have_Error_When_SortBy_Is_Valid(string sortBy)
    {
        var query = new SearchSpotsQuery(SortBy: sortBy);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
