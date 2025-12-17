using MediatR;

namespace SubExplore.Application.Commands.DiveLog;

/// <summary>
/// Command to delete a dive log.
/// </summary>
/// <param name="DiveLogId">The ID of the dive log to delete.</param>
/// <param name="UserId">The ID of the user requesting the deletion.</param>
public record DeleteDiveLogCommand(
    Guid DiveLogId,
    Guid UserId) : IRequest<DeleteDiveLogResult>;

/// <summary>
/// Result of deleting a dive log.
/// </summary>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="DiveLogId">The ID of the deleted dive log.</param>
public record DeleteDiveLogResult(
    bool Success,
    Guid DiveLogId);
