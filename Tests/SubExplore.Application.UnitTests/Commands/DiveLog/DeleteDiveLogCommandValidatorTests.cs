using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for DeleteDiveLogCommandValidator.
/// </summary>
public class DeleteDiveLogCommandValidatorTests
{
    private readonly DeleteDiveLogCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDiveLogCommandValidatorTests"/> class.
    /// </summary>
    public DeleteDiveLogCommandValidatorTests()
    {
        _validator = new DeleteDiveLogCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DiveLogId_Is_Empty()
    {
        var command = new DeleteDiveLogCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiveLogId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new DeleteDiveLogCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new DeleteDiveLogCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
