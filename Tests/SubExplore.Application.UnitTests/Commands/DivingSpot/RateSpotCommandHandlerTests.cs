using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for RateSpotCommandHandler.
/// </summary>
public class RateSpotCommandHandlerTests
{
    private readonly Mock<ILogger<RateSpotCommandHandler>> _loggerMock;
    private readonly RateSpotCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateSpotCommandHandlerTests"/> class.
    /// </summary>
    public RateSpotCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RateSpotCommandHandler>>();
        _handler = new RateSpotCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_RateSpotResult()
    {
        // Arrange
        var command = new RateSpotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            "Excellent spot!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.RatingId);
        Assert.InRange(result.AverageRating, 1, 5);
    }

    [Fact]
    public async Task Handle_Should_Log_Rating_Start()
    {
        // Arrange
        var command = new RateSpotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("rating diving spot")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Rating_Success()
    {
        // Arrange
        var command = new RateSpotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null);

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
