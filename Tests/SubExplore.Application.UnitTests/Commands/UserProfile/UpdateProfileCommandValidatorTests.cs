using FluentValidation.TestHelper;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpdateProfileCommandValidator.
/// </summary>
public class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProfileCommandValidatorTests"/> class.
    /// </summary>
    public UpdateProfileCommandValidatorTests()
    {
        _validator = new UpdateProfileCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UpdateProfileCommand(Guid.Empty, "John", "Doe", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), string.Empty, "Doe", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_MaxLength()
    {
        var command = new UpdateProfileCommand(
            Guid.NewGuid(),
            new string('a', 51),
            "Doe",
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", string.Empty, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_MaxLength()
    {
        var command = new UpdateProfileCommand(
            Guid.NewGuid(),
            "John",
            new string('a', 51),
            null,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Bio_Exceeds_MaxLength()
    {
        var command = new UpdateProfileCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            new string('a', 501),
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio must not exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Bio_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public void Should_Have_Error_When_ProfilePictureUrl_Exceeds_MaxLength()
    {
        var command = new UpdateProfileCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            null,
            new string('a', 501));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProfilePictureUrl)
            .WithErrorMessage("Profile picture URL must not exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProfilePictureUrl_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ProfilePictureUrl);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Required_Fields_Are_Valid()
    {
        var command = new UpdateProfileCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            "Experienced scuba diver",
            "https://example.com/avatar.jpg");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
