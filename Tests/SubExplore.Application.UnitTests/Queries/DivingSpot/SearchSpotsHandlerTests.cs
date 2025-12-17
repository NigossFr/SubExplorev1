using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DivingSpot;
using Xunit;

namespace SubExplore.Application.UnitTests.Queries.DivingSpot;

/// <summary>
/// Unit tests for SearchSpotsHandler.
/// </summary>
public class SearchSpotsHandlerTests
{
    private readonly Mock<ILogger<SearchSpotsHandler>> _mockLogger;
    private readonly SearchSpotsHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSpotsHandlerTests"/> class.
    /// </summary>
    public SearchSpotsHandlerTests()
    {
        _mockLogger = new Mock<ILogger<SearchSpotsHandler>>();
        _handler = new SearchSpotsHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new SearchSpotsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
        Assert.Equal(result.PageNumber, query.PageNumber);
        Assert.Equal(result.PageSize, query.PageSize);
    }

    [Fact]
    public async Task Handle_WithSearchText_ReturnsResults()
    {
        // Arrange
        var query = new SearchSpotsQuery("diving");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
    }

    [Fact]
    public async Task Handle_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var query = new SearchSpotsQuery(
            "test",
            1,
            3,
            10,
            50,
            4.0,
            15,
            25,
            10,
            "Rating",
            true,
            1,
            20);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Spots);
        Assert.True(result.TotalPages >= 1);
    }

    [Fact]
    public async Task Handle_LogsInformation()
    {
        // Arrange
        var query = new SearchSpotsQuery();

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
