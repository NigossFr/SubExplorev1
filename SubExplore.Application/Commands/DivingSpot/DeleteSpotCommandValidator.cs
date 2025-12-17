using FluentValidation;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Validator for DeleteSpotCommand.
/// </summary>
public class DeleteSpotCommandValidator : AbstractValidator<DeleteSpotCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSpotCommandValidator"/> class.
    /// </summary>
    public DeleteSpotCommandValidator()
    {
        RuleFor(x => x.SpotId)
            .NotEmpty().WithMessage("Spot ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
