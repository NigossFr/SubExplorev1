using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class MessageTests
{
    private readonly Guid _conversationId = Guid.NewGuid();
    private readonly Guid _senderId = Guid.NewGuid();
    private readonly Guid _receiverId = Guid.NewGuid();
    private const string ValidContent = "Hello, this is a test message.";

    #region Create Tests

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Act
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Assert
        message.Should().NotBeNull();
        message.Id.Should().NotBe(Guid.Empty);
        message.ConversationId.Should().Be(_conversationId);
        message.SenderId.Should().Be(_senderId);
        message.Content.Should().Be(ValidContent);
        message.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        message.ReadByUserIds.Should().HaveCount(1);
        message.ReadByUserIds.Should().Contain(_senderId);
    }

    [Fact]
    public void Create_SenderAutomaticallyReadsOwnMessage_ShouldSucceed()
    {
        // Act
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Assert
        message.IsReadBy(_senderId).Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyConversationId_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Message.Create(Guid.Empty, _senderId, ValidContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Conversation ID cannot be empty*");
    }

    [Fact]
    public void Create_WithEmptySenderId_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Message.Create(_conversationId, Guid.Empty, ValidContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Sender ID cannot be empty*");
    }

    [Fact]
    public void Create_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Message.Create(_conversationId, _senderId, "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot be empty*");
    }

    [Fact]
    public void Create_WithWhitespaceContent_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Message.Create(_conversationId, _senderId, "   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot be empty*");
    }

    [Fact]
    public void Create_WithContentTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longContent = new string('a', 2001);

        // Act
        var act = () => Message.Create(_conversationId, _senderId, longContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot exceed 2000 characters*");
    }

    [Fact]
    public void Create_WithContentExactly2000Characters_ShouldSucceed()
    {
        // Arrange
        var content = new string('a', 2000);

        // Act
        var message = Message.Create(_conversationId, _senderId, content);

        // Assert
        message.Content.Should().HaveLength(2000);
    }

    [Fact]
    public void Create_WithContentWithWhitespace_ShouldTrim()
    {
        // Arrange
        var content = "  Hello World  ";

        // Act
        var message = Message.Create(_conversationId, _senderId, content);

        // Assert
        message.Content.Should().Be("Hello World");
    }

    #endregion

    #region MarkAsReadBy Tests

    [Fact]
    public void MarkAsReadBy_WithValidUserId_ShouldSucceed()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act
        message.MarkAsReadBy(_receiverId);

        // Assert
        message.ReadByUserIds.Should().HaveCount(2);
        message.ReadByUserIds.Should().Contain(_receiverId);
        message.IsReadBy(_receiverId).Should().BeTrue();
    }

    [Fact]
    public void MarkAsReadBy_AlreadyRead_ShouldNotAddDuplicate()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        message.MarkAsReadBy(_receiverId);

        // Act
        message.MarkAsReadBy(_receiverId);

        // Assert
        message.ReadByUserIds.Should().HaveCount(2);
        message.ReadByUserIds.Where(id => id == _receiverId).Should().HaveCount(1);
    }

    [Fact]
    public void MarkAsReadBy_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act
        var act = () => message.MarkAsReadBy(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    [Fact]
    public void MarkAsReadBy_MultipleUsers_ShouldSucceed()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        var user2Id = Guid.NewGuid();
        var user3Id = Guid.NewGuid();

        // Act
        message.MarkAsReadBy(_receiverId);
        message.MarkAsReadBy(user2Id);
        message.MarkAsReadBy(user3Id);

        // Assert
        message.ReadByUserIds.Should().HaveCount(4); // sender + 3 others
        message.IsReadBy(_senderId).Should().BeTrue();
        message.IsReadBy(_receiverId).Should().BeTrue();
        message.IsReadBy(user2Id).Should().BeTrue();
        message.IsReadBy(user3Id).Should().BeTrue();
    }

    #endregion

    #region IsReadBy Tests

    [Fact]
    public void IsReadBy_WhenUserHasRead_ShouldReturnTrue()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        message.MarkAsReadBy(_receiverId);

        // Act & Assert
        message.IsReadBy(_receiverId).Should().BeTrue();
    }

    [Fact]
    public void IsReadBy_WhenUserHasNotRead_ShouldReturnFalse()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act & Assert
        message.IsReadBy(_receiverId).Should().BeFalse();
    }

    [Fact]
    public void IsReadBy_ForSender_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act & Assert
        message.IsReadBy(_senderId).Should().BeTrue();
    }

    #endregion

    #region UpdateContent Tests

    [Fact]
    public void UpdateContent_WithValidContent_ShouldSucceed()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        var newContent = "Updated message content.";

        // Act
        message.UpdateContent(newContent);

        // Assert
        message.Content.Should().Be(newContent);
    }

    [Fact]
    public void UpdateContent_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act
        var act = () => message.UpdateContent("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot be empty*");
    }

    [Fact]
    public void UpdateContent_WithWhitespaceContent_ShouldThrowArgumentException()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);

        // Act
        var act = () => message.UpdateContent("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot be empty*");
    }

    [Fact]
    public void UpdateContent_WithContentTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        var longContent = new string('a', 2001);

        // Act
        var act = () => message.UpdateContent(longContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message content cannot exceed 2000 characters*");
    }

    [Fact]
    public void UpdateContent_WithWhitespace_ShouldTrim()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        var newContent = "  Updated Content  ";

        // Act
        message.UpdateContent(newContent);

        // Assert
        message.Content.Should().Be("Updated Content");
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public void Message_ReadStatusProgression_ShouldWorkCorrectly()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        var user2Id = Guid.NewGuid();
        var user3Id = Guid.NewGuid();

        // Assert - Initial state (sender automatically reads)
        message.IsReadBy(_senderId).Should().BeTrue();
        message.IsReadBy(_receiverId).Should().BeFalse();
        message.ReadByUserIds.Should().HaveCount(1);

        // Act - First user reads
        message.MarkAsReadBy(_receiverId);
        message.IsReadBy(_receiverId).Should().BeTrue();
        message.ReadByUserIds.Should().HaveCount(2);

        // Act - Second user reads
        message.MarkAsReadBy(user2Id);
        message.IsReadBy(user2Id).Should().BeTrue();
        message.ReadByUserIds.Should().HaveCount(3);

        // Act - Third user reads
        message.MarkAsReadBy(user3Id);
        message.IsReadBy(user3Id).Should().BeTrue();
        message.ReadByUserIds.Should().HaveCount(4);
    }

    [Fact]
    public void Message_ContentUpdateAfterRead_ShouldMaintainReadStatus()
    {
        // Arrange
        var message = Message.Create(_conversationId, _senderId, ValidContent);
        message.MarkAsReadBy(_receiverId);

        // Act
        message.UpdateContent("Updated content");

        // Assert - Read status should be maintained
        message.IsReadBy(_senderId).Should().BeTrue();
        message.IsReadBy(_receiverId).Should().BeTrue();
        message.Content.Should().Be("Updated content");
    }

    #endregion
}
