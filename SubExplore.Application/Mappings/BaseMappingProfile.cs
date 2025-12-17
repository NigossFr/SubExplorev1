// <copyright file="BaseMappingProfile.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Mappings;

using AutoMapper;

/// <summary>
/// Base AutoMapper profile for common entity mappings.
/// </summary>
/// <remarks>
/// This profile provides basic mapping configurations for domain entities to DTOs.
/// Many DTO properties are calculated dynamically in handlers and are not mapped here.
/// This serves as a foundation for future mapping enhancements.
/// </remarks>
public class BaseMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMappingProfile"/> class.
    /// </summary>
    public BaseMappingProfile()
    {
        // Note: Most DTOs in this project contain calculated properties
        // (e.g., CurrentParticipants, TotalDives, Distance, etc.)
        // These properties are computed in the query handlers.
        // AutoMapper is configured here for future use when more direct
        // entity→DTO mappings are needed.
    }
}
