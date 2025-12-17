using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UploadAvatarCommandHandler.
/// </summary>
public class UploadAvatarCommandHandlerTests
{
    private readonly Mock<ILogger<UploadAvatarCommandHandler>> _loggerMock;
    private readonly UploadAvatarCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadAvatarCommandHandlerTests"/> class.
    /// </summary>
    public UploadAvatarCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UploadAvatarCommandHandler>>();
        _handler = new UploadAvatarCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_UploadAvatarResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UploadAvatarCommand(
            userId,
            "avatar.jpg",
            "image/jpeg",
            new byte[1024]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.AvatarUrl);
        Assert.Contains(userId.ToString(), result.AvatarUrl);
    }

    [Fact]
    public async Task Handle_Should_Log_Upload_Start()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UploadAvatarCommand(
            userId,
            "avatar.jpg",
            "image/jpeg",
            new byte[1024]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Uploading avatar for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Upload_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UploadAvatarCommand(
            userId,
            "avatar.jpg",
            "image/jpeg",
            new byte[1024]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Avatar uploaded successfully for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
