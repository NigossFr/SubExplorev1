# SubExplore.API

## 🌐 API Layer - ASP.NET Core Web API

API REST pour l'application mobile SubExplore.

### Responsabilités

- **Endpoints REST** : Exposition des use cases via HTTP
- **Authentification JWT** : Sécurisation des endpoints
- **Validation** : Validation des inputs
- **Documentation** : Swagger/OpenAPI
- **Middlewares** : Gestion d'erreurs, logging, CORS

### Structure

```
SubExplore.API/
├── Controllers/           # Endpoints REST
│   ├── AuthController.cs
│   ├── SpotsController.cs
│   ├── DiveLogsController.cs
│   ├── EventsController.cs
│   └── UsersController.cs
├── Middleware/            # Middlewares personnalisés
│   ├── ExceptionHandlerMiddleware.cs
│   └── JwtMiddleware.cs
├── Extensions/            # Extensions de configuration
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
└── Filters/               # Action filters
```

### Endpoints Principaux

#### Authentification
- `POST /api/auth/register` - Inscription
- `POST /api/auth/login` - Connexion
- `POST /api/auth/refresh` - Rafraîchir le token

#### Spots de plongée
- `GET /api/spots` - Liste des spots (avec filtres)
- `GET /api/spots/nearby?lat={lat}&lng={lng}&radius={radius}` - Spots à proximité
- `GET /api/spots/{id}` - Détails d'un spot
- `POST /api/spots` - Créer un spot
- `PUT /api/spots/{id}` - Modifier un spot
- `DELETE /api/spots/{id}` - Supprimer un spot

#### Carnet de plongée
- `GET /api/divelogs` - Liste des plongées
- `GET /api/divelogs/{id}` - Détails d'une plongée
- `POST /api/divelogs` - Ajouter une plongée
- `GET /api/divelogs/statistics` - Statistiques

### Configuration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Middleware pipeline
app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Sécurité

- ✅ **HTTPS only** en production
- ✅ **JWT** pour l'authentification
- ✅ **CORS** configuré
- ✅ **Rate limiting** pour prévenir les abus
- ✅ **Validation** des inputs

### Packages NuGet

- Swashbuckle.AspNetCore (Swagger)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Serilog (Logging)
- AspNetCoreRateLimit
