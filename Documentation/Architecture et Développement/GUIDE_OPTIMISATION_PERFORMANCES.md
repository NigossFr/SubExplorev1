# GUIDE D'OPTIMISATION DES PERFORMANCES - SUBEXPLORE
## Stratégies et Techniques d'Optimisation Mobile et Backend

---

## TABLE DES MATIÈRES

1. [Métriques de Performance](#1-métriques-de-performance)
2. [Optimisation Mobile MAUI](#2-optimisation-mobile-maui)
3. [Optimisation Backend](#3-optimisation-backend)
4. [Optimisation Base de Données](#4-optimisation-base-de-données)
5. [Optimisation Réseau](#5-optimisation-réseau)
6. [Gestion de la Mémoire](#6-gestion-de-la-mémoire)
7. [Optimisation des Images](#7-optimisation-des-images)
8. [Caching Stratégique](#8-caching-stratégique)
9. [Monitoring et Profiling](#9-monitoring-et-profiling)
10. [Checklist d'Optimisation](#10-checklist-doptimisation)

---

## 1. MÉTRIQUES DE PERFORMANCE

### 1.1 Objectifs de Performance

```yaml
Performance Targets:
  Application Mobile:
    App Launch Time:
      Cold Start: < 3s
      Warm Start: < 1s
      Resume: < 500ms
    
    Navigation:
      Page Load: < 500ms
      List Scrolling: 60 FPS
      Map Rendering: < 2s
      
    Memory:
      Peak Usage: < 200MB
      Idle Usage: < 100MB
      
    Battery:
      Background Drain: < 1% per hour
      Active Use: < 10% per hour
      
  Backend API:
    Response Times:
      Simple Queries: < 100ms
      Complex Queries: < 500ms
      File Upload: < 5s per MB
      
    Throughput:
      Requests per Second: > 1000
      Concurrent Users: > 10000
      
    Availability:
      Uptime: > 99.9%
      Error Rate: < 0.1%
```

### 1.2 Métriques Clés

```csharp
public class PerformanceMetrics
{
    // Métriques Application
    public class AppMetrics
    {
        public TimeSpan ColdStartTime { get; set; }
        public TimeSpan WarmStartTime { get; set; }
        public TimeSpan PageLoadTime { get; set; }
        public double FrameRate { get; set; }
        public long MemoryUsage { get; set; }
        public double BatteryConsumption { get; set; }
        public int CrashCount { get; set; }
        public double ANRRate { get; set; } // Application Not Responding
    }
    
    // Métriques API
    public class ApiMetrics
    {
        public TimeSpan ResponseTime { get; set; }
        public int RequestsPerSecond { get; set; }
        public double ErrorRate { get; set; }
        public TimeSpan DatabaseQueryTime { get; set; }
        public long DataTransferred { get; set; }
        public int ActiveConnections { get; set; }
    }
    
    // Métriques Business
    public class BusinessMetrics
    {
        public TimeSpan TimeToFirstSpot { get; set; }
        public TimeSpan SpotCreationTime { get; set; }
        public TimeSpan SearchTime { get; set; }
        public double UserEngagement { get; set; }
        public double SessionDuration { get; set; }
    }
}
```

---

## 2. OPTIMISATION MOBILE MAUI

### 2.1 Optimisation du Démarrage

```csharp
// Startup optimization
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Charger uniquement les fonts essentielles au démarrage
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
            })
            .ConfigureServices(services =>
            {
                // Services essentiels uniquement
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IAuthService, AuthService>();
                
                // Services différés
                services.AddLazy<ISpotService, SpotService>();
                services.AddLazy<IWeatherService, WeatherService>();
            });

        return builder.Build();
    }
}

// App.xaml.cs - Optimisé
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    
    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        
        // Initialisation minimale
        InitializeComponent();
        
        // Démarrage asynchrone des services non critiques
        _ = Task.Run(InitializeBackgroundServicesAsync);
        
        // Page de démarrage légère
        MainPage = new SplashPage();
        
        // Navigation vers la page principale après init
        _ = NavigateToMainAsync();
    }
    
    private async Task InitializeBackgroundServicesAsync()
    {
        // Initialisation différée
        await Task.Delay(1000); // Laisser l'UI se charger
        
        var cacheService = _serviceProvider.GetService<ICacheService>();
        await cacheService.InitializeAsync();
        
        var syncService = _serviceProvider.GetService<ISyncService>();
        await syncService.StartBackgroundSyncAsync();
    }
    
    private async Task NavigateToMainAsync()
    {
        // Vérifier l'auth pendant le splash
        var authService = _serviceProvider.GetRequiredService<IAuthService>();
        var isAuthenticated = await authService.IsAuthenticatedAsync();
        
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            MainPage = isAuthenticated 
                ? new AppShell() 
                : new LoginPage();
        });
    }
}
```

### 2.2 Optimisation des Listes

```csharp
// CollectionView optimisé avec virtualisation
public class OptimizedSpotListPage : ContentPage
{
    private readonly SpotListViewModel _viewModel;
    
    public OptimizedSpotListPage()
    {
        _viewModel = new SpotListViewModel();
        BindingContext = _viewModel;
        
        var collectionView = new CollectionView
        {
            // Virtualisation activée par défaut
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 10
            },
            
            // Template de taille fixe pour performance
            ItemTemplate = new DataTemplate(() => CreateItemTemplate()),
            
            // Stratégie de conservation des items
            ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
            
            // Chargement incrémental
            RemainingItemsThreshold = 5,
            RemainingItemsThresholdReachedCommand = _viewModel.LoadMoreCommand
        };
        
        // Recyclage des vues
        collectionView.ItemsSource = _viewModel.Spots;
        
        Content = collectionView;
    }
    
    private View CreateItemTemplate()
    {
        // Template léger et optimisé
        var grid = new Grid
        {
            HeightRequest = 120, // Hauteur fixe pour performance
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 120 },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };
        
        // Image avec chargement différé
        var image = new CachedImage
        {
            Aspect = Aspect.AspectFill,
            DownsampleToViewSize = true,
            LoadingPlaceholder = "placeholder.png",
            CacheDuration = TimeSpan.FromDays(7)
        };
        image.SetBinding(CachedImage.SourceProperty, "ThumbnailUrl");
        
        // Contenu texte
        var contentStack = new StackLayout
        {
            Padding = 10,
            Children =
            {
                CreateLabel("Name", 16, FontAttributes.Bold),
                CreateLabel("Description", 14, FontAttributes.None, 2),
                CreateLabel("Distance", 12, FontAttributes.Italic)
            }
        };
        
        grid.Children.Add(image);
        Grid.SetColumn(image, 0);
        
        grid.Children.Add(contentStack);
        Grid.SetColumn(contentStack, 1);
        
        return grid;
    }
    
    private Label CreateLabel(string binding, double fontSize, 
        FontAttributes attributes, int maxLines = 1)
    {
        var label = new Label
        {
            FontSize = fontSize,
            FontAttributes = attributes,
            MaxLines = maxLines,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        label.SetBinding(Label.TextProperty, binding);
        return label;
    }
}

// ViewModel avec pagination
public partial class SpotListViewModel : BaseViewModel
{
    private const int PageSize = 20;
    private int _currentPage = 0;
    private bool _hasMoreItems = true;
    
    [ObservableProperty]
    private ObservableCollection<SpotItemViewModel> _spots = new();
    
    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy || !_hasMoreItems)
            return;
        
        IsBusy = true;
        
        try
        {
            var newSpots = await _spotService.GetSpotsAsync(
                _currentPage * PageSize, 
                PageSize);
            
            if (newSpots.Count < PageSize)
                _hasMoreItems = false;
            
            foreach (var spot in newSpots)
            {
                // Création de ViewModels légers
                Spots.Add(new SpotItemViewModel
                {
                    Id = spot.Id,
                    Name = spot.Name,
                    ThumbnailUrl = GetOptimizedImageUrl(spot.CoverImage, 240),
                    Description = TruncateText(spot.Description, 100),
                    Distance = CalculateDistance(spot.Location)
                });
            }
            
            _currentPage++;
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private string GetOptimizedImageUrl(string url, int width)
    {
        // Utiliser un service CDN avec redimensionnement
        return $"{url}?w={width}&q=75&fm=webp";
    }
}
```

### 2.3 Optimisation de la Carte

```csharp
public class OptimizedMapView : ContentView
{
    private readonly Map _map;
    private readonly ClusterManager _clusterManager;
    private CancellationTokenSource _updateCancellation;
    
    public OptimizedMapView()
    {
        _map = new Map
        {
            MapType = MapType.Street,
            IsShowingUser = true
        };
        
        _clusterManager = new ClusterManager(_map);
        
        // Debounce des mouvements de carte
        _map.PropertyChanged += OnMapPropertyChanged;
        
        Content = _map;
    }
    
    private async void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Map.VisibleRegion))
        {
            // Annuler la mise à jour précédente
            _updateCancellation?.Cancel();
            _updateCancellation = new CancellationTokenSource();
            
            try
            {
                // Debounce de 500ms
                await Task.Delay(500, _updateCancellation.Token);
                await UpdateVisibleSpotsAsync(_updateCancellation.Token);
            }
            catch (TaskCanceledException)
            {
                // Mise à jour annulée, normale
            }
        }
    }
    
    private async Task UpdateVisibleSpotsAsync(CancellationToken cancellationToken)
    {
        var visibleRegion = _map.VisibleRegion;
        
        if (visibleRegion == null)
            return;
        
        // Calculer le niveau de zoom
        var zoomLevel = CalculateZoomLevel(visibleRegion);
        
        // Stratégie selon le zoom
        if (zoomLevel < 10)
        {
            // Vue large : afficher les clusters
            await ShowClustersAsync(visibleRegion, cancellationToken);
        }
        else if (zoomLevel < 15)
        {
            // Vue moyenne : mix clusters et pins
            await ShowMixedViewAsync(visibleRegion, cancellationToken);
        }
        else
        {
            // Vue proche : tous les pins
            await ShowAllPinsAsync(visibleRegion, cancellationToken);
        }
    }
    
    private async Task ShowClustersAsync(MapSpan region, CancellationToken ct)
    {
        // Récupérer les spots de la région
        var spots = await GetSpotsInRegionAsync(region);
        
        if (ct.IsCancellationRequested)
            return;
        
        // Créer les clusters
        var clusters = _clusterManager.CreateClusters(spots, region);
        
        // Mettre à jour la carte
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _map.Pins.Clear();
            
            foreach (var cluster in clusters)
            {
                if (cluster.Count > 1)
                {
                    // Pin de cluster
                    _map.Pins.Add(new Pin
                    {
                        Label = $"{cluster.Count} spots",
                        Position = cluster.Center,
                        Type = PinType.Generic
                    });
                }
                else
                {
                    // Pin individuel
                    var spot = cluster.Spots.First();
                    _map.Pins.Add(CreateSpotPin(spot));
                }
            }
        });
    }
}

// Gestionnaire de clusters
public class ClusterManager
{
    private const double ClusterRadiusPixels = 50;
    
    public List<Cluster> CreateClusters(List<Spot> spots, MapSpan region)
    {
        var clusters = new List<Cluster>();
        var processedSpots = new HashSet<Guid>();
        
        // Convertir le rayon en degrés selon le zoom
        var clusterRadius = PixelsToLatitudeDegrees(
            ClusterRadiusPixels, 
            region);
        
        foreach (var spot in spots)
        {
            if (processedSpots.Contains(spot.Id))
                continue;
            
            // Créer un nouveau cluster
            var cluster = new Cluster
            {
                Center = spot.Location
            };
            
            // Trouver les spots proches
            foreach (var nearbySpot in spots)
            {
                if (processedSpots.Contains(nearbySpot.Id))
                    continue;
                
                var distance = CalculateDistance(
                    spot.Location, 
                    nearbySpot.Location);
                
                if (distance <= clusterRadius)
                {
                    cluster.Spots.Add(nearbySpot);
                    processedSpots.Add(nearbySpot.Id);
                }
            }
            
            clusters.Add(cluster);
        }
        
        return clusters;
    }
}
```

---

## 3. OPTIMISATION BACKEND

### 3.1 Optimisation des Requêtes API

```csharp
// API Controller optimisé
[ApiController]
[Route("api/spots")]
public class SpotController : ControllerBase
{
    private readonly ISpotService _spotService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SpotController> _logger;
    
    // Compression des réponses
    [HttpGet("nearby")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "lat", "lon", "radius" })]
    [EnableCompression]
    public async Task<IActionResult> GetNearbySpots(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] double radius = 50)
    {
        // Validation rapide
        if (!IsValidCoordinates(lat, lon))
            return BadRequest("Invalid coordinates");
        
        // Cache key basée sur une grille
        var cacheKey = GetGridCacheKey(lat, lon, radius);
        
        // Try cache first
        if (_cache.TryGetValue<List<SpotDto>>(cacheKey, out var cached))
        {
            return Ok(new
            {
                data = cached,
                cached = true,
                count = cached.Count
            });
        }
        
        // Query avec projection
        var spots = await _spotService.GetNearbyOptimizedAsync(
            lat, lon, radius);
        
        // Cache avec expiration
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(spots.Count); // Pour limite mémoire
        
        _cache.Set(cacheKey, spots, cacheOptions);
        
        return Ok(new
        {
            data = spots,
            cached = false,
            count = spots.Count
        });
    }
    
    // Batch API pour réduire les appels
    [HttpPost("batch")]
    public async Task<IActionResult> BatchOperations(
        [FromBody] BatchRequest request)
    {
        var results = new Dictionary<string, object>();
        
        // Exécution parallèle des opérations
        var tasks = request.Operations.Select(async op =>
        {
            return op.Type switch
            {
                "get" => await GetSpotAsync(op.Id),
                "nearby" => await GetNearbyAsync(op.Parameters),
                "search" => await SearchAsync(op.Query),
                _ => null
            };
        });
        
        var responses = await Task.WhenAll(tasks);
        
        for (int i = 0; i < request.Operations.Count; i++)
        {
            results[request.Operations[i].Key] = responses[i];
        }
        
        return Ok(results);
    }
}

// Service optimisé
public class OptimizedSpotService : ISpotService
{
    private readonly ISpotRepository _repository;
    private readonly IDbConnection _connection;
    
    public async Task<List<SpotDto>> GetNearbyOptimizedAsync(
        double lat, double lon, double radius)
    {
        // Requête SQL optimisée avec index spatial
        var sql = @"
            SELECT 
                s.id,
                s.name,
                s.description,
                ST_Y(s.location::geometry) as latitude,
                ST_X(s.location::geometry) as longitude,
                ST_Distance(s.location, ST_MakePoint(@lon, @lat)::geography) / 1000 as distance,
                s.difficulty_level,
                s.average_rating,
                s.cover_image_url
            FROM spots s
            WHERE 
                s.is_active = true
                AND s.validation_status = 'Approved'
                AND ST_DWithin(
                    s.location::geography,
                    ST_MakePoint(@lon, @lat)::geography,
                    @radius * 1000
                )
            ORDER BY distance
            LIMIT 100";
        
        using var connection = _connection;
        var spots = await connection.QueryAsync<SpotDto>(
            sql,
            new { lat, lon, radius });
        
        return spots.ToList();
    }
    
    // Projection pour réduire les données transférées
    public async Task<SpotDetailDto> GetSpotDetailAsync(Guid id)
    {
        // Utiliser les projections pour charger uniquement nécessaire
        var spot = await _repository.GetQuery()
            .Where(s => s.Id == id)
            .Select(s => new SpotDetailDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Location = new LocationDto
                {
                    Latitude = s.Location.Y,
                    Longitude = s.Location.X
                },
                Images = s.Images.Select(i => new ImageDto
                {
                    Url = i.Url,
                    Caption = i.Caption
                }).Take(10).ToList(),
                // Charger uniquement les derniers avis
                RecentReviews = s.Reviews
                    .Where(r => r.Status == "Approved")
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new ReviewDto
                    {
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ReviewerName = r.User.Username,
                        Date = r.CreatedAt
                    }).ToList()
            })
            .FirstOrDefaultAsync();
        
        return spot;
    }
}
```

### 3.2 Connection Pooling et Ressources

```csharp
// Configuration du pool de connexions
public class DatabaseConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // Configuration Npgsql optimisée
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "your-host",
            Database = "subexplore",
            Username = "user",
            Password = "password",
            
            // Pool de connexions
            Pooling = true,
            MinPoolSize = 5,
            MaxPoolSize = 100,
            ConnectionLifetime = 300, // 5 minutes
            ConnectionIdleLifetime = 60, // 1 minute
            
            // Timeouts
            CommandTimeout = 30,
            Timeout = 15,
            
            // Performance
            Multiplexing = true,
            MaxAutoPrepare = 20,
            AutoPrepareMinUsages = 3,
            
            // Keep Alive
            KeepAlive = 30,
            TcpKeepAlive = true
        }.ToString();
        
        services.AddDbContext<SubExploreContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.UseNetTopologySuite();
                npgsqlOptions.CommandTimeout(30);
            });
            
            // Optimisations EF Core
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
        });
        
        // Connection factory pour Dapper
        services.AddSingleton<IDbConnectionFactory>(sp =>
            new NpgsqlConnectionFactory(connectionString));
    }
}

// Factory de connexions avec retry
public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private readonly IPollyPolicy _retryPolicy;
    
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        
        _retryPolicy = Policy
            .Handle<NpgsqlException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    Log.Warning($"Retry {retry} after {timeSpan}s");
                });
    }
    
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        });
    }
}
```

---

## 4. OPTIMISATION BASE DE DONNÉES

### 4.1 Index Stratégiques

```sql
-- Index pour recherche géographique (PostGIS)
CREATE INDEX idx_spots_location_gist ON spots USING GIST(location);

-- Index pour recherche par distance avec condition
CREATE INDEX idx_spots_active_location ON spots USING GIST(location) 
WHERE is_active = true AND validation_status = 'Approved';

-- Index composite pour filtres fréquents
CREATE INDEX idx_spots_filter ON spots(difficulty_level, water_type, validation_status)
WHERE is_active = true;

-- Index pour tri par popularité
CREATE INDEX idx_spots_popularity ON spots(popularity_score DESC, average_rating DESC)
WHERE is_active = true AND validation_status = 'Approved';

-- Index partiel pour les favoris actifs
CREATE INDEX idx_favorites_user_active ON favorites(user_id, entity_type)
WHERE deleted_at IS NULL;

-- Index pour recherche textuelle
CREATE INDEX idx_spots_search_text ON spots 
USING GIN(to_tsvector('french', name || ' ' || description));

-- Index BRIN pour données temporelles (économie d'espace)
CREATE INDEX idx_audit_created_brin ON audit_logs USING BRIN(created_at);

-- Analyse et statistiques
ANALYZE spots;
ANALYZE users;
ANALYZE reviews;
```

### 4.2 Requêtes Optimisées

```sql
-- Vue matérialisée pour statistiques
CREATE MATERIALIZED VIEW mv_spot_statistics AS
SELECT 
    s.id,
    s.name,
    COUNT(DISTINCT r.id) as review_count,
    AVG(r.rating) as average_rating,
    COUNT(DISTINCT f.user_id) as favorite_count,
    COUNT(DISTINCT b.id) as booking_count,
    -- Score de popularité calculé
    (
        COALESCE(AVG(r.rating), 0) * 20 +
        LEAST(COUNT(DISTINCT r.id), 50) +
        LEAST(COUNT(DISTINCT f.user_id), 100) * 0.3 +
        CASE 
            WHEN s.created_at > NOW() - INTERVAL '7 days' THEN 20
            WHEN s.created_at > NOW() - INTERVAL '30 days' THEN 10
            ELSE 0
        END
    ) as popularity_score
FROM spots s
LEFT JOIN reviews r ON r.entity_id = s.id AND r.entity_type = 'spot'
LEFT JOIN favorites f ON f.entity_id = s.id AND f.entity_type = 'spot'
LEFT JOIN bookings b ON b.spot_id = s.id
WHERE s.is_active = true
GROUP BY s.id, s.name, s.created_at;

-- Index sur la vue matérialisée
CREATE INDEX idx_mv_spots_popularity ON mv_spot_statistics(popularity_score DESC);

-- Rafraîchissement programmé
CREATE OR REPLACE FUNCTION refresh_spot_statistics()
RETURNS void AS $$
BEGIN
    REFRESH MATERIALIZED VIEW CONCURRENTLY mv_spot_statistics;
END;
$$ LANGUAGE plpgsql;

-- Fonction de recherche optimisée
CREATE OR REPLACE FUNCTION search_spots_optimized(
    search_query TEXT,
    user_lat DOUBLE PRECISION,
    user_lon DOUBLE PRECISION,
    max_distance_km INTEGER DEFAULT 50,
    limit_count INTEGER DEFAULT 20
)
RETURNS TABLE (
    id UUID,
    name VARCHAR,
    description TEXT,
    distance_km DOUBLE PRECISION,
    rank REAL
) AS $$
BEGIN
    RETURN QUERY
    WITH ranked_spots AS (
        SELECT 
            s.id,
            s.name,
            s.description,
            ST_Distance(s.location::geography, 
                ST_MakePoint(user_lon, user_lat)::geography) / 1000 as dist_km,
            ts_rank_cd(
                to_tsvector('french', s.name || ' ' || s.description),
                plainto_tsquery('french', search_query)
            ) as text_rank
        FROM spots s
        WHERE 
            s.is_active = true
            AND s.validation_status = 'Approved'
            AND ST_DWithin(
                s.location::geography,
                ST_MakePoint(user_lon, user_lat)::geography,
                max_distance_km * 1000
            )
            AND to_tsvector('french', s.name || ' ' || s.description) @@
                plainto_tsquery('french', search_query)
    )
    SELECT 
        id,
        name,
        description,
        dist_km as distance_km,
        text_rank + (1.0 / (1.0 + dist_km)) as rank
    FROM ranked_spots
    ORDER BY rank DESC
    LIMIT limit_count;
END;
$$ LANGUAGE plpgsql;
```

### 4.3 Partitionnement des Tables

```sql
-- Partitionnement de la table d'audit par mois
CREATE TABLE audit_logs_partitioned (
    LIKE audit_logs INCLUDING ALL
) PARTITION BY RANGE (created_at);

-- Créer les partitions
CREATE TABLE audit_logs_2024_01 PARTITION OF audit_logs_partitioned
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
    
CREATE TABLE audit_logs_2024_02 PARTITION OF audit_logs_partitioned
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');

-- Fonction pour créer automatiquement les partitions
CREATE OR REPLACE FUNCTION create_monthly_partition()
RETURNS void AS $$
DECLARE
    partition_date DATE;
    partition_name TEXT;
    start_date DATE;
    end_date DATE;
BEGIN
    partition_date := DATE_TRUNC('month', CURRENT_DATE + INTERVAL '1 month');
    partition_name := 'audit_logs_' || TO_CHAR(partition_date, 'YYYY_MM');
    start_date := partition_date;
    end_date := partition_date + INTERVAL '1 month';
    
    -- Vérifier si la partition existe déjà
    IF NOT EXISTS (
        SELECT 1 FROM pg_tables 
        WHERE tablename = partition_name
    ) THEN
        EXECUTE format(
            'CREATE TABLE %I PARTITION OF audit_logs_partitioned FOR VALUES FROM (%L) TO (%L)',
            partition_name, start_date, end_date
        );
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Programmer la création automatique
CREATE EXTENSION IF NOT EXISTS pg_cron;
SELECT cron.schedule('create-partition', '0 0 25 * *', 'SELECT create_monthly_partition()');
```

---

## 5. OPTIMISATION RÉSEAU

### 5.1 Compression et Minification

```csharp
// Middleware de compression
public class CompressionMiddleware
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseResponseCompression();
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            
            // Types MIME à compresser
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "application/json",
                "application/xml",
                "text/json",
                "image/svg+xml"
            });
        });
        
        // Configuration Brotli (meilleure compression)
        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
        
        // Configuration Gzip (fallback)
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });
    }
}

// Client HTTP avec compression
public class OptimizedHttpClient
{
    private readonly HttpClient _httpClient;
    
    public OptimizedHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | 
                                    DecompressionMethods.Deflate |
                                    DecompressionMethods.Brotli
        };
        
        _httpClient = new HttpClient(handler)
        {
            DefaultRequestHeaders =
            {
                { "Accept-Encoding", "br, gzip, deflate" },
                { "Accept", "application/json" }
            }
        };
    }
}
```

### 5.2 Stratégie de Cache HTTP

```csharp
// Configuration cache HTTP
public class CacheConfiguration
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Cache en mémoire
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // MB
            options.CompactionPercentage = 0.25;
        });
        
        // Cache distribué Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "SubExplore";
        });
        
        // Cache de réponse HTTP
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1MB
            options.UseCaseSensitivePaths = false;
        });
        
        // Politique de cache personnalisée
        services.AddSingleton<IHttpCachePolicy, CustomCachePolicy>();
    }
}

// Politique de cache personnalisée
public class CustomCachePolicy : IHttpCachePolicy
{
    public CachePolicyResult GetPolicy(HttpContext context)
    {
        var path = context.Request.Path.Value.ToLower();
        
        // Images : cache long terme
        if (path.Contains("/images/") || path.Contains("/photos/"))
        {
            return new CachePolicyResult
            {
                CacheControl = "public, max-age=31536000", // 1 an
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            };
        }
        
        // API spots : cache moyen terme
        if (path.Contains("/api/spots"))
        {
            return new CachePolicyResult
            {
                CacheControl = "public, max-age=300", // 5 minutes
                Vary = "Accept-Encoding, Accept-Language"
            };
        }
        
        // Données utilisateur : pas de cache
        if (path.Contains("/api/user"))
        {
            return new CachePolicyResult
            {
                CacheControl = "no-cache, no-store, must-revalidate",
                Pragma = "no-cache"
            };
        }
        
        // Par défaut : cache court
        return new CachePolicyResult
        {
            CacheControl = "public, max-age=60" // 1 minute
        };
    }
}

// Utilisation dans les controllers
[ApiController]
public class SpotController : ControllerBase
{
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetSpot(Guid id)
    {
        // Ajouter ETag pour cache conditionnel
        var spot = await _spotService.GetByIdAsync(id);
        
        if (spot == null)
            return NotFound();
        
        var etag = GenerateETag(spot);
        
        // Vérifier If-None-Match
        if (Request.Headers.TryGetValue("If-None-Match", out var incomingEtag))
        {
            if (etag == incomingEtag)
            {
                return StatusCode(304); // Not Modified
            }
        }
        
        Response.Headers.Add("ETag", etag);
        return Ok(spot);
    }
    
    private string GenerateETag(object data)
    {
        var json = JsonSerializer.Serialize(data);
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hash);
    }
}
```

---

## 6. GESTION DE LA MÉMOIRE

### 6.1 Prévention des Fuites Mémoire

```csharp
// BaseViewModel avec gestion mémoire
public abstract class BaseViewModel : ObservableObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private bool _disposed;
    
    // Abonnements gérés
    protected void RegisterDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }
    
    // Weak event handlers
    protected void SubscribeWeakly<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Action<T> handler)
    {
        var weakHandler = new WeakPropertyChangedHandler<T>(handler);
        source.PropertyChanged += weakHandler.OnPropertyChanged;
        RegisterDisposable(weakHandler);
    }
    
    // Nettoyage
    public virtual void Cleanup()
    {
        // Annuler toutes les tâches
        foreach (var cts in _cancellationTokenSources)
        {
            cts.Cancel();
            cts.Dispose();
        }
        
        // Nettoyer les collections
        ClearCollections();
        
        // Libérer les event handlers
        UnsubscribeEvents();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Cleanup();
                _disposables.Dispose();
            }
            
            _disposed = true;
        }
    }
}

// Gestion des images en mémoire
public class ImageMemoryManager
{
    private readonly MemoryCache _imageCache;
    private readonly long _maxCacheSizeBytes = 50 * 1024 * 1024; // 50MB
    private long _currentCacheSize = 0;
    
    public async Task<ImageSource> GetOptimizedImageAsync(string url, Size targetSize)
    {
        var cacheKey = $"{url}_{targetSize.Width}x{targetSize.Height}";
        
        // Vérifier le cache
        if (_imageCache.TryGetValue<byte[]>(cacheKey, out var cached))
        {
            return ImageSource.FromStream(() => new MemoryStream(cached));
        }
        
        // Télécharger et optimiser
        var imageData = await DownloadImageAsync(url);
        var optimized = await ResizeImageAsync(imageData, targetSize);
        
        // Gérer la taille du cache
        if (_currentCacheSize + optimized.Length > _maxCacheSizeBytes)
        {
            await EvictOldestEntriesAsync();
        }
        
        // Ajouter au cache avec priorité
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSize(optimized.Length)
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .RegisterPostEvictionCallback(OnImageEvicted);
        
        _imageCache.Set(cacheKey, optimized, cacheOptions);
        _currentCacheSize += optimized.Length;
        
        return ImageSource.FromStream(() => new MemoryStream(optimized));
    }
    
    private void OnImageEvicted(object key, object value, 
        EvictionReason reason, object state)
    {
        if (value is byte[] imageData)
        {
            _currentCacheSize -= imageData.Length;
        }
    }
    
    private async Task<byte[]> ResizeImageAsync(byte[] imageData, Size targetSize)
    {
        using var input = new MemoryStream(imageData);
        using var bitmap = SKBitmap.Decode(input);
        
        // Calculer la taille optimale
        var scale = Math.Min(
            targetSize.Width / bitmap.Width,
            targetSize.Height / bitmap.Height);
        
        var scaledSize = new SKSizeI(
            (int)(bitmap.Width * scale),
            (int)(bitmap.Height * scale));
        
        // Redimensionner
        using var scaledBitmap = bitmap.Resize(scaledSize, SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(scaledBitmap);
        
        // Encoder en JPEG avec qualité réduite
        return image.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
    }
}
```

### 6.2 Profiling et Monitoring Mémoire

```csharp
public class MemoryMonitor
{
    private readonly ILogger<MemoryMonitor> _logger;
    private Timer _monitorTimer;
    
    public void StartMonitoring()
    {
        _monitorTimer = new Timer(CheckMemory, null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }
    
    private void CheckMemory(object state)
    {
        var info = GC.GetMemoryInfo();
        var allocated = GC.GetTotalMemory(false);
        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);
        
        _logger.LogInformation(
            "Memory: {Allocated}MB, Heap: {Heap}MB, GC: {Gen0}/{Gen1}/{Gen2}",
            allocated / (1024 * 1024),
            info.HeapSizeBytes / (1024 * 1024),
            gen0, gen1, gen2);
        
        // Alerte si utilisation excessive
        if (allocated > 200 * 1024 * 1024) // 200MB
        {
            _logger.LogWarning("High memory usage detected: {MB}MB", 
                allocated / (1024 * 1024));
            
            // Forcer un GC si critique
            if (allocated > 300 * 1024 * 1024)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
```

---

## 7. OPTIMISATION DES IMAGES

### 7.1 Service d'Optimisation d'Images

```csharp
public class ImageOptimizationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    // Stratégie par type de dispositif
    public string GetOptimizedUrl(string originalUrl, DeviceType deviceType)
    {
        var parameters = deviceType switch
        {
            DeviceType.Phone => new ImageParameters { Width = 360, Quality = 75 },
            DeviceType.Tablet => new ImageParameters { Width = 768, Quality = 80 },
            DeviceType.Desktop => new ImageParameters { Width = 1920, Quality = 85 },
            _ => new ImageParameters { Width = 360, Quality = 75 }
        };
        
        // Utiliser un CDN avec transformation d'image
        return BuildCdnUrl(originalUrl, parameters);
    }
    
    private string BuildCdnUrl(string url, ImageParameters parameters)
    {
        // Exemple avec Cloudinary ou ImageKit
        var baseUrl = _configuration["CDN:BaseUrl"];
        var transformation = $"w_{parameters.Width},q_{parameters.Quality},f_auto";
        
        return $"{baseUrl}/{transformation}/{url}";
    }
    
    // Upload optimisé
    public async Task<string> UploadAndOptimizeAsync(Stream imageStream, string fileName)
    {
        // Valider le format
        var format = DetectImageFormat(imageStream);
        if (!IsFormatSupported(format))
        {
            throw new NotSupportedException($"Format {format} not supported");
        }
        
        // Redimensionner si nécessaire
        var optimized = await OptimizeImageAsync(imageStream);
        
        // Générer les variantes
        var variants = await GenerateVariantsAsync(optimized);
        
        // Upload vers stockage
        var urls = new Dictionary<string, string>();
        foreach (var variant in variants)
        {
            var variantUrl = await UploadToStorageAsync(
                variant.Stream, 
                $"{fileName}_{variant.Size}");
            urls[variant.Size] = variantUrl;
        }
        
        return urls["original"];
    }
    
    private async Task<Stream> OptimizeImageAsync(Stream input)
    {
        using var original = SKBitmap.Decode(input);
        
        // Limiter la taille maximale
        const int maxDimension = 2048;
        var needsResize = original.Width > maxDimension || 
                         original.Height > maxDimension;
        
        SKBitmap processed = original;
        
        if (needsResize)
        {
            var scale = Math.Min(
                (float)maxDimension / original.Width,
                (float)maxDimension / original.Height);
            
            var newSize = new SKSizeI(
                (int)(original.Width * scale),
                (int)(original.Height * scale));
            
            processed = original.Resize(newSize, SKFilterQuality.High);
        }
        
        // Convertir en JPEG optimisé
        using var image = SKImage.FromBitmap(processed);
        var encoded = image.Encode(SKEncodedImageFormat.Jpeg, 85);
        
        return new MemoryStream(encoded.ToArray());
    }
}

// Chargement progressif d'images
public class ProgressiveImageLoader : Image
{
    public static readonly BindableProperty ThumbnailSourceProperty =
        BindableProperty.Create(nameof(ThumbnailSource), typeof(ImageSource), 
            typeof(ProgressiveImageLoader));
    
    public ImageSource ThumbnailSource
    {
        get => (ImageSource)GetValue(ThumbnailSourceProperty);
        set => SetValue(ThumbnailSourceProperty, value);
    }
    
    protected override async void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        
        if (propertyName == SourceProperty.PropertyName)
        {
            await LoadProgressivelyAsync();
        }
    }
    
    private async Task LoadProgressivelyAsync()
    {
        // Charger d'abord le thumbnail
        if (ThumbnailSource != null)
        {
            base.Source = ThumbnailSource;
            Opacity = 0.8;
        }
        
        // Charger l'image complète en arrière-plan
        if (Source != null)
        {
            await Task.Delay(100); // Laisser le thumbnail s'afficher
            
            // Précharger
            var imageSource = Source;
            await imageSource.PreloadAsync();
            
            // Transition douce
            await this.FadeTo(0.5, 100);
            base.Source = imageSource;
            await this.FadeTo(1, 200);
        }
    }
}
```

---

## 8. CACHING STRATÉGIQUE

### 8.1 Cache Multi-Niveaux

```csharp
public class MultiLevelCache : ICache
{
    private readonly IMemoryCache _l1Cache; // Niveau 1 : Mémoire
    private readonly IDistributedCache _l2Cache; // Niveau 2 : Redis
    private readonly ILogger<MultiLevelCache> _logger;
    
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        CacheOptions options = null) where T : class
    {
        options ??= CacheOptions.Default;
        
        // Niveau 1 : Mémoire
        if (_l1Cache.TryGetValue<T>(key, out var l1Value))
        {
            _logger.LogDebug("L1 Cache hit: {Key}", key);
            return l1Value;
        }
        
        // Niveau 2 : Redis
        var l2Value = await GetFromL2Async<T>(key);
        if (l2Value != null)
        {
            _logger.LogDebug("L2 Cache hit: {Key}", key);
            
            // Promouvoir vers L1
            SetL1(key, l2Value, options);
            return l2Value;
        }
        
        // Miss : Créer la valeur
        _logger.LogDebug("Cache miss: {Key}", key);
        
        // Utiliser un lock distribué pour éviter le stampeding
        using var lockHandle = await AcquireDistributedLockAsync(key);
        
        // Double-check après le lock
        l2Value = await GetFromL2Async<T>(key);
        if (l2Value != null)
        {
            SetL1(key, l2Value, options);
            return l2Value;
        }
        
        // Créer la valeur
        var value = await factory();
        
        // Stocker dans les deux niveaux
        await SetL2Async(key, value, options);
        SetL1(key, value, options);
        
        return value;
    }
    
    private void SetL1<T>(string key, T value, CacheOptions options)
    {
        var entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.L1Duration,
            SlidingExpiration = options.L1SlidingDuration,
            Priority = options.Priority
        };
        
        if (options.Size.HasValue)
        {
            entryOptions.SetSize(options.Size.Value);
        }
        
        _l1Cache.Set(key, value, entryOptions);
    }
    
    private async Task SetL2Async<T>(string key, T value, CacheOptions options)
    {
        var json = JsonSerializer.Serialize(value);
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.L2Duration,
            SlidingExpiration = options.L2SlidingDuration
        };
        
        await _l2Cache.SetStringAsync(key, json, entryOptions);
    }
    
    public async Task InvalidateAsync(string pattern)
    {
        // Invalider L1
        if (pattern.Contains("*"))
        {
            // Pattern matching pour L1
            var regex = new Regex(pattern.Replace("*", ".*"));
            // Note: MemoryCache ne supporte pas nativement le pattern matching
            // Utiliser une liste de clés trackées
            foreach (var key in _trackedKeys.Where(k => regex.IsMatch(k)))
            {
                _l1Cache.Remove(key);
            }
        }
        else
        {
            _l1Cache.Remove(pattern);
        }
        
        // Invalider L2 (Redis supporte les patterns)
        await InvalidateL2PatternAsync(pattern);
    }
}

// Options de cache
public class CacheOptions
{
    public TimeSpan L1Duration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan L2Duration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan? L1SlidingDuration { get; set; }
    public TimeSpan? L2SlidingDuration { get; set; }
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
    public int? Size { get; set; }
    
    public static CacheOptions Default => new();
    
    public static CacheOptions Short => new()
    {
        L1Duration = TimeSpan.FromMinutes(1),
        L2Duration = TimeSpan.FromMinutes(5)
    };
    
    public static CacheOptions Long => new()
    {
        L1Duration = TimeSpan.FromHours(1),
        L2Duration = TimeSpan.FromHours(24)
    };
}
```

### 8.2 Cache Prédictif

```csharp
public class PredictiveCacheService
{
    private readonly ICache _cache;
    private readonly ISpotService _spotService;
    private readonly IUserBehaviorAnalyzer _behaviorAnalyzer;
    
    public async Task PreloadPredictiveDataAsync(Guid userId, Location currentLocation)
    {
        // Analyser le comportement utilisateur
        var predictions = await _behaviorAnalyzer.PredictNextActionsAsync(userId);
        
        // Précharger les données probables
        var tasks = new List<Task>();
        
        foreach (var prediction in predictions.Where(p => p.Probability > 0.7))
        {
            switch (prediction.ActionType)
            {
                case "ViewSpotDetails":
                    // Précharger les spots populaires proches
                    tasks.Add(PreloadNearbyPopularSpotsAsync(currentLocation));
                    break;
                    
                case "SearchByActivity":
                    // Précharger les résultats pour ses activités favorites
                    tasks.Add(PreloadActivitySpotsAsync(userId));
                    break;
                    
                case "ViewBookings":
                    // Précharger ses réservations
                    tasks.Add(PreloadUserBookingsAsync(userId));
                    break;
            }
        }
        
        await Task.WhenAll(tasks);
    }
    
    private async Task PreloadNearbyPopularSpotsAsync(Location location)
    {
        var cacheKey = $"popular_spots_{location.GridKey}";
        
        await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            return await _spotService.GetPopularNearbyAsync(location, 10);
        }, CacheOptions.Long);
    }
}
```

---

## 9. MONITORING ET PROFILING

### 9.1 Telemetry et Métriques

```csharp
public class PerformanceTelemetry
{
    private readonly TelemetryClient _telemetryClient;
    
    public IDisposable TrackOperation(string operationName)
    {
        return new OperationTimer(operationName, _telemetryClient);
    }
    
    public void TrackMetric(string name, double value, 
        Dictionary<string, string> properties = null)
    {
        _telemetryClient.TrackMetric(name, value, properties);
    }
    
    // Timer automatique
    private class OperationTimer : IDisposable
    {
        private readonly string _operationName;
        private readonly TelemetryClient _client;
        private readonly Stopwatch _stopwatch;
        private readonly IOperationHolder<RequestTelemetry> _operation;
        
        public OperationTimer(string operationName, TelemetryClient client)
        {
            _operationName = operationName;
            _client = client;
            _stopwatch = Stopwatch.StartNew();
            _operation = _client.StartOperation<RequestTelemetry>(operationName);
        }
        
        public void Dispose()
        {
            _stopwatch.Stop();
            _operation.Telemetry.Duration = _stopwatch.Elapsed;
            _operation.Telemetry.Success = true;
            
            // Métriques supplémentaires
            _client.TrackMetric($"{_operationName}.Duration", 
                _stopwatch.ElapsedMilliseconds);
            
            // Alerte si trop lent
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                _client.TrackEvent("SlowOperation", new Dictionary<string, string>
                {
                    ["Operation"] = _operationName,
                    ["Duration"] = _stopwatch.ElapsedMilliseconds.ToString()
                });
            }
            
            _operation.Dispose();
        }
    }
}

// Usage
public async Task<List<Spot>> GetSpotsAsync()
{
    using (_telemetry.TrackOperation("GetSpots"))
    {
        return await _repository.GetAllAsync();
    }
}
```

### 9.2 Dashboard de Performance

```csharp
public class PerformanceDashboard
{
    public async Task<PerformanceReport> GenerateReportAsync(DateRange period)
    {
        return new PerformanceReport
        {
            Period = period,
            
            ApplicationMetrics = new AppPerformanceMetrics
            {
                AverageColdStart = await GetAverageMetricAsync("ColdStart", period),
                AveragePageLoad = await GetAverageMetricAsync("PageLoad", period),
                CrashRate = await GetCrashRateAsync(period),
                ANRRate = await GetANRRateAsync(period),
                MemoryUsageP95 = await GetPercentileMetricAsync("MemoryUsage", 95, period)
            },
            
            ApiMetrics = new ApiPerformanceMetrics
            {
                AverageResponseTime = await GetAverageMetricAsync("ResponseTime", period),
                P95ResponseTime = await GetPercentileMetricAsync("ResponseTime", 95, period),
                P99ResponseTime = await GetPercentileMetricAsync("ResponseTime", 99, period),
                RequestsPerSecond = await GetAverageMetricAsync("RPS", period),
                ErrorRate = await GetErrorRateAsync(period)
            },
            
            DatabaseMetrics = new DatabasePerformanceMetrics
            {
                AverageQueryTime = await GetAverageMetricAsync("QueryTime", period),
                SlowQueries = await GetSlowQueriesAsync(period),
                ConnectionPoolUsage = await GetConnectionPoolMetricsAsync(period),
                CacheHitRate = await GetCacheHitRateAsync(period)
            },
            
            UserExperienceMetrics = new UXMetrics
            {
                AverageSessionDuration = await GetAverageMetricAsync("SessionDuration", period),
                BounceRate = await GetBounceRateAsync(period),
                FeatureUsage = await GetFeatureUsageAsync(period),
                UserSatisfaction = await GetUserSatisfactionAsync(period)
            }
        };
    }
}
```

---

## 10. CHECKLIST D'OPTIMISATION

### 10.1 Checklist Mobile

```yaml
Mobile Performance Checklist:
  Startup:
    ✅ Lazy loading des services non critiques
    ✅ Splash screen optimisé
    ✅ Précompilation AOT activée
    ✅ Linking SDK only ou Link All
    
  UI Performance:
    ✅ Virtualisation des listes
    ✅ Templates de taille fixe
    ✅ Images optimisées et cachées
    ✅ Animations GPU accélérées
    
  Memory:
    ✅ Dispose pattern implémenté
    ✅ Event handlers désenregistrés
    ✅ Collections vidées
    ✅ Images recyclées
    
  Network:
    ✅ Compression activée
    ✅ Cache HTTP configuré
    ✅ Requêtes batchées
    ✅ Pagination implémentée
    
  Battery:
    ✅ Background tasks optimisés
    ✅ Location updates throttled
    ✅ Network calls minimisés
    ✅ Wake locks gérés
```

### 10.2 Checklist Backend

```yaml
Backend Performance Checklist:
  Database:
    ✅ Index sur toutes les foreign keys
    ✅ Index sur les colonnes de filtrage
    ✅ Index spatial pour géolocalisation
    ✅ Statistiques à jour (ANALYZE)
    ✅ Requêtes N+1 éliminées
    ✅ Connection pooling configuré
    
  API:
    ✅ Response caching activé
    ✅ Compression Brotli/Gzip
    ✅ ETags implémentés
    ✅ Rate limiting configuré
    ✅ Pagination par défaut
    ✅ Projections utilisées
    
  Caching:
    ✅ Cache L1 (Memory)
    ✅ Cache L2 (Redis)
    ✅ Cache invalidation strategy
    ✅ Cache warming
    
  Monitoring:
    ✅ APM configuré
    ✅ Logs structurés
    ✅ Métriques custom
    ✅ Alertes configurées
    ✅ Dashboard temps réel
```

### 10.3 Tests de Performance

```csharp
[Fact]
public async Task GetNearbySpots_ShouldCompleteUnder500ms()
{
    // Arrange
    var stopwatch = new Stopwatch();
    
    // Act
    stopwatch.Start();
    var result = await _spotService.GetNearbyAsync(48.8566, 2.3522, 50);
    stopwatch.Stop();
    
    // Assert
    Assert.True(stopwatch.ElapsedMilliseconds < 500,
        $"Query took {stopwatch.ElapsedMilliseconds}ms");
}

[Fact]
public async Task ListView_ShouldMaintain60FPS()
{
    // Arrange
    var frameRates = new List<double>();
    
    // Act
    for (int i = 0; i < 100; i++)
    {
        await ScrollListAsync();
        frameRates.Add(GetCurrentFrameRate());
    }
    
    // Assert
    var averageFPS = frameRates.Average();
    Assert.True(averageFPS >= 58, $"Average FPS: {averageFPS}");
}
```

---

## CONCLUSION

Ce guide d'optimisation des performances assure:

- **Réactivité maximale** de l'application
- **Consommation minimale** des ressources
- **Expérience utilisateur** fluide
- **Scalabilité** de l'infrastructure
- **Coûts maîtrisés** d'exploitation

### Points Critiques

1. **Mesurer avant d'optimiser**
2. **Optimiser les chemins critiques**
3. **Cache à tous les niveaux**
4. **Monitoring permanent**
5. **Tests de performance automatisés**

### Objectifs Finaux

- Démarrage < 3 secondes
- Navigation instantanée
- 60 FPS constant
- Consommation batterie minimale
- Temps de réponse API < 100ms

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Statut**: Guide d'optimisation
**Prochaine révision**: Après tests de charge

*Ce document doit être utilisé comme référence pour toutes les optimisations de SubExplore.*