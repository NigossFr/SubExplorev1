using FluentValidation;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Validator for CreateDiveLogCommand.
/// </summary>
public class CreateDiveLogCommandValidator : AbstractValidator<CreateDiveLogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDiveLogCommandValidator"/> class.
    /// </summary>
    public CreateDiveLogCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DivingSpotId)
            .NotEmpty().WithMessage("Diving spot ID is required");

        RuleFor(x => x.DiveDate)
            .NotEmpty().WithMessage("Dive date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Dive date cannot be more than 1 day in the future");

        RuleFor(x => x.EntryTime)
            .LessThan(x => x.ExitTime).WithMessage("Entry time must be before exit time");

        RuleFor(x => x)
            .Must(x => (x.ExitTime - x.EntryTime).TotalMinutes >= 1)
            .WithMessage("Dive duration must be at least 1 minute")
            .Must(x => (x.ExitTime - x.EntryTime).TotalMinutes <= 600)
            .WithMessage("Dive duration must not exceed 600 minutes (10 hours)");

        RuleFor(x => x.MaxDepthMeters)
            .GreaterThan(0).WithMessage("Maximum depth must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Maximum depth must not exceed 500 meters");

        RuleFor(x => x.AverageDepthMeters)
            .GreaterThan(0).WithMessage("Average depth must be greater than 0")
            .LessThanOrEqualTo(x => x.MaxDepthMeters).WithMessage("Average depth cannot exceed maximum depth")
            .When(x => x.AverageDepthMeters.HasValue);

        RuleFor(x => x.WaterTemperatureCelsius)
            .InclusiveBetween(-5, 50).WithMessage("Water temperature must be between -5 and 50 degrees Celsius")
            .When(x => x.WaterTemperatureCelsius.HasValue);

        RuleFor(x => x.VisibilityMeters)
            .GreaterThan(0).WithMessage("Visibility must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Visibility must not exceed 100 meters")
            .When(x => x.VisibilityMeters.HasValue);

        RuleFor(x => x.DiveType)
            .InclusiveBetween(0, 7).WithMessage("Dive type must be between 0 (Recreational) and 7 (Deep)");

        RuleFor(x => x.Equipment)
            .MaximumLength(500).WithMessage("Equipment description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Equipment));

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
