using FluentValidation.TestHelper;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for LoginCommandValidator.
/// </summary>
public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidatorTests"/> class.
    /// </summary>
    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new LoginCommand(string.Empty, "Password123!");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    public void Should_Have_Error_When_Email_Format_Is_Invalid(string email)
    {
        var command = new LoginCommand(email, "Password123!");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new LoginCommand("test@example.com", string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_And_Password_Are_Valid()
    {
        var command = new LoginCommand("test@example.com", "Password123!");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Validate_Password_Complexity_During_Login()
    {
        // Login validation should not check password complexity
        // (only check if password is provided)
        var command = new LoginCommand("test@example.com", "weak");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
