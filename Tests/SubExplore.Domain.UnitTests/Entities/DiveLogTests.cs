using FluentAssertions;
using SubExplore.Domain.Entities;
using SubExplore.Domain.Enums;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.UnitTests.Entities;

public class DiveLogTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _divingSpotId = Guid.NewGuid();
    private readonly DateTime _validDiveDate = DateTime.UtcNow.AddDays(-1);
    private readonly TimeSpan _validDuration = TimeSpan.FromMinutes(45);
    private readonly Depth _validMaxDepth = Depth.FromMeters(30);
    private const decimal ValidStartPressure = 200m;
    private const decimal ValidEndPressure = 50m;
    private const decimal ValidTankVolume = 12m;

    [Fact]
    public void DiveLog_Create_Should_Create_DiveLog_With_Valid_Data()
    {
        // Act
        var diveLog = DiveLog.Create(
            _userId,
            _divingSpotId,
            _validDiveDate,
            _validDuration,
            _validMaxDepth,
            ValidStartPressure,
            ValidEndPressure,
            ValidTankVolume);

        // Assert
        diveLog.Should().NotBeNull();
        diveLog.Id.Should().NotBeEmpty();
        diveLog.UserId.Should().Be(_userId);
        diveLog.DivingSpotId.Should().Be(_divingSpotId);
        diveLog.DiveDate.Should().Be(_validDiveDate);
        diveLog.Duration.Should().Be(_validDuration);
        diveLog.MaxDepth.Should().Be(_validMaxDepth);
        diveLog.StartPressure.Should().Be(ValidStartPressure);
        diveLog.EndPressure.Should().Be(ValidEndPressure);
        diveLog.TankVolume.Should().Be(ValidTankVolume);
        diveLog.GasType.Should().Be(GasType.Air);
        diveLog.BuddyUserId.Should().BeNull();
        diveLog.AverageDepth.Should().BeNull();
        diveLog.WaterTemperature.Should().BeNull();
        diveLog.Visibility.Should().BeNull();
        diveLog.OxygenPercentage.Should().BeNull();
        diveLog.Notes.Should().BeNull();
        diveLog.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        diveLog.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DiveLog_Create_Should_Generate_Unique_Ids()
    {
        // Act
        var diveLog1 = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var diveLog2 = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        diveLog1.Id.Should().NotBe(diveLog2.Id);
    }

    [Fact]
    public void DiveLog_Create_Should_Accept_Different_GasTypes()
    {
        // Act
        var airLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume, GasType.Air);
        var nitroxLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume, GasType.Nitrox);

        // Assert
        airLog.GasType.Should().Be(GasType.Air);
        nitroxLog.GasType.Should().Be(GasType.Nitrox);
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_DiveDate_Is_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, futureDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Dive date cannot be in the future*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DiveLog_Create_Should_Throw_When_Duration_Is_Invalid(int minutes)
    {
        // Arrange
        var invalidDuration = TimeSpan.FromMinutes(minutes);

        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, invalidDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Dive duration must be greater than zero*");
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_Duration_Exceeds_24_Hours()
    {
        // Arrange
        var tooLongDuration = TimeSpan.FromHours(25);

        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, tooLongDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Dive duration cannot exceed 24 hours*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void DiveLog_Create_Should_Throw_When_StartPressure_Is_Invalid(decimal startPressure)
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, startPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Start pressure must be greater than zero*");
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_StartPressure_Exceeds_350_Bar()
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, 351m, ValidEndPressure, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Start pressure cannot exceed 350 bar*");
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_EndPressure_Is_Negative()
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, -10m, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*End pressure cannot be negative*");
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_EndPressure_Exceeds_StartPressure()
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidStartPressure + 10, ValidTankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*End pressure must be less than start pressure*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void DiveLog_Create_Should_Throw_When_TankVolume_Is_Invalid(decimal tankVolume)
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, tankVolume);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tank volume must be greater than zero*");
    }

    [Fact]
    public void DiveLog_Create_Should_Throw_When_TankVolume_Exceeds_50_Liters()
    {
        // Act
        Action act = () => DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, 51m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tank volume cannot exceed 50 liters*");
    }

    [Fact]
    public void DiveLog_AirConsumed_Should_Calculate_Correctly()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, 200m, 50m, 12m, GasType.Air);

        // Act
        var airConsumed = diveLog.AirConsumed;

        // Assert
        airConsumed.Should().Be(1800m); // (200 - 50) * 12 = 1800 liters
    }

    [Fact]
    public void DiveLog_SurfaceAirConsumptionRate_Should_Return_Zero_When_AverageDepth_Not_Set()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Act
        var sac = diveLog.SurfaceAirConsumptionRate;

        // Assert
        sac.Should().Be(0);
    }

    [Fact]
    public void DiveLog_SurfaceAirConsumptionRate_Should_Calculate_Correctly()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, TimeSpan.FromMinutes(45), _validMaxDepth, 200m, 50m, 12m, GasType.Air);
        var averageDepth = Depth.FromMeters(20);
        diveLog.UpdateDiveDetails(_validDiveDate, TimeSpan.FromMinutes(45), _validMaxDepth, averageDepth);

        // Act
        var sac = diveLog.SurfaceAirConsumptionRate;

        // Assert
        // AirConsumed = 1800 liters
        // Duration = 45 minutes
        // AveragePressure = (20/10) + 1 = 3
        // SAC = 1800 / 45 / 3 = 13.33 liters/minute
        sac.Should().BeApproximately(13.33m, 0.01m);
    }

    [Fact]
    public void DiveLog_UpdateDiveDetails_Should_Update_All_Fields()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        var newDate = DateTime.UtcNow.AddDays(-2);
        var newDuration = TimeSpan.FromMinutes(60);
        var newMaxDepth = Depth.FromMeters(40);
        var newAverageDepth = Depth.FromMeters(35);

        // Act
        diveLog.UpdateDiveDetails(newDate, newDuration, newMaxDepth, newAverageDepth);

        // Assert
        diveLog.DiveDate.Should().Be(newDate);
        diveLog.Duration.Should().Be(newDuration);
        diveLog.MaxDepth.Should().Be(newMaxDepth);
        diveLog.AverageDepth.Should().Be(newAverageDepth);
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DiveLog_UpdateDiveDetails_Should_Throw_When_AverageDepth_Exceeds_MaxDepth()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var maxDepth = Depth.FromMeters(30);
        var averageDepth = Depth.FromMeters(35); // Greater than max

        // Act
        Action act = () => diveLog.UpdateDiveDetails(_validDiveDate, _validDuration, maxDepth, averageDepth);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Average depth cannot exceed maximum depth*");
    }

    [Fact]
    public void DiveLog_UpdateEquipment_Should_Update_All_Fields()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        // Act
        diveLog.UpdateEquipment(220m, 60m, 15m, GasType.Nitrox, 32);

        // Assert
        diveLog.StartPressure.Should().Be(220m);
        diveLog.EndPressure.Should().Be(60m);
        diveLog.TankVolume.Should().Be(15m);
        diveLog.GasType.Should().Be(GasType.Nitrox);
        diveLog.OxygenPercentage.Should().Be(32);
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(101)]
    public void DiveLog_UpdateEquipment_Should_Throw_When_OxygenPercentage_Is_OutOfRange(int oxygenPercentage)
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Act
        Action act = () => diveLog.UpdateEquipment(ValidStartPressure, ValidEndPressure, ValidTankVolume, GasType.Nitrox, oxygenPercentage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Oxygen percentage must be between 21 and 100*");
    }

    [Fact]
    public void DiveLog_UpdateEquipment_Should_Throw_When_Air_Has_Wrong_OxygenPercentage()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Act
        Action act = () => diveLog.UpdateEquipment(ValidStartPressure, ValidEndPressure, ValidTankVolume, GasType.Air, 32);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Air must have 21% oxygen*");
    }

    [Fact]
    public void DiveLog_UpdateConditions_Should_Update_Temperature_And_Visibility()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        var temperature = WaterTemperature.FromCelsius(18);
        var visibility = Visibility.FromMeters(15);

        // Act
        diveLog.UpdateConditions(temperature, visibility);

        // Assert
        diveLog.WaterTemperature.Should().Be(temperature);
        diveLog.Visibility.Should().Be(visibility);
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DiveLog_UpdateNotes_Should_Update_Notes()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        const string notes = "Amazing dive! Saw a sea turtle and many colorful fish.";

        // Act
        diveLog.UpdateNotes(notes);

        // Assert
        diveLog.Notes.Should().Be(notes);
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DiveLog_UpdateNotes_Should_Accept_Null()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        diveLog.UpdateNotes("Some notes");

        // Act
        diveLog.UpdateNotes(null);

        // Assert
        diveLog.Notes.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DiveLog_UpdateNotes_Should_Convert_Empty_Notes_To_Null(string emptyNotes)
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Act
        diveLog.UpdateNotes(emptyNotes);

        // Assert
        diveLog.Notes.Should().BeNull();
    }

    [Fact]
    public void DiveLog_UpdateNotes_Should_Throw_When_Notes_Exceed_MaxLength()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var longNotes = new string('a', 2001);

        // Act
        Action act = () => diveLog.UpdateNotes(longNotes);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Notes cannot exceed 2000 characters*");
    }

    [Fact]
    public void DiveLog_UpdateNotes_Should_Accept_MaxLength_Notes()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var maxNotes = new string('a', 2000);

        // Act
        diveLog.UpdateNotes(maxNotes);

        // Assert
        diveLog.Notes.Should().HaveLength(2000);
    }

    [Fact]
    public void DiveLog_SetBuddy_Should_Set_Buddy()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var buddyUserId = Guid.NewGuid();
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        // Act
        diveLog.SetBuddy(buddyUserId);

        // Assert
        diveLog.BuddyUserId.Should().Be(buddyUserId);
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DiveLog_SetBuddy_Should_Throw_When_Buddy_Is_Same_As_Diver()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Act
        Action act = () => diveLog.SetBuddy(_userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Buddy cannot be the same as the diver*");
    }

    [Fact]
    public void DiveLog_RemoveBuddy_Should_Remove_Buddy()
    {
        // Arrange
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);
        var buddyUserId = Guid.NewGuid();
        diveLog.SetBuddy(buddyUserId);
        var originalUpdatedAt = diveLog.UpdatedAt;
        Thread.Sleep(10);

        // Act
        diveLog.RemoveBuddy();

        // Assert
        diveLog.BuddyUserId.Should().BeNull();
        diveLog.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DiveLog_Should_Accept_Valid_Pressure_Range()
    {
        // Act
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, _validDuration, _validMaxDepth, 1m, 0.5m, ValidTankVolume);

        // Assert
        diveLog.StartPressure.Should().Be(1m);
        diveLog.EndPressure.Should().Be(0.5m);
    }

    [Fact]
    public void DiveLog_Should_Accept_Maximum_Valid_Duration()
    {
        // Arrange
        var maxDuration = TimeSpan.FromHours(24);

        // Act
        var diveLog = DiveLog.Create(_userId, _divingSpotId, _validDiveDate, maxDuration, _validMaxDepth, ValidStartPressure, ValidEndPressure, ValidTankVolume);

        // Assert
        diveLog.Duration.Should().Be(maxDuration);
    }
}
