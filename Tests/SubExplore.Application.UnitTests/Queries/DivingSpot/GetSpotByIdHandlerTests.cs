using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetSpotByIdHandler.
/// </summary>
public class GetSpotByIdHandlerTests
{
    private readonly Mock<ILogger<GetSpotByIdHandler>> _mockLogger;
    private readonly GetSpotByIdHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpotByIdHandlerTests"/> class.
    /// </summary>
    public GetSpotByIdHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetSpotByIdHandler>>();
        _handler = new GetSpotByIdHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        var query = new GetSpotByIdQuery(spotId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spot);
        Assert.Equal(spotId, result.Spot.Id);
    }

    [Fact]
    public async Task Handle_WithIncludePhotos_ReturnsPhotos()
    {
        // Arrange
        var query = new GetSpotByIdQuery(Guid.NewGuid(), true, false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spot);
        Assert.NotEmpty(result.Spot.Photos);
        Assert.Empty(result.Spot.Ratings);
    }

    [Fact]
    public async Task Handle_WithIncludeRatings_ReturnsRatings()
    {
        // Arrange
        var query = new GetSpotByIdQuery(Guid.NewGuid(), false, true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spot);
        Assert.Empty(result.Spot.Photos);
        Assert.NotEmpty(result.Spot.Ratings);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var query = new GetSpotByIdQuery(Guid.NewGuid());

        // Act
        await _handler.Handle(query, CancellationToken.None);

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
