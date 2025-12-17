// <copyright file="SearchUsersValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="SearchUsers"/> query.
/// </summary>
public class SearchUsersValidator : AbstractValidator<SearchUsers>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchUsersValidator"/> class.
    /// </summary>
    public SearchUsersValidator()
    {
        this.RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        this.RuleFor(x => x.MinTotalDives)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum total dives must be a non-negative number.")
            .When(x => x.MinTotalDives.HasValue);

        this.RuleFor(x => x.CertificationLevel)
            .MaximumLength(50)
            .WithMessage("Certification level cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.CertificationLevel));

        this.RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        this.RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        this.RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort field.");
    }
}
