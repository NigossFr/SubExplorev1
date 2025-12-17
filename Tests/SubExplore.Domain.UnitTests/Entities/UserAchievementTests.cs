using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class UserAchievementTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _achievementId = Guid.NewGuid();

    [Fact]
    public void UserAchievement_Create_Should_Create_UserAchievement_With_Valid_Data()
    {
        // Act
        var userAchievement = UserAchievement.Create(_userId, _achievementId);

        // Assert
        userAchievement.Should().NotBeNull();
        userAchievement.Id.Should().NotBeEmpty();
        userAchievement.UserId.Should().Be(_userId);
        userAchievement.AchievementId.Should().Be(_achievementId);
        userAchievement.UnlockedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        userAchievement.Progress.Should().BeNull();
    }

    [Fact]
    public void UserAchievement_Create_Should_Create_With_Progress()
    {
        // Act
        var userAchievement = UserAchievement.Create(_userId, _achievementId, progress: 50);

        // Assert
        userAchievement.Progress.Should().Be(50);
    }

    [Fact]
    public void UserAchievement_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var userAchievement1 = UserAchievement.Create(_userId, _achievementId);
        var userAchievement2 = UserAchievement.Create(_userId, _achievementId);

        // Assert
        userAchievement1.Id.Should().NotBe(userAchievement2.Id);
    }

    [Fact]
    public void UserAchievement_Create_Should_Throw_When_Progress_Is_Negative()
    {
        // Act
        Action act = () => UserAchievement.Create(_userId, _achievementId, progress: -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Progress cannot be negative*");
    }

    [Fact]
    public void UserAchievement_Create_Should_Throw_When_Progress_Exceeds_Maximum()
    {
        // Act
        Action act = () => UserAchievement.Create(_userId, _achievementId, progress: 1000001);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Progress cannot exceed 1000000*");
    }

    [Fact]
    public void UserAchievement_Create_Should_Accept_Maximum_Progress()
    {
        // Act
        var userAchievement = UserAchievement.Create(_userId, _achievementId, progress: 1000000);

        // Assert
        userAchievement.Progress.Should().Be(1000000);
    }

    [Fact]
    public void UserAchievement_Create_Should_Accept_Zero_Progress()
    {
        // Act
        var userAchievement = UserAchievement.Create(_userId, _achievementId, progress: 0);

        // Assert
        userAchievement.Progress.Should().Be(0);
    }

    [Fact]
    public void UserAchievement_UpdateProgress_Should_Update_Progress()
    {
        // Arrange
        var userAchievement = UserAchievement.Create(_userId, _achievementId, progress: 10);

        // Act
        userAchievement.UpdateProgress(50);

        // Assert
        userAchievement.Progress.Should().Be(50);
    }

    [Fact]
    public void UserAchievement_UpdateProgress_Should_Allow_Decreasing_Progress()
    {
        // Arrange
        var userAchievement = UserAchievement.Create(_userId, _achievementId, progress: 100);

        // Act
        userAchievement.UpdateProgress(50);

        // Assert
        userAchievement.Progress.Should().Be(50);
    }

    [Fact]
    public void UserAchievement_UpdateProgress_Should_Throw_When_Progress_Is_Negative()
    {
        // Arrange
        var userAchievement = UserAchievement.Create(_userId, _achievementId);

        // Act
        Action act = () => userAchievement.UpdateProgress(-1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Progress cannot be negative*");
    }

    [Fact]
    public void UserAchievement_UpdateProgress_Should_Throw_When_Progress_Exceeds_Maximum()
    {
        // Arrange
        var userAchievement = UserAchievement.Create(_userId, _achievementId);

        // Act
        Action act = () => userAchievement.UpdateProgress(1000001);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Progress cannot exceed 1000000*");
    }
}
