using FluentValidation;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Validator for GetDiveLogsBySpotQuery.
/// </summary>
public class GetDiveLogsBySpotValidator : AbstractValidator<GetDiveLogsBySpotQuery>
{
    private static readonly string[] ValidSortFields = { "DiveDate", "MaxDepth", "Duration" };

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDiveLogsBySpotValidator"/> class.
    /// </summary>
    public GetDiveLogsBySpotValidator()
    {
        RuleFor(x => x.DivingSpotId)
            .NotEmpty().WithMessage("Diving spot ID is required");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.MinDepthMeters)
            .GreaterThan(0).WithMessage("Minimum depth must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Minimum depth must not exceed 500 meters")
            .When(x => x.MinDepthMeters.HasValue);

        RuleFor(x => x.MaxDepthMeters)
            .GreaterThan(0).WithMessage("Maximum depth must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Maximum depth must not exceed 500 meters")
            .When(x => x.MaxDepthMeters.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinDepthMeters.HasValue || !x.MaxDepthMeters.HasValue || x.MinDepthMeters <= x.MaxDepthMeters)
            .WithMessage("Minimum depth must be less than or equal to maximum depth")
            .When(x => x.MinDepthMeters.HasValue && x.MaxDepthMeters.HasValue);

        RuleFor(x => x.SortBy)
            .Must(x => ValidSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Sort field must be one of: {string.Join(", ", ValidSortFields)}");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");
    }
}
