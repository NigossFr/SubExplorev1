using MediatR;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Command to authenticate a user and generate access tokens.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
public record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResult>;

/// <summary>
/// Result of user login.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="Email">The user's email.</param>
/// <param name="AccessToken">JWT access token.</param>
/// <param name="RefreshToken">Refresh token for obtaining new access tokens.</param>
/// <param name="ExpiresIn">Access token expiration time in seconds.</param>
public record LoginResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
