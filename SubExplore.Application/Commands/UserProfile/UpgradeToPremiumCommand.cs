using MediatR;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Command to upgrade a user to premium membership.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="PaymentMethod">The payment method used (e.g., CreditCard, PayPal, Stripe).</param>
/// <param name="PaymentToken">The payment token/transaction ID.</param>
/// <param name="SubscriptionPlan">The subscription plan (Monthly, Yearly).</param>
public record UpgradeToPremiumCommand(
    Guid UserId,
    string PaymentMethod,
    string PaymentToken,
    SubscriptionPlan SubscriptionPlan) : IRequest<UpgradeToPremiumResult>;

/// <summary>
/// Subscription plan options.
/// </summary>
public enum SubscriptionPlan
{
    /// <summary>
    /// Monthly subscription plan.
    /// </summary>
    Monthly,

    /// <summary>
    /// Yearly subscription plan with discount.
    /// </summary>
    Yearly,
}

/// <summary>
/// Result of premium upgrade operation.
/// </summary>
/// <param name="Success">Indicates whether the upgrade was successful.</param>
/// <param name="UserId">The user's ID.</param>
/// <param name="IsPremium">Indicates whether the user is now premium.</param>
/// <param name="PremiumExpiresAt">When the premium membership expires.</param>
public record UpgradeToPremiumResult(
    bool Success,
    Guid UserId,
    bool IsPremium,
    DateTime? PremiumExpiresAt);
