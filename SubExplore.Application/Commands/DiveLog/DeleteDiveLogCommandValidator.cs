using FluentValidation;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Validator for DeleteDiveLogCommand.
/// </summary>
public class DeleteDiveLogCommandValidator : AbstractValidator<DeleteDiveLogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDiveLogCommandValidator"/> class.
    /// </summary>
    public DeleteDiveLogCommandValidator()
    {
        RuleFor(x => x.DiveLogId)
            .NotEmpty().WithMessage("Dive log ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
