using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for ShareDiveLogCommandValidator.
/// </summary>
public class ShareDiveLogCommandValidatorTests
{
    private readonly ShareDiveLogCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShareDiveLogCommandValidatorTests"/> class.
    /// </summary>
    public ShareDiveLogCommandValidatorTests()
    {
        _validator = new ShareDiveLogCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DiveLogId_Is_Empty()
    {
        var command = new ShareDiveLogCommand(
            Guid.Empty,
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiveLogId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.Empty,
            new List<Guid> { Guid.NewGuid() },
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Have_Error_When_SharedWithUserIds_Is_Null()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SharedWithUserIds);
    }

    [Fact]
    public void Should_Have_Error_When_SharedWithUserIds_Is_Empty()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<Guid>(),
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SharedWithUserIds);
    }

    [Fact]
    public void Should_Have_Error_When_SharedWithUserIds_Exceeds_Limit()
    {
        var sharedWithUsers = Enumerable.Range(0, 51).Select(_ => Guid.NewGuid()).ToList();
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            sharedWithUsers,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SharedWithUserIds);
    }

    [Fact]
    public void Should_Have_Error_When_SharedWithUserIds_Contains_Empty_Guid()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<Guid> { Guid.Empty, Guid.NewGuid() },
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("SharedWithUserIds[0]");
    }

    [Fact]
    public void Should_Have_Error_When_User_Shares_With_Themselves()
    {
        var userId = Guid.NewGuid();
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            userId,
            new List<Guid> { userId, Guid.NewGuid() },
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Have_Error_When_Message_Exceeds_MaxLength()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            new string('a', 501));

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            "Check out this amazing dive!");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Message_Is_Null()
    {
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Sharing_With_Maximum_Allowed_Users()
    {
        var sharedWithUsers = Enumerable.Range(0, 50).Select(_ => Guid.NewGuid()).ToList();
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            sharedWithUsers,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
