using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SubExplore.Services;
using Serilog;
using Serilog.Events;

namespace SubExplore
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configuration des services
            RegisterServices(builder.Services);

            // Configuration Serilog
            ConfigureLogging(builder);

            return builder.Build();
        }

        /// <summary>
        /// Configure Serilog pour le logging de l'application mobile
        /// </summary>
        private static void ConfigureLogging(MauiAppBuilder builder)
        {
            var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "subexplore-mobile-.log");

#if DEBUG
            var logLevel = LogEventLevel.Debug;
#else
            var logLevel = LogEventLevel.Information;
#endif

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "SubExplore.Mobile")
                .WriteTo.Debug(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            builder.Logging.AddSerilog(Log.Logger, dispose: true);

            Log.Information("SubExplore Mobile application starting");
        }

        /// <summary>
        /// Enregistre tous les services dans le conteneur DI
        /// </summary>
        private static void RegisterServices(IServiceCollection services)
        {
            // Services d'infrastructure
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels
            // TODO: Ajouter les ViewModels au fur et à mesure de leur création
            // services.AddTransient<MainViewModel>();
        }
    }
}
