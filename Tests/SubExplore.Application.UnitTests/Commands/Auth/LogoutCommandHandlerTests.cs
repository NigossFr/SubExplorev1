using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for LogoutCommandHandler.
/// </summary>
public class LogoutCommandHandlerTests
{
    private readonly Mock<ILogger<LogoutCommandHandler>> _loggerMock;
    private readonly LogoutCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandlerTests"/> class.
    /// </summary>
    public LogoutCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<LogoutCommandHandler>>();
        _handler = new LogoutCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_LogoutResult_With_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new LogoutCommand(userId, "valid_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handle_Should_Log_Logout_Attempt()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new LogoutCommand(userId, "valid_refresh_token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Logging out user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Logout_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new LogoutCommand(userId, "valid_refresh_token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User logged out successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
