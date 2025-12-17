using FluentAssertions;
using SubExplore.Domain.Entities;

namespace SubExplore.Domain.UnitTests.Entities;

public class DivingSpotPhotoTests
{
    private readonly Guid _divingSpotId = Guid.NewGuid();
    private readonly Guid _uploadedBy = Guid.NewGuid();
    private const string ValidUrl = "https://example.com/photo.jpg";
    private const string ValidCaption = "Beautiful coral reef";

    [Fact]
    public void DivingSpotPhoto_Create_Should_Create_Photo_With_Valid_Data()
    {
        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);

        // Assert
        photo.Should().NotBeNull();
        photo.Id.Should().NotBeEmpty();
        photo.DivingSpotId.Should().Be(_divingSpotId);
        photo.Url.Should().Be(ValidUrl);
        photo.Caption.Should().Be(ValidCaption);
        photo.UploadedBy.Should().Be(_uploadedBy);
        photo.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var photo1 = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);
        var photo2 = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);

        // Assert
        photo1.Id.Should().NotBe(photo2.Id);
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Accept_Null_Caption()
    {
        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, null, _uploadedBy);

        // Assert
        photo.Caption.Should().BeNull();
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Trim_Url()
    {
        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, "  https://example.com/photo.jpg  ", ValidCaption, _uploadedBy);

        // Assert
        photo.Url.Should().Be("https://example.com/photo.jpg");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void DivingSpotPhoto_Create_Should_Throw_When_Url_Is_Invalid(string invalidUrl)
    {
        // Act
        Action act = () => DivingSpotPhoto.Create(_divingSpotId, invalidUrl!, ValidCaption, _uploadedBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Photo URL cannot be null or empty*");
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Throw_When_Url_Exceeds_MaxLength()
    {
        // Arrange
        var longUrl = new string('a', 501);

        // Act
        Action act = () => DivingSpotPhoto.Create(_divingSpotId, longUrl, ValidCaption, _uploadedBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Photo URL cannot exceed 500 characters*");
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Accept_MaxLength_Url()
    {
        // Arrange
        var maxLengthUrl = new string('a', 500);

        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, maxLengthUrl, ValidCaption, _uploadedBy);

        // Assert
        photo.Url.Should().HaveLength(500);
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Trim_Caption()
    {
        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, "  Beautiful coral reef  ", _uploadedBy);

        // Assert
        photo.Caption.Should().Be("Beautiful coral reef");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DivingSpotPhoto_Create_Should_Convert_Empty_Caption_To_Null(string emptyCaption)
    {
        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, emptyCaption, _uploadedBy);

        // Assert
        photo.Caption.Should().BeNull();
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Throw_When_Caption_Exceeds_MaxLength()
    {
        // Arrange
        var longCaption = new string('a', 201);

        // Act
        Action act = () => DivingSpotPhoto.Create(_divingSpotId, ValidUrl, longCaption, _uploadedBy);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Photo caption cannot exceed 200 characters*");
    }

    [Fact]
    public void DivingSpotPhoto_Create_Should_Accept_MaxLength_Caption()
    {
        // Arrange
        var maxLengthCaption = new string('a', 200);

        // Act
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, maxLengthCaption, _uploadedBy);

        // Assert
        photo.Caption.Should().HaveLength(200);
    }

    [Fact]
    public void DivingSpotPhoto_UpdateCaption_Should_Update_Caption()
    {
        // Arrange
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);
        const string newCaption = "Updated caption";

        // Act
        photo.UpdateCaption(newCaption);

        // Assert
        photo.Caption.Should().Be(newCaption);
    }

    [Fact]
    public void DivingSpotPhoto_UpdateCaption_Should_Accept_Null()
    {
        // Arrange
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);

        // Act
        photo.UpdateCaption(null);

        // Assert
        photo.Caption.Should().BeNull();
    }

    [Fact]
    public void DivingSpotPhoto_UpdateCaption_Should_Throw_When_Caption_Exceeds_MaxLength()
    {
        // Arrange
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);
        var longCaption = new string('a', 201);

        // Act
        Action act = () => photo.UpdateCaption(longCaption);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Photo caption cannot exceed 200 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DivingSpotPhoto_UpdateCaption_Should_Convert_Empty_Caption_To_Null(string emptyCaption)
    {
        // Arrange
        var photo = DivingSpotPhoto.Create(_divingSpotId, ValidUrl, ValidCaption, _uploadedBy);

        // Act
        photo.UpdateCaption(emptyCaption);

        // Assert
        photo.Caption.Should().BeNull();
    }
}
