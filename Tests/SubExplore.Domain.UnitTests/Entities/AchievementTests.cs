using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.Enums;

namespace SubExplore.Domain.UnitTests.Entities;

public class AchievementTests
{
    private const string ValidTitle = "First Dive";
    private const string ValidDescription = "Complete your first dive";
    private const AchievementType ValidType = AchievementType.DiveCount;
    private const AchievementCategory ValidCategory = AchievementCategory.Bronze;
    private const int ValidPoints = 10;

    [Fact]
    public void Achievement_Create_Should_Create_Achievement_With_Valid_Data()
    {
        // Act
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement.Should().NotBeNull();
        achievement.Id.Should().NotBeEmpty();
        achievement.Title.Should().Be(ValidTitle);
        achievement.Description.Should().Be(ValidDescription);
        achievement.Type.Should().Be(ValidType);
        achievement.Category.Should().Be(ValidCategory);
        achievement.Points.Should().Be(ValidPoints);
        achievement.IconUrl.Should().BeNull();
        achievement.RequiredValue.Should().BeNull();
        achievement.IsSecret.Should().BeFalse();
        achievement.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        achievement.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Achievement_Create_Should_Create_Achievement_With_RequiredValue()
    {
        // Act
        var achievement = Achievement.Create(
            "100 Dives",
            "Complete 100 dives",
            AchievementType.DiveCount,
            AchievementCategory.Gold,
            100,
            requiredValue: 100);

        // Assert
        achievement.RequiredValue.Should().Be(100);
    }

    [Fact]
    public void Achievement_Create_Should_Create_Secret_Achievement()
    {
        // Act
        var achievement = Achievement.Create(
            ValidTitle,
            ValidDescription,
            ValidType,
            ValidCategory,
            ValidPoints,
            isSecret: true);

        // Assert
        achievement.IsSecret.Should().BeTrue();
    }

    [Fact]
    public void Achievement_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var achievement1 = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        var achievement2 = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement1.Id.Should().NotBe(achievement2.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Achievement_Create_Should_Throw_When_Title_Is_Invalid(string invalidTitle)
    {
        // Act
        Action act = () => Achievement.Create(invalidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement title cannot be null or empty*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Title_Is_Too_Short()
    {
        // Act
        Action act = () => Achievement.Create("AB", ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement title must be at least 3 characters*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Title_Is_Too_Long()
    {
        // Arrange
        var longTitle = new string('a', 101);

        // Act
        Action act = () => Achievement.Create(longTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement title cannot exceed 100 characters*");
    }

    [Fact]
    public void Achievement_Create_Should_Accept_MaxLength_Title()
    {
        // Arrange
        var maxTitle = new string('a', 100);

        // Act
        var achievement = Achievement.Create(maxTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement.Title.Should().HaveLength(100);
    }

    [Fact]
    public void Achievement_Create_Should_Trim_Title()
    {
        // Act
        var achievement = Achievement.Create("  First Dive  ", ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement.Title.Should().Be("First Dive");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Achievement_Create_Should_Throw_When_Description_Is_Invalid(string invalidDescription)
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, invalidDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement description cannot be null or empty*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Description_Is_Too_Short()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, "Short", ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement description must be at least 10 characters*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Description_Is_Too_Long()
    {
        // Arrange
        var longDescription = new string('a', 501);

        // Act
        Action act = () => Achievement.Create(ValidTitle, longDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement description cannot exceed 500 characters*");
    }

    [Fact]
    public void Achievement_Create_Should_Accept_MaxLength_Description()
    {
        // Arrange
        var maxDescription = new string('a', 500);

        // Act
        var achievement = Achievement.Create(ValidTitle, maxDescription, ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement.Description.Should().HaveLength(500);
    }

    [Fact]
    public void Achievement_Create_Should_Trim_Description()
    {
        // Act
        var achievement = Achievement.Create(ValidTitle, "  Complete your first dive  ", ValidType, ValidCategory, ValidPoints);

        // Assert
        achievement.Description.Should().Be("Complete your first dive");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Points_Is_Negative()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement points cannot be negative*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_Points_Exceeds_Maximum()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, 10001);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Achievement points cannot exceed 10000*");
    }

    [Fact]
    public void Achievement_Create_Should_Accept_Maximum_Points()
    {
        // Act
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, 10000);

        // Assert
        achievement.Points.Should().Be(10000);
    }

    [Fact]
    public void Achievement_Create_Should_Accept_Zero_Points()
    {
        // Act
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, 0);

        // Assert
        achievement.Points.Should().Be(0);
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_RequiredValue_Is_Zero()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints, requiredValue: 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Required value must be at least 1*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_RequiredValue_Is_Negative()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints, requiredValue: -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Required value must be at least 1*");
    }

    [Fact]
    public void Achievement_Create_Should_Throw_When_RequiredValue_Exceeds_Maximum()
    {
        // Act
        Action act = () => Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints, requiredValue: 1000001);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Required value cannot exceed 1000000*");
    }

    [Fact]
    public void Achievement_Create_Should_Accept_Maximum_RequiredValue()
    {
        // Act
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints, requiredValue: 1000000);

        // Assert
        achievement.RequiredValue.Should().Be(1000000);
    }

    [Fact]
    public void Achievement_UpdateDetails_Should_Update_Details()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        const string newTitle = "Updated Title";
        const string newDescription = "Updated description text";
        const int newPoints = 50;

        // Act
        achievement.UpdateDetails(newTitle, newDescription, newPoints);

        // Assert
        achievement.Title.Should().Be(newTitle);
        achievement.Description.Should().Be(newDescription);
        achievement.Points.Should().Be(newPoints);
        achievement.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Achievement_SetIconUrl_Should_Set_IconUrl()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        const string iconUrl = "https://example.com/icon.png";

        // Act
        achievement.SetIconUrl(iconUrl);

        // Assert
        achievement.IconUrl.Should().Be(iconUrl);
    }

    [Fact]
    public void Achievement_SetIconUrl_Should_Accept_Null()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        achievement.SetIconUrl("https://example.com/icon.png");

        // Act
        achievement.SetIconUrl(null);

        // Assert
        achievement.IconUrl.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Achievement_SetIconUrl_Should_Convert_Empty_To_Null(string emptyUrl)
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Act
        achievement.SetIconUrl(emptyUrl);

        // Assert
        achievement.IconUrl.Should().BeNull();
    }

    [Fact]
    public void Achievement_SetIconUrl_Should_Throw_When_Url_Too_Long()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        var longUrl = "https://example.com/" + new string('a', 500);

        // Act
        Action act = () => achievement.SetIconUrl(longUrl);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Icon URL cannot exceed 500 characters*");
    }

    [Fact]
    public void Achievement_SetIconUrl_Should_Trim_Url()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Act
        achievement.SetIconUrl("  https://example.com/icon.png  ");

        // Assert
        achievement.IconUrl.Should().Be("https://example.com/icon.png");
    }

    [Fact]
    public void Achievement_UpdateRequiredValue_Should_Update_RequiredValue()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Act
        achievement.UpdateRequiredValue(100);

        // Assert
        achievement.RequiredValue.Should().Be(100);
    }

    [Fact]
    public void Achievement_UpdateRequiredValue_Should_Accept_Null()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints, requiredValue: 100);

        // Act
        achievement.UpdateRequiredValue(null);

        // Assert
        achievement.RequiredValue.Should().BeNull();
    }

    [Fact]
    public void Achievement_ToggleSecret_Should_Toggle_IsSecret()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);
        var initialState = achievement.IsSecret;

        // Act
        achievement.ToggleSecret();

        // Assert
        achievement.IsSecret.Should().Be(!initialState);
    }

    [Fact]
    public void Achievement_ToggleSecret_Should_Toggle_Back_And_Forth()
    {
        // Arrange
        var achievement = Achievement.Create(ValidTitle, ValidDescription, ValidType, ValidCategory, ValidPoints);

        // Act
        achievement.ToggleSecret();
        achievement.ToggleSecret();

        // Assert
        achievement.IsSecret.Should().BeFalse();
    }
}
