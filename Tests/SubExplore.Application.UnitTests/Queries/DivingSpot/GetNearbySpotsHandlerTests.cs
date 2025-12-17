using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for GetNearbySpotsHandler.
/// </summary>
public class GetNearbySpotsHandlerTests
{
    private readonly Mock<ILogger<GetNearbySpotsHandler>> _mockLogger;
    private readonly GetNearbySpotsHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNearbySpotsHandlerTests"/> class.
    /// </summary>
    public GetNearbySpotsHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetNearbySpotsHandler>>();
        _handler = new GetNearbySpotsHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetNearbySpotsQuery(43.2965, 5.3698, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
        Assert.Equal(result.Spots.Count, result.TotalCount);
    }

    [Fact]
    public async Task Handle_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var query = new GetNearbySpotsQuery(43.2965, 5.3698, 50, 1, 3, 10, 50, 20);

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
        var query = new GetNearbySpotsQuery(43.2965, 5.3698);

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
