using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SubExplore.Services;

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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
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
