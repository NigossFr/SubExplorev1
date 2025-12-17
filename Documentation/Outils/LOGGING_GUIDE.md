# 📝 Guide de Configuration Logging avec Serilog - SubExplore

## 📋 Table des matières

1. [Présentation](#présentation)
2. [Architecture du Logging](#architecture-du-logging)
3. [Configuration API](#configuration-api)
4. [Configuration Mobile](#configuration-mobile)
5. [Niveaux de Log](#niveaux-de-log)
6. [Utilisation dans le Code](#utilisation-dans-le-code)
7. [Formats de Sortie](#formats-de-sortie)
8. [Fichiers de Logs](#fichiers-de-logs)
9. [Enrichers](#enrichers)
10. [Bonnes Pratiques](#bonnes-pratiques)
11. [Dépannage](#dépannage)

---

## 🎯 Présentation

SubExplore utilise **Serilog** comme système de logging structuré pour l'API et l'application mobile. Serilog offre :

- **Logging structuré** : Les logs contiennent des propriétés typées
- **Multiple sinks** : Console, fichiers, services externes
- **Configuration flexible** : Via appsettings.json ou code
- **Performance** : Logging asynchrone et efficient
- **Enrichment** : Ajout automatique de contexte

### Packages Installés

**API (SubExplore.API)** :
```xml
<PackageReference Include="Serilog.AspNetCore" Version="10.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.1.1" />
<PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
```

**Mobile (SubExplore)** :
```xml
<PackageReference Include="Serilog.Extensions.Logging" Version="10.0.0" />
<PackageReference Include="Serilog.Sinks.Debug" Version="3.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
```

---

## 🏗️ Architecture du Logging

### Vue d'ensemble

```
┌─────────────────────────────────────────────────────────────┐
│                    SubExplore Logging                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────┐              ┌─────────────────┐      │
│  │   API (ASP.NET) │              │  Mobile (MAUI)  │      │
│  │                 │              │                 │      │
│  │  Serilog        │              │  Serilog        │      │
│  │  AspNetCore     │              │  Extensions     │      │
│  └────────┬────────┘              └────────┬────────┘      │
│           │                                │               │
│           ├─── Console                     ├─── Debug      │
│           │                                │               │
│           └─── File (logs/subexplore-.log) └─── File       │
│                                        (AppData/logs/)     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Niveaux de Log par Environnement

| Environnement | Niveau par défaut | Override Microsoft | Destination |
|---------------|-------------------|-------------------|-------------|
| **API Production** | Information | Warning | Console + File (30 jours) |
| **API Development** | Debug | Information | Console + File (7 jours) |
| **Mobile Debug** | Debug | Warning | Debug + File (7 jours) |
| **Mobile Release** | Information | Warning | File (7 jours) |

---

## ⚙️ Configuration API

### Program.cs

Le fichier `SubExplore.API/Program.cs` configure Serilog au démarrage :

```csharp
using Serilog;
using Serilog.Events;

// Bootstrap logger (avant chargement configuration)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SubExplore API");

    var builder = WebApplication.CreateBuilder(args);

    // Configuration Serilog depuis appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/subexplore-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

    // ... reste de la configuration

    var app = builder.Build();

    // Logging des requêtes HTTP
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : LogEventLevel.Information;
    });

    // ... reste de la configuration

    Log.Information("SubExplore API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SubExplore API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### appsettings.json

Configuration de production dans `SubExplore.API/appsettings.json` :

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/subexplore-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ],
    "Properties": {
      "Application": "SubExplore.API"
    }
  }
}
```

### appsettings.Development.json

Configuration de développement dans `SubExplore.API/appsettings.Development.json` :

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Information",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/subexplore-dev-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithThreadId", "WithMachineName" ]
  }
}
```

---

## 📱 Configuration Mobile

### MauiProgram.cs

Le fichier `MauiProgram.cs` configure Serilog pour l'application mobile :

```csharp
using Serilog;
using Serilog.Events;

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
}
```

### Emplacement des Logs Mobile

Les logs mobile sont stockés dans le dossier AppData de l'application :

- **Android** : `/data/data/com.companyname.subexplore/files/logs/`
- **iOS** : `~/Library/Application Support/logs/`
- **Windows** : `%LOCALAPPDATA%\Packages\[PackageId]\LocalState\logs\`
- **macOS** : `~/Library/Containers/[BundleId]/Data/Library/Application Support/logs/`

Pour accéder aux logs mobile, utilisez :
```csharp
var logsPath = Path.Combine(FileSystem.AppDataDirectory, "logs");
Log.Information("Logs directory: {LogsPath}", logsPath);
```

---

## 📊 Niveaux de Log

Serilog utilise 6 niveaux de log, du plus verbeux au plus critique :

| Niveau | Utilisation | Exemple |
|--------|-------------|---------|
| **Verbose** | Détails de débogage très détaillés | Valeurs de variables dans une boucle |
| **Debug** | Informations de débogage | Entrée/sortie de méthodes, paramètres |
| **Information** | Messages informatifs normaux | Démarrage de l'app, opérations réussies |
| **Warning** | Situations anormales non critiques | Ressource indisponible, retry |
| **Error** | Erreurs récupérables | Exceptions gérées, échecs d'opérations |
| **Fatal** | Erreurs critiques nécessitant arrêt | Corruption de données, échec démarrage |

### Choix du Niveau

```csharp
// ✅ BON : Utiliser le bon niveau selon la situation
Log.Debug("User {UserId} requested data with filter {Filter}", userId, filter);
Log.Information("User {UserId} logged in successfully", userId);
Log.Warning("API rate limit approached: {Current}/{Limit}", current, limit);
Log.Error(ex, "Failed to process payment for order {OrderId}", orderId);
Log.Fatal(ex, "Database connection failed, application cannot start");

// ❌ MAUVAIS : Tout logger en Information
Log.Information("Debug: x = 5");  // Utiliser Debug
Log.Information("Error occurred"); // Utiliser Error avec exception
```

---

## 💻 Utilisation dans le Code

### Injection de Dépendance

Dans les contrôleurs, services et ViewModels :

```csharp
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        _logger.LogInformation("Fetching user {UserId}", id);

        try
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("User {UserId} retrieved successfully", id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500);
        }
    }
}
```

### Logging Structuré

Serilog capture les propriétés typées automatiquement :

```csharp
// ✅ BON : Logging structuré avec propriétés
var userId = Guid.NewGuid();
var userName = "john.doe";
var loginTime = DateTime.UtcNow;

Log.Information("User {UserId} with name {UserName} logged in at {LoginTime}",
    userId, userName, loginTime);

// Résultat : Les propriétés sont capturées et indexables
// {
//   "UserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "UserName": "john.doe",
//   "LoginTime": "2024-12-10T15:30:00Z"
// }

// ❌ MAUVAIS : Interpolation de string (perd le typage)
Log.Information($"User {userId} with name {userName} logged in at {loginTime}");
// Résultat : Tout est une chaîne de caractères, non queryable
```

### Logging d'Exceptions

```csharp
try
{
    await RiskyOperation();
}
catch (InvalidOperationException ex)
{
    // ✅ BON : Passer l'exception en premier paramètre
    _logger.LogError(ex, "Invalid operation while processing {Operation}", operationName);
}
catch (Exception ex)
{
    // ❌ MAUVAIS : Ne pas logger l'exception
    _logger.LogError("Error: " + ex.Message);

    // ✅ BON : Toujours passer l'exception
    _logger.LogError(ex, "Unexpected error during operation");
}
```

### Scopes de Logging

Utiliser des scopes pour enrichir tous les logs dans un contexte :

```csharp
using (_logger.BeginScope("Processing order {OrderId}", orderId))
{
    _logger.LogInformation("Validating order");
    await ValidateOrder(orderId);

    _logger.LogInformation("Charging payment");
    await ChargePayment(orderId);

    _logger.LogInformation("Shipping order");
    await ShipOrder(orderId);
}
// Tous les logs ci-dessus auront automatiquement OrderId dans leurs propriétés
```

### Logging Conditionnel

```csharp
// Éviter les opérations coûteuses si le log ne sera pas écrit
if (_logger.IsEnabled(LogLevel.Debug))
{
    var expensiveData = ComputeExpensiveDebugInfo();
    _logger.LogDebug("Debug data: {Data}", expensiveData);
}
```

---

## 📝 Formats de Sortie

### Templates de Console

**Production** :
```
[15:30:45 INF] HTTP GET /api/users/123 responded 200 in 45.2345 ms
```

**Development** (avec SourceContext) :
```
[15:30:45 DBG] SubExplore.API.Controllers.UserController User 123 requested with filter "active" {"UserId": 123, "Filter": "active"}
```

### Templates de Fichier

**Format avec timestamp complet** :
```
[2024-12-10 15:30:45.234 +01:00 INF] User 123 logged in successfully {"UserId": 123, "UserName": "john.doe"}
```

### Propriétés des Templates

| Propriété | Description | Exemple |
|-----------|-------------|---------|
| `{Timestamp}` | Date et heure | `2024-12-10 15:30:45.234` |
| `{Timestamp:HH:mm:ss}` | Heure simple | `15:30:45` |
| `{Level}` | Niveau de log | `Information` |
| `{Level:u3}` | Niveau abrégé | `INF` |
| `{Message:lj}` | Message (JSON escaped) | `User "john" logged in` |
| `{Properties:j}` | Propriétés en JSON | `{"UserId": 123}` |
| `{SourceContext}` | Classe qui log | `UserController` |
| `{Exception}` | Stack trace exception | Full exception details |
| `{NewLine}` | Retour à la ligne | `\n` |

---

## 📂 Fichiers de Logs

### Organisation des Fichiers

```
SubExplore.API/
├── logs/
│   ├── subexplore-20241210.log       # Production
│   ├── subexplore-20241211.log
│   ├── subexplore-dev-20241210.log   # Development
│   └── subexplore-dev-20241211.log

Mobile AppData/
└── logs/
    ├── subexplore-mobile-20241210.log
    └── subexplore-mobile-20241211.log
```

### Rolling Interval

Les logs sont automatiquement "roulés" (archivés) selon l'intervalle configuré :

- **Day** : Un nouveau fichier par jour (`subexplore-20241210.log`)
- **Hour** : Un nouveau fichier par heure
- **Month** : Un nouveau fichier par mois

### Rétention

Les anciens logs sont automatiquement supprimés :

- **API Production** : 30 jours de rétention
- **API Development** : 7 jours de rétention
- **Mobile** : 7 jours de rétention

### Rotation Manuelle

Si nécessaire, forcer la rotation :

```bash
# Supprimer les logs de plus de 7 jours (API)
find logs/ -name "subexplore-*.log" -mtime +7 -delete

# Supprimer les logs de plus de 30 jours (Production)
find logs/ -name "subexplore-*.log" -mtime +30 -delete
```

---

## 🎨 Enrichers

Les enrichers ajoutent automatiquement des propriétés à tous les logs.

### Enrichers Configurés

**API** :
- `FromLogContext` : Propriétés du contexte de log (scopes)
- `WithThreadId` (Dev) : ID du thread
- `WithMachineName` (Dev) : Nom de la machine
- `Application = "SubExplore.API"` : Identifiant de l'application

**Mobile** :
- `FromLogContext` : Propriétés du contexte de log
- `Application = "SubExplore.Mobile"` : Identifiant de l'application

### Enrichers Personnalisés

Créer un enricher custom :

```csharp
public class UserIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var property = propertyFactory.CreateProperty("UserId", userId);
                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}

// Configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .Enrich.With(new UserIdEnricher(services.GetRequiredService<IHttpContextAccessor>()))
    // ... reste de la configuration
);
```

---

## ✅ Bonnes Pratiques

### 1. Logging Structuré

```csharp
// ✅ BON
Log.Information("User {UserId} updated profile with {Changes} changes", userId, changeCount);

// ❌ MAUVAIS
Log.Information($"User {userId} updated profile with {changeCount} changes");
Log.Information("User " + userId + " updated profile");
```

### 2. Niveaux Appropriés

```csharp
// ✅ BON : Debug pour détails internes
Log.Debug("Validating input: {Input}", input);

// ✅ BON : Information pour événements normaux
Log.Information("User {UserId} created", userId);

// ✅ BON : Warning pour situations inhabituelles
Log.Warning("Cache miss for key {Key}, fetching from database", key);

// ✅ BON : Error pour erreurs récupérables
Log.Error(ex, "Failed to send email to {Email}, will retry", email);

// ✅ BON : Fatal pour erreurs critiques
Log.Fatal(ex, "Database unavailable, shutting down");
```

### 3. Ne Pas Logger de Données Sensibles

```csharp
// ❌ MAUVAIS : Logger des mots de passe, tokens, données personnelles
Log.Information("User logged in with password {Password}", password);
Log.Information("Payment with card {CardNumber}", cardNumber);

// ✅ BON : Masquer ou omettre les données sensibles
Log.Information("User {UserId} logged in successfully", userId);
Log.Information("Payment processed for card ending in {Last4Digits}", last4);
```

### 4. Contexte Suffisant

```csharp
// ❌ MAUVAIS : Pas assez de contexte
Log.Error("Save failed");

// ✅ BON : Contexte clair
Log.Error(ex, "Failed to save user profile for {UserId}", userId);
```

### 5. Éviter le Logging Excessif

```csharp
// ❌ MAUVAIS : Logger dans une boucle
foreach (var item in items)
{
    Log.Debug("Processing item {ItemId}", item.Id);
    ProcessItem(item);
}

// ✅ BON : Logger le résumé
Log.Debug("Processing {ItemCount} items", items.Count);
foreach (var item in items)
{
    ProcessItem(item);
}
Log.Debug("Completed processing {ItemCount} items", items.Count);
```

### 6. Utiliser des Scopes

```csharp
// ✅ BON : Scope pour enrichir automatiquement
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CustomerId"] = customerId
}))
{
    _logger.LogInformation("Processing payment");
    // OrderId et CustomerId sont automatiquement ajoutés

    _logger.LogInformation("Validating inventory");
    // OrderId et CustomerId sont automatiquement ajoutés
}
```

### 7. Performance

```csharp
// ❌ MAUVAIS : Opération coûteuse toujours exécutée
Log.Debug("Complex data: {Data}", CalculateComplexData());

// ✅ BON : Vérifier le niveau avant l'opération
if (_logger.IsEnabled(LogLevel.Debug))
{
    var data = CalculateComplexData();
    _logger.LogDebug("Complex data: {Data}", data);
}
```

---

## 🔧 Dépannage

### Problème : Les Logs N'apparaissent Pas

**Solution 1** : Vérifier le niveau de log

```json
// Dans appsettings.json
"Serilog": {
  "MinimumLevel": {
    "Default": "Debug"  // Baisser à Debug temporairement
  }
}
```

**Solution 2** : Vérifier que Serilog est initialisé

```csharp
// Dans Program.cs ou MauiProgram.cs
Log.Information("Test log message");
```

**Solution 3** : Vérifier les permissions du dossier logs

```bash
# Windows
icacls logs /grant Everyone:(OI)(CI)F

# Linux/Mac
chmod 755 logs
```

### Problème : Les Fichiers de Logs Deviennent Trop Volumineux

**Solution 1** : Réduire la rétention

```json
"File": {
  "Args": {
    "retainedFileCountLimit": 7  // Au lieu de 30
  }
}
```

**Solution 2** : Augmenter le niveau minimum

```json
"MinimumLevel": {
  "Default": "Warning"  // Au lieu de Information
}
```

**Solution 3** : Utiliser un rolling interval plus court

```json
"File": {
  "Args": {
    "rollingInterval": "Hour"  // Au lieu de Day
  }
}
```

### Problème : Logs Mobile Inaccessibles

**Android** : Utiliser `adb` pour récupérer les logs

```bash
# Récupérer le chemin AppData
adb shell run-as com.companyname.subexplore pwd

# Télécharger les logs
adb pull /data/data/com.companyname.subexplore/files/logs/ ./mobile-logs/
```

**iOS** : Utiliser Xcode

1. Window → Devices and Simulators
2. Sélectionner l'appareil
3. Cliquer sur l'app → Download Container
4. Naviguer vers AppData/Library/Application Support/logs/

**Solution Alternative** : Implémenter un endpoint pour télécharger les logs

```csharp
// Dans un ViewModel ou Service
public async Task<string> GetLogsContent()
{
    var logsPath = Path.Combine(FileSystem.AppDataDirectory, "logs");
    var logFiles = Directory.GetFiles(logsPath, "*.log");

    if (logFiles.Length == 0)
        return "No logs found";

    var latestLog = logFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
    return await File.ReadAllTextAsync(latestLog);
}
```

### Problème : Logs Non Structurés (Interpolation de String)

```csharp
// ❌ MAUVAIS
Log.Information($"User {userId} logged in");

// ✅ BON
Log.Information("User {UserId} logged in", userId);
```

**Rechercher les logs non structurés** :

```bash
# Rechercher les logs avec interpolation de string
grep -r "Log.*\$\"" --include="*.cs"
```

### Problème : Perte de Logs au Shutdown

**Solution** : Toujours flush avant de quitter

```csharp
// API Program.cs
finally
{
    Log.CloseAndFlush();  // ✅ IMPORTANT
}

// Mobile App.xaml.cs
protected override void OnSleep()
{
    Log.CloseAndFlush();
}
```

---

## 📚 Ressources

### Documentation Officielle

- [Serilog Documentation](https://github.com/serilog/serilog/wiki)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
- [Configuration from appsettings.json](https://github.com/serilog/serilog-settings-configuration)

### Packages Utiles

- **Serilog.Enrichers.Thread** : Enrichment avec thread ID
- **Serilog.Enrichers.Environment** : Enrichment avec nom machine, utilisateur
- **Serilog.Enrichers.Process** : Enrichment avec process ID
- **Serilog.Sinks.Seq** : Sink pour Seq (serveur de logs centralisé)
- **Serilog.Sinks.Elasticsearch** : Sink pour Elasticsearch

---

## ✅ Checklist de Configuration

- [x] Packages Serilog installés (API + Mobile)
- [x] Program.cs configuré avec Serilog (API)
- [x] MauiProgram.cs configuré avec Serilog (Mobile)
- [x] appsettings.json configuré (API)
- [x] appsettings.Development.json configuré (API)
- [x] Dossier `logs/` ajouté au `.gitignore`
- [x] Enrichers configurés (FromLogContext, Application)
- [x] Request logging activé (API)
- [x] Bootstrap logger configuré (API)
- [x] Log.CloseAndFlush() dans finally (API)
- [ ] Tests de logging effectués
- [ ] Vérification des fichiers de logs générés
- [ ] Documentation partagée avec l'équipe

---

**Dernière mise à jour** : 2024-12-10
**Version** : 1.0
**Auteur** : SubExplore Development Team
