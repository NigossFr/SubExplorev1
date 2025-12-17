using MediatR;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Command to share a dive log with other users.
/// </summary>
/// <param name="DiveLogId">The ID of the dive log to share.</param>
/// <param name="UserId">The ID of the user sharing the dive log.</param>
/// <param name="SharedWithUserIds">The list of user IDs to share with.</param>
/// <param name="Message">Optional message to include with the share (optional).</param>
public record ShareDiveLogCommand(
    Guid DiveLogId,
    Guid UserId,
    List<Guid> SharedWithUserIds,
    string? Message) : IRequest<ShareDiveLogResult>;

/// <summary>
/// Result of sharing a dive log.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="DiveLogId">The ID of the shared dive log.</param>
/// <param name="SharedCount">The number of users the dive log was shared with.</param>
public record ShareDiveLogResult(
    bool Success,
    Guid DiveLogId,
    int SharedCount);
