using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SubExplore.API.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for SubExplore API integration tests.
/// </summary>
public class SubExploreWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures the web host for testing.
    /// </summary>
    /// <param name="builder">Web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test-specific configuration
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Environment"] = "Test",
                ["Logging:LogLevel:Default"] = "Warning",
                ["Logging:LogLevel:Microsoft"] = "Warning",
                ["Logging:LogLevel:System"] = "Warning",
                ["Logging:LogLevel:SubExplore"] = "Information"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Configuration for test services will go here
            // For now, we'll use the default services from the API
        });

        // Use test environment
        builder.UseEnvironment("Test");

        // Suppress Serilog bootstrap logger errors during tests
        builder.UseSetting("DetailedErrors", "true");
        builder.UseSetting("Environment", "Test");
    }
}
