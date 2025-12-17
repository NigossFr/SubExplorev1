using SubExplore.Domain.Enums;
using SubExplore.Domain.ValueObjects;

namespace SubExplore.Domain.Entities;

/// <summary>
/// Represents a dive log entry recording details of a diving session.
/// This is an aggregate root that manages all dive-related information.
/// </summary>
public class DiveLog
{
    /// <summary>
    /// Gets the unique identifier for the dive log.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who performed the dive.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the identifier of the diving spot where the dive took place.
    /// </summary>
    public Guid DivingSpotId { get; private set; }

    /// <summary>
    /// Gets the identifier of the buddy diver (optional).
    /// </summary>
    public Guid? BuddyUserId { get; private set; }

    /// <summary>
    /// Gets the date and time when the dive started.
    /// </summary>
    public DateTime DiveDate { get; private set; }

    /// <summary>
    /// Gets the total duration of the dive.
    /// </summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>
    /// Gets the maximum depth reached during the dive.
    /// </summary>
    public Depth MaxDepth { get; private set; }

    /// <summary>
    /// Gets the average depth during the dive (optional).
    /// </summary>
    public Depth? AverageDepth { get; private set; }

    /// <summary>
    /// Gets the water temperature during the dive (optional).
    /// </summary>
    public WaterTemperature? WaterTemperature { get; private set; }

    /// <summary>
    /// Gets the visibility during the dive (optional).
    /// </summary>
    public Visibility? Visibility { get; private set; }

    /// <summary>
    /// Gets the tank pressure at the start of the dive (in bar).
    /// </summary>
    public decimal StartPressure { get; private set; }

    /// <summary>
    /// Gets the tank pressure at the end of the dive (in bar).
    /// </summary>
    public decimal EndPressure { get; private set; }

    /// <summary>
    /// Gets the tank volume (in liters).
    /// </summary>
    public decimal TankVolume { get; private set; }

    /// <summary>
    /// Gets the type of breathing gas used.
    /// </summary>
    public GasType GasType { get; private set; }

    /// <summary>
    /// Gets the oxygen percentage for nitrox dives (21-100%).
    /// </summary>
    public int? OxygenPercentage { get; private set; }

    /// <summary>
    /// Gets the diver's notes about the dive.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the date and time when the dive log was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the dive log was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the total air consumed during the dive (in liters).
    /// Calculated as: (StartPressure - EndPressure) * TankVolume
    /// </summary>
    public decimal AirConsumed => CalculateAirConsumed();

    /// <summary>
    /// Gets the Surface Air Consumption rate (liters per minute at surface).
    /// Calculated as: AirConsumed / Duration / ((AverageDepth/10) + 1)
    /// Returns 0 if AverageDepth is not set.
    /// </summary>
    public decimal SurfaceAirConsumptionRate => CalculateSAC();

    /// <summary>
    /// Initializes a new instance of the <see cref="DiveLog"/> class.
    /// Private constructor for EF Core.
    /// </summary>
    private DiveLog()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiveLog"/> class.
    /// </summary>
    /// <param name="userId">The user performing the dive.</param>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="diveDate">The date and time of the dive.</param>
    /// <param name="duration">The dive duration.</param>
    /// <param name="maxDepth">The maximum depth reached.</param>
    /// <param name="startPressure">The starting tank pressure.</param>
    /// <param name="endPressure">The ending tank pressure.</param>
    /// <param name="tankVolume">The tank volume.</param>
    /// <param name="gasType">The type of breathing gas.</param>
    private DiveLog(
        Guid userId,
        Guid divingSpotId,
        DateTime diveDate,
        TimeSpan duration,
        Depth maxDepth,
        decimal startPressure,
        decimal endPressure,
        decimal tankVolume,
        GasType gasType)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DivingSpotId = divingSpotId;
        DiveDate = ValidateDiveDate(diveDate);
        Duration = ValidateDuration(duration);
        MaxDepth = maxDepth;
        StartPressure = ValidateStartPressure(startPressure);
        EndPressure = ValidateEndPressure(endPressure, startPressure);
        TankVolume = ValidateTankVolume(tankVolume);
        GasType = gasType;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new dive log entry.
    /// </summary>
    /// <param name="userId">The user performing the dive.</param>
    /// <param name="divingSpotId">The diving spot identifier.</param>
    /// <param name="diveDate">The date and time of the dive.</param>
    /// <param name="duration">The dive duration.</param>
    /// <param name="maxDepth">The maximum depth reached.</param>
    /// <param name="startPressure">The starting tank pressure (in bar).</param>
    /// <param name="endPressure">The ending tank pressure (in bar).</param>
    /// <param name="tankVolume">The tank volume (in liters).</param>
    /// <param name="gasType">The type of breathing gas.</param>
    /// <returns>A new <see cref="DiveLog"/> instance.</returns>
    public static DiveLog Create(
        Guid userId,
        Guid divingSpotId,
        DateTime diveDate,
        TimeSpan duration,
        Depth maxDepth,
        decimal startPressure,
        decimal endPressure,
        decimal tankVolume,
        GasType gasType = GasType.Air)
    {
        return new DiveLog(
            userId,
            divingSpotId,
            diveDate,
            duration,
            maxDepth,
            startPressure,
            endPressure,
            tankVolume,
            gasType);
    }

    /// <summary>
    /// Updates the dive details (date, duration, depths).
    /// </summary>
    /// <param name="diveDate">The new dive date.</param>
    /// <param name="duration">The new duration.</param>
    /// <param name="maxDepth">The new maximum depth.</param>
    /// <param name="averageDepth">The new average depth (optional).</param>
    public void UpdateDiveDetails(DateTime diveDate, TimeSpan duration, Depth maxDepth, Depth? averageDepth = null)
    {
        DiveDate = ValidateDiveDate(diveDate);
        Duration = ValidateDuration(duration);
        MaxDepth = maxDepth;
        AverageDepth = averageDepth;

        if (averageDepth.HasValue && averageDepth.Value.ToMeters() > maxDepth.ToMeters())
        {
            throw new ArgumentException("Average depth cannot exceed maximum depth.", nameof(averageDepth));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the equipment details (pressures, tank, gas).
    /// </summary>
    /// <param name="startPressure">The starting pressure.</param>
    /// <param name="endPressure">The ending pressure.</param>
    /// <param name="tankVolume">The tank volume.</param>
    /// <param name="gasType">The gas type.</param>
    /// <param name="oxygenPercentage">The oxygen percentage for nitrox (optional).</param>
    public void UpdateEquipment(
        decimal startPressure,
        decimal endPressure,
        decimal tankVolume,
        GasType gasType,
        int? oxygenPercentage = null)
    {
        StartPressure = ValidateStartPressure(startPressure);
        EndPressure = ValidateEndPressure(endPressure, startPressure);
        TankVolume = ValidateTankVolume(tankVolume);
        GasType = gasType;
        OxygenPercentage = ValidateOxygenPercentage(oxygenPercentage, gasType);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the diving conditions (temperature and visibility).
    /// </summary>
    /// <param name="waterTemperature">The water temperature.</param>
    /// <param name="visibility">The visibility.</param>
    public void UpdateConditions(WaterTemperature waterTemperature, Visibility visibility)
    {
        WaterTemperature = waterTemperature;
        Visibility = visibility;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets or updates the diver's notes.
    /// </summary>
    /// <param name="notes">The notes to add.</param>
    public void UpdateNotes(string? notes)
    {
        Notes = ValidateNotes(notes);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets a buddy diver for this dive.
    /// </summary>
    /// <param name="buddyUserId">The buddy user identifier.</param>
    public void SetBuddy(Guid buddyUserId)
    {
        if (buddyUserId == UserId)
        {
            throw new ArgumentException("Buddy cannot be the same as the diver.", nameof(buddyUserId));
        }

        BuddyUserId = buddyUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes the buddy diver from this dive.
    /// </summary>
    public void RemoveBuddy()
    {
        BuddyUserId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total air consumed during the dive.
    /// </summary>
    /// <returns>The air consumed in liters.</returns>
    private decimal CalculateAirConsumed()
    {
        return (StartPressure - EndPressure) * TankVolume;
    }

    /// <summary>
    /// Calculates the Surface Air Consumption rate.
    /// SAC = AirConsumed / DurationMinutes / AveragePressure
    /// where AveragePressure = (AverageDepth/10) + 1
    /// </summary>
    /// <returns>The SAC rate in liters per minute, or 0 if average depth is not set.</returns>
    private decimal CalculateSAC()
    {
        if (!AverageDepth.HasValue || Duration.TotalMinutes == 0)
        {
            return 0;
        }

        var averageDepthMeters = AverageDepth.Value.ToMeters();
        var averagePressure = (averageDepthMeters / 10m) + 1m;
        var durationMinutes = (decimal)Duration.TotalMinutes;

        return AirConsumed / durationMinutes / averagePressure;
    }

    /// <summary>
    /// Validates the dive date.
    /// </summary>
    /// <param name="diveDate">The dive date to validate.</param>
    /// <returns>The validated dive date.</returns>
    /// <exception cref="ArgumentException">Thrown when dive date is in the future.</exception>
    private static DateTime ValidateDiveDate(DateTime diveDate)
    {
        if (diveDate > DateTime.UtcNow)
        {
            throw new ArgumentException("Dive date cannot be in the future.", nameof(diveDate));
        }

        return diveDate;
    }

    /// <summary>
    /// Validates the dive duration.
    /// </summary>
    /// <param name="duration">The duration to validate.</param>
    /// <returns>The validated duration.</returns>
    /// <exception cref="ArgumentException">Thrown when duration is invalid.</exception>
    private static TimeSpan ValidateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Dive duration must be greater than zero.", nameof(duration));
        }

        if (duration.TotalHours > 24)
        {
            throw new ArgumentException("Dive duration cannot exceed 24 hours.", nameof(duration));
        }

        return duration;
    }

    /// <summary>
    /// Validates the start pressure.
    /// </summary>
    /// <param name="startPressure">The start pressure to validate.</param>
    /// <returns>The validated start pressure.</returns>
    /// <exception cref="ArgumentException">Thrown when start pressure is invalid.</exception>
    private static decimal ValidateStartPressure(decimal startPressure)
    {
        if (startPressure <= 0)
        {
            throw new ArgumentException("Start pressure must be greater than zero.", nameof(startPressure));
        }

        if (startPressure > 350)
        {
            throw new ArgumentException("Start pressure cannot exceed 350 bar.", nameof(startPressure));
        }

        return startPressure;
    }

    /// <summary>
    /// Validates the end pressure.
    /// </summary>
    /// <param name="endPressure">The end pressure to validate.</param>
    /// <param name="startPressure">The start pressure for comparison.</param>
    /// <returns>The validated end pressure.</returns>
    /// <exception cref="ArgumentException">Thrown when end pressure is invalid.</exception>
    private static decimal ValidateEndPressure(decimal endPressure, decimal startPressure)
    {
        if (endPressure < 0)
        {
            throw new ArgumentException("End pressure cannot be negative.", nameof(endPressure));
        }

        if (endPressure >= startPressure)
        {
            throw new ArgumentException("End pressure must be less than start pressure.", nameof(endPressure));
        }

        return endPressure;
    }

    /// <summary>
    /// Validates the tank volume.
    /// </summary>
    /// <param name="tankVolume">The tank volume to validate.</param>
    /// <returns>The validated tank volume.</returns>
    /// <exception cref="ArgumentException">Thrown when tank volume is invalid.</exception>
    private static decimal ValidateTankVolume(decimal tankVolume)
    {
        if (tankVolume <= 0)
        {
            throw new ArgumentException("Tank volume must be greater than zero.", nameof(tankVolume));
        }

        if (tankVolume > 50)
        {
            throw new ArgumentException("Tank volume cannot exceed 50 liters.", nameof(tankVolume));
        }

        return tankVolume;
    }

    /// <summary>
    /// Validates the oxygen percentage.
    /// </summary>
    /// <param name="oxygenPercentage">The oxygen percentage to validate.</param>
    /// <param name="gasType">The gas type.</param>
    /// <returns>The validated oxygen percentage.</returns>
    /// <exception cref="ArgumentException">Thrown when oxygen percentage is invalid.</exception>
    private static int? ValidateOxygenPercentage(int? oxygenPercentage, GasType gasType)
    {
        if (!oxygenPercentage.HasValue)
        {
            return null;
        }

        if (oxygenPercentage.Value < 21 || oxygenPercentage.Value > 100)
        {
            throw new ArgumentException("Oxygen percentage must be between 21 and 100.", nameof(oxygenPercentage));
        }

        if (gasType == GasType.Air && oxygenPercentage.Value != 21)
        {
            throw new ArgumentException("Air must have 21% oxygen.", nameof(oxygenPercentage));
        }

        return oxygenPercentage;
    }

    /// <summary>
    /// Validates the notes.
    /// </summary>
    /// <param name="notes">The notes to validate.</param>
    /// <returns>The validated notes.</returns>
    /// <exception cref="ArgumentException">Thrown when notes exceed maximum length.</exception>
    private static string? ValidateNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return null;
        }

        notes = notes.Trim();

        if (notes.Length > 2000)
        {
            throw new ArgumentException("Notes cannot exceed 2000 characters.", nameof(notes));
        }

        return notes;
    }
}
