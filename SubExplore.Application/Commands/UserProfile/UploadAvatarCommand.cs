using MediatR;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Command to upload a new avatar/profile picture for a user.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="FileName">The original file name.</param>
/// <param name="ContentType">The file content type (e.g., image/jpeg).</param>
/// <param name="FileData">The file data as byte array.</param>
public record UploadAvatarCommand(
    Guid UserId,
    string FileName,
    string ContentType,
    byte[] FileData) : IRequest<UploadAvatarResult>;

/// <summary>
/// Result of avatar upload operation.
/// </summary>
/// <param name="Success">Indicates whether the upload was successful.</param>
/// <param name="AvatarUrl">The URL of the uploaded avatar.</param>
public record UploadAvatarResult(
    bool Success,
    string AvatarUrl);
