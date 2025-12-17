using FluentValidation;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Validator for GetSpotByIdQuery.
/// </summary>
public class GetSpotByIdValidator : AbstractValidator<GetSpotByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpotByIdValidator"/> class.
    /// </summary>
    public GetSpotByIdValidator()
    {
        RuleFor(x => x.SpotId)
            .NotEmpty().WithMessage("Spot ID is required");
    }
}
