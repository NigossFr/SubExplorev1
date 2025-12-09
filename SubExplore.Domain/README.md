# SubExplore.Domain

## 📦 Domain Layer - Clean Architecture

Ce projet contient la **logique métier pure** de l'application SubExplore.

### Responsabilités

- **Entités métier** : Modèles de domaine avec logique métier
- **Value Objects** : Objets immuables représentant des concepts métier
- **Interfaces de repositories** : Contrats pour l'accès aux données
- **Domain Events** : Événements métier pour la communication entre agrégats
- **Exceptions métier** : Exceptions spécifiques au domaine

### Principes

✅ **Pas de dépendances externes** - Le domaine ne dépend de rien
✅ **Logique métier pure** - Aucune logique d'infrastructure
✅ **Indépendant du framework** - Peut être testé unitairement facilement
✅ **Immutabilité préférée** - Value Objects immuables

### Structure

```
SubExplore.Domain/
├── Entities/              # Entités métier (User, DivingSpot, DiveLog, Event...)
├── ValueObjects/          # Objets de valeur (Coordinates, Depth, Temperature...)
├── Enums/                 # Énumérations métier
├── Interfaces/
│   └── Repositories/      # Interfaces des repositories
├── Events/                # Domain events
└── Exceptions/            # Exceptions métier personnalisées
```

### Exemple d'entité

```csharp
public class DivingSpot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Coordinates Location { get; private set; }
    public Depth MaxDepth { get; private set; }

    // Logique métier
    public void UpdateLocation(Coordinates newLocation)
    {
        if (newLocation == null)
            throw new DomainException("Location cannot be null");

        Location = newLocation;
    }
}
```

### Règles

- ❌ Pas de dépendances vers Application, Infrastructure ou API
- ❌ Pas de Entity Framework, Supabase ou autre framework
- ✅ Uniquement des classes C# pures
- ✅ Tests unitaires pour toute la logique métier
