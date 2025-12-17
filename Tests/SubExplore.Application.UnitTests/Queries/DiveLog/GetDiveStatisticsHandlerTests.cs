using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetDiveStatisticsHandlerTests
{
    private readonly Mock<ILogger<GetDiveStatisticsHandler>> _loggerMock;
    private readonly GetDiveStatisticsHandler _handler;

    public GetDiveStatisticsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetDiveStatisticsHandler>>();
        _handler = new GetDiveStatisticsHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        var query = new GetDiveStatisticsQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Statistics);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsComprehensiveStatistics()
    {
        var userId = Guid.NewGuid();
        var query = new GetDiveStatisticsQuery(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.Statistics!;
        Assert.Equal(userId, stats.UserId);
        Assert.True(stats.TotalDives >= 0);
        Assert.True(stats.TotalDiveTimeMinutes >= 0);
        Assert.True(stats.TotalDiveTimeHours >= 0);
        Assert.True(stats.MaxDepthMeters >= 0);
        Assert.True(stats.AverageDepthMeters >= 0);
        Assert.True(stats.AverageDiveTimeMinutes >= 0);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesDiveDistributions()
    {
        var query = new GetDiveStatisticsQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.Statistics!;
        Assert.NotNull(stats.DivesByType);
        Assert.NotNull(stats.DivesByMonth);
        Assert.NotEmpty(stats.DivesByType);
        Assert.NotEmpty(stats.DivesByMonth);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesRecordDives()
    {
        var query = new GetDiveStatisticsQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.Statistics!;
        Assert.NotNull(stats.DeepestDiveId);
        Assert.NotNull(stats.DeepestDiveSpotName);
        Assert.NotNull(stats.LongestDiveId);
        Assert.True(stats.LongestDiveDurationMinutes > 0);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesFavoriteSpot()
    {
        var query = new GetDiveStatisticsQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.Statistics!;
        Assert.NotNull(stats.FavoriteSpotId);
        Assert.NotNull(stats.FavoriteSpotName);
        Assert.True(stats.FavoriteSpotDiveCount > 0);
    }

    [Fact]
    public async Task Handle_ValidQuery_LogsInformation()
    {
        var userId = Guid.NewGuid();
        var query = new GetDiveStatisticsQuery(userId);

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

    [Fact]
    public async Task Handle_QueryWithDateRange_AppliesDateFilter()
    {
        var query = new GetDiveStatisticsQuery(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow);

        var result = await _handler.Handle(query, CancellationToken.None);

        var stats = result.Statistics!;
        Assert.Equal(query.StartDate, stats.PeriodStartDate);
        Assert.Equal(query.EndDate, stats.PeriodEndDate);
    }
}
