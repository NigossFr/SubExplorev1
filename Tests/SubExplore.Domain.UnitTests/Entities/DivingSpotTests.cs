using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.Entities;

public class DivingSpotTests
{
    private readonly Coordinates _validLocation = new(48.8566, 2.3522);
    private readonly Guid _createdBy = Guid.NewGuid();
    private const string ValidName = "Blue Hole";
    private const string ValidDescription = "Amazing diving spot with crystal clear water and diverse marine life.";

    [Fact]
    public void DivingSpot_Create_Should_Create_DivingSpot_With_Valid_Data()
    {
        // Act
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Should().NotBeNull();
        spot.Id.Should().NotBeEmpty();
        spot.Name.Should().Be(ValidName);
        spot.Description.Should().Be(ValidDescription);
        spot.Location.Should().Be(_validLocation);
        spot.CreatedBy.Should().Be(_createdBy);
        spot.CurrentTemperature.Should().BeNull();
        spot.CurrentVisibility.Should().BeNull();
        spot.MaximumDepth.Should().BeNull();
        spot.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        spot.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        spot.Photos.Should().BeEmpty();
        spot.Ratings.Should().BeEmpty();
        spot.AverageRating.Should().Be(0.0);
        spot.TotalRatings.Should().Be(0);
    }

    [Fact]
    public void DivingSpot_Create_Should_Create_DivingSpot_With_MaximumDepth()
    {
        // Arrange
        var depth = Depth.FromMeters(30);

        // Act
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy, depth);

        // Assert
        spot.MaximumDepth.Should().Be(depth);
    }

    [Fact]
    public void DivingSpot_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var spot1 = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var spot2 = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot1.Id.Should().NotBe(spot2.Id);
    }

    [Fact]
    public void DivingSpot_Create_Should_Trim_Name()
    {
        // Act
        var spot = DivingSpot.Create("  Blue Hole  ", ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Name.Should().Be("Blue Hole");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void DivingSpot_Create_Should_Throw_When_Name_Is_Invalid(string invalidName)
    {
        // Act
        Action act = () => DivingSpot.Create(invalidName!, ValidDescription, _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot name cannot be null or empty*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Throw_When_Name_Is_TooShort()
    {
        // Act
        Action act = () => DivingSpot.Create("ab", ValidDescription, _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot name must be at least 3 characters*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Throw_When_Name_Is_TooLong()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        Action act = () => DivingSpot.Create(longName, ValidDescription, _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot name cannot exceed 100 characters*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Accept_MinLength_Name()
    {
        // Act
        var spot = DivingSpot.Create("ABC", ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Name.Should().Be("ABC");
    }

    [Fact]
    public void DivingSpot_Create_Should_Accept_MaxLength_Name()
    {
        // Arrange
        var maxName = new string('a', 100);

        // Act
        var spot = DivingSpot.Create(maxName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Name.Should().HaveLength(100);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void DivingSpot_Create_Should_Throw_When_Description_Is_Invalid(string invalidDescription)
    {
        // Act
        Action act = () => DivingSpot.Create(ValidName, invalidDescription!, _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot description cannot be null or empty*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Throw_When_Description_Is_TooShort()
    {
        // Act
        Action act = () => DivingSpot.Create(ValidName, "Short", _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot description must be at least 10 characters*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Throw_When_Description_Is_TooLong()
    {
        // Arrange
        var longDescription = new string('a', 2001);

        // Act
        Action act = () => DivingSpot.Create(ValidName, longDescription, _validLocation, _createdBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Diving spot description cannot exceed 2000 characters*");
    }

    [Fact]
    public void DivingSpot_Create_Should_Accept_MinLength_Description()
    {
        // Act
        var spot = DivingSpot.Create(ValidName, "1234567890", _validLocation, _createdBy);

        // Assert
        spot.Description.Should().Be("1234567890");
    }

    [Fact]
    public void DivingSpot_Create_Should_Accept_MaxLength_Description()
    {
        // Arrange
        var maxDescription = new string('a', 2000);

        // Act
        var spot = DivingSpot.Create(ValidName, maxDescription, _validLocation, _createdBy);

        // Assert
        spot.Description.Should().HaveLength(2000);
    }

    [Fact]
    public void DivingSpot_UpdateInformation_Should_Update_Name_And_Description()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        const string newName = "Red Sea Reef";
        const string newDescription = "Updated: Spectacular coral formations and abundant fish.";

        // Act
        spot.UpdateInformation(newName, newDescription);

        // Assert
        spot.Name.Should().Be(newName);
        spot.Description.Should().Be(newDescription);
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_UpdateConditions_Should_Update_Temperature_And_Visibility()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        var temperature = WaterTemperature.FromCelsius(22);
        var visibility = Visibility.FromMeters(25);

        // Act
        spot.UpdateConditions(temperature, visibility);

        // Assert
        spot.CurrentTemperature.Should().Be(temperature);
        spot.CurrentVisibility.Should().Be(visibility);
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_UpdateMaximumDepth_Should_Update_Depth()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        var depth = Depth.FromMeters(40);

        // Act
        spot.UpdateMaximumDepth(depth);

        // Assert
        spot.MaximumDepth.Should().Be(depth);
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_AddPhoto_Should_Add_Photo_To_Collection()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var uploadedBy = Guid.NewGuid();
        const string url = "https://example.com/photo.jpg";
        const string caption = "Beautiful reef";

        // Act
        var photo = spot.AddPhoto(url, caption, uploadedBy);

        // Assert
        photo.Should().NotBeNull();
        photo.DivingSpotId.Should().Be(spot.Id);
        photo.Url.Should().Be(url);
        photo.Caption.Should().Be(caption);
        photo.UploadedBy.Should().Be(uploadedBy);
        spot.Photos.Should().HaveCount(1);
        spot.Photos.Should().Contain(photo);
    }

    [Fact]
    public void DivingSpot_AddPhoto_Should_Update_UpdatedAt()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        // Act
        spot.AddPhoto("https://example.com/photo.jpg", "Caption", Guid.NewGuid());

        // Assert
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_AddPhoto_Should_Add_Multiple_Photos()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Act
        var photo1 = spot.AddPhoto("https://example.com/photo1.jpg", "Photo 1", Guid.NewGuid());
        var photo2 = spot.AddPhoto("https://example.com/photo2.jpg", "Photo 2", Guid.NewGuid());
        var photo3 = spot.AddPhoto("https://example.com/photo3.jpg", "Photo 3", Guid.NewGuid());

        // Assert
        spot.Photos.Should().HaveCount(3);
        spot.Photos.Should().Contain(new[] { photo1, photo2, photo3 });
    }

    [Fact]
    public void DivingSpot_RemovePhoto_Should_Remove_Photo_From_Collection()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var photo = spot.AddPhoto("https://example.com/photo.jpg", "Caption", Guid.NewGuid());

        // Act
        spot.RemovePhoto(photo.Id);

        // Assert
        spot.Photos.Should().BeEmpty();
    }

    [Fact]
    public void DivingSpot_RemovePhoto_Should_Throw_When_Photo_Not_Found()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var nonExistentPhotoId = Guid.NewGuid();

        // Act
        Action act = () => spot.RemovePhoto(nonExistentPhotoId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*Photo with ID {nonExistentPhotoId} not found*");
    }

    [Fact]
    public void DivingSpot_RemovePhoto_Should_Update_UpdatedAt()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var photo = spot.AddPhoto("https://example.com/photo.jpg", "Caption", Guid.NewGuid());
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        // Act
        spot.RemovePhoto(photo.Id);

        // Assert
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_Rate_Should_Add_New_Rating()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var userId = Guid.NewGuid();
        const int score = 5;
        const string comment = "Excellent diving spot!";

        // Act
        var rating = spot.Rate(userId, score, comment);

        // Assert
        rating.Should().NotBeNull();
        rating.DivingSpotId.Should().Be(spot.Id);
        rating.UserId.Should().Be(userId);
        rating.Score.Should().Be(score);
        rating.Comment.Should().Be(comment);
        spot.Ratings.Should().HaveCount(1);
        spot.Ratings.Should().Contain(rating);
    }

    [Fact]
    public void DivingSpot_Rate_Should_Update_Existing_Rating()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var userId = Guid.NewGuid();
        var originalRating = spot.Rate(userId, 3, "Good");
        Thread.Sleep(10);

        // Act
        var updatedRating = spot.Rate(userId, 5, "Actually, it's excellent!");

        // Assert
        spot.Ratings.Should().HaveCount(1); // Still only one rating
        updatedRating.Should().BeSameAs(originalRating); // Same object reference
        updatedRating.Score.Should().Be(5);
        updatedRating.Comment.Should().Be("Actually, it's excellent!");
        updatedRating.UpdatedAt.Should().BeAfter(updatedRating.SubmittedAt);
    }

    [Fact]
    public void DivingSpot_Rate_Should_Update_UpdatedAt()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        // Act
        spot.Rate(Guid.NewGuid(), 4, "Nice spot");

        // Assert
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_Rate_Should_Allow_Multiple_Users_To_Rate()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Act
        var rating1 = spot.Rate(Guid.NewGuid(), 5, "Excellent");
        var rating2 = spot.Rate(Guid.NewGuid(), 4, "Very good");
        var rating3 = spot.Rate(Guid.NewGuid(), 3, "Good");

        // Assert
        spot.Ratings.Should().HaveCount(3);
        spot.Ratings.Should().Contain(new[] { rating1, rating2, rating3 });
    }

    [Fact]
    public void DivingSpot_RemoveRating_Should_Remove_Rating_From_Collection()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var userId = Guid.NewGuid();
        spot.Rate(userId, 4, "Good");

        // Act
        spot.RemoveRating(userId);

        // Assert
        spot.Ratings.Should().BeEmpty();
    }

    [Fact]
    public void DivingSpot_RemoveRating_Should_Throw_When_Rating_Not_Found()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var nonExistentUserId = Guid.NewGuid();

        // Act
        Action act = () => spot.RemoveRating(nonExistentUserId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*Rating by user {nonExistentUserId} not found*");
    }

    [Fact]
    public void DivingSpot_RemoveRating_Should_Update_UpdatedAt()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var userId = Guid.NewGuid();
        spot.Rate(userId, 4, "Good");
        var originalUpdatedAt = spot.UpdatedAt;
        Thread.Sleep(10);

        // Act
        spot.RemoveRating(userId);

        // Assert
        spot.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpot_AverageRating_Should_Return_Zero_When_No_Ratings()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.AverageRating.Should().Be(0.0);
    }

    [Fact]
    public void DivingSpot_AverageRating_Should_Calculate_Correct_Average()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        spot.Rate(Guid.NewGuid(), 5, "Excellent");
        spot.Rate(Guid.NewGuid(), 4, "Very good");
        spot.Rate(Guid.NewGuid(), 3, "Good");

        // Assert
        spot.AverageRating.Should().Be(4.0); // (5 + 4 + 3) / 3 = 4.0
    }

    [Fact]
    public void DivingSpot_AverageRating_Should_Update_When_Rating_Changes()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);
        var userId = Guid.NewGuid();
        spot.Rate(userId, 3, "Good");
        spot.Rate(Guid.NewGuid(), 5, "Excellent");

        // Initial average: (3 + 5) / 2 = 4.0
        spot.AverageRating.Should().Be(4.0);

        // Act - Update first user's rating
        spot.Rate(userId, 5, "Changed my mind, it's excellent!");

        // Assert - New average: (5 + 5) / 2 = 5.0
        spot.AverageRating.Should().Be(5.0);
    }

    [Fact]
    public void DivingSpot_TotalRatings_Should_Return_Correct_Count()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Act & Assert - Initially zero
        spot.TotalRatings.Should().Be(0);

        // Add ratings
        spot.Rate(Guid.NewGuid(), 5, "Excellent");
        spot.TotalRatings.Should().Be(1);

        spot.Rate(Guid.NewGuid(), 4, "Very good");
        spot.TotalRatings.Should().Be(2);

        spot.Rate(Guid.NewGuid(), 3, "Good");
        spot.TotalRatings.Should().Be(3);
    }

    [Fact]
    public void DivingSpot_Photos_Should_Be_ReadOnly_Collection()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Photos.Should().BeAssignableTo<IReadOnlyCollection<DivingSpotPhoto>>();
    }

    [Fact]
    public void DivingSpot_Ratings_Should_Be_ReadOnly_Collection()
    {
        // Arrange
        var spot = DivingSpot.Create(ValidName, ValidDescription, _validLocation, _createdBy);

        // Assert
        spot.Ratings.Should().BeAssignableTo<IReadOnlyCollection<DivingSpotRating>>();
    }
}
