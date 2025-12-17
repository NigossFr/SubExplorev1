// <copyright file="SearchEventsValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.Event;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="SearchEvents"/> query.
/// </summary>
public class SearchEventsValidator : AbstractValidator<SearchEvents>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchEventsValidator"/> class.
    /// </summary>
    public SearchEventsValidator()
    {
        this.RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        this.RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate.Value <= x.EndDate.Value)
            .WithMessage("Start date must be before or equal to end date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        this.RuleFor(x => x.MinParticipants)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum participants must be a non-negative number.")
            .When(x => x.MinParticipants.HasValue);

        this.RuleFor(x => x.MaxParticipants)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maximum participants must be a non-negative number.")
            .When(x => x.MaxParticipants.HasValue);

        this.RuleFor(x => x)
            .Must(x => !x.MinParticipants.HasValue || !x.MaxParticipants.HasValue || x.MinParticipants.Value <= x.MaxParticipants.Value)
            .WithMessage("Minimum participants must be less than or equal to maximum participants.")
            .When(x => x.MinParticipants.HasValue && x.MaxParticipants.HasValue);

        this.RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        this.RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        this.RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort field.");
    }
}
