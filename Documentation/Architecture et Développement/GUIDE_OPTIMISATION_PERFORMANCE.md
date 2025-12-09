# GUIDE D'OPTIMISATION ET PERFORMANCE - SUBEXPLORE
## Stratégies d'Amélioration des Performances

---

## TABLE DES MATIÈRES

1. [Performance Mobile](#1-performance-mobile)
2. [Optimisation Base de Données](#2-optimisation-base-de-données)
3. [Optimisation Réseau](#3-optimisation-réseau)
4. [Gestion de la Mémoire](#4-gestion-de-la-mémoire)
5. [Optimisation des Images](#5-optimisation-des-images)
6. [Cache Stratégies](#6-cache-stratégies)
7. [Monitoring Performance](#7-monitoring-performance)
8. [Checklist Performance](#8-checklist-performance)

---

## 1. PERFORMANCE MOBILE

### 1.1 Optimisation du Démarrage

```csharp
// App.xaml.cs - Démarrage optimisé
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<MainPage> _mainPage;
    
    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        
        // Différer l'initialisation de la page principale
        _mainPage = new Lazy<MainPage>(() => 
            _serviceProvider.GetRequiredService<MainPage>());
        
        InitializeComponent();
    }
    
    protected override async void OnStart()
    {
        // Démarrage asynchrone en parallèle
        var initTasks = new[]
        {
            InitializeCriticalServicesAsync(),
            LoadUserPreferencesAsync(),
            PreloadEssentialDataAsync()
        };
        
        await Task.WhenAll(initTasks);
        
        // Afficher la page principale
        MainPage = new NavigationPage(_mainPage.Value);
        
        // Initialisation différée des services non critiques
        _ = Task.Run(InitializeNonCriticalServicesAsync);
    }
    
    private async Task InitializeCriticalServicesAsync()
    {
        // Services essentiels uniquement
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
        await authService.InitializeAsync();
        
        var locationService = _serviceProvider.GetRequiredService<ILocationService>();
        await locationService.RequestPermissionAsync();
    }
    
    private async Task InitializeNonCriticalServicesAsync()
    {
        await Task.Delay(2000); // Attendre que l'UI soit prête
        
        // Analytics
        var analytics = _serviceProvider.GetRequiredService<IAnalyticsService>();
        await analytics.InitializeAsync();
        
        // Notifications
        var pushService = _serviceProvider.GetRequiredService<IPushNotificationService>();
        await pushService.RegisterAsync();
        
        // Cache des images
        var imageCache = _serviceProvider.GetRequiredService<IImageCacheService>();
        await imageCache.PreloadAsync();
    }
}

// Splash Screen optimisé
public class OptimizedSplashPage : ContentPage
{
    private readonly IStartupService _startupService;
    private readonly ActivityIndicator _loadingIndicator;
    private readonly Label _statusLabel;
    
    public OptimizedSplashPage(IStartupService startupService)
    {
        _startupService = startupService;
        
        // UI minimale pendant le chargement
        Content = new Grid
        {
            BackgroundColor = Color.FromHex("#006494"),
            Children =
            {
                new Image 
                { 
                    Source = "splash_logo.png",
                    VerticalOptions = LayoutOptions.Center
                },
                _loadingIndicator = new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Colors.White,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 0, 100)
                },
                _statusLabel = new Label
                {
                    TextColor = Colors.White,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 0, 50)
                }
            }
        };
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        try
        {
            // Étapes de démarrage avec feedback visuel
            _statusLabel.Text = "Vérification de la connexion...";
            await _startupService.CheckConnectivityAsync();
            
            _statusLabel.Text = "Chargement des données...";
            await _startupService.LoadInitialDataAsync();
            
            _statusLabel.Text = "Préparation de l'interface...";
            await Task.Delay(500); // Animation smooth
            
            // Navigation vers la page principale
            await Shell.Current.GoToAsync("//main");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", 
                "Impossible de démarrer l'application", "OK");
        }
    }
}
```

### 1.2 Optimisation des Listes

```csharp
// CollectionView avec virtualisation et recyclage
public class OptimizedSpotListPage : ContentPage
{
    private readonly CollectionView _collectionView;
    private readonly IncrementalLoadingCollection<SpotViewModel> _spots;
    
    public OptimizedSpotListPage()
    {
        _spots = new IncrementalLoadingCollection<SpotViewModel>(LoadMoreItemsAsync);
        
        _collectionView = new CollectionView
        {
            ItemsSource = _spots,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 10
            },
            // Optimisations importantes
            ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
            RemainingItemsThreshold = 5,
            ItemTemplate = new SpotDataTemplateSelector(),
            // Recycling des cellules
            CachingStrategy = ListViewCachingStrategy.RecycleElement
        };
        
        // Chargement incrémental
        _collectionView.RemainingItemsThresholdReached += async (s, e) =>
        {
            if (!_spots.IsLoading)
            {
                await _spots.LoadMoreAsync();
            }
        };
        
        Content = _collectionView;
    }
    
    private async Task<IEnumerable<SpotViewModel>> LoadMoreItemsAsync(
        int pageIndex, int pageSize)
    {
        var spots = await _spotService.GetSpotsAsync(pageIndex, pageSize);
        
        return spots.Select(s => new SpotViewModel(s)
        {
            // Lazy loading des images
            ImageSource = new UriImageSource
            {
                Uri = new Uri(s.ThumbnailUrl),
                CachingEnabled = true,
                CacheValidity = TimeSpan.FromDays(7)
            }
        });
    }
}

// DataTemplate optimisé
public class OptimizedSpotItemTemplate : DataTemplate
{
    public OptimizedSpotItemTemplate() : base(CreateTemplate)
    {
    }
    
    private static View CreateTemplate()
    {
        // Utiliser des layouts simples
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 100 },
                new ColumnDefinition { Width = GridLength.Star }
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            Padding = 10
        };
        
        // Image avec chargement lazy
        var image = new Image
        {
            Aspect = Aspect.AspectFill,
            HeightRequest = 80,
            WidthRequest = 100
        };
        image.SetBinding(Image.SourceProperty, "ImageSource");
        grid.Add(image, 0, 0);
        Grid.SetRowSpan(image, 2);
        
        // Labels simples sans formatage complexe
        var nameLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        nameLabel.SetBinding(Label.TextProperty, "Name");
        grid.Add(nameLabel, 1, 0);
        
        var detailsLabel = new Label
        {
            FontSize = 14,
            TextColor = Colors.Gray,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        detailsLabel.SetBinding(Label.TextProperty, "Details");
        grid.Add(detailsLabel, 1, 1);
        
        return new Frame
        {
            Content = grid,
            Padding = 0,
            HasShadow = false,
            BorderColor = Colors.LightGray,
            CornerRadius = 5
        };
    }
}

// Collection avec chargement incrémental
public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
{
    private readonly Func<int, int, Task<IEnumerable<T>>> _loadMoreFunc;
    private int _currentPage = 0;
    private const int PageSize = 20;
    
    public bool HasMoreItems { get; private set; } = true;
    public bool IsLoading { get; private set; }
    
    public IncrementalLoadingCollection(
        Func<int, int, Task<IEnumerable<T>>> loadMoreFunc)
    {
        _loadMoreFunc = loadMoreFunc;
    }
    
    public async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        if (IsLoading)
            return new LoadMoreItemsResult { Count = 0 };
        
        IsLoading = true;
        
        try
        {
            var items = await _loadMoreFunc(_currentPage, PageSize);
            var itemsList = items.ToList();
            
            if (itemsList.Count < PageSize)
            {
                HasMoreItems = false;
            }
            
            foreach (var item in itemsList)
            {
                Add(item);
            }
            
            _currentPage++;
            
            return new LoadMoreItemsResult
            {
                Count = (uint)itemsList.Count
            };
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### 1.3 Optimisation de la Navigation

```csharp
// Preloading des pages critiques
public class NavigationPreloader
{
    private readonly Dictionary<string, Lazy<Page>> _preloadedPages;
    private readonly IServiceProvider _serviceProvider;
    
    public NavigationPreloader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _preloadedPages = new Dictionary<string, Lazy<Page>>();
        
        // Précharger les pages fréquemment utilisées
        PreloadCriticalPages();
    }
    
    private void PreloadCriticalPages()
    {
        _preloadedPages["SpotList"] = new Lazy<Page>(() => 
            _serviceProvider.GetRequiredService<SpotListPage>());
            
        _preloadedPages["Map"] = new Lazy<Page>(() => 
            _serviceProvider.GetRequiredService<MapPage>());
            
        _preloadedPages["Profile"] = new Lazy<Page>(() => 
            _serviceProvider.GetRequiredService<ProfilePage>());
    }
    
    public Page GetPreloadedPage(string pageName)
    {
        if (_preloadedPages.TryGetValue(pageName, out var lazyPage))
        {
            return lazyPage.Value;
        }
        
        // Créer la page si non préchargée
        return CreatePage(pageName);
    }
    
    private Page CreatePage(string pageName)
    {
        return pageName switch
        {
            "SpotDetail" => _serviceProvider.GetRequiredService<SpotDetailPage>(),
            "Booking" => _serviceProvider.GetRequiredService<BookingPage>(),
            _ => throw new ArgumentException($"Unknown page: {pageName}")
        };
    }
}

// Navigation optimisée avec transition fluide
public class OptimizedNavigationService : INavigationService
{
    private readonly NavigationPreloader _preloader;
    
    public async Task NavigateToAsync(string route, object parameters = null)
    {
        // Préparation de la page avant navigation
        var targetPage = _preloader.GetPreloadedPage(GetPageNameFromRoute(route));
        
        if (targetPage is IInitializableView initializable)
        {
            // Initialisation asynchrone avant affichage
            await initializable.InitializeAsync(parameters);
        }
        
        // Navigation avec animation optimisée
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(targetPage, true);
        });
    }
    
    public async Task GoBackAsync()
    {
        // Libérer les ressources de la page actuelle
        if (Shell.Current.CurrentPage is IDisposableView disposable)
        {
            disposable.DisposeResources();
        }
        
        await Shell.Current.Navigation.PopAsync(true);
    }
}
```

---

## 2. OPTIMISATION BASE DE DONNÉES

### 2.1 Requêtes Optimisées

```sql
-- Index stratégiques pour les requêtes fréquentes
CREATE INDEX CONCURRENTLY idx_spots_location_approved 
ON spots USING GIST(location) 
WHERE is_active = true AND validation_status = 'Approved';

CREATE INDEX CONCURRENTLY idx_spots_created_at_desc 
ON spots(created_at DESC) 
WHERE is_active = true;

CREATE INDEX CONCURRENTLY idx_users_email_lower 
ON users(LOWER(email));

-- Index composite pour les recherches complexes
CREATE INDEX CONCURRENTLY idx_spots_search 
ON spots(validation_status, difficulty_level, water_type) 
WHERE is_active = true;

-- Fonction optimisée pour la recherche géographique
CREATE OR REPLACE FUNCTION search_nearby_spots_optimized(
    p_latitude DECIMAL,
    p_longitude DECIMAL,
    p_radius_km INTEGER,
    p_limit INTEGER DEFAULT 50
)
RETURNS TABLE(
    id UUID,
    name VARCHAR,
    distance_km DECIMAL,
    difficulty_level difficulty_level,
    average_rating DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    WITH nearby AS (
        SELECT 
            s.id,
            s.name,
            s.difficulty_level,
            s.average_rating,
            ST_Distance(
                s.location::geography,
                ST_MakePoint(p_longitude, p_latitude)::geography
            ) / 1000 AS distance_km
        FROM spots s
        WHERE 
            s.is_active = true 
            AND s.validation_status = 'Approved'
            AND ST_DWithin(
                s.location::geography,
                ST_MakePoint(p_longitude, p_latitude)::geography,
                p_radius_km * 1000
            )
        ORDER BY distance_km
        LIMIT p_limit
    )
    SELECT * FROM nearby;
END;
$$ LANGUAGE plpgsql 
STABLE 
PARALLEL SAFE
ROWS 50;

-- Vue matérialisée pour les statistiques
CREATE MATERIALIZED VIEW mv_spot_statistics AS
SELECT 
    s.id,
    s.name,
    COUNT(DISTINCT r.reviewer_id) as total_reviewers,
    AVG(r.rating) as avg_rating,
    COUNT(DISTINCT f.user_id) as total_favorites,
    COUNT(DISTINCT b.customer_id) as total_visitors,
    EXTRACT(EPOCH FROM (NOW() - s.created_at)) / 86400 as days_old,
    -- Score de popularité calculé
    (
        COALESCE(AVG(r.rating), 0) * 20 +
        LEAST(COUNT(DISTINCT r.reviewer_id), 100) * 0.5 +
        LEAST(COUNT(DISTINCT f.user_id), 100) * 0.3 +
        CASE 
            WHEN EXTRACT(EPOCH FROM (NOW() - s.created_at)) / 86400 < 7 THEN 20
            WHEN EXTRACT(EPOCH FROM (NOW() - s.created_at)) / 86400 < 30 THEN 10
            ELSE 0
        END
    ) as popularity_score
FROM spots s
LEFT JOIN reviews r ON r.entity_id = s.id AND r.entity_type = 'spot'
LEFT JOIN favorites f ON f.entity_id = s.id AND f.entity_type = 'spot'
LEFT JOIN bookings b ON b.structure_id = s.id
WHERE s.is_active = true
GROUP BY s.id, s.name, s.created_at
WITH DATA;

-- Refresh automatique
CREATE OR REPLACE FUNCTION refresh_spot_statistics()
RETURNS void AS $$
BEGIN
    REFRESH MATERIALIZED VIEW CONCURRENTLY mv_spot_statistics;
END;
$$ LANGUAGE plpgsql;

-- Planifier le refresh toutes les heures
SELECT cron.schedule('refresh-spot-stats', '0 * * * *', 
    'SELECT refresh_spot_statistics()');
```

### 2.2 Optimisation EF Core

```csharp
public class OptimizedSpotRepository : ISpotRepository
{
    private readonly SubExploreContext _context;
    
    // Requête avec projection pour réduire les données transférées
    public async Task<List<SpotListDto>> GetSpotsOptimizedAsync(
        double latitude, 
        double longitude, 
        double radiusKm,
        SpotFilters filters)
    {
        // Utiliser AsNoTracking pour les lectures
        var query = _context.Spots
            .AsNoTracking()
            .Where(s => s.IsActive && s.ValidationStatus == SpotValidationStatus.Approved);
        
        // Filtrage côté base de données
        if (filters.Difficulty.HasValue)
            query = query.Where(s => s.DifficultyLevel == filters.Difficulty.Value);
        
        if (filters.WaterType.HasValue)
            query = query.Where(s => s.WaterType == filters.WaterType.Value);
        
        if (filters.MinRating.HasValue)
            query = query.Where(s => s.AverageRating >= filters.MinRating.Value);
        
        // Projection pour ne récupérer que les champs nécessaires
        var spots = await query
            .Select(s => new SpotListDto
            {
                Id = s.Id,
                Name = s.Name,
                // Calcul de distance avec PostGIS
                Distance = s.Location.Distance(
                    NetTopologySuite.Geometries.Point(longitude, latitude)) / 1000,
                Difficulty = s.DifficultyLevel,
                Rating = s.AverageRating,
                ThumbnailUrl = s.CoverImageUrl,
                // N'inclure que les champs nécessaires à la liste
            })
            .Where(s => s.Distance <= radiusKm)
            .OrderBy(s => s.Distance)
            .Take(50)
            .ToListAsync();
        
        return spots;
    }
    
    // Chargement avec Include optimisé
    public async Task<Spot> GetSpotWithDetailsAsync(Guid id)
    {
        // Split queries pour éviter le problème cartésien
        var spot = await _context.Spots
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Images)
            .Include(s => s.Creator)
            .Include(s => s.Reviews.Take(5)) // Limiter les reviews
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        // Chargement séparé des données volumineuses
        if (spot != null)
        {
            // Charger les statistiques depuis la vue matérialisée
            var stats = await _context.SpotStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SpotId == id);
            
            spot.PopularityScore = stats?.PopularityScore ?? 0;
        }
        
        return spot;
    }
    
    // Batch operations
    public async Task<int> UpdateMultipleSpotsAsync(
        List<Guid> spotIds, 
        Action<Spot> updateAction)
    {
        // Utiliser ExecuteUpdate pour éviter de charger en mémoire
        return await _context.Spots
            .Where(s => spotIds.Contains(s.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.UpdatedAt, DateTime.UtcNow)
                // Autres propriétés à mettre à jour
            );
    }
    
    // Requête compilée pour les opérations fréquentes
    private static readonly Func<SubExploreContext, Guid, Task<bool>> 
        _spotExistsCompiled = EF.CompileAsyncQuery(
            (SubExploreContext context, Guid id) =>
                context.Spots.Any(s => s.Id == id && s.IsActive));
    
    public Task<bool> SpotExistsAsync(Guid id)
    {
        return _spotExistsCompiled(_context, id);
    }
}

// Configuration des entités pour les performances
public class SpotConfiguration : IEntityTypeConfiguration<Spot>
{
    public void Configure(EntityTypeBuilder<Spot> builder)
    {
        // Index
        builder.HasIndex(s => s.ValidationStatus)
            .HasFilter("is_active = true");
        
        builder.HasIndex(s => new { s.Country, s.Region, s.City });
        
        // Colonnes calculées stockées
        builder.Property(s => s.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('french', name || ' ' || coalesce(description, ''))",
                stored: true);
        
        // Lazy loading sélectif
        builder.Navigation(s => s.Images).AutoInclude();
        builder.Navigation(s => s.Reviews).UsePropertyAccessMode(PropertyAccessMode.Field);
        
        // Value conversions
        builder.Property(s => s.Activities)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
    }
}
```

---

## 3. OPTIMISATION RÉSEAU

### 3.1 Compression et Minification

```csharp
// Compression des requêtes/réponses
public class CompressionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // Compression de la requête si nécessaire
        if (request.Content != null && request.Content.Headers.ContentLength > 1024)
        {
            var originalContent = await request.Content.ReadAsByteArrayAsync();
            var compressedContent = Compress(originalContent);
            
            request.Content = new ByteArrayContent(compressedContent);
            request.Content.Headers.ContentType = 
                new MediaTypeHeaderValue("application/json");
            request.Content.Headers.ContentEncoding.Add("gzip");
        }
        
        // Accepter les réponses compressées
        request.Headers.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("gzip"));
        request.Headers.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("deflate"));
        
        var response = await base.SendAsync(request, cancellationToken);
        
        // Décompression automatique gérée par HttpClient
        
        return response;
    }
    
    private byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }
}

// Optimisation des payloads JSON
public class OptimizedJsonService
{
    private readonly JsonSerializerOptions _options;
    
    public OptimizedJsonService()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false, // Pas d'indentation en production
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }
    
    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }
    
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
```

### 3.2 Request Batching

```csharp
public class BatchRequestService
{
    private readonly HttpClient _httpClient;
    private readonly Queue<BatchRequest> _pendingRequests;
    private readonly Timer _batchTimer;
    private readonly SemaphoreSlim _semaphore;
    
    public BatchRequestService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _pendingRequests = new Queue<BatchRequest>();
        _semaphore = new SemaphoreSlim(1, 1);
        
        // Process batch every 100ms or when queue is full
        _batchTimer = new Timer(ProcessBatch, null, 100, 100);
    }
    
    public async Task<T> AddRequestAsync<T>(string endpoint, object data)
    {
        var tcs = new TaskCompletionSource<T>();
        
        await _semaphore.WaitAsync();
        try
        {
            _pendingRequests.Enqueue(new BatchRequest
            {
                Endpoint = endpoint,
                Data = data,
                ResponseHandler = (response) =>
                {
                    var result = JsonSerializer.Deserialize<T>(response);
                    tcs.SetResult(result);
                }
            });
            
            // Process immediately if queue is getting full
            if (_pendingRequests.Count >= 10)
            {
                await ProcessBatchAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
        
        return await tcs.Task;
    }
    
    private async void ProcessBatch(object state)
    {
        await ProcessBatchAsync();
    }
    
    private async Task ProcessBatchAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_pendingRequests.Count == 0)
                return;
            
            var batch = new List<BatchRequest>();
            while (_pendingRequests.Count > 0 && batch.Count < 10)
            {
                batch.Add(_pendingRequests.Dequeue());
            }
            
            // Send batch request
            var batchPayload = new
            {
                requests = batch.Select(r => new
                {
                    id = Guid.NewGuid(),
                    method = "POST",
                    url = r.Endpoint,
                    body = r.Data
                })
            };
            
            var response = await _httpClient.PostAsJsonAsync("/batch", batchPayload);
            
            if (response.IsSuccessStatusCode)
            {
                var batchResponse = await response.Content
                    .ReadFromJsonAsync<BatchResponse>();
                
                // Process individual responses
                foreach (var item in batchResponse.Responses)
                {
                    var request = batch.FirstOrDefault(r => r.Id == item.Id);
                    request?.ResponseHandler(item.Body);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

---

## 4. GESTION DE LA MÉMOIRE

### 4.1 Memory Management

```csharp
public class MemoryManager : IDisposable
{
    private readonly Timer _gcTimer;
    private readonly ILogger<MemoryManager> _logger;
    private long _lastMemoryUsage;
    
    public MemoryManager(ILogger<MemoryManager> logger)
    {
        _logger = logger;
        
        // Monitor memory every 30 seconds
        _gcTimer = new Timer(MonitorMemory, null, 
            TimeSpan.FromSeconds(30), 
            TimeSpan.FromSeconds(30));
    }
    
    private void MonitorMemory(object state)
    {
        var currentMemory = GC.GetTotalMemory(false);
        var memoryDelta = currentMemory - _lastMemoryUsage;
        
        _logger.LogInformation(
            "Memory usage: {Current:N0} bytes (Δ {Delta:N0})",
            currentMemory, memoryDelta);
        
        // Force GC if memory usage is high
        if (currentMemory > 100_000_000) // 100MB
        {
            _logger.LogWarning("High memory usage detected, forcing GC");
            ForceGarbageCollection();
        }
        
        _lastMemoryUsage = currentMemory;
    }
    
    public void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
    
    public void Dispose()
    {
        _gcTimer?.Dispose();
    }
}

// Weak reference cache pour les objets volumineux
public class WeakReferenceCache<TKey, TValue> where TValue : class
{
    private readonly Dictionary<TKey, WeakReference> _cache;
    private readonly Func<TKey, TValue> _factory;
    
    public WeakReferenceCache(Func<TKey, TValue> factory)
    {
        _cache = new Dictionary<TKey, WeakReference>();
        _factory = factory;
    }
    
    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var weakRef))
        {
            if (weakRef.IsAlive && weakRef.Target is TValue value)
            {
                return value;
            }
            
            // L'objet a été collecté par le GC
            _cache.Remove(key);
        }
        
        // Créer et cacher un nouvel objet
        var newValue = _factory(key);
        _cache[key] = new WeakReference(newValue);
        
        return newValue;
    }
    
    public void Clear()
    {
        _cache.Clear();
    }
}

// Pool d'objets pour réutilisation
public class ObjectPool<T> where T : class, new()
{
    private readonly ConcurrentBag<T> _pool;
    private readonly Func<T> _factory;
    private readonly Action<T> _reset;
    private readonly int _maxSize;
    
    public ObjectPool(
        int maxSize = 100,
        Func<T> factory = null,
        Action<T> reset = null)
    {
        _pool = new ConcurrentBag<T>();
        _maxSize = maxSize;
        _factory = factory ?? (() => new T());
        _reset = reset;
    }
    
    public T Rent()
    {
        if (_pool.TryTake(out var item))
        {
            return item;
        }
        
        return _factory();
    }
    
    public void Return(T item)
    {
        if (_pool.Count < _maxSize)
        {
            _reset?.Invoke(item);
            _pool.Add(item);
        }
    }
}
```

---

## 5. OPTIMISATION DES IMAGES

### 5.1 Compression et Redimensionnement

```csharp
public class ImageOptimizationService
{
    private readonly HttpClient _httpClient;
    
    public async Task<byte[]> OptimizeImageAsync(
        byte[] originalImage,
        ImageSize targetSize = ImageSize.Medium,
        int quality = 85)
    {
        using var inputStream = new MemoryStream(originalImage);
        using var original = SKBitmap.Decode(inputStream);
        
        // Calculer les dimensions optimales
        var (width, height) = GetTargetDimensions(original, targetSize);
        
        // Redimensionner
        using var resized = original.Resize(
            new SKImageInfo(width, height),
            SKFilterQuality.High);
        
        // Convertir en JPEG optimisé
        using var image = SKImage.FromBitmap(resized);
        
        // Encoder avec qualité spécifiée
        var encoded = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        
        return encoded.ToArray();
    }
    
    private (int width, int height) GetTargetDimensions(
        SKBitmap original, 
        ImageSize size)
    {
        var maxDimension = size switch
        {
            ImageSize.Thumbnail => 150,
            ImageSize.Small => 320,
            ImageSize.Medium => 640,
            ImageSize.Large => 1024,
            ImageSize.Original => Math.Max(original.Width, original.Height),
            _ => 640
        };
        
        var ratio = (float)original.Width / original.Height;
        
        if (original.Width > original.Height)
        {
            return (maxDimension, (int)(maxDimension / ratio));
        }
        else
        {
            return ((int)(maxDimension * ratio), maxDimension);
        }
    }
    
    // Génération de formats responsifs
    public async Task<Dictionary<ImageSize, byte[]>> GenerateResponsiveImagesAsync(
        byte[] originalImage)
    {
        var result = new Dictionary<ImageSize, byte[]>();
        
        var tasks = Enum.GetValues<ImageSize>()
            .Where(size => size != ImageSize.Original)
            .Select(async size =>
            {
                var optimized = await OptimizeImageAsync(originalImage, size);
                return (size, optimized);
            });
        
        var results = await Task.WhenAll(tasks);
        
        foreach (var (size, data) in results)
        {
            result[size] = data;
        }
        
        return result;
    }
}

// Lazy loading d'images
public class LazyImageView : ContentView
{
    private readonly Image _image;
    private readonly ActivityIndicator _loadingIndicator;
    private string _imageUrl;
    private bool _isLoaded;
    
    public LazyImageView()
    {
        _loadingIndicator = new ActivityIndicator
        {
            IsRunning = true,
            Color = Colors.Gray
        };
        
        _image = new Image
        {
            IsVisible = false
        };
        
        Content = new Grid
        {
            Children = { _loadingIndicator, _image }
        };
    }
    
    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            _imageUrl = value;
            if (!string.IsNullOrEmpty(value) && IsVisible)
            {
                LoadImageAsync();
            }
        }
    }
    
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        
        if (propertyName == nameof(IsVisible) && IsVisible && !_isLoaded)
        {
            LoadImageAsync();
        }
    }
    
    private async void LoadImageAsync()
    {
        if (_isLoaded || string.IsNullOrEmpty(_imageUrl))
            return;
        
        _isLoaded = true;
        
        try
        {
            // Charger depuis le cache ou télécharger
            var imageSource = await ImageCacheService.GetImageAsync(_imageUrl);
            
            _image.Source = imageSource;
            _image.IsVisible = true;
            _loadingIndicator.IsVisible = false;
        }
        catch (Exception ex)
        {
            // Afficher une image par défaut
            _image.Source = "image_placeholder.png";
            _image.IsVisible = true;
            _loadingIndicator.IsVisible = false;
        }
    }
}
```

---

## 6. CACHE STRATÉGIES

### 6.1 Multi-Level Caching

```csharp
public interface ICacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task ClearAsync();
}

public class MultiLevelCacheService : ICacheService
{
    private readonly IMemoryCache _l1Cache; // Memory cache (Level 1)
    private readonly IDistributedCache _l2Cache; // Redis cache (Level 2)
    private readonly IFileCache _l3Cache; // File cache (Level 3)
    
    public async Task<T> GetAsync<T>(string key)
    {
        // Level 1: Memory
        if (_l1Cache.TryGetValue<T>(key, out var l1Value))
        {
            return l1Value;
        }
        
        // Level 2: Redis
        var l2Data = await _l2Cache.GetAsync(key);
        if (l2Data != null)
        {
            var l2Value = JsonSerializer.Deserialize<T>(l2Data);
            
            // Promouvoir vers L1
            _l1Cache.Set(key, l2Value, TimeSpan.FromMinutes(5));
            
            return l2Value;
        }
        
        // Level 3: File
        var l3Value = await _l3Cache.GetAsync<T>(key);
        if (l3Value != null)
        {
            // Promouvoir vers L1 et L2
            _l1Cache.Set(key, l3Value, TimeSpan.FromMinutes(5));
            await _l2Cache.SetAsync(key, 
                JsonSerializer.SerializeToUtf8Bytes(l3Value),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            
            return l3Value;
        }
        
        return default(T);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var exp = expiration ?? TimeSpan.FromHours(1);
        
        // Écrire dans tous les niveaux
        _l1Cache.Set(key, value, TimeSpan.FromMinutes(5));
        
        await _l2Cache.SetAsync(key,
            JsonSerializer.SerializeToUtf8Bytes(value),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = exp
            });
        
        await _l3Cache.SetAsync(key, value, exp);
    }
    
    public async Task InvalidateCascadeAsync(string pattern)
    {
        // Invalider en cascade
        _l1Cache.RemovePattern(pattern);
        await _l2Cache.RemovePatternAsync(pattern);
        await _l3Cache.RemovePatternAsync(pattern);
    }
}

// Cache avec préchargement
public class PreloadedCache<T> where T : class
{
    private readonly Dictionary<string, T> _cache;
    private readonly Func<Task<Dictionary<string, T>>> _preloader;
    private readonly Timer _refreshTimer;
    
    public PreloadedCache(
        Func<Task<Dictionary<string, T>>> preloader,
        TimeSpan refreshInterval)
    {
        _cache = new Dictionary<string, T>();
        _preloader = preloader;
        
        // Précharger immédiatement
        _ = PreloadAsync();
        
        // Rafraîchir périodiquement
        _refreshTimer = new Timer(
            async _ => await PreloadAsync(),
            null,
            refreshInterval,
            refreshInterval);
    }
    
    private async Task PreloadAsync()
    {
        try
        {
            var data = await _preloader();
            
            lock (_cache)
            {
                _cache.Clear();
                foreach (var kvp in data)
                {
                    _cache[kvp.Key] = kvp.Value;
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but keep existing cache
        }
    }
    
    public T Get(string key)
    {
        lock (_cache)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }
    }
}
```

---

## 7. MONITORING PERFORMANCE

### 7.1 Performance Metrics

```csharp
public class PerformanceMonitor
{
    private readonly IMetricsCollector _metrics;
    private readonly ILogger<PerformanceMonitor> _logger;
    
    public IDisposable MeasureOperation(string operationName)
    {
        return new OperationTimer(operationName, _metrics, _logger);
    }
    
    private class OperationTimer : IDisposable
    {
        private readonly string _operationName;
        private readonly IMetricsCollector _metrics;
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;
        
        public OperationTimer(
            string operationName, 
            IMetricsCollector metrics,
            ILogger logger)
        {
            _operationName = operationName;
            _metrics = metrics;
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
        }
        
        public void Dispose()
        {
            _stopwatch.Stop();
            
            _metrics.RecordDuration(_operationName, _stopwatch.ElapsedMilliseconds);
            
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning(
                    "Slow operation detected: {Operation} took {Duration}ms",
                    _operationName, _stopwatch.ElapsedMilliseconds);
            }
        }
    }
}

// Application Insights Integration
public class ApplicationInsightsMetricsCollector : IMetricsCollector
{
    private readonly TelemetryClient _telemetryClient;
    
    public void RecordDuration(string operationName, long milliseconds)
    {
        _telemetryClient.TrackMetric($"Operation.{operationName}.Duration", milliseconds);
    }
    
    public void RecordCount(string metricName, long count)
    {
        _telemetryClient.TrackMetric(metricName, count);
    }
    
    public void RecordMemoryUsage()
    {
        var memory = GC.GetTotalMemory(false);
        _telemetryClient.TrackMetric("Memory.Usage.Bytes", memory);
        
        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);
        
        _telemetryClient.TrackMetric("GC.Gen0.Count", gen0);
        _telemetryClient.TrackMetric("GC.Gen1.Count", gen1);
        _telemetryClient.TrackMetric("GC.Gen2.Count", gen2);
    }
}

// Performance benchmarks
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class SpotServiceBenchmarks
{
    private SpotService _spotService;
    private List<Guid> _spotIds;
    
    [GlobalSetup]
    public void Setup()
    {
        _spotService = new SpotService(/* dependencies */);
        _spotIds = Enumerable.Range(0, 100)
            .Select(_ => Guid.NewGuid())
            .ToList();
    }
    
    [Benchmark]
    public async Task GetNearbySpots()
    {
        await _spotService.GetNearbyAsync(48.8566, 2.3522, 50);
    }
    
    [Benchmark]
    public async Task GetSpotDetails()
    {
        await _spotService.GetByIdAsync(_spotIds[0]);
    }
    
    [Benchmark]
    public async Task BatchGetSpots()
    {
        var tasks = _spotIds.Take(10)
            .Select(id => _spotService.GetByIdAsync(id));
        
        await Task.WhenAll(tasks);
    }
}
```

---

## 8. CHECKLIST PERFORMANCE

### 8.1 Mobile Performance

```yaml
Mobile_Performance_Checklist:
  Startup:
    ✅ Splash screen < 1s
    ✅ Main page load < 2s
    ✅ Lazy loading des services non critiques
    ✅ Préchargement des pages fréquentes
    
  UI_Responsiveness:
    ✅ 60 FPS animations
    ✅ Touch response < 100ms
    ✅ List scrolling smooth
    ✅ No UI thread blocking
    
  Memory:
    ✅ Memory usage < 200MB
    ✅ No memory leaks
    ✅ Images properly disposed
    ✅ Weak references for caches
    
  Network:
    ✅ Request compression enabled
    ✅ Response caching implemented
    ✅ Batch requests when possible
    ✅ Offline mode functional
```

### 8.2 Backend Performance

```yaml
Backend_Performance_Checklist:
  API:
    ✅ Response time < 200ms (p50)
    ✅ Response time < 1s (p99)
    ✅ Throughput > 1000 req/s
    ✅ Error rate < 1%
    
  Database:
    ✅ Query time < 50ms (average)
    ✅ Index usage > 95%
    ✅ Connection pool optimized
    ✅ N+1 queries eliminated
    
  Caching:
    ✅ Cache hit rate > 80%
    ✅ Redis latency < 5ms
    ✅ CDN for static assets
    ✅ Browser caching headers
    
  Infrastructure:
    ✅ Auto-scaling configured
    ✅ Load balancer health checks
    ✅ CDN configured
    ✅ Database replicas for reads
```

---

## CONCLUSION

Ce guide d'optimisation assure:

- **Performance optimale** sur tous les appareils
- **Temps de réponse rapides** pour une meilleure UX
- **Utilisation efficace** des ressources
- **Scalabilité** pour la croissance
- **Monitoring continu** des performances

### Objectifs de Performance

| Métrique | Cible | Critique |
|----------|--------|----------|
| Démarrage app | < 2s | < 3s |
| Chargement page | < 1s | < 2s |
| API response (p50) | < 200ms | < 500ms |
| API response (p99) | < 1s | < 2s |
| Memory usage | < 200MB | < 300MB |
| Crash rate | < 0.1% | < 1% |
| Cache hit rate | > 80% | > 60% |

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Statut**: Guide de performance
**Prochaine revue**: Mensuelle

*Les optimisations doivent être mesurées et ajustées continuellement.*