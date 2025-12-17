using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for UpdateSpotCommandValidator.
/// </summary>
public class UpdateSpotCommandValidatorTests
{
    private readonly UpdateSpotCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSpotCommandValidatorTests"/> class.
    /// </summary>
    public UpdateSpotCommandValidatorTests()
    {
        _validator = new UpdateSpotCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SpotId_Is_Empty()
    {
        var command = new UpdateSpotCommand(Guid.Empty, "Valid Name", "Valid description here", 30, 1, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SpotId);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), string.Empty, "Valid description here", 30, 1, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Short()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Ab", "Valid description here", 30, 1, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Short()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Short", 30, 1, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepth_Is_Zero()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 0, 1, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_Difficulty_Is_Out_Of_Range()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 5, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Difficulty);
    }

    [Fact]
    public void Should_Have_Error_When_Temperature_Is_Too_Low()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 1, -10, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentTemperatureCelsius);
    }

    [Fact]
    public void Should_Have_Error_When_Temperature_Is_Too_High()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 1, 60, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentTemperatureCelsius);
    }

    [Fact]
    public void Should_Have_Error_When_Visibility_Is_Zero()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 1, null, 0, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentVisibilityMeters);
    }

    [Fact]
    public void Should_Have_Error_When_Visibility_Exceeds_Limit()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 1, null, 150, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentVisibilityMeters);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UpdateSpotCommand(Guid.NewGuid(), "Valid Name", "Valid description here", 30, 1, null, null, Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateSpotCommand(
            Guid.NewGuid(),
            "Updated Spot Name",
            "This is an updated description with more details.",
            40,
            2,
            20,
            15,
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
    {
        var command = new UpdateSpotCommand(
            Guid.NewGuid(),
            "Updated Spot Name",
            "This is an updated description.",
            40,
            2,
            null,
            null,
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
