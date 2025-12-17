using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpgradeToPremiumCommandHandler.
/// </summary>
public class UpgradeToPremiumCommandHandlerTests
{
    private readonly Mock<ILogger<UpgradeToPremiumCommandHandler>> _loggerMock;
    private readonly UpgradeToPremiumCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeToPremiumCommandHandlerTests"/> class.
    /// </summary>
    public UpgradeToPremiumCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpgradeToPremiumCommandHandler>>();
        _handler = new UpgradeToPremiumCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_UpgradeToPremiumResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpgradeToPremiumCommand(
            userId,
            "Stripe",
            "tok_12345",
            SubscriptionPlan.Monthly);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.IsPremium);
        Assert.NotNull(result.PremiumExpiresAt);
    }

    [Fact]
    public async Task Handle_Should_Set_Monthly_Expiration_For_Monthly_Plan()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpgradeToPremiumCommand(
            userId,
            "Stripe",
            "tok_12345",
            SubscriptionPlan.Monthly);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.PremiumExpiresAt);
        var expiresAt = result.PremiumExpiresAt.Value;
        var expectedExpiry = DateTime.UtcNow.AddDays(30);

        // Allow 1 second tolerance for execution time
        Assert.InRange(expiresAt, expectedExpiry.AddSeconds(-1), expectedExpiry.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_Should_Set_Yearly_Expiration_For_Yearly_Plan()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpgradeToPremiumCommand(
            userId,
            "Stripe",
            "tok_12345",
            SubscriptionPlan.Yearly);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.PremiumExpiresAt);
        var expiresAt = result.PremiumExpiresAt.Value;
        var expectedExpiry = DateTime.UtcNow.AddDays(365);

        // Allow 1 second tolerance for execution time
        Assert.InRange(expiresAt, expectedExpiry.AddSeconds(-1), expectedExpiry.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_Should_Log_Upgrade_Start()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpgradeToPremiumCommand(
            userId,
            "Stripe",
            "tok_12345",
            SubscriptionPlan.Monthly);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing premium upgrade for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Upgrade_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpgradeToPremiumCommand(
            userId,
            "Stripe",
            "tok_12345",
            SubscriptionPlan.Monthly);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Premium upgrade successful for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
