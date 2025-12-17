using FluentValidation.TestHelper;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for RegisterUserCommandValidator.
/// </summary>
public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandValidatorTests"/> class.
    /// </summary>
    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new RegisterUserCommand(
            string.Empty,
            "Password123!",
            "testuser",
            "John",
            "Doe");

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
        var command = new RegisterUserCommand(
            email,
            "Password123!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_MaxLength()
    {
        // Create an email with 256 characters: local part + @ + domain
        var localPart = new string('a', 240); // 240 characters
        var command = new RegisterUserCommand(
            $"{localPart}@example.com", // 240 + 1 + 11 = 252 characters (valid email format)
            "Password123!",
            "testuser",
            "John",
            "Doe");

        // Create a command with email that definitely exceeds 255
        var longCommand = new RegisterUserCommand(
            $"{new string('a', 245)}@example.com", // 245 + 1 + 11 = 257 characters
            "Password123!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(longCommand);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 255 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            string.Empty,
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Pass1!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_MaxLength()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            new string('a', 101),
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Uppercase()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "password123!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Lowercase()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "PASSWORD123!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Digit()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_SpecialChar()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            string.Empty,
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Short()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "ab",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username must be at least 3 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Exceeds_MaxLength()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            new string('a', 51),
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username must not exceed 50 characters");
    }

    [Theory]
    [InlineData("user name")]
    [InlineData("user@name")]
    [InlineData("user#name")]
    [InlineData("user name!")]
    public void Should_Have_Error_When_Username_Contains_Invalid_Characters(string username)
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            username,
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username can only contain letters, numbers, hyphens and underscores");
    }

    [Theory]
    [InlineData("user_name")]
    [InlineData("user-name")]
    [InlineData("user123")]
    [InlineData("User_Name-123")]
    public void Should_Not_Have_Error_When_Username_Is_Valid(string username)
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            username,
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            string.Empty,
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_MaxLength()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            new string('a', 101),
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_MaxLength()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            new string('a', 101));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must not exceed 100 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
