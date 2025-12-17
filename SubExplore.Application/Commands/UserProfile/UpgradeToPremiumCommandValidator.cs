using FluentValidation;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Validator for UpgradeToPremiumCommand.
/// </summary>
public class UpgradeToPremiumCommandValidator : AbstractValidator<UpgradeToPremiumCommand>
{
    private static readonly string[] AllowedPaymentMethods = { "CreditCard", "PayPal", "Stripe", "ApplePay", "GooglePay" };

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeToPremiumCommandValidator"/> class.
    /// </summary>
    public UpgradeToPremiumCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required")
            .Must(BeAllowedPaymentMethod).WithMessage("Payment method must be one of: CreditCard, PayPal, Stripe, ApplePay, GooglePay");

        RuleFor(x => x.PaymentToken)
            .NotEmpty().WithMessage("Payment token is required")
            .MaximumLength(500).WithMessage("Payment token must not exceed 500 characters");

        RuleFor(x => x.SubscriptionPlan)
            .IsInEnum().WithMessage("Invalid subscription plan");
    }

    private static bool BeAllowedPaymentMethod(string paymentMethod)
    {
        return AllowedPaymentMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);
    }
}
