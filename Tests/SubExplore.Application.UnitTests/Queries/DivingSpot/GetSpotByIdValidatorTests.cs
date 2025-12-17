using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetSpotByIdValidator.
/// </summary>
public class GetSpotByIdValidatorTests
{
    private readonly GetSpotByIdValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpotByIdValidatorTests"/> class.
    /// </summary>
    public GetSpotByIdValidatorTests()
    {
        _validator = new GetSpotByIdValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SpotId_Is_Empty()
    {
        var query = new GetSpotByIdQuery(Guid.Empty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.SpotId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SpotId_Is_Valid()
    {
        var query = new GetSpotByIdQuery(Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_With_IncludePhotos_False()
    {
        var query = new GetSpotByIdQuery(Guid.NewGuid(), false);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_With_IncludeRatings_False()
    {
        var query = new GetSpotByIdQuery(Guid.NewGuid(), true, false);

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
