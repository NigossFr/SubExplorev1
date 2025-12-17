using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for RefreshTokenCommandHandler.
/// </summary>
public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock;
    private readonly RefreshTokenCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandHandlerTests"/> class.
    /// </summary>
    public RefreshTokenCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RefreshTokenCommandHandler>>();
        _handler = new RefreshTokenCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_RefreshTokenResult()
    {
        // Arrange
        var command = new RefreshTokenCommand("old_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public async Task Handle_Should_Log_Refresh_Attempt()
    {
        // Arrange
        var command = new RefreshTokenCommand("old_refresh_token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Refreshing access token")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Refresh_Success()
    {
        // Arrange
        var command = new RefreshTokenCommand("old_refresh_token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Access token refreshed successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_New_Tokens()
    {
        // Arrange
        var command = new RefreshTokenCommand("old_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // This test verifies the placeholder behavior
        Assert.Equal("new_access_token", result.AccessToken);
        Assert.Equal("new_refresh_token", result.RefreshToken);
    }
}
