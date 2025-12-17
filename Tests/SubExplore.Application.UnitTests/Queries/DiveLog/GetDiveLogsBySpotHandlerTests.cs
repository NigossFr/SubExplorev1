using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetDiveLogsBySpotHandlerTests
{
    private readonly Mock<ILogger<GetDiveLogsBySpotHandler>> _loggerMock;
    private readonly GetDiveLogsBySpotHandler _handler;

    public GetDiveLogsBySpotHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetDiveLogsBySpotHandler>>();
        _handler = new GetDiveLogsBySpotHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        var query = new GetDiveLogsBySpotQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.DiveLogs);
        Assert.NotNull(result.SpotStatistics);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPaginatedResult()
    {
        var query = new GetDiveLogsBySpotQuery(
            Guid.NewGuid(),
            PageNumber: 1,
            PageSize: 20);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.True(result.TotalPages >= 1);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesSpotInformation()
    {
        var spotId = Guid.NewGuid();
        var query = new GetDiveLogsBySpotQuery(spotId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(spotId, result.DivingSpotId);
        Assert.NotEmpty(result.DivingSpotName);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesSpotStatistics()
    {
        var query = new GetDiveLogsBySpotQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.SpotStatistics;
        Assert.True(stats.TotalDives >= 0);
        Assert.True(stats.UniqueDivers >= 0);
        Assert.True(stats.AverageDepthMeters >= 0);
        Assert.True(stats.AverageDurationMinutes >= 0);
    }

    [Fact]
    public async Task Handle_ValidQuery_LogsInformation()
    {
        var spotId = Guid.NewGuid();
        var query = new GetDiveLogsBySpotQuery(spotId);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
