using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for DeleteSpotCommandValidator.
/// </summary>
public class DeleteSpotCommandValidatorTests
{
    private readonly DeleteSpotCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSpotCommandValidatorTests"/> class.
    /// </summary>
    public DeleteSpotCommandValidatorTests()
    {
        _validator = new DeleteSpotCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SpotId_Is_Empty()
    {
        var command = new DeleteSpotCommand(Guid.Empty, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SpotId)
            .WithErrorMessage("Spot ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new DeleteSpotCommand(Guid.NewGuid(), Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new DeleteSpotCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
