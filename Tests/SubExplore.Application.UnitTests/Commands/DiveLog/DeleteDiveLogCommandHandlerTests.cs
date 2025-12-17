using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for DeleteDiveLogCommandHandler.
/// </summary>
public class DeleteDiveLogCommandHandlerTests
{
    private readonly Mock<ILogger<DeleteDiveLogCommandHandler>> _mockLogger;
    private readonly DeleteDiveLogCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDiveLogCommandHandlerTests"/> class.
    /// </summary>
    public DeleteDiveLogCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DeleteDiveLogCommandHandler>>();
        _handler = new DeleteDiveLogCommandHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new DeleteDiveLogCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(command.DiveLogId, result.DiveLogId);
    }

    [Fact]
    public async Task Handle_DifferentDiveLogIds_ReturnsDifferentResults()
    {
        // Arrange
        var diveLogId1 = Guid.NewGuid();
        var diveLogId2 = Guid.NewGuid();
        var command1 = new DeleteDiveLogCommand(diveLogId1, Guid.NewGuid());
        var command2 = new DeleteDiveLogCommand(diveLogId2, Guid.NewGuid());

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.DiveLogId, result2.DiveLogId);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var command = new DeleteDiveLogCommand(Guid.NewGuid(), Guid.NewGuid());

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
