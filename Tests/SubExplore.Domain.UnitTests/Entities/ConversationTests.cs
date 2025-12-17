using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class ConversationTests
{
    private readonly Guid _user1Id = Guid.NewGuid();
    private readonly Guid _user2Id = Guid.NewGuid();
    private readonly Guid _user3Id = Guid.NewGuid();
    private readonly Guid _user4Id = Guid.NewGuid();

    #region CreatePrivate Tests

    [Fact]
    public void CreatePrivate_WithValidUsers_ShouldSucceed()
    {
        // Act
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Assert
        conversation.Should().NotBeNull();
        conversation.Id.Should().NotBe(Guid.Empty);
        conversation.IsGroupConversation.Should().BeFalse();
        conversation.ParticipantIds.Should().HaveCount(2);
        conversation.ParticipantIds.Should().Contain(_user1Id);
        conversation.ParticipantIds.Should().Contain(_user2Id);
        conversation.Title.Should().BeNull();
        conversation.LastMessageAt.Should().BeNull();
        conversation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        conversation.Messages.Should().BeEmpty();
    }

    [Fact]
    public void CreatePrivate_WithSameUser_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Conversation.CreatePrivate(_user1Id, _user1Id);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Cannot create conversation with the same user*");
    }

    [Fact]
    public void CreatePrivate_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Conversation.CreatePrivate(Guid.Empty, _user2Id);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Participant IDs cannot be empty*");
    }

    #endregion

    #region CreateGroup Tests

    [Fact]
    public void CreateGroup_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var title = "Team Discussion";
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var conversation = Conversation.CreateGroup(title, participants);

        // Assert
        conversation.Should().NotBeNull();
        conversation.Id.Should().NotBe(Guid.Empty);
        conversation.IsGroupConversation.Should().BeTrue();
        conversation.ParticipantIds.Should().HaveCount(3);
        conversation.ParticipantIds.Should().Contain(participants);
        conversation.Title.Should().Be(title);
        conversation.LastMessageAt.Should().BeNull();
        conversation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateGroup_WithTwoParticipants_ShouldSucceed()
    {
        // Arrange
        var title = "Duo Chat";
        var participants = new[] { _user1Id, _user2Id };

        // Act
        var conversation = Conversation.CreateGroup(title, participants);

        // Assert
        conversation.ParticipantIds.Should().HaveCount(2);
        conversation.IsGroupConversation.Should().BeTrue();
    }

    [Fact]
    public void CreateGroup_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var act = () => Conversation.CreateGroup("", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Group conversation must have a title*");
    }

    [Fact]
    public void CreateGroup_WithWhitespaceTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var act = () => Conversation.CreateGroup("   ", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Group conversation must have a title*");
    }

    [Fact]
    public void CreateGroup_WithTitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longTitle = new string('a', 101);
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var act = () => Conversation.CreateGroup(longTitle, participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot exceed 100 characters*");
    }

    [Fact]
    public void CreateGroup_WithTitleExactly100Characters_ShouldSucceed()
    {
        // Arrange
        var title = new string('a', 100);
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var conversation = Conversation.CreateGroup(title, participants);

        // Assert
        conversation.Title.Should().HaveLength(100);
    }

    [Fact]
    public void CreateGroup_WithTitleWithWhitespace_ShouldTrim()
    {
        // Arrange
        var title = "  Team Discussion  ";
        var participants = new[] { _user1Id, _user2Id, _user3Id };

        // Act
        var conversation = Conversation.CreateGroup(title, participants);

        // Assert
        conversation.Title.Should().Be("Team Discussion");
    }

    [Fact]
    public void CreateGroup_WithOneParticipant_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = new[] { _user1Id };

        // Act
        var act = () => Conversation.CreateGroup("Solo", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Group conversation must have at least 2 participants*");
    }

    [Fact]
    public void CreateGroup_WithNoParticipants_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = Array.Empty<Guid>();

        // Act
        var act = () => Conversation.CreateGroup("Empty", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Conversation must have at least one participant*");
    }

    [Fact]
    public void CreateGroup_WithDuplicateParticipants_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = new[] { _user1Id, _user2Id, _user1Id };

        // Act
        var act = () => Conversation.CreateGroup("Duplicate", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Participant IDs must be unique*");
    }

    [Fact]
    public void CreateGroup_WithEmptyParticipantId_ShouldThrowArgumentException()
    {
        // Arrange
        var participants = new[] { _user1Id, Guid.Empty, _user2Id };

        // Act
        var act = () => Conversation.CreateGroup("Invalid", participants);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Participant IDs cannot be empty*");
    }

    #endregion

    #region AddParticipant Tests

    [Fact]
    public void AddParticipant_ToGroupConversation_ShouldSucceed()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id });

        // Act
        conversation.AddParticipant(_user3Id);

        // Assert
        conversation.ParticipantIds.Should().HaveCount(3);
        conversation.ParticipantIds.Should().Contain(_user3Id);
    }

    [Fact]
    public void AddParticipant_ToPrivateConversation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Act
        var act = () => conversation.AddParticipant(_user3Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add participants to a private conversation*");
    }

    [Fact]
    public void AddParticipant_AlreadyParticipant_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id });

        // Act
        var act = () => conversation.AddParticipant(_user1Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is already a participant in this conversation*");
    }

    [Fact]
    public void AddParticipant_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id });

        // Act
        var act = () => conversation.AddParticipant(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    #endregion

    #region RemoveParticipant Tests

    [Fact]
    public void RemoveParticipant_FromGroupConversation_ShouldSucceed()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id, _user3Id });

        // Act
        conversation.RemoveParticipant(_user3Id);

        // Assert
        conversation.ParticipantIds.Should().HaveCount(2);
        conversation.ParticipantIds.Should().NotContain(_user3Id);
    }

    [Fact]
    public void RemoveParticipant_FromPrivateConversation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Act
        var act = () => conversation.RemoveParticipant(_user1Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot remove participants from a private conversation*");
    }

    [Fact]
    public void RemoveParticipant_NotParticipant_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id, _user3Id });

        // Act
        var act = () => conversation.RemoveParticipant(_user4Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is not a participant in this conversation*");
    }

    [Fact]
    public void RemoveParticipant_LeavingOnlyOne_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id });

        // Act
        var act = () => conversation.RemoveParticipant(_user1Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Group conversation must have at least 2 participants*");
    }

    [Fact]
    public void RemoveParticipant_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id, _user3Id });

        // Act
        var act = () => conversation.RemoveParticipant(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    #endregion

    #region UpdateTitle Tests

    [Fact]
    public void UpdateTitle_OnGroupConversation_ShouldSucceed()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Old Title", new[] { _user1Id, _user2Id });

        // Act
        conversation.UpdateTitle("New Title");

        // Assert
        conversation.Title.Should().Be("New Title");
    }

    [Fact]
    public void UpdateTitle_OnPrivateConversation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Act
        var act = () => conversation.UpdateTitle("New Title");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot update title of a private conversation*");
    }

    [Fact]
    public void UpdateTitle_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Old Title", new[] { _user1Id, _user2Id });

        // Act
        var act = () => conversation.UpdateTitle("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void UpdateTitle_WithWhitespaceTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Old Title", new[] { _user1Id, _user2Id });

        // Act
        var act = () => conversation.UpdateTitle("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void UpdateTitle_WithTitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Old Title", new[] { _user1Id, _user2Id });
        var longTitle = new string('a', 101);

        // Act
        var act = () => conversation.UpdateTitle(longTitle);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot exceed 100 characters*");
    }

    [Fact]
    public void UpdateTitle_WithWhitespace_ShouldTrim()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Old Title", new[] { _user1Id, _user2Id });

        // Act
        conversation.UpdateTitle("  New Title  ");

        // Assert
        conversation.Title.Should().Be("New Title");
    }

    #endregion

    #region IsParticipant Tests

    [Fact]
    public void IsParticipant_WhenUserIsParticipant_ShouldReturnTrue()
    {
        // Arrange
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Act & Assert
        conversation.IsParticipant(_user1Id).Should().BeTrue();
        conversation.IsParticipant(_user2Id).Should().BeTrue();
    }

    [Fact]
    public void IsParticipant_WhenUserIsNotParticipant_ShouldReturnFalse()
    {
        // Arrange
        var conversation = Conversation.CreatePrivate(_user1Id, _user2Id);

        // Act & Assert
        conversation.IsParticipant(_user3Id).Should().BeFalse();
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public void Conversation_AddAndRemoveParticipants_ShouldWorkCorrectly()
    {
        // Arrange
        var conversation = Conversation.CreateGroup("Team", new[] { _user1Id, _user2Id });

        // Act - Add participants
        conversation.AddParticipant(_user3Id);
        conversation.AddParticipant(_user4Id);

        // Assert - Added
        conversation.ParticipantIds.Should().HaveCount(4);
        conversation.IsParticipant(_user3Id).Should().BeTrue();
        conversation.IsParticipant(_user4Id).Should().BeTrue();

        // Act - Remove participants
        conversation.RemoveParticipant(_user3Id);
        conversation.RemoveParticipant(_user4Id);

        // Assert - Removed
        conversation.ParticipantIds.Should().HaveCount(2);
        conversation.IsParticipant(_user3Id).Should().BeFalse();
        conversation.IsParticipant(_user4Id).Should().BeFalse();
    }

    #endregion
}
