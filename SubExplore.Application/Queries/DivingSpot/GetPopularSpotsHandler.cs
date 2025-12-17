using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Queries.DivingSpot;

/// <summary>
/// Handler for GetPopularSpotsQuery.
/// </summary>
public class GetPopularSpotsHandler : IRequestHandler<GetPopularSpotsQuery, GetPopularSpotsResult>
{
    private readonly ILogger<GetPopularSpotsHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPopularSpotsHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public GetPopularSpotsHandler(ILogger<GetPopularSpotsHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetPopularSpotsQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result containing popular diving spots.</returns>
    public async Task<GetPopularSpotsResult> Handle(GetPopularSpotsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting top {Limit} popular diving spots (MinRatings: {MinRatings}, DaysBack: {DaysBack})",
            request.Limit,
            request.MinimumRatings,
            request.DaysBack);

        // TODO: Implement actual popular spots retrieval when repositories are ready
        // 1. Calculate date threshold from DaysBack parameter
        // 2. Get diving spots from repository with rating count >= MinimumRatings
        // 3. For each spot, count recent dive logs within DaysBack period
        // 4. Calculate popularity score using formula:
        //    PopularityScore = (AverageRating * 0.5) + (RatingCount * 0.3) + (RecentDiveLogsCount * 0.2)
        //    This weights: quality (50%), popularity (30%), recent activity (20%)
        // 5. Order spots by PopularityScore descending
        // 6. Take top Limit spots
        // 7. Map spots to PopularDivingSpotDto with popularity metrics
        // 8. Return result with popular spots and count
        var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysBack);

        var mockSpots = new List<PopularDivingSpotDto>
        {
            new PopularDivingSpotDto(
                Guid.NewGuid(),
                "Most Popular Spot",
                "A highly rated and frequently visited diving spot",
                43.2965,
                5.3698,
                30.0,
                2,
                4.8,
                50,
                25,
                4.65,
                22.0,
                15.0),
            new PopularDivingSpotDto(
                Guid.NewGuid(),
                "Second Popular Spot",
                "Another great diving location",
                43.3,
                5.4,
                25.0,
                1,
                4.5,
                30,
                15,
                4.15,
                21.0,
                18.0)
        };

        _logger.LogInformation(
            "Found {Count} popular diving spots",
            mockSpots.Count);

        return await Task.FromResult(new GetPopularSpotsResult(true, mockSpots, mockSpots.Count));
    }
}
