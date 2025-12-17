using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetPopularSpotsValidator.
/// </summary>
public class GetPopularSpotsValidatorTests
{
    private readonly GetPopularSpotsValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPopularSpotsValidatorTests"/> class.
    /// </summary>
    public GetPopularSpotsValidatorTests()
    {
        _validator = new GetPopularSpotsValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_Limit_Is_Zero_Or_Negative(int limit)
    {
        var query = new GetPopularSpotsQuery(limit);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Should_Have_Error_When_Limit_Exceeds_Maximum()
    {
        var query = new GetPopularSpotsQuery(51);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Should_Have_Error_When_MinimumRatings_Is_Negative()
    {
        var query = new GetPopularSpotsQuery(10, -1);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinimumRatings);
    }

    [Fact]
    public void Should_Have_Error_When_MinimumRatings_Exceeds_Maximum()
    {
        var query = new GetPopularSpotsQuery(10, 1001);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinimumRatings);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_DaysBack_Is_Zero_Or_Negative(int daysBack)
    {
        var query = new GetPopularSpotsQuery(10, 5, daysBack);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.DaysBack);
    }

    [Fact]
    public void Should_Have_Error_When_DaysBack_Exceeds_Maximum()
    {
        var query = new GetPopularSpotsQuery(10, 5, 366);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.DaysBack);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var query = new GetPopularSpotsQuery(20, 10, 30);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Using_Default_Values()
    {
        var query = new GetPopularSpotsQuery();

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_MinimumRatings_Is_Zero()
    {
        var query = new GetPopularSpotsQuery(10, 0);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
