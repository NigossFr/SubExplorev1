using FluentAssertions;

namespace SubExplore.API.IntegrationTests;

/// <summary>
/// Integration tests for API setup verification.
/// </summary>
/// <remarks>
/// These are basic setup tests to verify the integration test infrastructure is configured correctly.
/// Full API integration tests will be added after completing the API implementation.
/// </remarks>
public class ApiSetupVerificationTests
{
    /// <summary>
    /// Verifies that the SubExploreWebApplicationFactory can be instantiated.
    /// </summary>
    [Fact]
    public void WebApplicationFactory_Should_Be_Instantiable()
    {
        // Act
        var factory = new SubExploreWebApplicationFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeAssignableTo<SubExploreWebApplicationFactory>();
    }

    /// <summary>
    /// Verifies that Microsoft.AspNetCore.Mvc.Testing package is available.
    /// </summary>
    [Fact]
    public void MvcTesting_Package_Should_Be_Available()
    {
        // Act
        var factoryType = typeof(Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<>);

        // Assert
        factoryType.Should().NotBeNull();
        factoryType.FullName.Should().Contain("Microsoft.AspNetCore.Mvc.Testing");
    }

    /// <summary>
    /// Verifies that FluentAssertions package is available.
    /// </summary>
    [Fact]
    public void FluentAssertions_Package_Should_Be_Available()
    {
        // Arrange
        var testValue = "test";

        // Act & Assert
        testValue.Should().Be("test");
        testValue.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that Testcontainers.PostgreSql package is available.
    /// </summary>
    [Fact]
    public void TestcontainersPostgreSql_Package_Should_Be_Available()
    {
        // Act
        var containerType = typeof(Testcontainers.PostgreSql.PostgreSqlContainer);

        // Assert
        containerType.Should().NotBeNull();
        containerType.FullName.Should().Contain("Testcontainers.PostgreSql");
    }
}
