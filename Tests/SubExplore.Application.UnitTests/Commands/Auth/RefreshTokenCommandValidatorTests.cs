using FluentValidation.TestHelper;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for RefreshTokenCommandValidator.
/// </summary>
public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandValidatorTests"/> class.
    /// </summary>
    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_Is_Empty()
    {
        var command = new RefreshTokenCommand(string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_RefreshToken_Is_Provided()
    {
        var command = new RefreshTokenCommand("valid_refresh_token");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
