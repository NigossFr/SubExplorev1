using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for AddSpotPhotoCommandValidator.
/// </summary>
public class AddSpotPhotoCommandValidatorTests
{
    private readonly AddSpotPhotoCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpotPhotoCommandValidatorTests"/> class.
    /// </summary>
    public AddSpotPhotoCommandValidatorTests()
    {
        _validator = new AddSpotPhotoCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SpotId_Is_Empty()
    {
        var command = new AddSpotPhotoCommand(Guid.Empty, "https://example.com/photo.jpg", null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SpotId);
    }

    [Fact]
    public void Should_Have_Error_When_Url_Is_Empty()
    {
        var command = new AddSpotPhotoCommand(Guid.NewGuid(), string.Empty, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Url);
    }

    [Fact]
    public void Should_Have_Error_When_Url_Exceeds_MaxLength()
    {
        var command = new AddSpotPhotoCommand(Guid.NewGuid(), new string('a', 501), null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage("Photo URL must not exceed 500 characters");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/photo.jpg")]
    [InlineData("file:///local/photo.jpg")]
    [InlineData("javascript:alert('xss')")]
    public void Should_Have_Error_When_Url_Is_Not_Valid_Http_Url(string url)
    {
        var command = new AddSpotPhotoCommand(Guid.NewGuid(), url, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage("Photo URL must be a valid URL");
    }

    [Theory]
    [InlineData("https://example.com/photo.jpg")]
    [InlineData("http://example.com/photo.png")]
    [InlineData("https://cdn.example.com/images/diving/photo123.jpg")]
    public void Should_Not_Have_Error_When_Url_Is_Valid(string url)
    {
        var command = new AddSpotPhotoCommand(Guid.NewGuid(), url, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Url);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photo.jpg",
            new string('a', 501),
            Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photo.jpg",
            null,
            Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new AddSpotPhotoCommand(Guid.NewGuid(), "https://example.com/photo.jpg", null, Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photos/diving-spot-123.jpg",
            "Beautiful coral reef formation",
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
