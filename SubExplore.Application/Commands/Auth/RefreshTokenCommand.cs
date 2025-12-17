using MediatR;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Command to refresh an access token using a refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token.</param>
public record RefreshTokenCommand(
    string RefreshToken) : IRequest<RefreshTokenResult>;

/// <summary>
/// Result of token refresh.
/// </summary>
/// <param name="AccessToken">New JWT access token.</param>
/// <param name="RefreshToken">New refresh token.</param>
/// <param name="ExpiresIn">Access token expiration time in seconds.</param>
public record RefreshTokenResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
