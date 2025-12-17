using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for CreateDiveLogCommandValidator.
/// </summary>
public class CreateDiveLogCommandValidatorTests
{
    private readonly CreateDiveLogCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDiveLogCommandValidatorTests"/> class.
    /// </summary>
    public CreateDiveLogCommandValidatorTests()
    {
        _validator = new CreateDiveLogCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new CreateDiveLogCommand(
            Guid.Empty,
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            25,
            20,
            15,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Have_Error_When_DivingSpotId_Is_Empty()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.Empty,
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DivingSpotId);
    }

    [Fact]
    public void Should_Have_Error_When_DiveDate_Is_More_Than_One_Day_In_Future()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(2),
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiveDate);
    }

    [Fact]
    public void Should_Have_Error_When_EntryTime_Is_After_ExitTime()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(11),
            TimeSpan.FromHours(10),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EntryTime);
    }

    [Fact]
    public void Should_Have_Error_When_Dive_Duration_Is_Too_Short()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(10).Add(TimeSpan.FromSeconds(30)),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Have_Error_When_Dive_Duration_Is_Too_Long()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(12),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_MaxDepth_Is_Zero_Or_Negative(double depth)
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            depth,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepth_Exceeds_Limit()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            501,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_AverageDepth_Exceeds_MaxDepth()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            35,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AverageDepthMeters);
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(51)]
    public void Should_Have_Error_When_WaterTemperature_Is_Out_Of_Range(double temperature)
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            temperature,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WaterTemperatureCelsius);
    }

    [Fact]
    public void Should_Have_Error_When_Visibility_Exceeds_Limit()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            101,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VisibilityMeters);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void Should_Have_Error_When_DiveType_Is_Out_Of_Range(int diveType)
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            diveType,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiveType);
    }

    [Fact]
    public void Should_Have_Error_When_Equipment_Exceeds_MaxLength()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
            null,
            new string('a', 501),
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Equipment);
    }

    [Fact]
    public void Should_Have_Error_When_Notes_Exceeds_MaxLength()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            new string('a', 2001));

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Required_Fields_Are_Valid()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            25,
            20,
            15,
            0,
            Guid.NewGuid(),
            "Wetsuit 5mm, BCD, Regulator",
            "Great dive with excellent visibility!");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
    {
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
