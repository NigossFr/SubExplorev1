using FluentValidation.TestHelper;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpgradeToPremiumCommandValidator.
/// </summary>
public class UpgradeToPremiumCommandValidatorTests
{
    private readonly UpgradeToPremiumCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeToPremiumCommandValidatorTests"/> class.
    /// </summary>
    public UpgradeToPremiumCommandValidatorTests()
    {
        _validator = new UpgradeToPremiumCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UpgradeToPremiumCommand(
            Guid.Empty,
            "CreditCard",
            "token123",
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_PaymentMethod_Is_Empty()
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            string.Empty,
            "token123",
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod)
            .WithErrorMessage("Payment method is required");
    }

    [Theory]
    [InlineData("Bitcoin")]
    [InlineData("Cash")]
    [InlineData("Unknown")]
    public void Should_Have_Error_When_PaymentMethod_Is_Not_Allowed(string paymentMethod)
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            paymentMethod,
            "token123",
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod)
            .WithErrorMessage("Payment method must be one of: CreditCard, PayPal, Stripe, ApplePay, GooglePay");
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("PayPal")]
    [InlineData("Stripe")]
    [InlineData("ApplePay")]
    [InlineData("GooglePay")]
    [InlineData("creditcard")] // Test case insensitivity
    [InlineData("PAYPAL")] // Test case insensitivity
    public void Should_Not_Have_Error_When_PaymentMethod_Is_Allowed(string paymentMethod)
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            paymentMethod,
            "token123",
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Should_Have_Error_When_PaymentToken_Is_Empty()
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            "CreditCard",
            string.Empty,
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PaymentToken)
            .WithErrorMessage("Payment token is required");
    }

    [Fact]
    public void Should_Have_Error_When_PaymentToken_Exceeds_MaxLength()
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            "CreditCard",
            new string('a', 501),
            SubscriptionPlan.Monthly);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PaymentToken)
            .WithErrorMessage("Payment token must not exceed 500 characters");
    }

    [Theory]
    [InlineData(SubscriptionPlan.Monthly)]
    [InlineData(SubscriptionPlan.Yearly)]
    public void Should_Not_Have_Error_When_SubscriptionPlan_Is_Valid(SubscriptionPlan plan)
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            "CreditCard",
            "token123",
            plan);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.SubscriptionPlan);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpgradeToPremiumCommand(
            Guid.NewGuid(),
            "Stripe",
            "tok_1234567890abcdef",
            SubscriptionPlan.Yearly);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
