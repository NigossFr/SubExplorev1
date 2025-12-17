using FluentValidation;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Validator for GetDiveStatisticsQuery.
/// </summary>
public class GetDiveStatisticsValidator : AbstractValidator<GetDiveStatisticsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDiveStatisticsValidator"/> class.
    /// </summary>
    public GetDiveStatisticsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("End date cannot be in the future")
            .When(x => x.EndDate.HasValue);
    }
}
