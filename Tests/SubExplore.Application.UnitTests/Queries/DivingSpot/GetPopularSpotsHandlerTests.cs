using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetPopularSpotsHandler.
/// </summary>
public class GetPopularSpotsHandlerTests
{
    private readonly Mock<ILogger<GetPopularSpotsHandler>> _mockLogger;
    private readonly GetPopularSpotsHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPopularSpotsHandlerTests"/> class.
    /// </summary>
    public GetPopularSpotsHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetPopularSpotsHandler>>();
        _handler = new GetPopularSpotsHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetPopularSpotsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
        Assert.Equal(result.Spots.Count, result.TotalCount);
    }

    [Fact]
    public async Task Handle_WithCustomLimit_ReturnsResults()
    {
        // Arrange
        var query = new GetPopularSpotsQuery(20);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
    }

    [Fact]
    public async Task Handle_WithMinimumRatings_ReturnsResults()
    {
        // Arrange
        var query = new GetPopularSpotsQuery(10, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
    }

    [Fact]
    public async Task Handle_WithDaysBack_ReturnsResults()
    {
        // Arrange
        var query = new GetPopularSpotsQuery(10, 5, 30);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var query = new GetPopularSpotsQuery();

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
