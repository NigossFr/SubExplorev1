// <copyright file="GetUserEventsValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetUserEvents"/> query.
/// </summary>
public class GetUserEventsValidator : AbstractValidator<GetUserEvents>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserEventsValidator"/> class.
    /// </summary>
    public GetUserEventsValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        this.RuleFor(x => x)
            .Must(x => x.IncludeOrganized || x.IncludeRegistered)
            .WithMessage("At least one of IncludeOrganized or IncludeRegistered must be true.");

        this.RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        this.RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");
    }
}
