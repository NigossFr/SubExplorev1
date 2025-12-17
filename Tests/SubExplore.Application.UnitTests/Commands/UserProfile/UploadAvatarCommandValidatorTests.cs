using FluentValidation.TestHelper;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UploadAvatarCommandValidator.
/// </summary>
public class UploadAvatarCommandValidatorTests
{
    private readonly UploadAvatarCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadAvatarCommandValidatorTests"/> class.
    /// </summary>
    public UploadAvatarCommandValidatorTests()
    {
        _validator = new UploadAvatarCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UploadAvatarCommand(
            Guid.Empty,
            "avatar.jpg",
            "image/jpeg",
            new byte[] { 1, 2, 3 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Empty()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            string.Empty,
            "image/jpeg",
            new byte[] { 1, 2, 3 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Exceeds_MaxLength()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            new string('a', 256),
            "image/jpeg",
            new byte[] { 1, 2, 3 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name must not exceed 255 characters");
    }

    [Fact]
    public void Should_Have_Error_When_ContentType_Is_Empty()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            string.Empty,
            new byte[] { 1, 2, 3 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Content type is required");
    }

    [Theory]
    [InlineData("image/gif")]
    [InlineData("image/bmp")]
    [InlineData("application/pdf")]
    [InlineData("text/plain")]
    public void Should_Have_Error_When_ContentType_Is_Not_Allowed(string contentType)
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            contentType,
            new byte[] { 1, 2, 3 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Content type must be one of: image/jpeg, image/png, image/webp");
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/jpg")]
    [InlineData("image/png")]
    [InlineData("image/webp")]
    [InlineData("IMAGE/JPEG")] // Test case insensitivity
    public void Should_Not_Have_Error_When_ContentType_Is_Allowed(string contentType)
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            contentType,
            new byte[1000]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void Should_Have_Error_When_FileData_Is_Empty()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            "image/jpeg",
            Array.Empty<byte>());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileData)
            .WithErrorMessage("File data is required");
    }

    [Fact]
    public void Should_Have_Error_When_FileData_Exceeds_MaxSize()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            "image/jpeg",
            new byte[6 * 1024 * 1024]); // 6 MB

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileData)
            .WithErrorMessage("File size must not exceed 5 MB");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UploadAvatarCommand(
            Guid.NewGuid(),
            "avatar.jpg",
            "image/jpeg",
            new byte[1024 * 1024]); // 1 MB

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
