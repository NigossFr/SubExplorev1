using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Handler for LogoutCommand.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResult>
{
    private readonly ILogger<LogoutCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the LogoutCommand.
    /// </summary>
    /// <param name="request">The logout command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The logout result.</returns>
    public async Task<LogoutResult> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Logging out user: {UserId}",
            request.UserId);

        // TODO: Implement actual logout logic when auth service is ready
        // 1. Validate refresh token belongs to user
        // 2. Invalidate refresh token in database
        // 3. Optionally: Add access token to blacklist (if using token blacklisting)
        // 4. Log logout event

        _logger.LogInformation(
            "User logged out successfully: {UserId}",
            request.UserId);

        return await Task.FromResult(new LogoutResult(true));
    }
}
