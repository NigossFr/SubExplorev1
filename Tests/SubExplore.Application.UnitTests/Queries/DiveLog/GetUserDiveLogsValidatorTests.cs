using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetUserDiveLogsValidatorTests
{
    private readonly GetUserDiveLogsValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null,
            null,
            "DiveDate",
            true,
            1,
            20);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Validate_StartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
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
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MinDepthNegative_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MinDepthMeters: -10);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.MinDepthMeters)
            .WithErrorMessage("Minimum depth must be greater than 0");
    }

    [Fact]
    public void Validate_MinDepthExceedsMaximum_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MinDepthMeters: 600);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.MinDepthMeters)
            .WithErrorMessage("Minimum depth must not exceed 500 meters");
    }

    [Fact]
    public void Validate_MaxDepthNegative_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MaxDepthMeters: -10);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters)
            .WithErrorMessage("Maximum depth must be greater than 0");
    }

    [Fact]
    public void Validate_MaxDepthExceedsMaximum_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MaxDepthMeters: 600);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.MaxDepthMeters)
            .WithErrorMessage("Maximum depth must not exceed 500 meters");
    }

    [Fact]
    public void Validate_MinDepthGreaterThanMaxDepth_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MinDepthMeters: 50,
            MaxDepthMeters: 30);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum depth must be less than or equal to maximum depth");
    }

    [Fact]
    public void Validate_ValidDepthRange_ShouldNotHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            MinDepthMeters: 10,
            MaxDepthMeters: 50);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DiveTypeBelowMinimum_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            DiveType: -1);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.DiveType)
            .WithErrorMessage("Dive type must be between 0 and 7");
    }

    [Fact]
    public void Validate_DiveTypeAboveMaximum_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            DiveType: 8);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.DiveType)
            .WithErrorMessage("Dive type must be between 0 and 7");
    }

    [Fact]
    public void Validate_ValidDiveType_ShouldNotHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            DiveType: 3);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidSortField_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            SortBy: "InvalidField");

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("Sort field must be one of: DiveDate, MaxDepth, Duration");
    }

    [Theory]
    [InlineData("DiveDate")]
    [InlineData("MaxDepth")]
    [InlineData("Duration")]
    public void Validate_ValidSortFields_ShouldNotHaveValidationError(string sortField)
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            SortBy: sortField);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PageNumberZero_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            PageNumber: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than 0");
    }

    [Fact]
    public void Validate_PageSizeZero_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            PageSize: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be greater than 0");
    }

    [Fact]
    public void Validate_PageSizeExceedsMaximum_ShouldHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            PageSize: 101);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must not exceed 100");
    }

    [Fact]
    public void Validate_ValidPagination_ShouldNotHaveValidationError()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            PageNumber: 2,
            PageSize: 50);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
