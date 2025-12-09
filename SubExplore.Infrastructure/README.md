# SubExplore.Infrastructure

## 🔧 Infrastructure Layer - Clean Architecture

Ce projet contient les **implémentations techniques** et l'accès aux ressources externes.

### Responsabilités

- **Repositories** : Implémentation de l'accès aux données (Supabase)
- **Services externes** : APIs météo, marées, géolocalisation
- **Storage** : Gestion des fichiers (photos, avatars)
- **Authentification** : Intégration Supabase Auth
- **Cache** : Implémentation du caching

### Structure

```
SubExplore.Infrastructure/
├── Persistence/
│   └── Repositories/      # Implémentations des repositories
├── Services/              # Services d'infrastructure
│   ├── Storage/           # Upload/download fichiers
│   ├── Email/             # Envoi d'emails
│   └── Cache/             # Gestion du cache
├── External/              # APIs externes
│   ├── Weather/           # API météo
│   ├── Tides/             # API marées
│   └── Geolocation/       # Services de géolocalisation
└── Configuration/         # Configuration des services
```

### Exemple de Repository

```csharp
public class SpotRepository : ISpotRepository
{
    private readonly Supabase.Client _supabase;

    public async Task<DivingSpot?> GetByIdAsync(Guid id)
    {
        var response = await _supabase
            .From<SpotDto>()
            .Where(x => x.Id == id)
            .Single();

        return response?.ToEntity();
    }
}
```

### Technologies

- **Supabase** : Base de données PostgreSQL + PostGIS
- **Supabase Storage** : Stockage de fichiers
- **Supabase Auth** : Authentification JWT
- **External APIs** : OpenWeatherMap, etc.

### Dépendances

- ✅ **Domain** : Implémente les interfaces du domaine
- ❌ **Application** : Ne dépend pas de l'Application
- ❌ **API/Mobile** : Indépendant de la présentation

### Packages NuGet

- Supabase (>= 1.0)
- Npgsql (PostgreSQL)
- NetTopologySuite (PostGIS)
