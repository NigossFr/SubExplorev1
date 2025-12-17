using FluentValidation;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Validator for GetNearbySpotsQuery.
/// </summary>
public class GetNearbySpotsValidator : AbstractValidator<GetNearbySpotsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetNearbySpotsValidator"/> class.
    /// </summary>
    public GetNearbySpotsValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees");

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0).WithMessage("Radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Radius must not exceed 100 kilometers");

        RuleFor(x => x.MinDifficulty)
            .InclusiveBetween(0, 3).WithMessage("Minimum difficulty must be between 0 (Beginner) and 3 (Expert)")
            .When(x => x.MinDifficulty.HasValue);

        RuleFor(x => x.MaxDifficulty)
            .InclusiveBetween(0, 3).WithMessage("Maximum difficulty must be between 0 (Beginner) and 3 (Expert)")
            .When(x => x.MaxDifficulty.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinDifficulty.HasValue || !x.MaxDifficulty.HasValue || x.MinDifficulty <= x.MaxDifficulty)
            .WithMessage("Minimum difficulty must be less than or equal to maximum difficulty")
            .When(x => x.MinDifficulty.HasValue && x.MaxDifficulty.HasValue);

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

        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Limit must not exceed 100 results");
    }
}
