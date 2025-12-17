using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Handler for CreateDiveLogCommand.
/// </summary>
public class CreateDiveLogCommandHandler : IRequestHandler<CreateDiveLogCommand, CreateDiveLogResult>
{
    private readonly ILogger<CreateDiveLogCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDiveLogCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public CreateDiveLogCommandHandler(ILogger<CreateDiveLogCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateDiveLogCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of creating the dive log.</returns>
    public async Task<CreateDiveLogResult> Handle(CreateDiveLogCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating dive log for user {UserId} at spot {SpotId} on {DiveDate}",
            request.UserId,
            request.DivingSpotId,
            request.DiveDate);

        // TODO: Implement actual dive log creation logic when repositories are ready
        // 1. Validate diving spot exists
        // 2. Validate buddy user exists (if provided)
        // 3. Create Depth value objects for MaxDepth and AverageDepth
        // 4. Create WaterTemperature and Visibility value objects (if provided)
        // 5. Calculate duration from EntryTime and ExitTime
        // 6. Create DiveLog entity
        // 7. Save to repository
        // 8. Publish DiveLogCreatedEvent
        var diveLogId = Guid.NewGuid();
        var durationMinutes = (int)(request.ExitTime - request.EntryTime).TotalMinutes;

        _logger.LogInformation(
            "Dive log {DiveLogId} created successfully with duration {Duration} minutes",
            diveLogId,
            durationMinutes);

        return await Task.FromResult(new CreateDiveLogResult(true, diveLogId, durationMinutes));
    }
}
