using FluentValidation.TestHelper;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetDiveLogByIdValidatorTests
{
    private readonly GetDiveLogByIdValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveValidationError()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyDiveLogId_ShouldHaveValidationError()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.Empty,
            Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.DiveLogId)
            .WithErrorMessage("Dive log ID is required");
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldHaveValidationError()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.NewGuid(),
            Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Validate_BothIdsEmpty_ShouldHaveMultipleValidationErrors()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.Empty,
            Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.DiveLogId);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
