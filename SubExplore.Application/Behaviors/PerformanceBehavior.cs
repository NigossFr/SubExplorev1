using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SubExplore.Application.Behaviors;

/// <summary>
/// Pipeline behavior that tracks and logs performance metrics for requests.
/// Logs a warning when requests exceed the configured threshold.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    /// <summary>
    /// Performance threshold in milliseconds. Requests exceeding this will be logged as warnings.
    /// </summary>
    private const int PerformanceThresholdMs = 500;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    /// <summary>
    /// Pipeline handler for tracking performance metrics.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > PerformanceThresholdMs)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning(
                "Long Running Request: {RequestName} ({ElapsedMilliseconds}ms) exceeded threshold of {ThresholdMs}ms",
                requestName,
                elapsedMilliseconds,
                PerformanceThresholdMs);
        }

        return response;
    }
}
