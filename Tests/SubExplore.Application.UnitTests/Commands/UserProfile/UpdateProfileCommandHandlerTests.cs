using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpdateProfileCommandHandler.
/// </summary>
public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateProfileCommandHandler>> _loggerMock;
    private readonly UpdateProfileCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProfileCommandHandlerTests"/> class.
    /// </summary>
    public UpdateProfileCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateProfileCommandHandler>>();
        _handler = new UpdateProfileCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_UpdateProfileResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateProfileCommand(userId, "John", "Doe", "Bio", "https://example.com/avatar.jpg");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task Handle_Should_Log_Profile_Update_Start()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateProfileCommand(userId, "John", "Doe", null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Updating profile for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Profile_Update_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateProfileCommand(userId, "John", "Doe", null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Profile updated successfully for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
