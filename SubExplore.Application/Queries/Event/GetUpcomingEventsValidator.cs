// <copyright file="GetUpcomingEventsValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetUpcomingEvents"/> query.
/// </summary>
public class GetUpcomingEventsValidator : AbstractValidator<GetUpcomingEvents>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUpcomingEventsValidator"/> class.
    /// </summary>
    public GetUpcomingEventsValidator()
    {
        this.RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        this.RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        this.RuleFor(x => x.MaxDistanceKm)
            .GreaterThan(0)
            .WithMessage("Maximum distance must be greater than 0.")
            .LessThanOrEqualTo(10000)
            .WithMessage("Maximum distance cannot exceed 10,000 km.")
            .When(x => x.MaxDistanceKm.HasValue);

        this.RuleFor(x => x.DaysAhead)
            .InclusiveBetween(1, 365)
            .WithMessage("Days ahead must be between 1 and 365.");

        this.RuleFor(x => x.MaxResults)
            .InclusiveBetween(1, 100)
            .WithMessage("Max results must be between 1 and 100.");
    }
}
