using FluentValidation;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Validator for SearchSpotsQuery.
/// </summary>
public class SearchSpotsValidator : AbstractValidator<SearchSpotsQuery>
{
    private static readonly string[] ValidSortFields = { "Name", "Rating", "Depth", "CreatedAt" };

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSpotsValidator"/> class.
    /// </summary>
    public SearchSpotsValidator()
    {
        RuleFor(x => x.SearchText)
            .MaximumLength(100).WithMessage("Search text must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchText));

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

        RuleFor(x => x.MinRating)
            .GreaterThanOrEqualTo(1).WithMessage("Minimum rating must be at least 1")
            .LessThanOrEqualTo(5).WithMessage("Minimum rating must not exceed 5")
            .When(x => x.MinRating.HasValue);

        RuleFor(x => x.MinTemperatureCelsius)
            .GreaterThanOrEqualTo(-5).WithMessage("Minimum temperature must be at least -5°C")
            .LessThanOrEqualTo(50).WithMessage("Minimum temperature must not exceed 50°C")
            .When(x => x.MinTemperatureCelsius.HasValue);

        RuleFor(x => x.MaxTemperatureCelsius)
            .GreaterThanOrEqualTo(-5).WithMessage("Maximum temperature must be at least -5°C")
            .LessThanOrEqualTo(50).WithMessage("Maximum temperature must not exceed 50°C")
            .When(x => x.MaxTemperatureCelsius.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinTemperatureCelsius.HasValue || !x.MaxTemperatureCelsius.HasValue || x.MinTemperatureCelsius <= x.MaxTemperatureCelsius)
            .WithMessage("Minimum temperature must be less than or equal to maximum temperature")
            .When(x => x.MinTemperatureCelsius.HasValue && x.MaxTemperatureCelsius.HasValue);

        RuleFor(x => x.MinVisibilityMeters)
            .GreaterThan(0).WithMessage("Minimum visibility must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Minimum visibility must not exceed 100 meters")
            .When(x => x.MinVisibilityMeters.HasValue);

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
