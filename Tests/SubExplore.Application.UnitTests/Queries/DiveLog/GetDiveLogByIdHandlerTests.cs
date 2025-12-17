using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Queries.DiveLog;

namespace SubExplore.Application.UnitTests.Queries.DiveLog;

public class GetDiveLogByIdHandlerTests
{
    private readonly Mock<ILogger<GetDiveLogByIdHandler>> _loggerMock;
    private readonly GetDiveLogByIdHandler _handler;

    public GetDiveLogByIdHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetDiveLogByIdHandler>>();
        _handler = new GetDiveLogByIdHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccessResult()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.DiveLog);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsDetailedDiveLog()
    {
        var diveLogId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetDiveLogByIdQuery(diveLogId, userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(diveLogId, result.DiveLog!.Id);
        Assert.Equal(userId, result.DiveLog.UserId);
        Assert.NotEmpty(result.DiveLog.UserName);
        Assert.NotEmpty(result.DiveLog.DivingSpotName);
        Assert.NotEmpty(result.DiveLog.DiveTypeName);
    }

    [Fact]
    public async Task Handle_ValidQuery_IncludesDivingSpotCoordinates()
    {
        var query = new GetDiveLogByIdQuery(
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.DiveLog);
        Assert.InRange(result.DiveLog.DivingSpotLatitude, -90, 90);
        Assert.InRange(result.DiveLog.DivingSpotLongitude, -180, 180);
    }

    [Fact]
    public async Task Handle_ValidQuery_LogsInformation()
    {
        var diveLogId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetDiveLogByIdQuery(diveLogId, userId);

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
