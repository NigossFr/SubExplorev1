using FluentValidation;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Validator for ShareDiveLogCommand.
/// </summary>
public class ShareDiveLogCommandValidator : AbstractValidator<ShareDiveLogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShareDiveLogCommandValidator"/> class.
    /// </summary>
    public ShareDiveLogCommandValidator()
    {
        RuleFor(x => x.DiveLogId)
            .NotEmpty().WithMessage("Dive log ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.SharedWithUserIds)
            .NotNull().WithMessage("Shared with users list cannot be null")
            .NotEmpty().WithMessage("At least one user must be specified to share with")
            .Must(list => list != null && list.Count <= 50).WithMessage("Cannot share with more than 50 users at once");

        RuleForEach(x => x.SharedWithUserIds)
            .NotEmpty().WithMessage("User IDs in the shared list cannot be empty")
            .When(x => x.SharedWithUserIds != null);

        RuleFor(x => x)
            .Must(x => x.SharedWithUserIds != null && !x.SharedWithUserIds.Contains(x.UserId))
            .WithMessage("Cannot share dive log with yourself")
            .When(x => x.SharedWithUserIds != null);

        RuleFor(x => x.Message)
            .MaximumLength(500).WithMessage("Share message must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Message));
    }
}
