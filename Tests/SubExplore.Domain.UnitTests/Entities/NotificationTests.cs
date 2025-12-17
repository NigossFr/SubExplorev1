using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.Enums;

namespace SubExplore.Domain.UnitTests.Entities;

public class NotificationTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private const string ValidTitle = "Test Notification";
    private const string ValidMessage = "This is a test notification message.";

    #region Create Tests

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage,
            NotificationPriority.Normal);

        // Assert
        notification.Should().NotBeNull();
        notification.Id.Should().NotBe(Guid.Empty);
        notification.UserId.Should().Be(_validUserId);
        notification.Type.Should().Be(NotificationType.Event);
        notification.Title.Should().Be(ValidTitle);
        notification.Message.Should().Be(ValidMessage);
        notification.Priority.Should().Be(NotificationPriority.Normal);
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
        notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        notification.ReferenceId.Should().BeNull();
    }

    [Fact]
    public void Create_WithReferenceId_ShouldSucceed()
    {
        // Arrange
        var referenceId = Guid.NewGuid();

        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Achievement,
            ValidTitle,
            ValidMessage,
            NotificationPriority.High,
            referenceId);

        // Assert
        notification.ReferenceId.Should().Be(referenceId);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Notification.Create(
            Guid.Empty,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            "",
            ValidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            "   ",
            ValidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void Create_WithTitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longTitle = new string('a', 201);

        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            longTitle,
            ValidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot exceed 200 characters*");
    }

    [Fact]
    public void Create_WithTitleExactly200Characters_ShouldSucceed()
    {
        // Arrange
        var title = new string('a', 200);

        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            title,
            ValidMessage);

        // Assert
        notification.Title.Should().HaveLength(200);
    }

    [Fact]
    public void Create_WithEmptyMessage_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message cannot be empty*");
    }

    [Fact]
    public void Create_WithWhitespaceMessage_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            "   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message cannot be empty*");
    }

    [Fact]
    public void Create_WithMessageTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longMessage = new string('a', 1001);

        // Act
        var act = () => Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            longMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message cannot exceed 1000 characters*");
    }

    [Fact]
    public void Create_WithMessageExactly1000Characters_ShouldSucceed()
    {
        // Arrange
        var message = new string('a', 1000);

        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            message);

        // Assert
        notification.Message.Should().HaveLength(1000);
    }

    [Fact]
    public void Create_WithTitleAndMessageWithWhitespace_ShouldTrim()
    {
        // Arrange
        var titleWithWhitespace = "  Test Title  ";
        var messageWithWhitespace = "  Test Message  ";

        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            titleWithWhitespace,
            messageWithWhitespace);

        // Assert
        notification.Title.Should().Be("Test Title");
        notification.Message.Should().Be("Test Message");
    }

    [Theory]
    [InlineData(NotificationType.Event)]
    [InlineData(NotificationType.Message)]
    [InlineData(NotificationType.Achievement)]
    [InlineData(NotificationType.System)]
    public void Create_WithAllNotificationTypes_ShouldSucceed(NotificationType type)
    {
        // Act
        var notification = Notification.Create(
            _validUserId,
            type,
            ValidTitle,
            ValidMessage);

        // Assert
        notification.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(NotificationPriority.Low)]
    [InlineData(NotificationPriority.Normal)]
    [InlineData(NotificationPriority.High)]
    [InlineData(NotificationPriority.Urgent)]
    public void Create_WithAllPriorityLevels_ShouldSucceed(NotificationPriority priority)
    {
        // Act
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage,
            priority);

        // Assert
        notification.Priority.Should().Be(priority);
    }

    #endregion

    #region MarkAsRead Tests

    [Fact]
    public void MarkAsRead_OnUnreadNotification_ShouldSucceed()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Act
        notification.MarkAsRead();

        // Assert
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        notification.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsRead_OnAlreadyReadNotification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);
        notification.MarkAsRead();

        // Act
        var act = () => notification.MarkAsRead();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already marked as read*");
    }

    #endregion

    #region MarkAsUnread Tests

    [Fact]
    public void MarkAsUnread_OnReadNotification_ShouldSucceed()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);
        notification.MarkAsRead();

        // Act
        notification.MarkAsUnread();

        // Assert
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsUnread_OnAlreadyUnreadNotification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Act
        var act = () => notification.MarkAsUnread();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already marked as unread*");
    }

    #endregion

    #region UpdatePriority Tests

    [Fact]
    public void UpdatePriority_OnUnreadNotification_ShouldSucceed()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage,
            NotificationPriority.Low);

        // Act
        notification.UpdatePriority(NotificationPriority.Urgent);

        // Assert
        notification.Priority.Should().Be(NotificationPriority.Urgent);
    }

    [Fact]
    public void UpdatePriority_OnReadNotification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);
        notification.MarkAsRead();

        // Act
        var act = () => notification.UpdatePriority(NotificationPriority.High);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot update priority of a read notification*");
    }

    #endregion

    #region UpdateContent Tests

    [Fact]
    public void UpdateContent_OnUnreadNotificationWithValidParameters_ShouldSucceed()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        var newTitle = "Updated Title";
        var newMessage = "Updated message content.";

        // Act
        notification.UpdateContent(newTitle, newMessage);

        // Assert
        notification.Title.Should().Be(newTitle);
        notification.Message.Should().Be(newMessage);
    }

    [Fact]
    public void UpdateContent_OnReadNotification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);
        notification.MarkAsRead();

        // Act
        var act = () => notification.UpdateContent("New Title", "New Message");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot update content of a read notification*");
    }

    [Fact]
    public void UpdateContent_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Act
        var act = () => notification.UpdateContent("", "New Message");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void UpdateContent_WithTitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        var longTitle = new string('a', 201);

        // Act
        var act = () => notification.UpdateContent(longTitle, "New Message");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot exceed 200 characters*");
    }

    [Fact]
    public void UpdateContent_WithEmptyMessage_ShouldThrowArgumentException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Act
        var act = () => notification.UpdateContent("New Title", "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message cannot be empty*");
    }

    [Fact]
    public void UpdateContent_WithMessageTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        var longMessage = new string('a', 1001);

        // Act
        var act = () => notification.UpdateContent("New Title", longMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Message cannot exceed 1000 characters*");
    }

    [Fact]
    public void UpdateContent_WithWhitespaceInTitleAndMessage_ShouldTrim()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        var titleWithWhitespace = "  New Title  ";
        var messageWithWhitespace = "  New Message  ";

        // Act
        notification.UpdateContent(titleWithWhitespace, messageWithWhitespace);

        // Assert
        notification.Title.Should().Be("New Title");
        notification.Message.Should().Be("New Message");
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public void Notification_ReadUnreadCycle_ShouldWorkCorrectly()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.Event,
            ValidTitle,
            ValidMessage);

        // Act & Assert - Initial state
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();

        // Act - Mark as read
        notification.MarkAsRead();
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        var firstReadAt = notification.ReadAt;

        // Act - Mark as unread
        notification.MarkAsUnread();
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();

        // Act - Mark as read again
        notification.MarkAsRead();
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        notification.ReadAt.Should().BeAfter(firstReadAt!.Value);
    }

    [Fact]
    public void Notification_PriorityEscalation_ShouldWorkCorrectly()
    {
        // Arrange
        var notification = Notification.Create(
            _validUserId,
            NotificationType.System,
            ValidTitle,
            ValidMessage,
            NotificationPriority.Low);

        // Act - Escalate priority
        notification.UpdatePriority(NotificationPriority.Normal);
        notification.Priority.Should().Be(NotificationPriority.Normal);

        notification.UpdatePriority(NotificationPriority.High);
        notification.Priority.Should().Be(NotificationPriority.High);

        notification.UpdatePriority(NotificationPriority.Urgent);
        notification.Priority.Should().Be(NotificationPriority.Urgent);
    }

    #endregion
}
