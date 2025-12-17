using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.Auth;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.Auth;

/// <summary>
/// Unit tests for RegisterUserCommandHandler.
/// </summary>
public class RegisterUserCommandHandlerTests
{
    private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
    private readonly RegisterUserCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandlerTests"/> class.
    /// </summary>
    public RegisterUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
        _handler = new RegisterUserCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_RegisterUserResult()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            "Doe");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Email, result.Email);
        Assert.Equal(command.Username, result.Username);
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task Handle_Should_Log_Registration_Attempt()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            "Doe");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Registering new user with email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Registration_Success()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            "Doe");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User registered successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Generate_Unique_UserId()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "testuser",
            "John",
            "Doe");

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.UserId, result2.UserId);
    }
}
