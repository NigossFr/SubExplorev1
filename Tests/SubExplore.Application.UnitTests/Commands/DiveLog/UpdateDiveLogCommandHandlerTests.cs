using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for UpdateDiveLogCommandHandler.
/// </summary>
public class UpdateDiveLogCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateDiveLogCommandHandler>> _mockLogger;
    private readonly UpdateDiveLogCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDiveLogCommandHandlerTests"/> class.
    /// </summary>
    public UpdateDiveLogCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UpdateDiveLogCommandHandler>>();
        _handler = new UpdateDiveLogCommandHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            25,
            20,
            15,
            "Wetsuit 5mm, BCD, Regulator",
            "Great dive with excellent visibility!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(command.DiveLogId, result.DiveLogId);
    }

    [Fact]
    public async Task Handle_CommandWithMinimalData_ReturnsSuccessResult()
    {
        // Arrange
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(command.DiveLogId, result.DiveLogId);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var command = new UpdateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            30,
            null,
            null,
            null,
            null,
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
