using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Handler for LoginCommand.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LoginCommandHandler(ILogger<LoginCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the LoginCommand.
    /// </summary>
    /// <param name="request">The login command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The login result with tokens.</returns>
    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Login attempt for email: {Email}",
            request.Email);

        // TODO: Implement actual login logic when repositories and auth service are ready
        // 1. Find user by email
        // 2. Verify password hash
        // 3. Generate JWT access token
        // 4. Generate refresh token
        // 5. Store refresh token
        // 6. Return tokens

        var userId = Guid.NewGuid();
        var accessToken = "temporary_access_token";
        var refreshToken = "temporary_refresh_token";
        var expiresIn = 3600; // 1 hour

        _logger.LogInformation(
            "User logged in successfully: {UserId}",
            userId);

        return await Task.FromResult(new LoginResult(
            userId,
            request.Email,
            accessToken,
            refreshToken,
            expiresIn));
    }
}
