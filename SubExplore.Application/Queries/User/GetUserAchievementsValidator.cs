// <copyright file="GetUserAchievementsValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetUserAchievements"/> query.
/// </summary>
public class GetUserAchievementsValidator : AbstractValidator<GetUserAchievements>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserAchievementsValidator"/> class.
    /// </summary>
    public GetUserAchievementsValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        this.RuleFor(x => x.CategoryFilter)
            .MaximumLength(50)
            .WithMessage("Category filter cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.CategoryFilter));
    }
}
