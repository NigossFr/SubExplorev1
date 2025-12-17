using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class EventParticipantTests
{
    private readonly Guid _eventId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private const string ValidComment = "Looking forward to this dive!";

    [Fact]
    public void EventParticipant_Create_Should_Create_Participant_With_Valid_Data()
    {
        // Act
        var participant = EventParticipant.Create(_eventId, _userId, ValidComment);

        // Assert
        participant.Should().NotBeNull();
        participant.Id.Should().NotBeEmpty();
        participant.EventId.Should().Be(_eventId);
        participant.UserId.Should().Be(_userId);
        participant.Comment.Should().Be(ValidComment);
        participant.RegisteredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EventParticipant_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var participant1 = EventParticipant.Create(_eventId, _userId, ValidComment);
        var participant2 = EventParticipant.Create(_eventId, _userId, ValidComment);

        // Assert
        participant1.Id.Should().NotBe(participant2.Id);
    }

    [Fact]
    public void EventParticipant_Create_Should_Accept_Null_Comment()
    {
        // Act
        var participant = EventParticipant.Create(_eventId, _userId, null);

        // Assert
        participant.Comment.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EventParticipant_Create_Should_Convert_Empty_Comment_To_Null(string emptyComment)
    {
        // Act
        var participant = EventParticipant.Create(_eventId, _userId, emptyComment);

        // Assert
        participant.Comment.Should().BeNull();
    }

    [Fact]
    public void EventParticipant_Create_Should_Trim_Comment()
    {
        // Act
        var participant = EventParticipant.Create(_eventId, _userId, "  Great event!  ");

        // Assert
        participant.Comment.Should().Be("Great event!");
    }

    [Fact]
    public void EventParticipant_Create_Should_Throw_When_Comment_Exceeds_MaxLength()
    {
        // Arrange
        var longComment = new string('a', 501);

        // Act
        Action act = () => EventParticipant.Create(_eventId, _userId, longComment);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Participant comment cannot exceed 500 characters*");
    }

    [Fact]
    public void EventParticipant_Create_Should_Accept_MaxLength_Comment()
    {
        // Arrange
        var maxComment = new string('a', 500);

        // Act
        var participant = EventParticipant.Create(_eventId, _userId, maxComment);

        // Assert
        participant.Comment.Should().HaveLength(500);
    }

    [Fact]
    public void EventParticipant_UpdateComment_Should_Update_Comment()
    {
        // Arrange
        var participant = EventParticipant.Create(_eventId, _userId, ValidComment);
        const string newComment = "Changed my mind, super excited!";

        // Act
        participant.UpdateComment(newComment);

        // Assert
        participant.Comment.Should().Be(newComment);
    }

    [Fact]
    public void EventParticipant_UpdateComment_Should_Accept_Null()
    {
        // Arrange
        var participant = EventParticipant.Create(_eventId, _userId, ValidComment);

        // Act
        participant.UpdateComment(null);

        // Assert
        participant.Comment.Should().BeNull();
    }

    [Fact]
    public void EventParticipant_UpdateComment_Should_Throw_When_Comment_Exceeds_MaxLength()
    {
        // Arrange
        var participant = EventParticipant.Create(_eventId, _userId, ValidComment);
        var longComment = new string('a', 501);

        // Act
        Action act = () => participant.UpdateComment(longComment);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Participant comment cannot exceed 500 characters*");
    }
}
