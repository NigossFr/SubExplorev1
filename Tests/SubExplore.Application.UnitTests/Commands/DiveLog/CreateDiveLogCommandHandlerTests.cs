using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DiveLog;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DiveLog;

/// <summary>
/// Unit tests for CreateDiveLogCommandHandler.
/// </summary>
public class CreateDiveLogCommandHandlerTests
{
    private readonly Mock<ILogger<CreateDiveLogCommandHandler>> _mockLogger;
    private readonly CreateDiveLogCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDiveLogCommandHandlerTests"/> class.
    /// </summary>
    public CreateDiveLogCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<CreateDiveLogCommandHandler>>();
        _handler = new CreateDiveLogCommandHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            25,
            20,
            15,
            0,
            Guid.NewGuid(),
            "Wetsuit 5mm, BCD, Regulator",
            "Great dive with excellent visibility!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.DiveLogId);
        Assert.Equal(60, result.DurationMinutes);
    }

    [Fact]
    public async Task Handle_CalculatesDurationCorrectly()
    {
        // Arrange
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(14).Add(TimeSpan.FromMinutes(30)),
            TimeSpan.FromHours(15).Add(TimeSpan.FromMinutes(15)),
            30,
            null,
            null,
            null,
            0,
            null,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(45, result.DurationMinutes);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var command = new CreateDiveLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            30,
            null,
            null,
            null,
            0,
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
