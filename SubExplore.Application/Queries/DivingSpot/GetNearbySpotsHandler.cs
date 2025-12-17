using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Handler for GetNearbySpotsQuery.
/// </summary>
public class GetNearbySpotsHandler : IRequestHandler<GetNearbySpotsQuery, GetNearbySpotsResult>
{
    private readonly ILogger<GetNearbySpotsHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNearbySpotsHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public GetNearbySpotsHandler(ILogger<GetNearbySpotsHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetNearbySpotsQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result containing nearby diving spots.</returns>
    public async Task<GetNearbySpotsResult> Handle(GetNearbySpotsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting nearby diving spots at ({Latitude}, {Longitude}) within {Radius}km radius",
            request.Latitude,
            request.Longitude,
            request.RadiusKm);

        // TODO: Implement actual geospatial search when repositories are ready
        // 1. Get diving spots from repository using spatial query (e.g., PostGIS ST_Distance)
        // 2. Calculate distance from search point for each spot using Haversine formula
        // 3. Filter by difficulty range if specified (MinDifficulty, MaxDifficulty)
        // 4. Filter by depth range if specified (MinDepthMeters, MaxDepthMeters)
        // 5. Order by distance ascending
        // 6. Apply limit to results
        // 7. Map spots to DivingSpotDto with distance information
        // 8. Return result with spots and total count
        var mockSpots = new List<DivingSpotDto>
        {
            new DivingSpotDto(
                Guid.NewGuid(),
                "Mock Spot Near You",
                "A beautiful diving spot for testing",
                request.Latitude + 0.01,
                request.Longitude + 0.01,
                25.0,
                1,
                4.5,
                10,
                1.5,
                22.0,
                15.0)
        };

        _logger.LogInformation(
            "Found {Count} nearby diving spots within {Radius}km",
            mockSpots.Count,
            request.RadiusKm);

        return await Task.FromResult(new GetNearbySpotsResult(true, mockSpots, mockSpots.Count));
    }
}
