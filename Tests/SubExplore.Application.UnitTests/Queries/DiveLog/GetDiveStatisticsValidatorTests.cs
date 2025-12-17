using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetDiveStatisticsValidatorTests
{
    private readonly GetDiveStatisticsValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            null,
            null);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Validate_StartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(-1));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Start date must be before or equal to end date");
    }

    [Fact]
    public void Validate_ValidDateRange_ShouldNotHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EndDateInFuture_ShouldHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow.AddDays(2));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date cannot be in the future");
    }

    [Fact]
    public void Validate_EndDateToday_ShouldNotHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyStartDateProvided_ShouldNotHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-30),
            null);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyEndDateProvided_ShouldNotHaveValidationError()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            null,
            DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
