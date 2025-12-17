namespace SubExplore.Domain.Enums;

/// <summary>
/// Represents the type of breathing gas used during a dive.
/// </summary>
public enum GasType
{
    /// <summary>
    /// Regular air (21% oxygen, 79% nitrogen).
    /// </summary>
    Air = 0,

    /// <summary>
    /// Enriched air nitrox (higher oxygen percentage, typically 32% or 36%).
    /// </summary>
    Nitrox = 1,

    /// <summary>
    /// Trimix (helium, oxygen, and nitrogen blend for deep diving).
    /// </summary>
    Trimix = 2,

    /// <summary>
    /// Heliox (helium and oxygen blend).
    /// </summary>
    Heliox = 3
}
