using FluentValidation.TestHelper;
using SubExplore.Application.Commands.UserProfile;
using Xunit;

namespace SubExplore.Application.UnitTests.Commands.UserProfile;

/// <summary>
/// Unit tests for UpdateDivingCertificationsCommandValidator.
/// </summary>
public class UpdateDivingCertificationsCommandValidatorTests
{
    private readonly UpdateDivingCertificationsCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDivingCertificationsCommandValidatorTests"/> class.
    /// </summary>
    public UpdateDivingCertificationsCommandValidatorTests()
    {
        _validator = new UpdateDivingCertificationsCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new UpdateDivingCertificationsCommand(
            Guid.Empty,
            new List<CertificationDto>());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_Certifications_Is_Null()
    {
        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Certifications)
            .WithErrorMessage("Certifications list cannot be null");
    }

    [Fact]
    public void Should_Have_Error_When_Certifications_Exceeds_Max_Count()
    {
        var certifications = Enumerable.Range(0, 21)
            .Select(_ => new CertificationDto("PADI", "Open Water", null, null))
            .ToList();

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Certifications)
            .WithErrorMessage("Cannot add more than 20 certifications");
    }

    [Fact]
    public void Should_Have_Error_When_Certification_Organization_Is_Empty()
    {
        var certifications = new List<CertificationDto>
        {
            new(string.Empty, "Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].Organization")
            .WithErrorMessage("Organization is required");
    }

    [Fact]
    public void Should_Have_Error_When_Certification_Organization_Exceeds_MaxLength()
    {
        var certifications = new List<CertificationDto>
        {
            new(new string('a', 51), "Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].Organization")
            .WithErrorMessage("Organization must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Certification_Level_Is_Empty()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", string.Empty, null, null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].Level")
            .WithErrorMessage("Certification level is required");
    }

    [Fact]
    public void Should_Have_Error_When_Certification_Level_Exceeds_MaxLength()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", new string('a', 101), null, null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].Level")
            .WithErrorMessage("Level must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_CertificationNumber_Exceeds_MaxLength()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", new string('a', 51), null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].CertificationNumber")
            .WithErrorMessage("Certification number must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_IssueDate_Is_In_Future()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", null, DateTime.UtcNow.AddDays(1)),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].IssueDate")
            .WithErrorMessage("Issue date cannot be in the future");
    }

    [Fact]
    public void Should_Have_Error_When_IssueDate_Is_Before_1950()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", null, new DateTime(1949, 12, 31)),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Certifications[0].IssueDate")
            .WithErrorMessage("Issue date must be after 1950");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var certifications = new List<CertificationDto>
        {
            new("PADI", "Open Water", "12345", new DateTime(2020, 1, 1)),
            new("SSI", "Advanced Open Water", null, null),
        };

        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            certifications);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Certifications_List_Is_Empty()
    {
        var command = new UpdateDivingCertificationsCommand(
            Guid.NewGuid(),
            new List<CertificationDto>());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
