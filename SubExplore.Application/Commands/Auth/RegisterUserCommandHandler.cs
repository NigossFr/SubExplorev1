using MediatR;
using Microsoft.Extensions.Logging;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Handler for RegisterUserCommand.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the RegisterUserCommand.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registration result.</returns>
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Registering new user with email: {Email}",
            request.Email);

        // TODO: Implement actual registration logic when repositories are ready
        // 1. Check if email/username already exists
        // 2. Hash password
        // 3. Create User entity
        // 4. Save to repository
        // 5. Publish UserRegisteredEvent
        // 6. Send welcome email

        var userId = Guid.NewGuid();

        _logger.LogInformation(
            "User registered successfully with ID: {UserId}",
            userId);

        return await Task.FromResult(new RegisterUserResult(
            userId,
            request.Email,
            request.Username));
    }
}
