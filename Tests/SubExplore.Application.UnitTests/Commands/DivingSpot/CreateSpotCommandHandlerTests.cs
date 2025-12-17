using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for CreateSpotCommandHandler.
/// </summary>
public class CreateSpotCommandHandlerTests
{
    private readonly Mock<ILogger<CreateSpotCommandHandler>> _loggerMock;
    private readonly CreateSpotCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSpotCommandHandlerTests"/> class.
    /// </summary>
    public CreateSpotCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateSpotCommandHandler>>();
        _handler = new CreateSpotCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_CreateSpotResult()
    {
        // Arrange
        var command = new CreateSpotCommand(
            "Great Diving Spot",
            "This is a wonderful diving spot with clear waters.",
            45.5,
            10.5,
            30,
            2,
            Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.SpotId);
    }

    [Fact]
    public async Task Handle_Should_Log_Spot_Creation_Start()
    {
        // Arrange
        var command = new CreateSpotCommand(
            "Great Diving Spot",
            "This is a wonderful diving spot.",
            45.5,
            10.5,
            30,
            2,
            Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new diving spot")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Spot_Creation_Success()
    {
        // Arrange
        var command = new CreateSpotCommand(
            "Great Diving Spot",
            "This is a wonderful diving spot.",
            45.5,
            10.5,
            30,
            2,
            Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
