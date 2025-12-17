using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class DivingSpotRatingTests
{
    private readonly Guid _divingSpotId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private const int ValidScore = 4;
    private const string ValidComment = "Great diving spot with excellent visibility!";

    [Fact]
    public void DivingSpotRating_Create_Should_Create_Rating_With_Valid_Data()
    {
        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);

        // Assert
        rating.Should().NotBeNull();
        rating.Id.Should().NotBeEmpty();
        rating.DivingSpotId.Should().Be(_divingSpotId);
        rating.UserId.Should().Be(_userId);
        rating.Score.Should().Be(ValidScore);
        rating.Comment.Should().Be(ValidComment);
        rating.SubmittedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        rating.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DivingSpotRating_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var rating1 = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);
        var rating2 = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);

        // Assert
        rating1.Id.Should().NotBe(rating2.Id);
    }

    [Fact]
    public void DivingSpotRating_Create_Should_Accept_Null_Comment()
    {
        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, null);

        // Assert
        rating.Comment.Should().BeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void DivingSpotRating_Create_Should_Accept_Valid_Scores(int score)
    {
        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, score, ValidComment);

        // Assert
        rating.Score.Should().Be(score);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void DivingSpotRating_Create_Should_Throw_When_Score_Is_OutOfRange(int invalidScore)
    {
        // Act
        Action act = () => DivingSpotRating.Create(_divingSpotId, _userId, invalidScore, ValidComment);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Rating score must be between 1 and 5*");
    }

    [Fact]
    public void DivingSpotRating_Create_Should_Trim_Comment()
    {
        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, "  Great diving spot  ");

        // Assert
        rating.Comment.Should().Be("Great diving spot");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DivingSpotRating_Create_Should_Convert_Empty_Comment_To_Null(string emptyComment)
    {
        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, emptyComment);

        // Assert
        rating.Comment.Should().BeNull();
    }

    [Fact]
    public void DivingSpotRating_Create_Should_Throw_When_Comment_Exceeds_MaxLength()
    {
        // Arrange
        var longComment = new string('a', 1001);

        // Act
        Action act = () => DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, longComment);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rating comment cannot exceed 1000 characters*");
    }

    [Fact]
    public void DivingSpotRating_Create_Should_Accept_MaxLength_Comment()
    {
        // Arrange
        var maxLengthComment = new string('a', 1000);

        // Act
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, maxLengthComment);

        // Assert
        rating.Comment.Should().HaveLength(1000);
    }

    [Fact]
    public void DivingSpotRating_Update_Should_Update_Score_And_Comment()
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);
        var originalUpdatedAt = rating.UpdatedAt;
        Thread.Sleep(10); // Ensure time difference

        const int newScore = 5;
        const string newComment = "Updated: Even better than before!";

        // Act
        rating.Update(newScore, newComment);

        // Assert
        rating.Score.Should().Be(newScore);
        rating.Comment.Should().Be(newComment);
        rating.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpotRating_Update_Should_Accept_Null_Comment()
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);

        // Act
        rating.Update(ValidScore, null);

        // Assert
        rating.Comment.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void DivingSpotRating_Update_Should_Throw_When_Score_Is_OutOfRange(int invalidScore)
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);

        // Act
        Action act = () => rating.Update(invalidScore, ValidComment);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Rating score must be between 1 and 5*");
    }

    [Fact]
    public void DivingSpotRating_Update_Should_Throw_When_Comment_Exceeds_MaxLength()
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);
        var longComment = new string('a', 1001);

        // Act
        Action act = () => rating.Update(ValidScore, longComment);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rating comment cannot exceed 1000 characters*");
    }

    [Fact]
    public void DivingSpotRating_Update_Should_Update_UpdatedAt_Timestamp()
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);
        var originalUpdatedAt = rating.UpdatedAt;
        Thread.Sleep(10);

        // Act
        rating.Update(5, "New comment");

        // Assert
        rating.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DivingSpotRating_Update_Should_Not_Change_SubmittedAt()
    {
        // Arrange
        var rating = DivingSpotRating.Create(_divingSpotId, _userId, ValidScore, ValidComment);
        var originalSubmittedAt = rating.SubmittedAt;
        Thread.Sleep(10);

        // Act
        rating.Update(5, "New comment");

        // Assert
        rating.SubmittedAt.Should().Be(originalSubmittedAt);
    }
}
