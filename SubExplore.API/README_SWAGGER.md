# Configuration Swagger/OpenAPI - SubExplore API

## Vue d'ensemble

L'API SubExplore utilise **Swagger/OpenAPI** pour documenter automatiquement tous les endpoints de l'API et fournir une interface interactive pour les tester.

## Technologies utilisées

- **Swashbuckle.AspNetCore 7.2.0** : Génération automatique de documentation OpenAPI
- **Microsoft.AspNetCore.OpenApi 9.0.10** : Support OpenAPI pour .NET 9
- **XML Documentation** : Commentaires XML pour enrichir la documentation

## Accès à Swagger UI

### En développement

L'interface Swagger UI est accessible à l'adresse :

```
https://localhost:5001/swagger
http://localhost:5000/swagger
```

### Fonctionnalités activées

- ✅ Documentation complète de tous les endpoints
- ✅ Interface interactive pour tester les endpoints
- ✅ Affichage de la durée des requêtes
- ✅ Deep linking (liens directs vers les endpoints)
- ✅ Filtrage des endpoints
- ✅ Validation des requêtes
- ✅ Support JWT Bearer Authentication
- ✅ Commentaires XML dans la documentation

## Configuration

### Swagger Generation (Program.cs)

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // API Information
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "SubExplore API",
        Description = "API pour l'application SubExplore - Gestion de plongées sous-marines",
        Contact = new OpenApiContact
        {
            Name = "SubExplore Development Team",
            Email = "dev@subexplore.com",
            Url = new Uri("https://github.com/subexplore")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez 'Bearer' suivi d'un espace et du token JWT."
    });
});
```

### Swagger UI (Program.cs)

```csharp
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SubExplore API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "SubExplore API Documentation";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
    options.EnableValidator();
});
```

### Documentation XML (.csproj)

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn> <!-- Suppress missing XML comment warnings -->
</PropertyGroup>
```

## Utilisation

### Documenter un Controller

```csharp
/// <summary>
/// Gestion des plongeurs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DiversController : ControllerBase
{
    /// <summary>
    /// Récupère tous les plongeurs.
    /// </summary>
    /// <returns>Liste des plongeurs.</returns>
    /// <response code="200">Liste des plongeurs récupérée avec succès.</response>
    /// <response code="401">Non autorisé - Token JWT manquant ou invalide.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DiverDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DiverDto>>> GetAllDivers()
    {
        // Implementation...
    }

    /// <summary>
    /// Récupère un plongeur par son ID.
    /// </summary>
    /// <param name="id">ID du plongeur.</param>
    /// <returns>Le plongeur demandé.</returns>
    /// <response code="200">Plongeur trouvé.</response>
    /// <response code="404">Plongeur non trouvé.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DiverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiverDto>> GetDiverById(Guid id)
    {
        // Implementation...
    }
}
```

### Tester avec Authentication JWT

1. Obtenir un token JWT via l'endpoint `/api/auth/login`
2. Dans Swagger UI, cliquer sur le bouton **"Authorize"** 🔒
3. Entrer : `Bearer <votre-token-jwt>`
4. Cliquer sur **"Authorize"**
5. Les requêtes suivantes incluront automatiquement le token

## Bonnes pratiques

### Documentation XML

- ✅ Documenter tous les endpoints publics
- ✅ Utiliser `<summary>` pour la description courte
- ✅ Utiliser `<param>` pour décrire les paramètres
- ✅ Utiliser `<returns>` pour décrire le retour
- ✅ Utiliser `<response>` pour documenter les codes de statut

### ProducesResponseType

- ✅ Spécifier tous les codes de statut possibles
- ✅ Inclure le type de retour pour les codes 2xx
- ✅ Documenter les erreurs (400, 401, 404, 500)

### Organisation

- ✅ Grouper les endpoints par tags logiques
- ✅ Utiliser des noms d'opération explicites (OperationId)
- ✅ Fournir des exemples de requêtes/réponses

## Désactivation en Production

Par défaut, Swagger est **uniquement activé en développement** :

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(/* ... */);
}
```

Pour activer en production (déconseillé sauf API publique) :

```csharp
app.UseSwagger();
app.UseSwaggerUI(/* ... */);
```

## Export de la spécification OpenAPI

La spécification OpenAPI est disponible en JSON :

```
https://localhost:5001/swagger/v1/swagger.json
```

Cette spécification peut être utilisée pour :
- Générer des clients API (C#, TypeScript, etc.)
- Importer dans Postman
- Générer de la documentation statique
- Tester automatiquement l'API

## Ressources

- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Core Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

---

**Dernière mise à jour** : 2025-12-11
**Version** : 1.0
**Status** : Configuration complétée et opérationnelle
