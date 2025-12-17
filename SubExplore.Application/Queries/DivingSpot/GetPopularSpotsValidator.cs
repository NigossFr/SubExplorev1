using FluentValidation;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Validator for GetPopularSpotsQuery.
/// </summary>
public class GetPopularSpotsValidator : AbstractValidator<GetPopularSpotsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPopularSpotsValidator"/> class.
    /// </summary>
    public GetPopularSpotsValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Limit must not exceed 50 results");

        RuleFor(x => x.MinimumRatings)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum ratings must be 0 or greater")
            .LessThanOrEqualTo(1000).WithMessage("Minimum ratings must not exceed 1000");

        RuleFor(x => x.DaysBack)
            .GreaterThan(0).WithMessage("Days back must be greater than 0")
            .LessThanOrEqualTo(365).WithMessage("Days back must not exceed 365 days");
    }
}
