using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for CreateSpotCommandValidator.
/// </summary>
public class CreateSpotCommandValidatorTests
{
    private readonly CreateSpotCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSpotCommandValidatorTests"/> class.
    /// </summary>
    public CreateSpotCommandValidatorTests()
    {
        _validator = new CreateSpotCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateSpotCommand(string.Empty, "Valid description here", 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Short()
    {
        var command = new CreateSpotCommand("Ab", "Valid description here", 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Spot name must be at least 3 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        var command = new CreateSpotCommand(new string('a', 101), "Valid description here", 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Spot name must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new CreateSpotCommand("Valid Name", string.Empty, 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Short()
    {
        var command = new CreateSpotCommand("Valid Name", "Short", 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must be at least 10 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var command = new CreateSpotCommand("Valid Name", new string('a', 1001), 45.5, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-100)]
    [InlineData(100)]
    public void Should_Have_Error_When_Latitude_Is_Out_Of_Range(double latitude)
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", latitude, 10.5, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90 degrees");
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(-200)]
    [InlineData(200)]
    public void Should_Have_Error_When_Longitude_Is_Out_Of_Range(double longitude)
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", 45.5, longitude, 30, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180 degrees");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Should_Have_Error_When_MaxDepth_Is_Zero_Or_Negative(double depth)
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", 45.5, 10.5, depth, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters)
            .WithErrorMessage("Maximum depth must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepth_Exceeds_Limit()
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", 45.5, 10.5, 501, 1, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters)
            .WithErrorMessage("Maximum depth must not exceed 500 meters");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(10)]
    public void Should_Have_Error_When_Difficulty_Is_Out_Of_Range(int difficulty)
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", 45.5, 10.5, 30, difficulty, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Difficulty)
            .WithErrorMessage("Difficulty must be between 0 (Beginner) and 3 (Expert)");
    }

    [Fact]
    public void Should_Have_Error_When_CreatedBy_Is_Empty()
    {
        var command = new CreateSpotCommand("Valid Name", "Valid description here", 45.5, 10.5, 30, 1, Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy)
            .WithErrorMessage("Creator user ID is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new CreateSpotCommand(
            "Great Diving Spot",
            "This is a wonderful diving spot with clear waters and abundant marine life.",
            45.5,
            10.5,
            30,
            2,
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-90, -180)] // Min valid values
    [InlineData(90, 180)]   // Max valid values
    [InlineData(0, 0)]      // Equator, Prime Meridian
    public void Should_Not_Have_Error_When_Coordinates_Are_Valid(double latitude, double longitude)
    {
        var command = new CreateSpotCommand(
            "Valid Name",
            "Valid description here",
            latitude,
            longitude,
            30,
            1,
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Latitude);
        result.ShouldNotHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(0)] // Beginner
    [InlineData(1)] // Intermediate
    [InlineData(2)] // Advanced
    [InlineData(3)] // Expert
    public void Should_Not_Have_Error_When_Difficulty_Is_Valid(int difficulty)
    {
        var command = new CreateSpotCommand(
            "Valid Name",
            "Valid description here",
            45.5,
            10.5,
            30,
            difficulty,
            Guid.NewGuid());

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Difficulty);
    }
}
