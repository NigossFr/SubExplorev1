using FluentValidation;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Validator for UploadAvatarCommand.
/// </summary>
public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadAvatarCommandValidator"/> class.
    /// </summary>
    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .Must(BeAllowedContentType).WithMessage("Content type must be one of: image/jpeg, image/png, image/webp");

        RuleFor(x => x.FileData)
            .NotEmpty().WithMessage("File data is required")
            .Must(data => data.Length <= MaxFileSizeBytes).WithMessage("File size must not exceed 5 MB");
    }

    private static bool BeAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(contentType.ToLowerInvariant());
    }
}
