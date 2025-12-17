using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Commands.DivingSpot;

/// <summary>
/// Handler for CreateSpotCommand.
/// </summary>
public class CreateSpotCommandHandler : IRequestHandler<CreateSpotCommand, CreateSpotResult>
{
    private readonly ILogger<CreateSpotCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSpotCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public CreateSpotCommandHandler(ILogger<CreateSpotCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateSpotCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of creating the diving spot.</returns>
    public async Task<CreateSpotResult> Handle(CreateSpotCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating new diving spot: {Name} at ({Latitude}, {Longitude}) by user {UserId}",
            request.Name,
            request.Latitude,
            request.Longitude,
            request.CreatedBy);

        // TODO: Implement actual diving spot creation logic when repositories are ready
        // 1. Create Coordinates value object from latitude/longitude
        // 2. Create Depth value object from maxDepthMeters
        // 3. Create DivingSpot entity with provided data
        // 4. Validate coordinates (valid lat/long ranges)
        // 5. Save to repository
        // 6. Publish DivingSpotCreatedEvent
        var spotId = Guid.NewGuid();

        _logger.LogInformation("Diving spot created successfully with ID: {SpotId}", spotId);

        return await Task.FromResult(new CreateSpotResult(true, spotId));
    }
}
