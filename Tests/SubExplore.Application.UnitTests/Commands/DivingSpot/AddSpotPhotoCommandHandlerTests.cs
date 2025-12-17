using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.DivingSpot;

/// <summary>
/// Unit tests for AddSpotPhotoCommandHandler.
/// </summary>
public class AddSpotPhotoCommandHandlerTests
{
    private readonly Mock<ILogger<AddSpotPhotoCommandHandler>> _loggerMock;
    private readonly AddSpotPhotoCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpotPhotoCommandHandlerTests"/> class.
    /// </summary>
    public AddSpotPhotoCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AddSpotPhotoCommandHandler>>();
        _handler = new AddSpotPhotoCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_AddSpotPhotoResult()
    {
        // Arrange
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photo.jpg",
            "Beautiful coral reef",
            Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.PhotoId);
    }

    [Fact]
    public async Task Handle_Should_Log_Photo_Addition_Start()
    {
        // Arrange
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photo.jpg",
            null,
            Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Adding photo to diving spot")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Photo_Addition_Success()
    {
        // Arrange
        var command = new AddSpotPhotoCommand(
            Guid.NewGuid(),
            "https://example.com/photo.jpg",
            null,
            Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("added successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
