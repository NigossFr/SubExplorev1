using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for DeleteSpotCommandHandler.
/// </summary>
public class DeleteSpotCommandHandlerTests
{
    private readonly Mock<ILogger<DeleteSpotCommandHandler>> _loggerMock;
    private readonly DeleteSpotCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSpotCommandHandlerTests"/> class.
    /// </summary>
    public DeleteSpotCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DeleteSpotCommandHandler>>();
        _handler = new DeleteSpotCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_DeleteSpotResult()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        var command = new DeleteSpotCommand(spotId, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(spotId, result.SpotId);
    }

    [Fact]
    public async Task Handle_Should_Log_Deletion_Start()
    {
        // Arrange
        var command = new DeleteSpotCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Deleting diving spot")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Deletion_Success()
    {
        // Arrange
        var command = new DeleteSpotCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
