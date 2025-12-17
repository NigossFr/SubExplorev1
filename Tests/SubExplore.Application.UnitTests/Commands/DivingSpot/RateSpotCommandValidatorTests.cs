using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for RateSpotCommandValidator.
/// </summary>
public class RateSpotCommandValidatorTests
{
    private readonly RateSpotCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateSpotCommandValidatorTests"/> class.
    /// </summary>
    public RateSpotCommandValidatorTests()
    {
        _validator = new RateSpotCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SpotId_Is_Empty()
    {
        var command = new RateSpotCommand(Guid.Empty, Guid.NewGuid(), 5, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SpotId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new RateSpotCommand(Guid.NewGuid(), Guid.Empty, 5, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void Should_Have_Error_When_Rating_Is_Out_Of_Range(int rating)
    {
        var command = new RateSpotCommand(Guid.NewGuid(), Guid.NewGuid(), rating, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 5 stars");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Should_Not_Have_Error_When_Rating_Is_Valid(int rating)
    {
        var command = new RateSpotCommand(Guid.NewGuid(), Guid.NewGuid(), rating, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Should_Have_Error_When_Comment_Exceeds_MaxLength()
    {
        var command = new RateSpotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            new string('a', 1001));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage("Comment must not exceed 1000 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Comment_Is_Null()
    {
        var command = new RateSpotCommand(Guid.NewGuid(), Guid.NewGuid(), 5, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Comment_Is_Empty()
    {
        var command = new RateSpotCommand(Guid.NewGuid(), Guid.NewGuid(), 5, string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new RateSpotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            "Excellent diving spot with amazing coral formations!");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
