using FluentValidation;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Validator for UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProfileCommandValidator"/> class.
    /// </summary>
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.ProfilePictureUrl)
            .MaximumLength(500).WithMessage("Profile picture URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl));
    }
}
