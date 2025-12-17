using FluentValidation;

namespace SubExplore.Application.Queries.DiveLog;

/// <summary>
/// Validator for GetDiveLogByIdQuery.
/// </summary>
public class GetDiveLogByIdValidator : AbstractValidator<GetDiveLogByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDiveLogByIdValidator"/> class.
    /// </summary>
    public GetDiveLogByIdValidator()
    {
        RuleFor(x => x.DiveLogId)
            .NotEmpty().WithMessage("Dive log ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
