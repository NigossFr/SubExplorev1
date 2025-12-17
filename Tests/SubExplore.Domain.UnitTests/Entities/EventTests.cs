using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.Enums;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.Entities;

public class EventTests
{
    private readonly Guid _organizerId = Guid.NewGuid();
    private readonly DateTime _futureDate = DateTime.UtcNow.AddDays(7);
    private const string ValidTitle = "Night Dive Adventure";
    private const string ValidDescription = "Join us for an exciting night dive to observe nocturnal marine life.";
    private const string ValidLocationName = "Crystal Bay Marina";

    [Fact]
    public void Event_Create_Should_Create_Event_With_Valid_Data()
    {
        // Act
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        evt.Should().NotBeNull();
        evt.Id.Should().NotBeEmpty();
        evt.Title.Should().Be(ValidTitle);
        evt.Description.Should().Be(ValidDescription);
        evt.EventDate.Should().Be(_futureDate);
        evt.LocationName.Should().Be(ValidLocationName);
        evt.OrganizerId.Should().Be(_organizerId);
        evt.MaxParticipants.Should().BeNull();
        evt.Status.Should().Be(EventStatus.Scheduled);
        evt.Location.Should().BeNull();
        evt.DivingSpotId.Should().BeNull();
        evt.Participants.Should().BeEmpty();
        evt.ParticipantCount.Should().Be(0);
        evt.IsFull.Should().BeFalse();
        evt.AvailableSpots.Should().BeNull();
        evt.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        evt.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Event_Create_Should_Create_Event_With_MaxParticipants()
    {
        // Act
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 10);

        // Assert
        evt.MaxParticipants.Should().Be(10);
        evt.AvailableSpots.Should().Be(10);
    }

    [Fact]
    public void Event_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var evt1 = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var evt2 = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        evt1.Id.Should().NotBe(evt2.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Event_Create_Should_Throw_When_Title_Is_Invalid(string invalidTitle)
    {
        // Act
        Action act = () => Event.Create(invalidTitle!, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event title cannot be null or empty*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_Title_Is_TooShort()
    {
        // Act
        Action act = () => Event.Create("ab", ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event title must be at least 3 characters*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_Title_Is_TooLong()
    {
        // Arrange
        var longTitle = new string('a', 101);

        // Act
        Action act = () => Event.Create(longTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event title cannot exceed 100 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Event_Create_Should_Throw_When_Description_Is_Invalid(string invalidDescription)
    {
        // Act
        Action act = () => Event.Create(ValidTitle, invalidDescription!, _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event description cannot be null or empty*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_Description_Is_TooShort()
    {
        // Act
        Action act = () => Event.Create(ValidTitle, "Short", _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event description must be at least 10 characters*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_Description_Is_TooLong()
    {
        // Arrange
        var longDescription = new string('a', 2001);

        // Act
        Action act = () => Event.Create(ValidTitle, longDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Event description cannot exceed 2000 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Event_Create_Should_Throw_When_LocationName_Is_Invalid(string invalidLocationName)
    {
        // Act
        Action act = () => Event.Create(ValidTitle, ValidDescription, _futureDate, invalidLocationName!, _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Location name cannot be null or empty*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_LocationName_Is_TooShort()
    {
        // Act
        Action act = () => Event.Create(ValidTitle, ValidDescription, _futureDate, "ab", _organizerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Location name must be at least 3 characters*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Event_Create_Should_Throw_When_MaxParticipants_Is_Invalid(int invalidMax)
    {
        // Act
        Action act = () => Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, invalidMax);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Maximum participants must be at least 1*");
    }

    [Fact]
    public void Event_Create_Should_Throw_When_MaxParticipants_Exceeds_Limit()
    {
        // Act
        Action act = () => Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 1001);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Maximum participants cannot exceed 1000*");
    }

    [Fact]
    public void Event_UpdateDetails_Should_Update_All_Fields()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var originalUpdatedAt = evt.UpdatedAt;
        Thread.Sleep(10);

        var newDate = DateTime.UtcNow.AddDays(14);
        const string newTitle = "Updated Title";
        const string newDescription = "Updated description with more details.";
        const string newLocation = "New Marina";

        // Act
        evt.UpdateDetails(newTitle, newDescription, newDate, newLocation);

        // Assert
        evt.Title.Should().Be(newTitle);
        evt.Description.Should().Be(newDescription);
        evt.EventDate.Should().Be(newDate);
        evt.LocationName.Should().Be(newLocation);
        evt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Event_UpdateDetails_Should_Throw_When_Event_Is_Cancelled()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        evt.Cancel();

        // Act
        Action act = () => evt.UpdateDetails(ValidTitle, ValidDescription, _futureDate, ValidLocationName);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot update a cancelled event*");
    }

    [Fact]
    public void Event_SetLocation_Should_Set_Coordinates()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var location = new Coordinates(48.8566, 2.3522);

        // Act
        evt.SetLocation(location);

        // Assert
        evt.Location.Should().Be(location);
    }

    [Fact]
    public void Event_SetDivingSpot_Should_Set_DivingSpotId()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var spotId = Guid.NewGuid();

        // Act
        evt.SetDivingSpot(spotId);

        // Assert
        evt.DivingSpotId.Should().Be(spotId);
    }

    [Fact]
    public void Event_UpdateMaxParticipants_Should_Update_Maximum()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Act
        evt.UpdateMaxParticipants(20);

        // Assert
        evt.MaxParticipants.Should().Be(20);
        evt.AvailableSpots.Should().Be(20);
    }

    [Fact]
    public void Event_UpdateMaxParticipants_Should_Throw_When_LessThan_CurrentParticipants()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 50);
        evt.RegisterParticipant(Guid.NewGuid());
        evt.RegisterParticipant(Guid.NewGuid());
        evt.RegisterParticipant(Guid.NewGuid());

        // Act
        Action act = () => evt.UpdateMaxParticipants(2);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot set max participants to 2 when 3 are already registered*");
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Add_Participant()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var userId = Guid.NewGuid();

        // Act
        var participant = evt.RegisterParticipant(userId, "Excited to join!");

        // Assert
        participant.Should().NotBeNull();
        participant.UserId.Should().Be(userId);
        evt.Participants.Should().HaveCount(1);
        evt.Participants.Should().Contain(participant);
        evt.ParticipantCount.Should().Be(1);
        evt.IsUserRegistered(userId).Should().BeTrue();
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Update_AvailableSpots()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 5);

        // Act
        evt.RegisterParticipant(Guid.NewGuid());
        evt.RegisterParticipant(Guid.NewGuid());

        // Assert
        evt.ParticipantCount.Should().Be(2);
        evt.AvailableSpots.Should().Be(3);
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Throw_When_Event_Is_Full()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 2);
        evt.RegisterParticipant(Guid.NewGuid());
        evt.RegisterParticipant(Guid.NewGuid());

        // Act
        Action act = () => evt.RegisterParticipant(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Event is full. Maximum participants reached*");
        evt.IsFull.Should().BeTrue();
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Throw_When_User_Already_Registered()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var userId = Guid.NewGuid();
        evt.RegisterParticipant(userId);

        // Act
        Action act = () => evt.RegisterParticipant(userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is already registered for this event*");
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Throw_When_User_Is_Organizer()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Act
        Action act = () => evt.RegisterParticipant(_organizerId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Organizer is automatically a participant*");
    }

    [Fact]
    public void Event_RegisterParticipant_Should_Throw_When_Event_Is_Cancelled()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        evt.Cancel();

        // Act
        Action act = () => evt.RegisterParticipant(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot register for a cancelled event*");
    }

    [Fact]
    public void Event_UnregisterParticipant_Should_Remove_Participant()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var userId = Guid.NewGuid();
        evt.RegisterParticipant(userId);

        // Act
        evt.UnregisterParticipant(userId);

        // Assert
        evt.Participants.Should().BeEmpty();
        evt.ParticipantCount.Should().Be(0);
        evt.IsUserRegistered(userId).Should().BeFalse();
    }

    [Fact]
    public void Event_UnregisterParticipant_Should_Throw_When_User_Not_Registered()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var userId = Guid.NewGuid();

        // Act
        Action act = () => evt.UnregisterParticipant(userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is not registered for this event*");
    }

    [Fact]
    public void Event_Cancel_Should_Cancel_Event()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Act
        evt.Cancel();

        // Assert
        evt.Status.Should().Be(EventStatus.Cancelled);
    }

    [Fact]
    public void Event_Cancel_Should_Throw_When_Already_Cancelled()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        evt.Cancel();

        // Act
        Action act = () => evt.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Event is already cancelled*");
    }

    [Fact]
    public void Event_Complete_Should_Mark_Event_As_Completed()
    {
        // Arrange
        var pastEventDate = DateTime.UtcNow.AddMinutes(-1);
        var evt = Event.Create(ValidTitle, ValidDescription, pastEventDate, ValidLocationName, _organizerId);

        // Act
        evt.Complete();

        // Assert
        evt.Status.Should().Be(EventStatus.Completed);
    }

    [Fact]
    public void Event_Complete_Should_Throw_When_Event_HasNot_Occurred()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Act
        Action act = () => evt.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot complete an event that hasn't occurred yet*");
    }

    [Fact]
    public void Event_Complete_Should_Throw_When_Already_Completed()
    {
        // Arrange
        var pastEventDate = DateTime.UtcNow.AddMinutes(-1);
        var evt = Event.Create(ValidTitle, ValidDescription, pastEventDate, ValidLocationName, _organizerId);
        evt.Complete();

        // Act
        Action act = () => evt.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Event is already completed*");
    }

    [Fact]
    public void Event_Complete_Should_Throw_When_Event_Is_Cancelled()
    {
        // Arrange
        var pastEventDate = DateTime.UtcNow.AddMinutes(-1);
        var evt = Event.Create(ValidTitle, ValidDescription, pastEventDate, ValidLocationName, _organizerId);
        evt.Cancel();

        // Act
        Action act = () => evt.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot complete a cancelled event*");
    }

    [Fact]
    public void Event_IsUserRegistered_Should_Return_False_When_Not_Registered()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);
        var userId = Guid.NewGuid();

        // Act
        var isRegistered = evt.IsUserRegistered(userId);

        // Assert
        isRegistered.Should().BeFalse();
    }

    [Fact]
    public void Event_AvailableSpots_Should_Return_Null_When_No_Limit()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        evt.AvailableSpots.Should().BeNull();
        evt.IsFull.Should().BeFalse();
    }

    [Fact]
    public void Event_Should_Allow_Multiple_Participants()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId, 5);

        // Act
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();

        evt.RegisterParticipant(user1);
        evt.RegisterParticipant(user2);
        evt.RegisterParticipant(user3);

        // Assert
        evt.ParticipantCount.Should().Be(3);
        evt.IsUserRegistered(user1).Should().BeTrue();
        evt.IsUserRegistered(user2).Should().BeTrue();
        evt.IsUserRegistered(user3).Should().BeTrue();
    }

    [Fact]
    public void Event_Participants_Should_Be_ReadOnly_Collection()
    {
        // Arrange
        var evt = Event.Create(ValidTitle, ValidDescription, _futureDate, ValidLocationName, _organizerId);

        // Assert
        evt.Participants.Should().BeAssignableTo<IReadOnlyCollection<EventParticipant>>();
    }
}
