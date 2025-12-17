using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetUserDiveLogsHandlerTests
{
    private readonly Mock<ILogger<GetUserDiveLogsHandler>> _loggerMock;
    private readonly GetUserDiveLogsHandler _handler;

    public GetUserDiveLogsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetUserDiveLogsHandler>>();
        _handler = new GetUserDiveLogsHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        var query = new GetUserDiveLogsQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.DiveLogs);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPaginatedResult()
    {
        var query = new GetUserDiveLogsQuery(
            Guid.NewGuid(),
            PageNumber: 1,
            PageSize: 20);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.True(result.TotalPages >= 1);
    }

    [Fact]
    public async Task Handle_ValidQuery_LogsInformation()
    {
        var userId = Guid.NewGuid();
        var query = new GetUserDiveLogsQuery(userId);

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
