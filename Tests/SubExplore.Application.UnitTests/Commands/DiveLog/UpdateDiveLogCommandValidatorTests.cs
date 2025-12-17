using FluentValidation.TestHelper;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for UpdateDiveLogCommandValidator.
/// </summary>
public class UpdateDiveLogCommandValidatorTests
{
    private readonly UpdateDiveLogCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDiveLogCommandValidatorTests"/> class.
    /// </summary>
    public UpdateDiveLogCommandValidatorTests()
    {
        _validator = new UpdateDiveLogCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DiveLogId_Is_Empty()
    {
        var command = new UpdateDiveLogCommand(
            Guid.Empty,
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiveLogId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.Empty,
            30,
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_MaxDepth_Is_Zero_Or_Negative(double depth)
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            depth,
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_MaxDepth_Exceeds_Limit()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            501,
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters);
    }

    [Fact]
    public void Should_Have_Error_When_AverageDepth_Exceeds_MaxDepth()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            35,
            null,
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
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            temperature,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WaterTemperatureCelsius);
    }

    [Fact]
    public void Should_Have_Error_When_Visibility_Exceeds_Limit()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            101,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VisibilityMeters);
    }

    [Fact]
    public void Should_Have_Error_When_Equipment_Exceeds_MaxLength()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            new string('a', 501),
            null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Equipment);
    }

    [Fact]
    public void Should_Have_Error_When_Notes_Exceeds_MaxLength()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            null,
            new string('a', 2001));

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            25,
            20,
            15,
            "Wetsuit 5mm, BCD, Regulator",
            "Great dive with excellent visibility!");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
    {
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            null,
            null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
