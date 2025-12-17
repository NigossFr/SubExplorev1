using MediatR;

namespace SubExplore.Application.Commands.Auth;

/// <summary>
/// Command to register a new user in the system.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
/// <param name="Username">User's username.</param>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
public record RegisterUserCommand(
    string Email,
    string Password,
    string Username,
    string FirstName,
    string LastName) : IRequest<RegisterUserResult>;

/// <summary>
/// Result of user registration.
/// </summary>
/// <param name="UserId">The ID of the newly created user.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Username">The user's username.</param>
public record RegisterUserResult(
    Guid UserId,
    string Email,
    string Username);
