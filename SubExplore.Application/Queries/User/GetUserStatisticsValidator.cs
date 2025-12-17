// <copyright file="GetUserStatisticsValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetUserStatistics"/> query.
/// </summary>
public class GetUserStatisticsValidator : AbstractValidator<GetUserStatistics>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserStatisticsValidator"/> class.
    /// </summary>
    public GetUserStatisticsValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
