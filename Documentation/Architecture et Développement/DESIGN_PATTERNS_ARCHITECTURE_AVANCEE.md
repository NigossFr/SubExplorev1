# DESIGN PATTERNS ET ARCHITECTURE AVANCÉE - SUBEXPLORE
## Guide des Patterns et Meilleures Pratiques

---

## TABLE DES MATIÈRES

1. [Patterns d'Architecture Globale](#1-patterns-darchitecture-globale)
2. [Patterns de Gestion d'État](#2-patterns-de-gestion-détat)
3. [Patterns de Communication](#3-patterns-de-communication)
4. [Patterns de Performance](#4-patterns-de-performance)
5. [Patterns de Sécurité](#5-patterns-de-sécurité)
6. [Patterns de Tests](#6-patterns-de-tests)
7. [Patterns Spécifiques MAUI](#7-patterns-spécifiques-maui)
8. [Anti-Patterns à Éviter](#8-anti-patterns-à-éviter)

---

## 1. PATTERNS D'ARCHITECTURE GLOBALE

### 1.1 Clean Architecture avec MAUI

```
┌─────────────────────────────────────────────────────┐
│                   UI Layer (MAUI)                   │
│  • Pages XAML                                       │
│  • Custom Controls                                  │
│  • Platform-Specific Code                          │
├─────────────────────────────────────────────────────┤
│              Presentation Layer (MVVM)              │
│  • ViewModels                                       │
│  • Commands                                         │
│  • Converters                                       │
├─────────────────────────────────────────────────────┤
│              Application Layer                      │
│  • Use Cases / Application Services                 │
│  • DTOs                                            │
│  • Mappers                                         │
├─────────────────────────────────────────────────────┤
│                 Domain Layer                        │
│  • Entities                                        │
│  • Value Objects                                   │
│  • Domain Services                                 │
│  • Specifications                                  │
├─────────────────────────────────────────────────────┤
│            Infrastructure Layer                     │
│  • Repositories                                    │
│  • External Services                               │
│  • Data Access                                     │
└─────────────────────────────────────────────────────┘
```

#### Implementation Clean Architecture

```csharp
// Domain Layer - Entity
namespace SubExplore.Domain.Entities
{
    public class Spot : Entity, IAggregateRoot
    {
        private readonly List<SpotImage> _images = new();
        private readonly List<SpotReview> _reviews = new();
        
        public SpotId Id { get; private set; }
        public string Name { get; private set; }
        public Location Location { get; private set; }
        public DifficultyLevel Difficulty { get; private set; }
        public SpotValidationStatus ValidationStatus { get; private set; }
        
        public IReadOnlyCollection<SpotImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<SpotReview> Reviews => _reviews.AsReadOnly();
        
        protected Spot() { } // Pour EF
        
        public static Result<Spot> Create(
            string name, 
            Location location, 
            DifficultyLevel difficulty,
            UserId creatorId)
        {
            var spot = new Spot
            {
                Id = new SpotId(Guid.NewGuid()),
                Name = name,
                Location = location,
                Difficulty = difficulty,
                ValidationStatus = SpotValidationStatus.Pending
            };
            
            spot.AddDomainEvent(new SpotCreatedEvent(spot.Id, creatorId));
            
            return Result.Success(spot);
        }
        
        public Result AddImage(string url, string caption)
        {
            if (_images.Count >= 10)
                return Result.Failure("Maximum 10 images allowed");
                
            _images.Add(new SpotImage(url, caption));
            return Result.Success();
        }
        
        public Result Approve(UserId validatorId)
        {
            if (ValidationStatus != SpotValidationStatus.Pending)
                return Result.Failure("Spot must be pending to approve");
                
            ValidationStatus = SpotValidationStatus.Approved;
            AddDomainEvent(new SpotApprovedEvent(Id, validatorId));
            
            return Result.Success();
        }
    }
}

// Application Layer - Use Case
namespace SubExplore.Application.UseCases.Spots
{
    public class CreateSpotUseCase : ICreateSpotUseCase
    {
        private readonly ISpotRepository _spotRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        
        public async Task<Result<SpotDto>> ExecuteAsync(CreateSpotCommand command)
        {
            // Validation
            var validationResult = await new CreateSpotValidator()
                .ValidateAsync(command);
                
            if (!validationResult.IsValid)
                return Result.Failure<SpotDto>(validationResult.Errors);
            
            // Création du spot
            var location = Location.Create(command.Latitude, command.Longitude);
            var spotResult = Spot.Create(
                command.Name,
                location.Value,
                command.Difficulty,
                _currentUser.UserId);
                
            if (spotResult.IsFailure)
                return Result.Failure<SpotDto>(spotResult.Error);
            
            // Persistence
            await _spotRepository.AddAsync(spotResult.Value);
            await _unitOfWork.CommitAsync();
            
            // Notification
            await _mediator.Publish(new SpotCreatedNotification(spotResult.Value.Id));
            
            // Mapping et retour
            return Result.Success(_mapper.Map<SpotDto>(spotResult.Value));
        }
    }
}
```

### 1.2 Domain-Driven Design (DDD)

#### Aggregate Pattern

```csharp
// Aggregate Root
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

// Value Object
public class Location : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    
    private Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public static Result<Location> Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            return Result.Failure<Location>("Invalid latitude");
            
        if (longitude < -180 || longitude > 180)
            return Result.Failure<Location>("Invalid longitude");
            
        return Result.Success(new Location(latitude, longitude));
    }
    
    public double DistanceTo(Location other)
    {
        var d1 = Latitude * (Math.PI / 180.0);
        var d2 = other.Latitude * (Math.PI / 180.0);
        var num1 = (other.Latitude - Latitude) * (Math.PI / 180.0);
        var num2 = (other.Longitude - Longitude) * (Math.PI / 180.0);
        
        var a = Math.Sin(num1 / 2) * Math.Sin(num1 / 2) +
                Math.Cos(d1) * Math.Cos(d2) *
                Math.Sin(num2 / 2) * Math.Sin(num2 / 2);
                
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return 6371 * c; // Distance en km
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}

// Specification Pattern
public class ApprovedSpotsSpecification : Specification<Spot>
{
    public override Expression<Func<Spot, bool>> ToExpression()
    {
        return spot => spot.ValidationStatus == SpotValidationStatus.Approved &&
                      spot.IsActive;
    }
}

public class NearbyLocationSpecification : Specification<Spot>
{
    private readonly Location _center;
    private readonly double _radiusKm;
    
    public NearbyLocationSpecification(Location center, double radiusKm)
    {
        _center = center;
        _radiusKm = radiusKm;
    }
    
    public override Expression<Func<Spot, bool>> ToExpression()
    {
        // Note: Cette expression sera traduite en requête PostGIS
        return spot => spot.Location.DistanceTo(_center) <= _radiusKm;
    }
}

// Usage combiné
var spec = new ApprovedSpotsSpecification()
    .And(new NearbyLocationSpecification(userLocation, 50));
    
var spots = await _repository.FindAsync(spec);
```

### 1.3 CQRS Pattern

```csharp
// Command
public record CreateSpotCommand : ICommand<SpotDto>
{
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Description { get; init; }
    public DifficultyLevel Difficulty { get; init; }
    public List<string> Activities { get; init; }
}

// Command Handler
public class CreateSpotCommandHandler : ICommandHandler<CreateSpotCommand, SpotDto>
{
    private readonly ISpotWriteRepository _writeRepository;
    private readonly IEventBus _eventBus;
    
    public async Task<Result<SpotDto>> HandleAsync(CreateSpotCommand command)
    {
        // Logique de création
        var spot = await _writeRepository.CreateAsync(...);
        
        // Publier l'événement
        await _eventBus.PublishAsync(new SpotCreatedEvent(spot));
        
        return Result.Success(new SpotDto(spot));
    }
}

// Query
public record GetNearbySpots Query : IQuery<List<SpotDto>>
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double RadiusKm { get; init; }
}

// Query Handler
public class GetNearbySpotsQueryHandler : IQueryHandler<GetNearbySpotsQuery, List<SpotDto>>
{
    private readonly ISpotReadRepository _readRepository;
    private readonly IMemoryCache _cache;
    
    public async Task<List<SpotDto>> HandleAsync(GetNearbySpotsQuery query)
    {
        var cacheKey = $"nearby_{query.Latitude}_{query.Longitude}_{query.RadiusKm}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            
            var spots = await _readRepository.GetNearbyAsync(
                query.Latitude, 
                query.Longitude, 
                query.RadiusKm);
                
            return _mapper.Map<List<SpotDto>>(spots);
        });
    }
}

// Mediator Implementation
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));
            
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        
        return await handler.HandleAsync((dynamic)query);
    }
}
```

---

## 2. PATTERNS DE GESTION D'ÉTAT

### 2.1 State Management avec ObservableObject

```csharp
// Global State Manager
public class AppStateManager : ObservableObject
{
    private static readonly Lazy<AppStateManager> _instance = 
        new(() => new AppStateManager());
    
    public static AppStateManager Instance => _instance.Value;
    
    private User? _currentUser;
    private bool _isOnline = true;
    private Location? _lastKnownLocation;
    
    public User? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }
    
    public bool IsOnline
    {
        get => _isOnline;
        set => SetProperty(ref _isOnline, value);
    }
    
    public Location? LastKnownLocation
    {
        get => _lastKnownLocation;
        set => SetProperty(ref _lastKnownLocation, value);
    }
    
    private AppStateManager()
    {
        // Initialisation
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }
    
    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        IsOnline = e.NetworkAccess == NetworkAccess.Internet;
    }
}

// ViewModel utilisant le State Manager
public partial class MapViewModel : BaseViewModel
{
    private readonly AppStateManager _appState;
    
    [ObservableProperty]
    private ObservableCollection<SpotPin> _visibleSpots;
    
    public MapViewModel(AppStateManager appState)
    {
        _appState = appState;
        
        // Réagir aux changements d'état
        _appState.PropertyChanged += OnAppStateChanged;
    }
    
    private void OnAppStateChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppStateManager.LastKnownLocation))
        {
            CenterMapOnUserLocation();
        }
    }
}
```

### 2.2 Redux-like Pattern pour État Complexe

```csharp
// State
public record AppState
{
    public UserState User { get; init; }
    public SpotsState Spots { get; init; }
    public BookingsState Bookings { get; init; }
    public UIState UI { get; init; }
}

// Actions
public interface IAction { }

public record LoginSuccessAction(User User) : IAction;
public record AddSpotToFavoritesAction(Guid SpotId) : IAction;
public record SetLoadingAction(bool IsLoading) : IAction;

// Reducers
public static class AppReducer
{
    public static AppState Reduce(AppState state, IAction action)
    {
        return state with
        {
            User = UserReducer.Reduce(state.User, action),
            Spots = SpotsReducer.Reduce(state.Spots, action),
            Bookings = BookingsReducer.Reduce(state.Bookings, action),
            UI = UIReducer.Reduce(state.UI, action)
        };
    }
}

public static class UserReducer
{
    public static UserState Reduce(UserState state, IAction action)
    {
        return action switch
        {
            LoginSuccessAction loginAction => state with 
            { 
                CurrentUser = loginAction.User,
                IsAuthenticated = true 
            },
            LogoutAction => UserState.Initial,
            _ => state
        };
    }
}

// Store
public class StateStore : ObservableObject
{
    private AppState _state = AppState.Initial;
    
    public AppState State
    {
        get => _state;
        private set => SetProperty(ref _state, value);
    }
    
    public void Dispatch(IAction action)
    {
        State = AppReducer.Reduce(State, action);
        
        // Side effects
        _ = HandleSideEffectsAsync(action);
    }
    
    private async Task HandleSideEffectsAsync(IAction action)
    {
        switch (action)
        {
            case LoginSuccessAction:
                await SecureStorage.SetAsync("user_token", State.User.Token);
                break;
                
            case AddSpotToFavoritesAction addFav:
                await _apiService.AddToFavoritesAsync(addFav.SpotId);
                break;
        }
    }
}
```

### 2.3 Event Aggregator Pattern

```csharp
// Event Aggregator
public interface IEventAggregator
{
    void Publish<TEvent>(TEvent eventToPublish);
    void Subscribe<TEvent>(Action<TEvent> action);
    void Unsubscribe<TEvent>(Action<TEvent> action);
}

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    
    public void Publish<TEvent>(TEvent eventToPublish)
    {
        if (!_subscribers.TryGetValue(typeof(TEvent), out var subscribers))
            return;
        
        foreach (var subscriber in subscribers.Cast<Action<TEvent>>())
        {
            subscriber(eventToPublish);
        }
    }
    
    public void Subscribe<TEvent>(Action<TEvent> action)
    {
        if (!_subscribers.ContainsKey(typeof(TEvent)))
            _subscribers[typeof(TEvent)] = new List<Delegate>();
        
        _subscribers[typeof(TEvent)].Add(action);
    }
    
    public void Unsubscribe<TEvent>(Action<TEvent> action)
    {
        if (_subscribers.TryGetValue(typeof(TEvent), out var subscribers))
        {
            subscribers.Remove(action);
        }
    }
}

// Events
public record SpotCreatedEvent(Guid SpotId, string Name);
public record UserLoggedInEvent(User User);
public record NotificationReceivedEvent(string Title, string Message);

// Usage
public class NotificationViewModel : BaseViewModel
{
    private readonly IEventAggregator _eventAggregator;
    
    public NotificationViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.Subscribe<NotificationReceivedEvent>(OnNotificationReceived);
    }
    
    private void OnNotificationReceived(NotificationReceivedEvent evt)
    {
        ShowToast(evt.Title, evt.Message);
    }
}
```

---

## 3. PATTERNS DE COMMUNICATION

### 3.1 Circuit Breaker Pattern

```csharp
public class CircuitBreaker
{
    private readonly int _threshold;
    private readonly TimeSpan _timeout;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;
    
    public CircuitBreaker(int threshold = 3, int timeoutSeconds = 60)
    {
        _threshold = threshold;
        _timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime > _timeout)
            {
                _state = CircuitState.HalfOpen;
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }
        
        try
        {
            var result = await action();
            
            if (_state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Closed;
                _failureCount = 0;
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _lastFailureTime = DateTime.UtcNow;
            _failureCount++;
            
            if (_failureCount >= _threshold)
            {
                _state = CircuitState.Open;
            }
            
            throw;
        }
    }
}

// Usage avec Polly
public class ResilientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    
    public ResilientApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var reason = outcome.Result?.StatusCode.ToString() ?? 
                                outcome.Exception?.Message;
                    Debug.WriteLine($"Retry {retryCount} after {timespan}s: {reason}");
                });
    }
    
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(endpoint));
        
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }
}
```

### 3.2 Offline-First Pattern

```csharp
public class OfflineFirstRepository<T> where T : class, IEntity
{
    private readonly ILocalDatabase _localDb;
    private readonly IRemoteApi _remoteApi;
    private readonly ISyncQueue _syncQueue;
    private readonly IConnectivityService _connectivity;
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        // Toujours lire depuis le cache local en premier
        var local = await _localDb.GetAsync<T>(id);
        
        if (local != null)
        {
            // Sync en arrière-plan si online
            if (_connectivity.IsOnline)
            {
                _ = SyncEntityAsync(id);
            }
            
            return local;
        }
        
        // Si pas en local et online, récupérer depuis l'API
        if (_connectivity.IsOnline)
        {
            var remote = await _remoteApi.GetAsync<T>(id);
            if (remote != null)
            {
                await _localDb.UpsertAsync(remote);
            }
            return remote;
        }
        
        return null;
    }
    
    public async Task<T> CreateAsync(T entity)
    {
        // Sauvegarder localement d'abord
        entity.Id = Guid.NewGuid();
        entity.SyncStatus = SyncStatus.Pending;
        await _localDb.InsertAsync(entity);
        
        // Ajouter à la queue de sync
        await _syncQueue.EnqueueAsync(new SyncOperation
        {
            EntityId = entity.Id,
            EntityType = typeof(T).Name,
            Operation = OperationType.Create,
            Data = JsonSerializer.Serialize(entity)
        });
        
        // Tenter de synchroniser si online
        if (_connectivity.IsOnline)
        {
            _ = ProcessSyncQueueAsync();
        }
        
        return entity;
    }
    
    private async Task ProcessSyncQueueAsync()
    {
        var operations = await _syncQueue.GetPendingAsync();
        
        foreach (var op in operations)
        {
            try
            {
                switch (op.Operation)
                {
                    case OperationType.Create:
                        await _remoteApi.CreateAsync(op.Data);
                        break;
                    case OperationType.Update:
                        await _remoteApi.UpdateAsync(op.EntityId, op.Data);
                        break;
                    case OperationType.Delete:
                        await _remoteApi.DeleteAsync(op.EntityId);
                        break;
                }
                
                await _syncQueue.MarkAsCompletedAsync(op.Id);
            }
            catch (Exception ex)
            {
                await _syncQueue.MarkAsFailedAsync(op.Id, ex.Message);
            }
        }
    }
}
```

### 3.3 Real-time Updates Pattern

```csharp
public interface IRealtimeService
{
    Task ConnectAsync();
    Task DisconnectAsync();
    Task SubscribeToChannel<T>(string channel, Action<T> onMessage);
    Task UnsubscribeFromChannel(string channel);
    Task SendMessageAsync<T>(string channel, T message);
}

public class SupabaseRealtimeService : IRealtimeService
{
    private readonly Supabase.Client _supabaseClient;
    private readonly Dictionary<string, RealtimeChannel> _channels;
    
    public async Task SubscribeToChannel<T>(string channelName, Action<T> onMessage)
    {
        var channel = _supabaseClient.Realtime.Channel(channelName);
        
        channel
            .On<T>(RealtimeListenType.Insert, (sender, change) => onMessage(change.Model))
            .On<T>(RealtimeListenType.Update, (sender, change) => onMessage(change.Model))
            .On<T>(RealtimeListenType.Delete, (sender, change) => onMessage(change.Model));
        
        await channel.Subscribe();
        _channels[channelName] = channel;
    }
    
    public async Task SendMessageAsync<T>(string channel, T message)
    {
        await _supabaseClient
            .From<T>()
            .Insert(message);
    }
}

// Usage dans ViewModel
public partial class ChatViewModel : BaseViewModel
{
    private readonly IRealtimeService _realtimeService;
    
    [ObservableProperty]
    private ObservableCollection<Message> _messages;
    
    protected override async Task OnInitializeAsync()
    {
        await _realtimeService.SubscribeToChannel<Message>(
            $"conversation:{ConversationId}",
            OnNewMessage);
    }
    
    private void OnNewMessage(Message message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Add(message);
            ScrollToBottom();
        });
    }
}
```

---

## 4. PATTERNS DE PERFORMANCE

### 4.1 Lazy Loading Pattern

```csharp
public class LazyLoader<T>
{
    private readonly Func<Task<T>> _loader;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private T? _value;
    private bool _isLoaded;
    
    public LazyLoader(Func<Task<T>> loader)
    {
        _loader = loader;
    }
    
    public bool IsLoaded => _isLoaded;
    public T? Value => _value;
    
    public async Task<T> GetValueAsync()
    {
        if (_isLoaded)
            return _value!;
        
        await _semaphore.WaitAsync();
        try
        {
            if (!_isLoaded)
            {
                _value = await _loader();
                _isLoaded = true;
            }
            
            return _value!;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public void Reset()
    {
        _isLoaded = false;
        _value = default;
    }
}

// Usage dans un ViewModel
public partial class SpotDetailViewModel : BaseViewModel
{
    private readonly LazyLoader<List<Review>> _reviewsLoader;
    private readonly LazyLoader<WeatherInfo> _weatherLoader;
    
    public SpotDetailViewModel()
    {
        _reviewsLoader = new LazyLoader<List<Review>>(LoadReviewsAsync);
        _weatherLoader = new LazyLoader<WeatherInfo>(LoadWeatherAsync);
    }
    
    [RelayCommand]
    private async Task ShowReviewsAsync()
    {
        var reviews = await _reviewsLoader.GetValueAsync();
        // Afficher les reviews
    }
}
```

### 4.2 Virtual Scrolling Pattern

```csharp
public class VirtualizingObservableCollection<T> : ObservableCollection<T>, IDisposable
{
    private readonly Func<int, int, Task<IEnumerable<T>>> _fetchDataFunc;
    private readonly int _pageSize;
    private readonly Dictionary<int, IList<T>> _pages = new();
    private int _count = -1;
    
    public VirtualizingObservableCollection(
        Func<int, int, Task<IEnumerable<T>>> fetchDataFunc,
        int pageSize = 20)
    {
        _fetchDataFunc = fetchDataFunc;
        _pageSize = pageSize;
    }
    
    public override int Count
    {
        get
        {
            if (_count == -1)
            {
                LoadCount();
            }
            return _count;
        }
    }
    
    public new T this[int index]
    {
        get
        {
            int pageIndex = index / _pageSize;
            int pageOffset = index % _pageSize;
            
            RequestPage(pageIndex);
            
            if (_pages.ContainsKey(pageIndex))
            {
                if (_pages[pageIndex].Count > pageOffset)
                {
                    return _pages[pageIndex][pageOffset];
                }
            }
            
            return default(T);
        }
        set { throw new NotSupportedException(); }
    }
    
    private void RequestPage(int pageIndex)
    {
        if (!_pages.ContainsKey(pageIndex))
        {
            _pages[pageIndex] = new List<T>();
            LoadPage(pageIndex);
        }
    }
    
    private async void LoadPage(int pageIndex)
    {
        var data = await _fetchDataFunc(pageIndex * _pageSize, _pageSize);
        _pages[pageIndex] = data.ToList();
        
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Reset));
    }
    
    public void Dispose()
    {
        _pages.Clear();
    }
}
```

### 4.3 Image Caching Pattern

```csharp
public class ImageCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IFileService _fileService;
    private readonly HttpClient _httpClient;
    private readonly string _cacheDirectory;
    
    public ImageCacheService()
    {
        _cacheDirectory = Path.Combine(FileSystem.CacheDirectory, "images");
        Directory.CreateDirectory(_cacheDirectory);
    }
    
    public async Task<ImageSource> GetImageAsync(string url, ImageSize size = ImageSize.Medium)
    {
        // Check memory cache
        var memoryKey = $"{url}_{size}";
        if (_memoryCache.TryGetValue<byte[]>(memoryKey, out var cached))
        {
            return ImageSource.FromStream(() => new MemoryStream(cached));
        }
        
        // Check disk cache
        var fileName = GetCacheFileName(url, size);
        var filePath = Path.Combine(_cacheDirectory, fileName);
        
        if (File.Exists(filePath))
        {
            var fileData = await File.ReadAllBytesAsync(filePath);
            _memoryCache.Set(memoryKey, fileData, TimeSpan.FromMinutes(10));
            return ImageSource.FromFile(filePath);
        }
        
        // Download and cache
        var imageData = await DownloadAndResizeImageAsync(url, size);
        
        // Save to disk
        await File.WriteAllBytesAsync(filePath, imageData);
        
        // Save to memory
        _memoryCache.Set(memoryKey, imageData, TimeSpan.FromMinutes(10));
        
        return ImageSource.FromStream(() => new MemoryStream(imageData));
    }
    
    private async Task<byte[]> DownloadAndResizeImageAsync(string url, ImageSize size)
    {
        var originalData = await _httpClient.GetByteArrayAsync(url);
        
        // Resize using SkiaSharp
        using var original = SKBitmap.Decode(originalData);
        var targetWidth = GetTargetWidth(size);
        var scale = (float)targetWidth / original.Width;
        var targetHeight = (int)(original.Height * scale);
        
        using var resized = original.Resize(new SKImageInfo(targetWidth, targetHeight), 
                                           SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(resized);
        
        return image.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
    }
    
    private string GetCacheFileName(string url, ImageSize size)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(url));
        var hashString = BitConverter.ToString(hash).Replace("-", "");
        return $"{hashString}_{size}.jpg";
    }
    
    public async Task ClearCacheAsync()
    {
        _memoryCache.Clear();
        
        if (Directory.Exists(_cacheDirectory))
        {
            Directory.Delete(_cacheDirectory, true);
            Directory.CreateDirectory(_cacheDirectory);
        }
    }
}
```

---

## 5. PATTERNS DE SÉCURITÉ

### 5.1 Authentication Token Pattern

```csharp
public class SecureTokenManager
{
    private readonly ISecureStorage _secureStorage;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    private JwtSecurityToken? _accessToken;
    private string? _refreshToken;
    private Timer? _refreshTimer;
    
    public async Task<string?> GetValidAccessTokenAsync()
    {
        // Vérifier si le token est valide
        if (_accessToken != null && _accessToken.ValidTo > DateTime.UtcNow.AddMinutes(5))
        {
            return _accessToken.RawData;
        }
        
        // Refresh si nécessaire
        await _refreshSemaphore.WaitAsync();
        try
        {
            if (_accessToken == null || _accessToken.ValidTo <= DateTime.UtcNow.AddMinutes(5))
            {
                await RefreshTokensAsync();
            }
            
            return _accessToken?.RawData;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }
    
    private async Task RefreshTokensAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken))
        {
            _refreshToken = await _secureStorage.GetAsync("refresh_token");
        }
        
        if (string.IsNullOrEmpty(_refreshToken))
        {
            throw new UnauthorizedException("No refresh token available");
        }
        
        var response = await _authService.RefreshTokenAsync(_refreshToken);
        
        // Stocker les nouveaux tokens
        await StoreTokensAsync(response.AccessToken, response.RefreshToken);
        
        // Programmer le prochain refresh
        ScheduleTokenRefresh();
    }
    
    private void ScheduleTokenRefresh()
    {
        _refreshTimer?.Dispose();
        
        if (_accessToken != null)
        {
            var refreshIn = _accessToken.ValidTo.Subtract(DateTime.UtcNow)
                                                 .Subtract(TimeSpan.FromMinutes(5));
            
            if (refreshIn > TimeSpan.Zero)
            {
                _refreshTimer = new Timer(
                    async _ => await RefreshTokensAsync(),
                    null,
                    refreshIn,
                    Timeout.InfiniteTimeSpan);
            }
        }
    }
}
```

### 5.2 Input Validation Pattern

```csharp
public class InputSanitizer
{
    private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled);
    private static readonly Regex SqlInjectionRegex = new(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|UNION|ALTER|CREATE)\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        // Supprimer les tags HTML
        input = HtmlTagRegex.Replace(input, string.Empty);
        
        // Encoder les caractères spéciaux
        input = WebUtility.HtmlEncode(input);
        
        return input.Trim();
    }
    
    public string SanitizeForDatabase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        // Vérifier les patterns d'injection SQL
        if (SqlInjectionRegex.IsMatch(input))
        {
            throw new SecurityException("Potential SQL injection detected");
        }
        
        // Échapper les caractères spéciaux
        return input.Replace("'", "''").Trim();
    }
    
    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    public bool IsValidPassword(string password)
    {
        // Au moins 8 caractères, 1 majuscule, 1 minuscule, 1 chiffre, 1 caractère spécial
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }
}

// Validation Attributes
[AttributeUsage(AttributeTargets.Property)]
public class SanitizeHtmlAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string str)
        {
            var sanitizer = new InputSanitizer();
            var sanitized = sanitizer.SanitizeHtml(str);
            
            if (sanitized != str)
            {
                return new ValidationResult("Input contains invalid HTML");
            }
        }
        
        return ValidationResult.Success;
    }
}
```

---

## 6. PATTERNS DE TESTS

### 6.1 Test Data Builder Pattern

```csharp
public class SpotBuilder
{
    private string _name = "Test Spot";
    private Location _location = Location.Create(48.8566, 2.3522).Value;
    private DifficultyLevel _difficulty = DifficultyLevel.Intermediate;
    private SpotValidationStatus _status = SpotValidationStatus.Pending;
    private Guid _creatorId = Guid.NewGuid();
    
    public SpotBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public SpotBuilder WithLocation(double lat, double lon)
    {
        _location = Location.Create(lat, lon).Value;
        return this;
    }
    
    public SpotBuilder WithDifficulty(DifficultyLevel difficulty)
    {
        _difficulty = difficulty;
        return this;
    }
    
    public SpotBuilder AsApproved()
    {
        _status = SpotValidationStatus.Approved;
        return this;
    }
    
    public Spot Build()
    {
        var spot = Spot.Create(_name, _location, _difficulty, _creatorId).Value;
        
        // Utiliser la réflexion pour définir les propriétés privées si nécessaire
        if (_status != SpotValidationStatus.Pending)
        {
            typeof(Spot).GetProperty("ValidationStatus")!
                        .SetValue(spot, _status);
        }
        
        return spot;
    }
}

// Usage dans les tests
[Fact]
public void Spot_Should_Calculate_Distance_Correctly()
{
    // Arrange
    var spot1 = new SpotBuilder()
        .WithLocation(48.8566, 2.3522) // Paris
        .Build();
        
    var spot2 = new SpotBuilder()
        .WithLocation(51.5074, -0.1278) // Londres
        .Build();
    
    // Act
    var distance = spot1.Location.DistanceTo(spot2.Location);
    
    // Assert
    Assert.InRange(distance, 340, 350); // ~344km
}
```

### 6.2 Mock Repository Pattern

```csharp
public class MockRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> _data = new();
    
    public Task<T?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_data.TryGetValue(id, out var entity) ? entity : null);
    }
    
    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult(_data.Values.AsEnumerable());
    }
    
    public Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid();
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }
    
    public Task<T> UpdateAsync(T entity)
    {
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_data.Remove(id));
    }
    
    // Helper methods pour les tests
    public void AddTestData(params T[] entities)
    {
        foreach (var entity in entities)
        {
            _data[entity.Id] = entity;
        }
    }
    
    public void Clear()
    {
        _data.Clear();
    }
}
```

---

## 7. PATTERNS SPÉCIFIQUES MAUI

### 7.1 Platform-Specific Implementation Pattern

```csharp
// Interface commune
public interface IDeviceService
{
    Task<bool> RequestLocationPermissionAsync();
    Task<Location?> GetCurrentLocationAsync();
    void Vibrate(int milliseconds);
}

// Implémentation Android
#if ANDROID
public class AndroidDeviceService : IDeviceService
{
    public async Task<bool> RequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        
        return status == PermissionStatus.Granted;
    }
    
    public async Task<Location?> GetCurrentLocationAsync()
    {
        var location = await Geolocation.GetLocationAsync(new GeolocationRequest
        {
            DesiredAccuracy = GeolocationAccuracy.Best,
            Timeout = TimeSpan.FromSeconds(10)
        });
        
        return location != null 
            ? new Location(location.Latitude, location.Longitude) 
            : null;
    }
    
    public void Vibrate(int milliseconds)
    {
        var vibrator = Platform.CurrentActivity?.GetSystemService(Context.VibratorService) 
                      as Vibrator;
        vibrator?.Vibrate(VibrationEffect.CreateOneShot(milliseconds, 
                          VibrationEffect.DefaultAmplitude));
    }
}
#endif

// Implémentation iOS
#if IOS
public class IOSDeviceService : IDeviceService
{
    public async Task<bool> RequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        
        if (status != PermissionStatus.Granted)
        {
            // iOS specific: Show custom alert before system prompt
            var result = await Application.Current.MainPage.DisplayAlert(
                "Autorisation requise",
                "SubExplore a besoin d'accéder à votre position pour afficher les spots à proximité.",
                "Autoriser",
                "Annuler");
                
            if (result)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }
        
        return status == PermissionStatus.Granted;
    }
    
    // ... autres méthodes
}
#endif

// Registration dans MauiProgram
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    
#if ANDROID
    builder.Services.AddSingleton<IDeviceService, AndroidDeviceService>();
#elif IOS
    builder.Services.AddSingleton<IDeviceService, IOSDeviceService>();
#endif
    
    return builder.Build();
}
```

### 7.2 Custom Control Pattern

```csharp
public class RatingControl : ContentView
{
    public static readonly BindableProperty RatingProperty = BindableProperty.Create(
        nameof(Rating),
        typeof(double),
        typeof(RatingControl),
        0.0,
        propertyChanged: OnRatingChanged);
    
    public static readonly BindableProperty MaxRatingProperty = BindableProperty.Create(
        nameof(MaxRating),
        typeof(int),
        typeof(RatingControl),
        5);
    
    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(RatingControl),
        false);
    
    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }
    
    public int MaxRating
    {
        get => (int)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }
    
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
    
    private readonly StackLayout _starsContainer;
    
    public RatingControl()
    {
        _starsContainer = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 5
        };
        
        Content = _starsContainer;
        UpdateStars();
    }
    
    private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is RatingControl control)
        {
            control.UpdateStars();
        }
    }
    
    private void UpdateStars()
    {
        _starsContainer.Children.Clear();
        
        for (int i = 1; i <= MaxRating; i++)
        {
            var star = new Image
            {
                WidthRequest = 24,
                HeightRequest = 24,
                Source = GetStarImage(i)
            };
            
            if (!IsReadOnly)
            {
                var tapGesture = new TapGestureRecognizer();
                var starIndex = i;
                tapGesture.Tapped += (s, e) => Rating = starIndex;
                star.GestureRecognizers.Add(tapGesture);
            }
            
            _starsContainer.Children.Add(star);
        }
    }
    
    private ImageSource GetStarImage(int position)
    {
        if (position <= Math.Floor(Rating))
            return "star_filled.png";
        else if (position - 0.5 <= Rating)
            return "star_half.png";
        else
            return "star_empty.png";
    }
}
```

---

## 8. ANTI-PATTERNS À ÉVITER

### 8.1 Anti-Pattern: God Object

❌ **À éviter:**
```csharp
public class AppManager
{
    // Trop de responsabilités dans une seule classe
    public void LoginUser(string email, string password) { }
    public void CreateSpot(Spot spot) { }
    public void SendMessage(Message message) { }
    public void ProcessPayment(Payment payment) { }
    public void ValidateSpot(Guid spotId) { }
    public void GenerateReport(ReportType type) { }
    // ... 100+ autres méthodes
}
```

✅ **Solution: Single Responsibility**
```csharp
public class AuthenticationService { }
public class SpotService { }
public class MessagingService { }
public class PaymentService { }
public class ValidationService { }
public class ReportingService { }
```

### 8.2 Anti-Pattern: Anemic Domain Model

❌ **À éviter:**
```csharp
// Entité sans logique métier
public class Spot
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
}

// Logique métier dans les services
public class SpotService
{
    public void ApproveSpot(Spot spot)
    {
        if (spot.Status != "Pending")
            throw new Exception("Cannot approve");
        
        spot.Status = "Approved";
        // ...
    }
}
```

✅ **Solution: Rich Domain Model**
```csharp
public class Spot
{
    public SpotId Id { get; private set; }
    public string Name { get; private set; }
    public SpotStatus Status { get; private set; }
    
    public Result Approve(UserId validatorId)
    {
        if (Status != SpotStatus.Pending)
            return Result.Failure("Spot must be pending to approve");
        
        Status = SpotStatus.Approved;
        AddDomainEvent(new SpotApprovedEvent(Id, validatorId));
        
        return Result.Success();
    }
}
```

### 8.3 Anti-Pattern: Circular Dependencies

❌ **À éviter:**
```csharp
public class UserService
{
    private readonly SpotService _spotService;
    
    public UserService(SpotService spotService)
    {
        _spotService = spotService;
    }
}

public class SpotService
{
    private readonly UserService _userService;
    
    public SpotService(UserService userService)
    {
        _userService = userService;
    }
}
```

✅ **Solution: Dependency Inversion**
```csharp
public interface IUserProvider
{
    Task<User> GetUserAsync(Guid id);
}

public interface ISpotProvider
{
    Task<Spot> GetSpotAsync(Guid id);
}

public class UserService : IUserProvider
{
    private readonly ISpotProvider _spotProvider;
}

public class SpotService : ISpotProvider
{
    private readonly IUserProvider _userProvider;
}
```

---

## CONCLUSION

Ce guide présente les patterns essentiels pour une architecture robuste et maintenable de SubExplore. L'application cohérente de ces patterns garantira:

- **Maintenabilité**: Code facile à comprendre et modifier
- **Testabilité**: Composants isolés et testables
- **Scalabilité**: Architecture évolutive
- **Performance**: Optimisations intégrées
- **Sécurité**: Patterns de sécurité appliqués systématiquement

### Checklist d'Architecture

- [ ] Clean Architecture respectée
- [ ] MVVM correctement implémenté
- [ ] Injection de dépendances configurée
- [ ] Repository Pattern pour l'accès aux données
- [ ] Caching stratégique en place
- [ ] Gestion d'erreurs cohérente
- [ ] Tests unitaires pour la logique métier
- [ ] Sécurité validée
- [ ] Performance optimisée
- [ ] Documentation à jour

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Statut**: Guide de référence architecture
**Mise à jour**: À maintenir pendant le développement

*Ce document doit être utilisé comme référence pour toutes les décisions d'architecture dans SubExplore.*