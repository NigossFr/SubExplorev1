using FluentValidation;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Validator for RateSpotCommand.
/// </summary>
public class RateSpotCommandValidator : AbstractValidator<RateSpotCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateSpotCommandValidator"/> class.
    /// </summary>
    public RateSpotCommandValidator()
    {
        RuleFor(x => x.SpotId)
            .NotEmpty().WithMessage("Spot ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
