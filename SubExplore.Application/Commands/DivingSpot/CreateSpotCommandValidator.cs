using FluentValidation;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Validator for CreateSpotCommand.
/// </summary>
public class CreateSpotCommandValidator : AbstractValidator<CreateSpotCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSpotCommandValidator"/> class.
    /// </summary>
    public CreateSpotCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Spot name is required")
            .MinimumLength(3).WithMessage("Spot name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Spot name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees");

        RuleFor(x => x.MaxDepthMeters)
            .GreaterThan(0).WithMessage("Maximum depth must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Maximum depth must not exceed 500 meters");

        RuleFor(x => x.Difficulty)
            .InclusiveBetween(0, 3).WithMessage("Difficulty must be between 0 (Beginner) and 3 (Expert)");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Creator user ID is required");
    }
}
