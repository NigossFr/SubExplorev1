using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.Entities;

public class UserTests
{
    private readonly UserProfile _validProfile = new("John", "Doe", "Passionate diver", null);

    [Fact]
    public void User_Create_Should_Create_User_With_Valid_Data()
    {
        // Arrange
        const string email = "john.doe@example.com";
        const string username = "johndoe";

        // Act
        var user = User.Create(email, username, _validProfile);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(email);
        user.Username.Should().Be(username);
        user.Profile.Should().Be(_validProfile);
        user.IsPremium.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.PremiumSince.Should().BeNull();
    }

    [Fact]
    public void User_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var user1 = User.Create("user1@example.com", "user1", _validProfile);
        var user2 = User.Create("user2@example.com", "user2", _validProfile);

        // Assert
        user1.Id.Should().NotBe(user2.Id);
    }

    [Fact]
    public void User_Create_Should_Normalize_Email_ToLowercase()
    {
        // Act
        var user = User.Create("JOHN.DOE@EXAMPLE.COM", "johndoe", _validProfile);

        // Assert
        user.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void User_Create_Should_Trim_Email()
    {
        // Act
        var user = User.Create("  john.doe@example.com  ", "johndoe", _validProfile);

        // Assert
        user.Email.Should().Be("john.doe@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_Create_Should_Throw_When_Email_Is_Invalid(string invalidEmail)
    {
        // Act
        Action act = () => User.Create(invalidEmail!, "johndoe", _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email cannot be null or empty*");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("no-at-sign.com")]
    [InlineData("no-dot@domain")]
    public void User_Create_Should_Throw_When_Email_Format_Is_Invalid(string invalidEmail)
    {
        // Act
        Action act = () => User.Create(invalidEmail, "johndoe", _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email must be in valid format*");
    }

    [Fact]
    public void User_Create_Should_Throw_When_Email_Exceeds_MaxLength()
    {
        // Arrange
        var longEmail = new string('a', 92) + "@test.com"; // 92 + 9 = 101 characters

        // Act
        Action act = () => User.Create(longEmail, "johndoe", _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email cannot exceed 100 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_Create_Should_Throw_When_Username_Is_Invalid(string invalidUsername)
    {
        // Act
        Action act = () => User.Create("john.doe@example.com", invalidUsername!, _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Username cannot be null or empty*");
    }

    [Fact]
    public void User_Create_Should_Throw_When_Username_Is_TooShort()
    {
        // Act
        Action act = () => User.Create("john.doe@example.com", "ab", _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Username must be at least 3 characters*");
    }

    [Fact]
    public void User_Create_Should_Throw_When_Username_Is_TooLong()
    {
        // Arrange
        var longUsername = new string('a', 31);

        // Act
        Action act = () => User.Create("john.doe@example.com", longUsername, _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Username cannot exceed 30 characters*");
    }

    [Theory]
    [InlineData("user name")]
    [InlineData("user@name")]
    [InlineData("user#name")]
    [InlineData("user.name")]
    public void User_Create_Should_Throw_When_Username_Contains_Invalid_Characters(string invalidUsername)
    {
        // Act
        Action act = () => User.Create("john.doe@example.com", invalidUsername, _validProfile);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Username can only contain letters, numbers, underscores, and hyphens*");
    }

    [Theory]
    [InlineData("john_doe")]
    [InlineData("john-doe")]
    [InlineData("johndoe123")]
    [InlineData("JOHNDOE")]
    public void User_Create_Should_Accept_Valid_Usernames(string validUsername)
    {
        // Act
        var user = User.Create("john.doe@example.com", validUsername, _validProfile);

        // Assert
        user.Username.Should().Be(validUsername);
    }

    [Fact]
    public void User_UpdateProfile_Should_Update_Profile_And_UpdatedAt()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        var originalUpdatedAt = user.UpdatedAt;
        Thread.Sleep(10); // Ensure time difference

        var newProfile = new UserProfile("Jane", "Smith", "New bio", "new-url");

        // Act
        user.UpdateProfile(newProfile);

        // Assert
        user.Profile.Should().Be(newProfile);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_UpdateProfile_Should_Throw_When_Profile_Is_Default()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);

        // Act
        Action act = () => user.UpdateProfile(default);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Profile cannot be null*");
    }

    [Fact]
    public void User_UpgradeToPremium_Should_Set_Premium_Status()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);

        // Act
        user.UpgradeToPremium();

        // Assert
        user.IsPremium.Should().BeTrue();
        user.PremiumSince.Should().NotBeNull();
        user.PremiumSince.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void User_UpgradeToPremium_Should_Update_UpdatedAt()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        var originalUpdatedAt = user.UpdatedAt;
        Thread.Sleep(10);

        // Act
        user.UpgradeToPremium();

        // Assert
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_UpgradeToPremium_Should_Throw_When_Already_Premium()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        user.UpgradeToPremium();

        // Act
        Action act = () => user.UpgradeToPremium();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is already premium*");
    }

    [Fact]
    public void User_DowngradeToPremium_Should_Remove_Premium_Status()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        user.UpgradeToPremium();

        // Act
        user.DowngradeToPremium();

        // Assert
        user.IsPremium.Should().BeFalse();
        user.PremiumSince.Should().BeNull();
    }

    [Fact]
    public void User_DowngradeToPremium_Should_Throw_When_Not_Premium()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);

        // Act
        Action act = () => user.DowngradeToPremium();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User is not premium*");
    }

    [Fact]
    public void User_UpdateEmail_Should_Update_Email_And_UpdatedAt()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        var originalUpdatedAt = user.UpdatedAt;
        Thread.Sleep(10);

        // Act
        user.UpdateEmail("newemail@example.com");

        // Assert
        user.Email.Should().Be("newemail@example.com");
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_UpdateUsername_Should_Update_Username_And_UpdatedAt()
    {
        // Arrange
        var user = User.Create("john.doe@example.com", "johndoe", _validProfile);
        var originalUpdatedAt = user.UpdatedAt;
        Thread.Sleep(10);

        // Act
        user.UpdateUsername("newusername");

        // Assert
        user.Username.Should().Be("newusername");
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
