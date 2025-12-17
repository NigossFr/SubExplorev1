using FluentValidation.TestHelper;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for LogoutCommandValidator.
/// </summary>
public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandValidatorTests"/> class.
    /// </summary>
    public LogoutCommandValidatorTests()
    {
        _validator = new LogoutCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new LogoutCommand(Guid.Empty, "valid_refresh_token");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_Is_Empty()
    {
        var command = new LogoutCommand(Guid.NewGuid(), string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new LogoutCommand(Guid.NewGuid(), "valid_refresh_token");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
