# GUIDE D'IMPLÉMENTATION PRATIQUE - SUBEXPLORE
## Instructions pour Claude Code dans Visual Studio 2022

---

## TABLE DES MATIÈRES

1. [Setup Initial du Projet](#1-setup-initial-du-projet)
2. [Structure Détaillée de la Solution](#2-structure-détaillée-de-la-solution)
3. [Configuration Supabase](#3-configuration-supabase)
4. [Implémentation MVVM avec MAUI](#4-implémentation-mvvm-avec-maui)
5. [Patterns et Exemples de Code](#5-patterns-et-exemples-de-code)
6. [Gestion des États et Navigation](#6-gestion-des-états-et-navigation)
7. [Services Essentiels](#7-services-essentiels)
8. [Guidelines et Bonnes Pratiques](#8-guidelines-et-bonnes-pratiques)

---

## 1. SETUP INITIAL DU PROJET

### 1.1 Création de la Solution

```bash
# Créer la solution et les projets
dotnet new sln -n SubExplore
dotnet new maui -n SubExplore.Mobile -f net8.0
dotnet new classlib -n SubExplore.Core -f net8.0
dotnet new classlib -n SubExplore.Infrastructure -f net8.0
dotnet new classlib -n SubExplore.Shared -f net8.0

# Ajouter les projets à la solution
dotnet sln add SubExplore.Mobile/SubExplore.Mobile.csproj
dotnet sln add SubExplore.Core/SubExplore.Core.csproj
dotnet sln add SubExplore.Infrastructure/SubExplore.Infrastructure.csproj
dotnet sln add SubExplore.Shared/SubExplore.Shared.csproj

# Ajouter les références entre projets
dotnet add SubExplore.Mobile reference SubExplore.Core
dotnet add SubExplore.Mobile reference SubExplore.Infrastructure
dotnet add SubExplore.Core reference SubExplore.Shared
dotnet add SubExplore.Infrastructure reference SubExplore.Core
```

### 1.2 Packages NuGet Essentiels

```xml
<!-- SubExplore.Mobile.csproj -->
<ItemGroup>
  <!-- MVVM et Navigation -->
  <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
  <PackageReference Include="CommunityToolkit.Maui" Version="7.0.1" />
  
  <!-- Maps -->
  <PackageReference Include="Microsoft.Maui.Controls.Maps" Version="8.0.3" />
  
  <!-- HTTP et API -->
  <PackageReference Include="Refit" Version="7.0.0" />
  <PackageReference Include="Polly" Version="8.2.0" />
  
  <!-- Supabase -->
  <PackageReference Include="supabase-csharp" Version="0.15.0" />
  
  <!-- Images -->
  <PackageReference Include="FFImageLoading.Maui" Version="1.1.0" />
  
  <!-- Logging -->
  <PackageReference Include="Serilog.Extensions.Logging" Version="8.0.0" />
  <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
</ItemGroup>

<!-- SubExplore.Infrastructure.csproj -->
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
</ItemGroup>
```

---

## 2. STRUCTURE DÉTAILLÉE DE LA SOLUTION

### 2.1 SubExplore.Mobile (Projet MAUI)

```
SubExplore.Mobile/
├── Platforms/
│   ├── Android/
│   │   ├── MainActivity.cs
│   │   ├── MainApplication.cs
│   │   └── Services/
│   │       └── LocationService.cs
│   ├── iOS/
│   │   ├── AppDelegate.cs
│   │   ├── Program.cs
│   │   └── Services/
│   │       └── LocationService.cs
│   └── Windows/
│       └── App.xaml.cs
│
├── Views/
│   ├── Authentication/
│   │   ├── LoginPage.xaml
│   │   ├── RegisterPage.xaml
│   │   └── ForgotPasswordPage.xaml
│   ├── Main/
│   │   ├── MapPage.xaml
│   │   ├── HomePage.xaml
│   │   └── TabContainer.xaml
│   ├── Spots/
│   │   ├── SpotListPage.xaml
│   │   ├── SpotDetailPage.xaml
│   │   ├── SpotCreation/
│   │   │   ├── BasicInfoPage.xaml
│   │   │   ├── LocationPage.xaml
│   │   │   ├── CharacteristicsPage.xaml
│   │   │   ├── SafetyPage.xaml
│   │   │   └── PhotosPage.xaml
│   │   └── SpotValidationPage.xaml
│   ├── Community/
│   │   ├── BlogListPage.xaml
│   │   ├── ArticleDetailPage.xaml
│   │   └── CreateArticlePage.xaml
│   ├── BuddyFinder/
│   │   ├── BuddyProfilePage.xaml
│   │   ├── BuddySwipePage.xaml
│   │   └── MatchesPage.xaml
│   ├── Messaging/
│   │   ├── ConversationsPage.xaml
│   │   └── ChatPage.xaml
│   ├── Booking/
│   │   ├── StructureListPage.xaml
│   │   ├── StructureDetailPage.xaml
│   │   ├── BookingCalendarPage.xaml
│   │   └── BookingConfirmationPage.xaml
│   └── Profile/
│       ├── ProfilePage.xaml
│       ├── EditProfilePage.xaml
│       ├── SettingsPage.xaml
│       └── MyBookingsPage.xaml
│
├── ViewModels/
│   ├── Base/
│   │   ├── BaseViewModel.cs
│   │   └── ViewModelLocator.cs
│   ├── Authentication/
│   │   ├── LoginViewModel.cs
│   │   └── RegisterViewModel.cs
│   ├── Main/
│   │   ├── MapViewModel.cs
│   │   └── HomeViewModel.cs
│   ├── Spots/
│   │   ├── SpotListViewModel.cs
│   │   ├── SpotDetailViewModel.cs
│   │   └── SpotCreationViewModel.cs
│   └── [... autres ViewModels]
│
├── Controls/
│   ├── CustomMap.cs
│   ├── RatingControl.xaml
│   ├── LoadingIndicator.xaml
│   └── SwipeCard.xaml
│
├── Converters/
│   ├── BoolToVisibilityConverter.cs
│   ├── DistanceFormatter.cs
│   └── DateTimeConverter.cs
│
├── Resources/
│   ├── Styles/
│   │   ├── Colors.xaml
│   │   ├── Styles.xaml
│   │   └── Themes/
│   │       ├── LightTheme.xaml
│   │       └── DarkTheme.xaml
│   ├── Images/
│   ├── Fonts/
│   └── Strings/
│       ├── AppResources.resx
│       └── AppResources.fr.resx
│
├── AppShell.xaml
├── MauiProgram.cs
└── App.xaml
```

### 2.2 SubExplore.Core (Logique Métier)

```
SubExplore.Core/
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Spot.cs
│   │   ├── Structure.cs
│   │   ├── Shop.cs
│   │   ├── Booking.cs
│   │   ├── Review.cs
│   │   ├── Message.cs
│   │   └── BuddyProfile.cs
│   ├── Enums/
│   │   ├── AccountType.cs
│   │   ├── SpotValidationStatus.cs
│   │   ├── DifficultyLevel.cs
│   │   └── BookingStatus.cs
│   └── ValueObjects/
│       ├── Location.cs
│       ├── TimeSlot.cs
│       └── DateRange.cs
│
├── Interfaces/
│   ├── Services/
│   │   ├── IAuthenticationService.cs
│   │   ├── ISpotService.cs
│   │   ├── IBookingService.cs
│   │   ├── IMessagingService.cs
│   │   └── IBuddyMatchingService.cs
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── ISpotRepository.cs
│   │   ├── IUserRepository.cs
│   │   └── IBookingRepository.cs
│   └── External/
│       ├── IWeatherService.cs
│       ├── IMapService.cs
│       └── INotificationService.cs
│
├── Services/
│   ├── AuthenticationService.cs
│   ├── SpotService.cs
│   ├── BookingService.cs
│   ├── ValidationService.cs
│   ├── ModerationService.cs
│   └── BuddyMatchingService.cs
│
├── Validators/
│   ├── UserValidator.cs
│   ├── SpotValidator.cs
│   └── BookingValidator.cs
│
├── Specifications/
│   ├── SpotSpecifications.cs
│   └── UserSpecifications.cs
│
└── Extensions/
    ├── QueryableExtensions.cs
    └── EnumExtensions.cs
```

### 2.3 SubExplore.Infrastructure

```
SubExplore.Infrastructure/
├── Data/
│   ├── Configurations/
│   │   ├── UserConfiguration.cs
│   │   ├── SpotConfiguration.cs
│   │   └── [autres configurations]
│   ├── Migrations/
│   └── SubExploreDbContext.cs
│
├── Repositories/
│   ├── Base/
│   │   └── Repository.cs
│   ├── SpotRepository.cs
│   ├── UserRepository.cs
│   └── BookingRepository.cs
│
├── ApiClients/
│   ├── Supabase/
│   │   ├── SupabaseClient.cs
│   │   └── SupabaseConfiguration.cs
│   ├── Weather/
│   │   ├── OpenWeatherClient.cs
│   │   └── WeatherModels.cs
│   └── Maps/
│       └── MapsApiClient.cs
│
├── Services/
│   ├── CacheService.cs
│   ├── FileStorageService.cs
│   └── PushNotificationService.cs
│
└── Security/
    ├── TokenManager.cs
    └── EncryptionService.cs
```

---

## 3. CONFIGURATION SUPABASE

### 3.1 Setup Initial Supabase

```csharp
// appsettings.json
{
  "Supabase": {
    "Url": "https://YOUR_PROJECT.supabase.co",
    "AnonKey": "YOUR_ANON_KEY",
    "ServiceKey": "YOUR_SERVICE_KEY", // Ne jamais exposer côté client
    "JwtSecret": "YOUR_JWT_SECRET"
  },
  "ConnectionStrings": {
    "PostgreSQL": "Host=db.YOUR_PROJECT.supabase.co;Database=postgres;Username=postgres;Password=YOUR_PASSWORD"
  }
}

// SupabaseConfiguration.cs
public class SupabaseConfiguration
{
    public string Url { get; set; }
    public string AnonKey { get; set; }
    public string ServiceKey { get; set; }
    public string JwtSecret { get; set; }
}

// Program.cs ou MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .UseMauiMaps(); // Pour les cartes
        
        // Configuration Supabase
        var supabaseUrl = "YOUR_SUPABASE_URL";
        var supabaseKey = "YOUR_ANON_KEY";
        
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
            ShouldInitializeHeaders = true
        };
        
        builder.Services.AddSingleton(provider => 
            new Supabase.Client(supabaseUrl, supabaseKey, options));
        
        // Services DI
        ConfigureServices(builder.Services);
        
        return builder.Build();
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        // Services Core
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddTransient<ISpotService, SpotService>();
        services.AddTransient<IBookingService, BookingService>();
        
        // Repositories
        services.AddTransient<ISpotRepository, SpotRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        
        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MapViewModel>();
        services.AddTransient<SpotListViewModel>();
        
        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Cache
        services.AddMemoryCache();
        
        // HTTP Clients
        services.AddHttpClient<IWeatherService, OpenWeatherService>(client =>
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        });
    }
}
```

### 3.2 Initialisation Base de Données

```sql
-- Créer les tables et configurer RLS
-- Ce script doit être exécuté dans Supabase SQL Editor

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "postgis";

-- Create tables (voir cahier des charges pour structure complète)

-- Enable RLS
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.spots ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.messages ENABLE ROW LEVEL SECURITY;

-- Create policies
CREATE POLICY "Users can view their own profile" ON public.users
    FOR SELECT USING (auth.uid() = auth_id);

CREATE POLICY "Users can update their own profile" ON public.users
    FOR UPDATE USING (auth.uid() = auth_id);

CREATE POLICY "Public spots are viewable by everyone" ON public.spots
    FOR SELECT USING (is_active = true AND validation_status = 'Approved');

-- Create functions and triggers
CREATE OR REPLACE FUNCTION handle_new_user()
RETURNS trigger AS $$
BEGIN
  INSERT INTO public.users (auth_id, email, created_at)
  VALUES (new.id, new.email, now());
  RETURN new;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW EXECUTE FUNCTION handle_new_user();
```

---

## 4. IMPLÉMENTATION MVVM AVEC MAUI

### 4.1 BaseViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubExplore.Mobile.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    public bool IsNotBusy => !IsBusy;

    protected readonly INavigationService NavigationService;
    protected readonly IDialogService DialogService;
    protected readonly ILogger<BaseViewModel> Logger;

    protected BaseViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<BaseViewModel> logger)
    {
        NavigationService = navigationService;
        DialogService = dialogService;
        Logger = logger;
    }

    [RelayCommand]
    protected virtual async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            await OnInitializeAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during initialization");
            await DialogService.ShowAlertAsync("Erreur", 
                "Une erreur est survenue lors du chargement.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected virtual Task OnInitializeAsync() => Task.CompletedTask;

    [RelayCommand]
    protected async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync();
    }

    public virtual Task OnAppearingAsync() => Task.CompletedTask;
    public virtual Task OnDisappearingAsync() => Task.CompletedTask;
    
    protected async Task ExecuteAsync(Func<Task> operation, 
        string? loadingMessage = null)
    {
        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(loadingMessage))
                await DialogService.ShowLoadingAsync(loadingMessage);
                
            await operation();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Operation failed");
            await DialogService.ShowAlertAsync("Erreur", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            await DialogService.HideLoadingAsync();
        }
    }
}
```

### 4.2 Exemple de ViewModel Complet

```csharp
namespace SubExplore.Mobile.ViewModels.Spots;

public partial class SpotDetailViewModel : BaseViewModel
{
    private readonly ISpotService _spotService;
    private readonly IAuthenticationService _authService;
    private readonly IWeatherService _weatherService;
    private readonly IShareService _shareService;

    [ObservableProperty]
    private Spot? spot;

    [ObservableProperty]
    private WeatherInfo? currentWeather;

    [ObservableProperty]
    private bool isFavorite;

    [ObservableProperty]
    private bool canEdit;

    [ObservableProperty]
    private bool canValidate;

    public ObservableCollection<Review> Reviews { get; } = new();

    public Guid SpotId { get; set; }

    public SpotDetailViewModel(
        ISpotService spotService,
        IAuthenticationService authService,
        IWeatherService weatherService,
        IShareService shareService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<SpotDetailViewModel> logger)
        : base(navigationService, dialogService, logger)
    {
        _spotService = spotService;
        _authService = authService;
        _weatherService = weatherService;
        _shareService = shareService;
    }

    protected override async Task OnInitializeAsync()
    {
        await LoadSpotAsync();
        await LoadWeatherAsync();
        await LoadReviewsAsync();
        CheckPermissions();
    }

    private async Task LoadSpotAsync()
    {
        Spot = await _spotService.GetByIdAsync(SpotId);
        IsFavorite = await _spotService.IsFavoriteAsync(SpotId, 
            _authService.CurrentUserId!.Value);
    }

    private async Task LoadWeatherAsync()
    {
        if (Spot?.Location != null)
        {
            CurrentWeather = await _weatherService.GetCurrentWeatherAsync(
                Spot.Location.Latitude, 
                Spot.Location.Longitude);
        }
    }

    private async Task LoadReviewsAsync()
    {
        var reviews = await _spotService.GetReviewsAsync(SpotId);
        Reviews.Clear();
        foreach (var review in reviews.Take(5))
        {
            Reviews.Add(review);
        }
    }

    private void CheckPermissions()
    {
        var currentUser = _authService.CurrentUser;
        if (currentUser != null && Spot != null)
        {
            CanEdit = Spot.CreatorId == currentUser.Id && 
                     Spot.ValidationStatus == SpotValidationStatus.Pending;
                     
            CanValidate = currentUser.HasPermission(UserPermissions.ValidateSpots);
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (!_authService.IsAuthenticated)
        {
            await DialogService.ShowAlertAsync("Connexion requise", 
                "Vous devez être connecté pour ajouter des favoris.", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (IsFavorite)
            {
                await _spotService.RemoveFavoriteAsync(SpotId, 
                    _authService.CurrentUserId!.Value);
            }
            else
            {
                await _spotService.AddFavoriteAsync(SpotId, 
                    _authService.CurrentUserId!.Value);
            }
            
            IsFavorite = !IsFavorite;
        });
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        if (Spot == null) return;

        await _shareService.ShareTextAsync(
            $"Découvrez {Spot.Name} sur SubExplore",
            $"https://subexplore.app/spots/{Spot.Slug}");
    }

    [RelayCommand]
    private async Task NavigateToLocationAsync()
    {
        if (Spot?.Location == null) return;

        var location = new Location(Spot.Location.Latitude, Spot.Location.Longitude);
        var options = new MapLaunchOptions { Name = Spot.Name };

        await Map.Default.OpenAsync(location, options);
    }

    [RelayCommand]
    private async Task EditSpotAsync()
    {
        await NavigationService.NavigateToAsync(
            $"SpotEdit?spotId={SpotId}");
    }

    [RelayCommand]
    private async Task ValidateSpotAsync()
    {
        await NavigationService.NavigateToAsync(
            $"SpotValidation?spotId={SpotId}");
    }

    [RelayCommand]
    private async Task ShowAllReviewsAsync()
    {
        await NavigationService.NavigateToAsync(
            $"Reviews?entityType=spot&entityId={SpotId}");
    }

    [RelayCommand]
    private async Task CreateReviewAsync()
    {
        if (!_authService.IsAuthenticated)
        {
            await DialogService.ShowAlertAsync("Connexion requise", 
                "Vous devez être connecté pour laisser un avis.", "OK");
            return;
        }

        await NavigationService.NavigateToAsync(
            $"CreateReview?entityType=spot&entityId={SpotId}");
    }
}
```

### 4.3 Page XAML avec Binding

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SubExplore.Mobile.ViewModels.Spots"
             xmlns:controls="clr-namespace:SubExplore.Mobile.Controls"
             x:Class="SubExplore.Mobile.Views.Spots.SpotDetailPage"
             x:DataType="vm:SpotDetailViewModel"
             Title="{Binding Spot.Name}">

    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Partager" 
                     Command="{Binding ShareCommand}"
                     IconImageSource="share.png"/>
        <ToolbarItem Text="Modifier" 
                     Command="{Binding EditSpotCommand}"
                     IsVisible="{Binding CanEdit}"
                     IconImageSource="edit.png"/>
    </ContentPage.ToolbarItems>

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding InitializeCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="0" Spacing="0">
                
                <!-- Image de couverture -->
                <Grid HeightRequest="250">
                    <Image Source="{Binding Spot.CoverImageUrl}"
                           Aspect="AspectFill"/>
                    
                    <!-- Bouton Favori -->
                    <ImageButton Command="{Binding ToggleFavoriteCommand}"
                                HorizontalOptions="End"
                                VerticalOptions="Start"
                                Margin="20"
                                BackgroundColor="White"
                                CornerRadius="25"
                                HeightRequest="50"
                                WidthRequest="50">
                        <ImageButton.Source>
                            <FontImageSource FontFamily="MaterialIcons"
                                            Glyph="{Binding IsFavorite, 
                                                Converter={StaticResource BoolToFavoriteIcon}}"
                                            Color="{StaticResource Primary}"
                                            Size="24"/>
                        </ImageButton.Source>
                    </ImageButton>
                </Grid>

                <!-- Informations principales -->
                <VerticalStackLayout Padding="20" Spacing="15">
                    
                    <!-- Badges -->
                    <HorizontalStackLayout Spacing="10">
                        <Frame BackgroundColor="{Binding Spot.DifficultyLevel, 
                                               Converter={StaticResource DifficultyToColor}}"
                               Padding="10,5"
                               CornerRadius="15"
                               BorderColor="Transparent">
                            <Label Text="{Binding Spot.DifficultyLevel}"
                                   TextColor="White"
                                   FontSize="12"
                                   FontAttributes="Bold"/>
                        </Frame>
                        
                        <Frame BackgroundColor="{StaticResource Secondary}"
                               Padding="10,5"
                               CornerRadius="15"
                               BorderColor="Transparent">
                            <Label Text="{Binding Spot.SpotType}"
                                   TextColor="White"
                                   FontSize="12"/>
                        </Frame>
                    </HorizontalStackLayout>

                    <!-- Description -->
                    <Label Text="{Binding Spot.Description}"
                           FontSize="16"
                           LineHeight="1.5"/>

                    <!-- Caractéristiques -->
                    <Frame BackgroundColor="{StaticResource Gray50}"
                           Padding="15"
                           CornerRadius="10"
                           BorderColor="Transparent">
                        <Grid RowDefinitions="Auto,Auto,Auto"
                              ColumnDefinitions="*,*"
                              RowSpacing="10"
                              ColumnSpacing="10">
                            
                            <Label Grid.Row="0" Grid.Column="0">
                                <Label.FormattedText>
                                    <FormattedString>
                                        <Span Text="Profondeur: " 
                                              FontAttributes="Bold"/>
                                        <Span Text="{Binding Spot.MaxDepth, 
                                                    StringFormat='{0}m'}"/>
                                    </FormattedString>
                                </Label.FormattedText>
                            </Label>

                            <Label Grid.Row="0" Grid.Column="1">
                                <Label.FormattedText>
                                    <FormattedString>
                                        <Span Text="Visibilité: " 
                                              FontAttributes="Bold"/>
                                        <Span Text="{Binding Spot.VisibilityMin}"/>
                                        <Span Text="-"/>
                                        <Span Text="{Binding Spot.VisibilityMax}"/>
                                        <Span Text="m"/>
                                    </FormattedString>
                                </Label.FormattedText>
                            </Label>
                            
                            <!-- Autres caractéristiques -->
                        </Grid>
                    </Frame>

                    <!-- Météo actuelle -->
                    <Frame IsVisible="{Binding CurrentWeather, 
                                     Converter={StaticResource IsNotNullConverter}}"
                           BackgroundColor="{StaticResource Primary}"
                           Padding="15"
                           CornerRadius="10">
                        <Grid ColumnDefinitions="*,Auto">
                            <VerticalStackLayout>
                                <Label Text="Météo actuelle"
                                       TextColor="White"
                                       FontAttributes="Bold"/>
                                <Label TextColor="White">
                                    <Label.FormattedText>
                                        <FormattedString>
                                            <Span Text="{Binding CurrentWeather.Temperature}"/>
                                            <Span Text="°C • "/>
                                            <Span Text="{Binding CurrentWeather.Description}"/>
                                        </FormattedString>
                                    </Label.FormattedText>
                                </Label>
                            </VerticalStackLayout>
                            
                            <Image Grid.Column="1"
                                   Source="{Binding CurrentWeather.IconUrl}"
                                   WidthRequest="50"
                                   HeightRequest="50"/>
                        </Grid>
                    </Frame>

                    <!-- Notes de sécurité -->
                    <Frame BackgroundColor="{StaticResource Warning}"
                           Padding="15"
                           CornerRadius="10"
                           IsVisible="{Binding Spot.SafetyNotes, 
                                     Converter={StaticResource IsNotNullOrEmptyConverter}}">
                        <VerticalStackLayout Spacing="5">
                            <Label Text="⚠️ Sécurité"
                                   FontAttributes="Bold"
                                   TextColor="{StaticResource Gray900}"/>
                            <Label Text="{Binding Spot.SafetyNotes}"
                                   TextColor="{StaticResource Gray700}"/>
                        </VerticalStackLayout>
                    </Frame>

                    <!-- Avis -->
                    <VerticalStackLayout Spacing="10">
                        <Grid ColumnDefinitions="*,Auto">
                            <Label Text="Avis"
                                   FontSize="20"
                                   FontAttributes="Bold"/>
                            <Button Grid.Column="1"
                                    Text="Voir tout"
                                    Command="{Binding ShowAllReviewsCommand}"
                                    BackgroundColor="Transparent"
                                    TextColor="{StaticResource Primary}"/>
                        </Grid>

                        <controls:RatingControl Rating="{Binding Spot.AverageRating}"
                                              TotalReviews="{Binding Spot.TotalReviews}"
                                              IsReadOnly="True"/>

                        <CollectionView ItemsSource="{Binding Reviews}"
                                       VerticalOptions="Start">
                            <CollectionView.ItemTemplate>
                                <DataTemplate>
                                    <Frame Padding="10" 
                                           Margin="0,5"
                                           BackgroundColor="{StaticResource Gray50}">
                                        <!-- Template des avis -->
                                    </Frame>
                                </DataTemplate>
                            </CollectionView.ItemTemplate>
                        </CollectionView>

                        <Button Text="Laisser un avis"
                                Command="{Binding CreateReviewCommand}"
                                BackgroundColor="{StaticResource Primary}"/>
                    </VerticalStackLayout>

                </VerticalStackLayout>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>

    <!-- Boutons d'action flottants -->
    <Grid VerticalOptions="End"
          HorizontalOptions="End"
          Margin="20">
        <Button Text="📍 Y aller"
                Command="{Binding NavigateToLocationCommand}"
                BackgroundColor="{StaticResource Secondary}"
                CornerRadius="25"
                Padding="15"/>
    </Grid>

</ContentPage>
```

---

## 5. PATTERNS ET EXEMPLES DE CODE

### 5.1 Repository Pattern

```csharp
// IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

// Repository.cs - Implémentation de base
public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly Supabase.Client _supabaseClient;
    protected readonly ILogger<Repository<T>> _logger;

    protected Repository(Supabase.Client supabaseClient, ILogger<Repository<T>> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _supabaseClient
                .From<T>()
                .Where(x => x.Id == id)
                .Single();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id: {Id}", id);
            return null;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<T>()
            .Get();

        return response.Models;
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        var response = await _supabaseClient
            .From<T>()
            .Insert(entity);

        return response.Models.First();
    }

    // ... autres méthodes
}

// SpotRepository.cs - Implémentation spécifique
public class SpotRepository : Repository<Spot>, ISpotRepository
{
    public SpotRepository(Supabase.Client supabaseClient, 
        ILogger<SpotRepository> logger) 
        : base(supabaseClient, logger)
    {
    }

    public async Task<IEnumerable<Spot>> GetNearbyAsync(
        double latitude, 
        double longitude, 
        double radiusKm)
    {
        // Utilisation de PostGIS pour la recherche géospatiale
        var response = await _supabaseClient
            .Rpc("get_nearby_spots", new Dictionary<string, object>
            {
                { "lat", latitude },
                { "lng", longitude },
                { "radius_km", radiusKm }
            });

        return JsonSerializer.Deserialize<IEnumerable<Spot>>(response);
    }

    public async Task<IEnumerable<Spot>> GetByFiltersAsync(SpotFilters filters)
    {
        var query = _supabaseClient.From<Spot>();

        // Application des filtres
        if (filters.Activities?.Any() == true)
        {
            query = query.Filter("activities", Operator.Contains, filters.Activities);
        }

        if (!string.IsNullOrEmpty(filters.DifficultyLevel))
        {
            query = query.Where(x => x.DifficultyLevel == filters.DifficultyLevel);
        }

        if (filters.MinDepth.HasValue)
        {
            query = query.Where(x => x.MaxDepth >= filters.MinDepth.Value);
        }

        if (filters.MaxDepth.HasValue)
        {
            query = query.Where(x => x.MaxDepth <= filters.MaxDepth.Value);
        }

        // Tri et pagination
        query = query.Order(x => x.PopularityScore, Ordering.Descending)
                    .Range(filters.Page * filters.PageSize, 
                          (filters.Page + 1) * filters.PageSize - 1);

        var response = await query.Get();
        return response.Models;
    }

    public async Task<bool> AddFavoriteAsync(Guid spotId, Guid userId)
    {
        var favorite = new Favorite
        {
            UserId = userId,
            EntityType = "spot",
            EntityId = spotId
        };

        await _supabaseClient
            .From<Favorite>()
            .Insert(favorite);

        // Incrémenter le compteur de favoris
        await _supabaseClient
            .Rpc("increment_favorite_count", new Dictionary<string, object>
            {
                { "spot_id", spotId }
            });

        return true;
    }
}
```

### 5.2 Service Pattern avec Cache

```csharp
public class SpotService : ISpotService
{
    private readonly ISpotRepository _spotRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SpotService> _logger;

    public SpotService(
        ISpotRepository spotRepository,
        IMemoryCache cache,
        ILogger<SpotService> logger)
    {
        _spotRepository = spotRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Spot?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"spot_{id}";
        
        if (_cache.TryGetValue<Spot>(cacheKey, out var cached))
        {
            _logger.LogDebug("Spot {Id} retrieved from cache", id);
            return cached;
        }

        var spot = await _spotRepository.GetByIdAsync(id);
        
        if (spot != null)
        {
            _cache.Set(cacheKey, spot, TimeSpan.FromMinutes(5));
        }

        return spot;
    }

    public async Task<IEnumerable<Spot>> GetNearbyAsync(
        Location userLocation,
        double radiusKm)
    {
        var cacheKey = $"nearby_{userLocation.Latitude}_{userLocation.Longitude}_{radiusKm}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            
            var spots = await _spotRepository.GetNearbyAsync(
                userLocation.Latitude,
                userLocation.Longitude,
                radiusKm);
                
            // Enrichir avec la distance
            foreach (var spot in spots)
            {
                spot.Distance = CalculateDistance(userLocation, spot.Location);
            }
            
            return spots.OrderBy(s => s.Distance);
        });
    }

    public async Task<Spot> CreateSpotAsync(CreateSpotRequest request, Guid creatorId)
    {
        // Validation
        var validator = new SpotValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Mapping
        var spot = new Spot
        {
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            CreatorId = creatorId,
            ValidationStatus = SpotValidationStatus.Pending,
            CreatedAt = DateTime.UtcNow
            // ... autres propriétés
        };

        // Création
        var created = await _spotRepository.CreateAsync(spot);
        
        // Invalider le cache des spots nearby
        _cache.Remove($"nearby_*");
        
        // Notification pour validation
        await NotifyModeratorsAsync(created);
        
        return created;
    }

    private double CalculateDistance(Location from, Location to)
    {
        // Formule de Haversine pour calculer la distance
        const double R = 6371; // Rayon de la Terre en km
        
        var dLat = ToRad(to.Latitude - from.Latitude);
        var dLon = ToRad(to.Longitude - from.Longitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(from.Latitude)) * Math.Cos(ToRad(to.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return R * c;
    }
    
    private double ToRad(double degrees) => degrees * (Math.PI / 180);
}
```

---

## 6. GESTION DES ÉTATS ET NAVIGATION

### 6.1 Service de Navigation

```csharp
public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, Dictionary<string, object> parameters);
    Task GoBackAsync();
    Task GoToRootAsync();
    Task ShowModalAsync(string route);
    Task CloseModalAsync();
}

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task GoToRootAsync()
    {
        await Shell.Current.GoToAsync("//Main");
    }

    public async Task ShowModalAsync(string route)
    {
        await Shell.Current.GoToAsync(route, true);
    }

    public async Task CloseModalAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
```

### 6.2 Configuration AppShell

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell x:Class="SubExplore.Mobile.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:SubExplore.Mobile.Views"
       Shell.FlyoutBehavior="Disabled">

    <TabBar x:Name="MainTabBar">
        <Tab Title="Carte" Icon="map.png">
            <ShellContent ContentTemplate="{DataTemplate views:MapPage}"
                         Route="Map"/>
        </Tab>
        
        <Tab Title="Spots" Icon="location.png">
            <ShellContent ContentTemplate="{DataTemplate views:SpotListPage}"
                         Route="Spots"/>
        </Tab>
        
        <Tab Title="Communauté" Icon="community.png">
            <ShellContent ContentTemplate="{DataTemplate views:CommunityPage}"
                         Route="Community"/>
        </Tab>
        
        <Tab Title="Messages" Icon="message.png">
            <ShellContent ContentTemplate="{DataTemplate views:ConversationsPage}"
                         Route="Messages"/>
        </Tab>
        
        <Tab Title="Profil" Icon="profile.png">
            <ShellContent ContentTemplate="{DataTemplate views:ProfilePage}"
                         Route="Profile"/>
        </Tab>
    </TabBar>

</Shell>
```

```csharp
// AppShell.xaml.cs
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        // Authentication
        Routing.RegisterRoute("Login", typeof(LoginPage));
        Routing.RegisterRoute("Register", typeof(RegisterPage));
        
        // Spots
        Routing.RegisterRoute("SpotDetail", typeof(SpotDetailPage));
        Routing.RegisterRoute("SpotCreation", typeof(SpotCreationWizardPage));
        Routing.RegisterRoute("SpotValidation", typeof(SpotValidationPage));
        
        // Community
        Routing.RegisterRoute("ArticleDetail", typeof(ArticleDetailPage));
        Routing.RegisterRoute("CreateArticle", typeof(CreateArticlePage));
        
        // Buddy Finder (18+)
        Routing.RegisterRoute("BuddyFinder", typeof(BuddyFinderPage));
        Routing.RegisterRoute("BuddyProfile", typeof(BuddyProfilePage));
        
        // Booking
        Routing.RegisterRoute("StructureDetail", typeof(StructureDetailPage));
        Routing.RegisterRoute("BookingCalendar", typeof(BookingCalendarPage));
        
        // Messaging
        Routing.RegisterRoute("Chat", typeof(ChatPage));
    }
}
```

---

## 7. SERVICES ESSENTIELS

### 7.1 Service d'Authentification Complet

```csharp
public class AuthenticationService : IAuthenticationService
{
    private readonly Supabase.Client _supabaseClient;
    private readonly ISecureStorage _secureStorage;
    private readonly IUserRepository _userRepository;
    private User? _currentUser;
    private Timer? _refreshTimer;

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;
    public Guid? CurrentUserId => _currentUser?.Id;

    public event EventHandler<AuthStateChangedEventArgs>? AuthStateChanged;

    public AuthenticationService(
        Supabase.Client supabaseClient,
        IUserRepository userRepository)
    {
        _supabaseClient = supabaseClient;
        _userRepository = userRepository;
        _secureStorage = SecureStorage.Default;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignIn(email, password);
            
            if (session?.User != null)
            {
                // Stocker les tokens
                await _secureStorage.SetAsync("access_token", session.AccessToken);
                await _secureStorage.SetAsync("refresh_token", session.RefreshToken);
                
                // Récupérer le profil utilisateur complet
                _currentUser = await _userRepository.GetByAuthIdAsync(session.User.Id);
                
                // Démarrer le refresh automatique
                StartTokenRefreshTimer();
                
                // Notifier
                AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(true, _currentUser));
                
                return new AuthResult { Success = true, User = _currentUser };
            }

            return new AuthResult 
            { 
                Success = false, 
                ErrorMessage = "Identifiants invalides" 
            };
        }
        catch (Exception ex)
        {
            return new AuthResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message 
            };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Créer le compte Supabase Auth
            var session = await _supabaseClient.Auth.SignUp(
                request.Email, 
                request.Password);
            
            if (session?.User != null)
            {
                // Créer le profil utilisateur
                var user = new User
                {
                    AuthId = Guid.Parse(session.User.Id),
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Username = request.Username,
                    ExpertiseLevel = request.ExpertiseLevel,
                    CreatedAt = DateTime.UtcNow
                };
                
                _currentUser = await _userRepository.CreateAsync(user);
                
                return new AuthResult 
                { 
                    Success = true, 
                    User = _currentUser,
                    RequiresEmailVerification = true 
                };
            }

            return new AuthResult 
            { 
                Success = false, 
                ErrorMessage = "Erreur lors de la création du compte" 
            };
        }
        catch (Exception ex)
        {
            return new AuthResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message 
            };
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _secureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var session = await _supabaseClient.Auth.RefreshSession();
            
            if (session != null)
            {
                await _secureStorage.SetAsync("access_token", session.AccessToken);
                await _secureStorage.SetAsync("refresh_token", session.RefreshToken);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        // Arrêter le timer
        _refreshTimer?.Dispose();
        
        // Déconnexion Supabase
        await _supabaseClient.Auth.SignOut();
        
        // Nettoyer le stockage
        _secureStorage.Remove("access_token");
        _secureStorage.Remove("refresh_token");
        
        // Reset l'utilisateur
        _currentUser = null;
        
        // Notifier
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false, null));
    }

    public async Task InitializeAsync()
    {
        try
        {
            var accessToken = await _secureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Récupérer la session
                var user = _supabaseClient.Auth.CurrentUser;
                if (user != null)
                {
                    _currentUser = await _userRepository.GetByAuthIdAsync(
                        Guid.Parse(user.Id));
                    
                    StartTokenRefreshTimer();
                    
                    AuthStateChanged?.Invoke(this, 
                        new AuthStateChangedEventArgs(true, _currentUser));
                }
            }
        }
        catch
        {
            // Session invalide, nettoyer
            await LogoutAsync();
        }
    }

    private void StartTokenRefreshTimer()
    {
        _refreshTimer?.Dispose();
        _refreshTimer = new Timer(
            async _ => await RefreshTokenAsync(),
            null,
            TimeSpan.FromMinutes(55),
            TimeSpan.FromMinutes(55));
    }

    public bool HasPermission(UserPermissions permission)
    {
        if (_currentUser == null) return false;
        
        // Les admins ont toutes les permissions
        if (_currentUser.AccountType == AccountType.Administrator)
            return true;
        
        return (_currentUser.Permissions & permission) == permission;
    }
}
```

### 7.2 Service de Messagerie Temps Réel

```csharp
public class RealtimeMessagingService : IMessagingService, IDisposable
{
    private readonly Supabase.Client _supabaseClient;
    private readonly Dictionary<Guid, RealtimeChannel> _activeChannels;
    private readonly INotificationService _notificationService;

    public RealtimeMessagingService(
        Supabase.Client supabaseClient,
        INotificationService notificationService)
    {
        _supabaseClient = supabaseClient;
        _notificationService = notificationService;
        _activeChannels = new Dictionary<Guid, RealtimeChannel>();
    }

    public async Task<Conversation> CreateConversationAsync(
        ConversationType type, 
        Guid[] participants)
    {
        var conversation = new Conversation
        {
            ConversationType = type,
            Participants = participants,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _supabaseClient
            .From<Conversation>()
            .Insert(conversation);

        return result.Models.First();
    }

    public async Task SubscribeToConversationAsync(
        Guid conversationId, 
        Action<Message> onNewMessage)
    {
        if (_activeChannels.ContainsKey(conversationId))
            return;

        var channel = _supabaseClient.Realtime.Channel($"messages:{conversationId}");
        
        channel.On<Message>(
            RealtimeListenType.Insert,
            (sender, change) =>
            {
                var newMessage = change.Model;
                
                // Appeler le callback
                onNewMessage?.Invoke(newMessage);
                
                // Notification si l'app est en arrière-plan
                if (App.Current?.MainPage?.IsFocused == false)
                {
                    _notificationService.ShowLocalNotification(
                        "Nouveau message",
                        newMessage.Content,
                        conversationId.ToString());
                }
            });

        await channel.Subscribe();
        _activeChannels[conversationId] = channel;
    }

    public async Task<Message> SendMessageAsync(
        Guid conversationId, 
        string content, 
        Guid senderId)
    {
        // Validation anti-spam basique
        if (await IsSpammingAsync(senderId))
        {
            throw new InvalidOperationException(
                "Trop de messages envoyés. Veuillez patienter.");
        }

        // Filtrage contenu (mots interdits, etc.)
        content = SanitizeContent(content);

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            MessageType = MessageType.Text,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _supabaseClient
            .From<Message>()
            .Insert(message);

        // Mettre à jour la conversation
        await UpdateConversationLastMessage(conversationId, message);

        return result.Models.First();
    }

    public async Task UnsubscribeFromConversationAsync(Guid conversationId)
    {
        if (_activeChannels.TryGetValue(conversationId, out var channel))
        {
            await channel.Unsubscribe();
            _activeChannels.Remove(conversationId);
        }
    }

    private async Task<bool> IsSpammingAsync(Guid userId)
    {
        // Vérifier le nombre de messages dans la dernière minute
        var recentMessages = await _supabaseClient
            .From<Message>()
            .Where(m => m.SenderId == userId)
            .Where(m => m.CreatedAt > DateTime.UtcNow.AddMinutes(-1))
            .Get();

        return recentMessages.Models.Count > 10; // Max 10 messages/minute
    }

    private string SanitizeContent(string content)
    {
        // Supprimer les balises HTML
        content = Regex.Replace(content, "<.*?>", string.Empty);
        
        // Limiter la longueur
        if (content.Length > 1000)
            content = content.Substring(0, 1000);
        
        return content.Trim();
    }

    public void Dispose()
    {
        foreach (var channel in _activeChannels.Values)
        {
            channel.Unsubscribe().Wait();
        }
        _activeChannels.Clear();
    }
}
```

---

## 8. GUIDELINES ET BONNES PRATIQUES

### 8.1 Conventions de Nommage

```csharp
// Classes et Interfaces
public interface ISpotService { }         // Interface : préfixe I
public class SpotService { }              // Classe : PascalCase
public abstract class BaseViewModel { }   // Classe abstraite : préfixe Base

// Propriétés et Méthodes
public string Name { get; set; }          // Propriété : PascalCase
public async Task<Spot> GetSpotAsync() { } // Méthode async : suffixe Async

// Variables et Paramètres
private readonly ILogger _logger;         // Champ privé : underscore + camelCase
public void Process(string inputData) { } // Paramètre : camelCase

// Constantes et Enums
public const int MaxRetries = 3;          // Constante : PascalCase
public enum DifficultyLevel { }           // Enum : PascalCase
```

### 8.2 Gestion des Erreurs

```csharp
public class ErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;
    private readonly IDialogService _dialogService;

    public async Task<T> ExecuteSafelyAsync<T>(
        Func<Task<T>> operation,
        string errorMessage = "Une erreur est survenue")
    {
        try
        {
            return await operation();
        }
        catch (NetworkException ex)
        {
            _logger.LogWarning(ex, "Network error");
            await _dialogService.ShowAlertAsync(
                "Erreur réseau",
                "Vérifiez votre connexion internet.",
                "OK");
            return default(T);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await _dialogService.ShowAlertAsync(
                "Erreur de validation",
                ex.Message,
                "OK");
            return default(T);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await _dialogService.ShowAlertAsync(
                "Accès non autorisé",
                "Vous n'avez pas les permissions nécessaires.",
                "OK");
            return default(T);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            
            #if DEBUG
            await _dialogService.ShowAlertAsync(
                "Erreur",
                ex.ToString(),
                "OK");
            #else
            await _dialogService.ShowAlertAsync(
                "Erreur",
                errorMessage,
                "OK");
            #endif
            
            return default(T);
        }
    }
}
```

### 8.3 Validation des Données

```csharp
using FluentValidation;

public class SpotValidator : AbstractValidator<CreateSpotRequest>
{
    public SpotValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom est requis")
            .MinimumLength(3).WithMessage("Le nom doit faire au moins 3 caractères")
            .MaximumLength(200).WithMessage("Le nom ne peut dépasser 200 caractères");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La description est requise")
            .MinimumLength(50).WithMessage("La description doit faire au moins 50 caractères");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("La localisation est requise")
            .Must(BeValidCoordinates).WithMessage("Coordonnées invalides");

        RuleFor(x => x.MaxDepth)
            .InclusiveBetween(0, 200).WithMessage("La profondeur doit être entre 0 et 200m");

        RuleFor(x => x.Activities)
            .NotEmpty().WithMessage("Au moins une activité doit être sélectionnée");

        RuleFor(x => x.SafetyNotes)
            .NotEmpty().When(x => x.DifficultyLevel == DifficultyLevel.Expert)
            .WithMessage("Les notes de sécurité sont obligatoires pour les spots experts");

        RuleFor(x => x.Images)
            .Must(x => x == null || x.Count <= 10)
            .WithMessage("Maximum 10 photos autorisées");
    }

    private bool BeValidCoordinates(Location location)
    {
        if (location == null) return false;
        
        return location.Latitude >= -90 && location.Latitude <= 90 &&
               location.Longitude >= -180 && location.Longitude <= 180;
    }
}
```

### 8.4 Tests Unitaires

```csharp
using Xunit;
using Moq;

public class SpotServiceTests
{
    private readonly Mock<ISpotRepository> _mockRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly SpotService _service;

    public SpotServiceTests()
    {
        _mockRepository = new Mock<ISpotRepository>();
        _mockCache = new Mock<IMemoryCache>();
        var mockLogger = new Mock<ILogger<SpotService>>();
        
        _service = new SpotService(
            _mockRepository.Object,
            _mockCache.Object,
            mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSpotExists_ReturnsSpot()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        var expectedSpot = new Spot { Id = spotId, Name = "Test Spot" };
        
        _mockRepository
            .Setup(x => x.GetByIdAsync(spotId))
            .ReturnsAsync(expectedSpot);
        
        object cached;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cached))
            .Returns(false);

        // Act
        var result = await _service.GetByIdAsync(spotId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSpot.Id, result.Id);
        Assert.Equal(expectedSpot.Name, result.Name);
        
        _mockRepository.Verify(x => x.GetByIdAsync(spotId), Times.Once);
    }

    [Fact]
    public async Task CreateSpotAsync_WithInvalidData_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateSpotRequest
        {
            Name = "A", // Trop court
            Description = "Short" // Trop court
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateSpotAsync(request, Guid.NewGuid()));
    }
}
```

### 8.5 Configuration et Secrets

```json
// appsettings.json (NE JAMAIS commiter les vrais secrets)
{
  "Supabase": {
    "Url": "https://YOUR_PROJECT.supabase.co",
    "AnonKey": "YOUR_ANON_KEY"
  },
  "OpenWeatherMap": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/"
  },
  "AppSettings": {
    "Environment": "Development",
    "MaxPhotoSizeMB": 10,
    "MaxPhotosPerSpot": 10,
    "MinimumAge": 13,
    "BuddyFinderMinimumAge": 18
  }
}

// appsettings.Development.json (environnement dev)
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

// appsettings.Production.json (environnement prod)
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "System": "Error",
      "Microsoft": "Error"
    }
  }
}
```

```csharp
// Configuration dans MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // Charger la configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(
            "SubExplore.Mobile.appsettings.json");
        
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .AddEnvironmentVariables() // Pour override en prod
            .Build();
        
        builder.Configuration.AddConfiguration(config);
        
        // Bind configuration
        builder.Services.Configure<SupabaseConfiguration>(
            config.GetSection("Supabase"));
        builder.Services.Configure<AppSettings>(
            config.GetSection("AppSettings"));
        
        return builder.Build();
    }
}
```

### 8.6 Performance et Optimisation

```csharp
// Lazy Loading d'images
public class OptimizedImageService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public async Task<ImageSource> LoadImageAsync(string url, ImageSize size = ImageSize.Medium)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        // Générer l'URL optimisée
        var optimizedUrl = GetOptimizedImageUrl(url, size);
        
        // Vérifier le cache
        var cacheKey = $"img_{optimizedUrl}";
        if (_cache.TryGetValue<byte[]>(cacheKey, out var cached))
        {
            return ImageSource.FromStream(() => new MemoryStream(cached));
        }

        // Charger l'image
        var imageData = await _httpClient.GetByteArrayAsync(optimizedUrl);
        
        // Mettre en cache (max 50MB total)
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSize(imageData.Length)
            .SetSlidingExpiration(TimeSpan.FromHours(1));
            
        _cache.Set(cacheKey, imageData, cacheOptions);
        
        return ImageSource.FromStream(() => new MemoryStream(imageData));
    }

    private string GetOptimizedImageUrl(string originalUrl, ImageSize size)
    {
        // Utiliser un service de transformation d'image (Cloudinary, etc.)
        var width = size switch
        {
            ImageSize.Thumbnail => 150,
            ImageSize.Small => 300,
            ImageSize.Medium => 600,
            ImageSize.Large => 1200,
            _ => 600
        };

        return $"{originalUrl}?w={width}&q=80&f=auto";
    }
}

// Pagination et Virtualisation
public partial class SpotListViewModel : BaseViewModel
{
    private readonly ObservableCollection<Spot> _spots = new();
    private int _currentPage = 0;
    private bool _hasMoreItems = true;

    [RelayCommand]
    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy || !_hasMoreItems)
            return;

        IsBusy = true;

        try
        {
            var items = await _spotService.GetSpotsAsync(
                page: _currentPage,
                pageSize: 20);

            if (items.Count < 20)
                _hasMoreItems = false;

            foreach (var item in items)
            {
                _spots.Add(item);
            }

            _currentPage++;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

---

## NOTES IMPORTANTES POUR CLAUDE CODE

### ⚠️ Points d'Attention Critiques

1. **Sécurité**
   - JAMAIS exposer les clés API côté client
   - TOUJOURS valider les entrées utilisateur
   - Utiliser SecureStorage pour les tokens
   - Implémenter Rate Limiting sur les API

2. **Performance**
   - Utiliser la pagination pour les listes
   - Implémenter le cache pour les données fréquentes
   - Lazy loading pour les images
   - Éviter les requêtes N+1

3. **UX/UI**
   - Feedback immédiat sur les actions
   - États de chargement clairs
   - Gestion offline gracieuse
   - Messages d'erreur explicites

4. **Architecture**
   - Respecter MVVM strictement
   - Injection de dépendances systématique
   - Séparation des responsabilités
   - Tests unitaires pour la logique métier

5. **Supabase**
   - Activer RLS sur toutes les tables
   - Utiliser les policies pour la sécurité
   - Optimiser les requêtes avec des index
   - Surveiller les quotas d'utilisation

### 📋 Checklist de Démarrage

- [ ] Créer le projet Supabase
- [ ] Configurer les tables et RLS
- [ ] Créer la solution Visual Studio
- [ ] Installer tous les packages NuGet
- [ ] Configurer l'injection de dépendances
- [ ] Implémenter l'authentification
- [ ] Créer les pages principales
- [ ] Tester sur Android et iOS
- [ ] Configurer CI/CD
- [ ] Préparer le déploiement

### 🚀 Ordre de Développement Recommandé

1. **Base** : Auth, Navigation, Services de base
2. **Carte** : Affichage, Géolocalisation, Filtres
3. **Spots** : CRUD, Validation, Favoris
4. **Profils** : Gestion utilisateur, Préférences
5. **Communauté** : Articles, Reviews
6. **Messaging** : Chat temps réel
7. **Booking** : Calendrier, Réservations
8. **BuddyFinder** : Matching, Swipe
9. **Monétisation** : Pub, Abonnements

---

**Document créé le** : {{DATE}}
**Version** : 1.0
**Statut** : Guide d'implémentation pour Claude Code
**Mise à jour** : À maintenir pendant le développement

*Ce document doit être utilisé en conjonction avec le cahier des charges complet pour le développement de SubExplore.*