using FluentValidation;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Validator for AddSpotPhotoCommand.
/// </summary>
public class AddSpotPhotoCommandValidator : AbstractValidator<AddSpotPhotoCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpotPhotoCommandValidator"/> class.
    /// </summary>
    public AddSpotPhotoCommandValidator()
    {
        RuleFor(x => x.SpotId)
            .NotEmpty().WithMessage("Spot ID is required");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Photo URL is required")
            .MaximumLength(500).WithMessage("Photo URL must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("Photo URL must be a valid URL");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
