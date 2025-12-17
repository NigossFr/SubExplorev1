using MediatR;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Command to update user profile information.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Bio">The user's bio/description.</param>
/// <param name="ProfilePictureUrl">URL to the user's profile picture.</param>
public record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Bio,
    string? ProfilePictureUrl) : IRequest<UpdateProfileResult>;

/// <summary>
/// Result of profile update operation.
/// </summary>
/// <param name="Success">Indicates whether the update was successful.</param>
/// <param name="UserId">The updated user's ID.</param>
public record UpdateProfileResult(
    bool Success,
    Guid UserId);
