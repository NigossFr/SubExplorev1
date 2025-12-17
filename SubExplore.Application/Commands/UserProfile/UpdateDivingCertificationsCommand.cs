using MediatR;

namespace SubExplore.Application.Commands.UserProfile;

/// <summary>
/// Command to update user's diving certifications.
/// </summary>
/// <param name="UserId">The user's ID.</param>
/// <param name="Certifications">List of certifications with organization and level.</param>
public record UpdateDivingCertificationsCommand(
    Guid UserId,
    List<CertificationDto> Certifications) : IRequest<UpdateDivingCertificationsResult>;

/// <summary>
/// Diving certification data transfer object.
/// </summary>
/// <param name="Organization">Certification organization (e.g., PADI, SSI, CMAS).</param>
/// <param name="Level">Certification level (e.g., Open Water, Advanced, Rescue, Divemaster).</param>
/// <param name="CertificationNumber">The certification number/ID.</param>
/// <param name="IssueDate">Date when the certification was issued.</param>
public record CertificationDto(
    string Organization,
    string Level,
    string? CertificationNumber,
    DateTime? IssueDate);

/// <summary>
/// Result of diving certifications update operation.
/// </summary>
/// <param name="Success">Indicates whether the update was successful.</param>
/// <param name="UserId">The user's ID.</param>
/// <param name="CertificationCount">Number of certifications updated.</param>
public record UpdateDivingCertificationsResult(
    bool Success,
    Guid UserId,
    int CertificationCount);
