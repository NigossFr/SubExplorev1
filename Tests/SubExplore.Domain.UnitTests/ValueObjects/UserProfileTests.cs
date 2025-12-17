using FluentAssertions;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.ValueObjects;

public class UserProfileTests
{
    [Fact]
    public void UserProfile_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        const string firstName = "John";
        const string lastName = "Doe";
        const string bio = "Passionate diver";
        const string pictureUrl = "https://example.com/profile.jpg";

        // Act
        var profile = new UserProfile(firstName, lastName, bio, pictureUrl);

        // Assert
        profile.FirstName.Should().Be(firstName);
        profile.LastName.Should().Be(lastName);
        profile.Bio.Should().Be(bio);
        profile.ProfilePictureUrl.Should().Be(pictureUrl);
    }

    [Fact]
    public void UserProfile_Should_Trim_Names()
    {
        // Arrange & Act
        var profile = new UserProfile("  John  ", "  Doe  ", null, null);

        // Assert
        profile.FirstName.Should().Be("John");
        profile.LastName.Should().Be("Doe");
    }

    [Fact]
    public void UserProfile_Should_Calculate_FullName()
    {
        // Arrange
        var profile = new UserProfile("John", "Doe", null, null);

        // Act
        var fullName = profile.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UserProfile_Should_Throw_When_FirstName_Is_Invalid(string invalidFirstName)
    {
        // Act
        Action act = () => new UserProfile(invalidFirstName!, "Doe", null, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*First name cannot be null or empty*");
    }

    [Fact]
    public void UserProfile_Should_Throw_When_FirstName_Exceeds_MaxLength()
    {
        // Arrange
        var longName = new string('A', 51);

        // Act
        Action act = () => new UserProfile(longName, "Doe", null, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 50 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UserProfile_Should_Throw_When_LastName_Is_Invalid(string invalidLastName)
    {
        // Act
        Action act = () => new UserProfile("John", invalidLastName!, null, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Last name cannot be null or empty*");
    }

    [Fact]
    public void UserProfile_Should_Throw_When_LastName_Exceeds_MaxLength()
    {
        // Arrange
        var longName = new string('A', 51);

        // Act
        Action act = () => new UserProfile("John", longName, null, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 50 characters*");
    }

    [Fact]
    public void UserProfile_Should_Accept_Null_Bio()
    {
        // Act
        var profile = new UserProfile("John", "Doe", null, null);

        // Assert
        profile.Bio.Should().BeNull();
    }

    [Fact]
    public void UserProfile_Should_Trim_Bio()
    {
        // Arrange & Act
        var profile = new UserProfile("John", "Doe", "  Bio text  ", null);

        // Assert
        profile.Bio.Should().Be("Bio text");
    }

    [Fact]
    public void UserProfile_Should_Set_Null_For_Empty_Bio()
    {
        // Act
        var profile = new UserProfile("John", "Doe", "   ", null);

        // Assert
        profile.Bio.Should().BeNull();
    }

    [Fact]
    public void UserProfile_Should_Throw_When_Bio_Exceeds_MaxLength()
    {
        // Arrange
        var longBio = new string('A', 501);

        // Act
        Action act = () => new UserProfile("John", "Doe", longBio, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Biography cannot exceed 500 characters*");
    }

    [Fact]
    public void UserProfile_Should_Accept_Null_ProfilePictureUrl()
    {
        // Act
        var profile = new UserProfile("John", "Doe", null, null);

        // Assert
        profile.ProfilePictureUrl.Should().BeNull();
    }

    [Fact]
    public void UserProfile_With_Should_Create_New_Instance_With_Updated_FirstName()
    {
        // Arrange
        var original = new UserProfile("John", "Doe", "Bio", "url");

        // Act
        var updated = original.With(firstName: "Jane");

        // Assert
        updated.FirstName.Should().Be("Jane");
        updated.LastName.Should().Be("Doe");
        updated.Bio.Should().Be("Bio");
        updated.ProfilePictureUrl.Should().Be("url");
        original.FirstName.Should().Be("John"); // Original unchanged
    }

    [Fact]
    public void UserProfile_With_Should_Keep_Original_Values_When_Null_Provided()
    {
        // Arrange
        var original = new UserProfile("John", "Doe", "Bio", "url");

        // Act
        var updated = original.With();

        // Assert
        updated.Should().Be(original);
    }

    [Fact]
    public void UserProfile_Should_Be_Equal_When_Values_Are_Same()
    {
        // Arrange
        var profile1 = new UserProfile("John", "Doe", "Bio", "url");
        var profile2 = new UserProfile("John", "Doe", "Bio", "url");

        // Act & Assert
        profile1.Should().Be(profile2);
        (profile1 == profile2).Should().BeTrue();
    }

    [Fact]
    public void UserProfile_Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        var profile1 = new UserProfile("John", "Doe", "Bio", "url");
        var profile2 = new UserProfile("Jane", "Doe", "Bio", "url");

        // Act & Assert
        profile1.Should().NotBe(profile2);
        (profile1 == profile2).Should().BeFalse();
    }

    [Fact]
    public void UserProfile_ToString_Should_Return_FullName()
    {
        // Arrange
        var profile = new UserProfile("John", "Doe", null, null);

        // Act
        var result = profile.ToString();

        // Assert
        result.Should().Be("John Doe");
    }
}
