using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for ShareDiveLogCommandHandler.
/// </summary>
public class ShareDiveLogCommandHandlerTests
{
    private readonly Mock<ILogger<ShareDiveLogCommandHandler>> _mockLogger;
    private readonly ShareDiveLogCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShareDiveLogCommandHandlerTests"/> class.
    /// </summary>
    public ShareDiveLogCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<ShareDiveLogCommandHandler>>();
        _handler = new ShareDiveLogCommandHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var sharedWithUsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            sharedWithUsers,
            "Check out this amazing dive!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(command.DiveLogId, result.DiveLogId);
        Assert.Equal(2, result.SharedCount);
    }

    [Fact]
    public async Task Handle_SharesWithCorrectCount()
    {
        // Arrange
        var sharedWithUsers = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList();
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            sharedWithUsers,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(10, result.SharedCount);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var sharedWithUsers = new List<Guid> { Guid.NewGuid() };
        var command = new ShareDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            sharedWithUsers,
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }
}
