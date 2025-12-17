using FluentValidation;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Validator for UpdateDivingCertificationsCommand.
/// </summary>
public class UpdateDivingCertificationsCommandValidator : AbstractValidator<UpdateDivingCertificationsCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDivingCertificationsCommandValidator"/> class.
    /// </summary>
    public UpdateDivingCertificationsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Certifications)
            .NotNull().WithMessage("Certifications list cannot be null");

        When(x => x.Certifications != null, () =>
        {
            RuleFor(x => x.Certifications)
                .Must(list => list!.Count <= 20).WithMessage("Cannot add more than 20 certifications");
        });

        RuleForEach(x => x.Certifications).ChildRules(certification =>
        {
            certification.RuleFor(c => c.Organization)
                .NotEmpty().WithMessage("Organization is required")
                .MaximumLength(50).WithMessage("Organization must not exceed 50 characters");

            certification.RuleFor(c => c.Level)
                .NotEmpty().WithMessage("Certification level is required")
                .MaximumLength(100).WithMessage("Level must not exceed 100 characters");

            certification.RuleFor(c => c.CertificationNumber)
                .MaximumLength(50).WithMessage("Certification number must not exceed 50 characters")
                .When(c => !string.IsNullOrEmpty(c.CertificationNumber));

            certification.RuleFor(c => c.IssueDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Issue date cannot be in the future")
                .GreaterThan(new DateTime(1950, 1, 1)).WithMessage("Issue date must be after 1950")
                .When(c => c.IssueDate.HasValue);
        });
    }
}
