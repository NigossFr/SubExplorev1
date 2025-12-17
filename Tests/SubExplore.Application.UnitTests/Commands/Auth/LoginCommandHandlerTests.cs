using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for LoginCommandHandler.
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandlerTests"/> class.
    /// </summary>
    public LoginCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _handler = new LoginCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_LoginResult()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal("test@example.com", result.Email);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public async Task Handle_Should_Log_Login_Attempt()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Login attempt for email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Login_Success()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User logged in successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Temporary_Tokens()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // This test verifies the placeholder behavior
        Assert.Equal("temporary_access_token", result.AccessToken);
        Assert.Equal("temporary_refresh_token", result.RefreshToken);
    }
}
