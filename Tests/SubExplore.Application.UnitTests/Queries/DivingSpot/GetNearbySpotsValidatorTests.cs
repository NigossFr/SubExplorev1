using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetNearbySpotsValidator.
/// </summary>
public class GetNearbySpotsValidatorTests
{
    private readonly GetNearbySpotsValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNearbySpotsValidatorTests"/> class.
    /// </summary>
    public GetNearbySpotsValidatorTests()
    {
        _validator = new GetNearbySpotsValidator();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Should_Have_Error_When_Latitude_Is_Out_Of_Range(double latitude)
    {
        var query = new GetNearbySpotsQuery(latitude, 5.0);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Should_Have_Error_When_Longitude_Is_Out_Of_Range(double longitude)
    {
        var query = new GetNearbySpotsQuery(43.0, longitude);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_RadiusKm_Is_Zero_Or_Negative(double radius)
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, radius);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.RadiusKm);
    }

    [Fact]
    public void Should_Have_Error_When_RadiusKm_Exceeds_Limit()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 101);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.RadiusKm);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Should_Have_Error_When_MinDifficulty_Is_Out_Of_Range(int difficulty)
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, difficulty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinDifficulty);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Should_Have_Error_When_MaxDifficulty_Is_Out_Of_Range(int difficulty)
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, difficulty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MaxDifficulty);
    }

    [Fact]
    public void Should_Have_Error_When_MinDifficulty_Greater_Than_MaxDifficulty()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, 3, 1);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_MinDepthMeters_Is_Zero_Or_Negative(double depth)
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, depth);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MinDepthMeters_Exceeds_Limit()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, 501);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MinDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepthMeters_Exceeds_Limit()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, null, 501);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MinDepthMeters_Greater_Than_MaxDepthMeters()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, 50, 30);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_Limit_Is_Zero_Or_Negative(int limit)
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, null, null, limit);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Should_Have_Error_When_Limit_Exceeds_Maximum()
    {
        var query = new GetNearbySpotsQuery(43.0, 5.0, 10, null, null, null, null, 101);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var query = new GetNearbySpotsQuery(43.2965, 5.3698, 50, 1, 3, 10, 50, 20);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Using_Default_Values()
    {
        var query = new GetNearbySpotsQuery(43.2965, 5.3698);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
