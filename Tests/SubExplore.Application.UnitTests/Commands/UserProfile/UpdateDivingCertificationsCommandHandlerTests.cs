using Microsoft.Extensions.Logging;
using Moq;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpdateDivingCertificationsCommandHandler.
/// </summary>
public class UpdateDivingCertificationsCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateDivingCertificationsCommandHandler>> _loggerMock;
    private readonly UpdateDivingCertificationsCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDivingCertificationsCommandHandlerTests"/> class.
    /// </summary>
    public UpdateDivingCertificationsCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateDivingCertificationsCommandHandler>>();
        _handler = new UpdateDivingCertificationsCommandHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_UpdateDivingCertificationsResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", "12345", new DateTime(2020, 1, 1)),
            new("SSI", "Advanced Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(userId, certifications);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.CertificationCount);
    }

    [Fact]
    public async Task Handle_Should_Log_Certifications_Update_Start()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(userId, certifications);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Updating diving certifications for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Certifications_Update_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(userId, certifications);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Diving certifications updated successfully for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Zero_Count_For_Empty_List()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateDivingCertificationsCommand(userId, new List<CertificationDto>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(0, result.CertificationCount);
    }
}
