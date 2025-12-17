using MediatR;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Command to log out a user and invalidate their tokens.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="RefreshToken">The refresh token to invalidate.</param>
public record LogoutCommand(
    Guid UserId,
    string RefreshToken) : IRequest<LogoutResult>;

/// <summary>
/// Result of user logout.
/// </summary>
/// <param name="Success">Indicates whether logout was successful.</param>
public record LogoutResult(bool Success);
