# SubExplore.Application

## ⚙️ Application Layer - Clean Architecture

Ce projet contient la **logique applicative** et les **use cases** de SubExplore.

### Responsabilités

- **Commands & Queries (CQRS)** : Opérations de lecture et d'écriture séparées
- **Handlers MediatR** : Traitement des commandes et requêtes
- **DTOs** : Objets de transfert de données
- **Validators** : Validation avec FluentValidation
- **Mappings** : AutoMapper pour les conversions

### Architecture CQRS

```
Commands (Write)          Queries (Read)
     ↓                         ↓
  Handler                   Handler
     ↓                         ↓
 Repository               Repository
     ↓                         ↓
  Database ←────────────────────
```

### Structure

```
SubExplore.Application/
├── Commands/              # Commandes d'écriture
│   ├── Spots/
│   ├── DiveLogs/
│   └── Users/
├── Queries/               # Requêtes de lecture
│   ├── Spots/
│   ├── DiveLogs/
│   └── Users/
├── DTOs/                  # Data Transfer Objects
├── Validators/            # FluentValidation validators
├── Mappings/              # AutoMapper profiles
├── Interfaces/            # Interfaces de services
├── Services/              # Services applicatifs
└── Common/                # Classes communes (PaginatedResult, etc.)
```

### Exemple de Command

```csharp
// Command
public record CreateSpotCommand(
    string Name,
    double Latitude,
    double Longitude,
    int MaxDepth
) : IRequest<Result<SpotDto>>;

// Handler
public class CreateSpotCommandHandler
    : IRequestHandler<CreateSpotCommand, Result<SpotDto>>
{
    private readonly ISpotRepository _repository;

    public async Task<Result<SpotDto>> Handle(
        CreateSpotCommand request,
        CancellationToken ct)
    {
        // Validation, création, sauvegarde
    }
}
```

### Dépendances

- ✅ **Domain** : Utilise les entités et interfaces
- ❌ **Infrastructure** : Ne connaît pas l'implémentation
- ❌ **API/Mobile** : Indépendant de la couche présentation

### Packages NuGet

- MediatR
- FluentValidation
- AutoMapper
- ErrorOr (gestion d'erreurs)
