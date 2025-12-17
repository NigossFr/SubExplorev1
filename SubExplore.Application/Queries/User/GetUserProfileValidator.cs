// <copyright file="GetUserProfileValidator.cs" company="SubExplore">
// Copyright (c) SubExplore. All rights reserved.
// </copyright>

namespace SubExplore.Application.Queries.User;

using FluentValidation;

/// <summary>
/// Validator for the <see cref="GetUserProfile"/> query.
/// </summary>
public class GetUserProfileValidator : AbstractValidator<GetUserProfile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfileValidator"/> class.
    /// </summary>
    public GetUserProfileValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
