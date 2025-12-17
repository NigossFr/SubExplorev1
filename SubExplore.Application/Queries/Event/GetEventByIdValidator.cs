// <copyright file="GetEventByIdValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetEventById"/> query.
/// </summary>
public class GetEventByIdValidator : AbstractValidator<GetEventById>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEventByIdValidator"/> class.
    /// </summary>
    public GetEventByIdValidator()
    {
        this.RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
