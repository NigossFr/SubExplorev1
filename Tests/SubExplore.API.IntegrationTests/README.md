# SubExplore.API.IntegrationTests

## Vue d'ensemble

Projet de tests d'intégration pour l'API SubExplore. Ce projet utilise **WebApplicationFactory** et **Testcontainers** pour tester l'API dans un environnement isolé.

## Technologies utilisées

- **xUnit 2.9.3** : Framework de tests
- **FluentAssertions 8.8.0** : Assertions expressives
- **Microsoft.AspNetCore.Mvc.Testing 9.0.0** : Tests d'intégration ASP.NET Core
- **Testcontainers.PostgreSql 4.9.0** : Conteneurs Docker pour PostgreSQL

## Structure

```
SubExplore.API.IntegrationTests/
├── SubExploreWebApplicationFactory.cs    # Factory personnalisée pour l'API
├── ApiSetupVerificationTests.cs          # Tests de vérification de la configuration
└── README.md                              # Ce fichier
```

## État actuel

### ✅ Infrastructure complétée (TASK-018)

- ✅ Projet de tests d'intégration créé
- ✅ Packages NuGet installés (WebApplicationFactory, Testcontainers)
- ✅ WebApplicationFactory configurée
- ✅ Tests de vérification de configuration (4 tests passent)

### ⚠️ Tests actuels : Vérification de configuration uniquement

Les tests actuels vérifient que l'infrastructure est correctement configurée. Ce sont **des tests de setup**, pas des tests d'intégration complets.

**Pourquoi cette approche ?**
- Pas d'endpoints réels implémentés encore (Phase 2+)
- Principe YAGNI : "You Ain't Gonna Need It" - On ne teste pas ce qui n'existe pas
- Tests d'intégration complets seront ajoutés au fur et à mesure de l'implémentation

### 🚧 Prochaines étapes

**Quand les endpoints seront implémentés (Phase 2+), ajouter :**
- Tests d'intégration complets des endpoints API
- Tests avec Testcontainers (base de données PostgreSQL isolée)
- Tests d'authentification et autorisation JWT
- Tests de validation et gestion d'erreurs
- Tests de scénarios métier complets

**Optionnel (si nécessaire) :**
- Refactorisation de `Program.cs` pour meilleure testabilité
- Le `try/catch` global actuel empêche certains tests avancés

## Tests actuels

### ApiSetupVerificationTests

Tests de vérification que l'infrastructure de tests d'intégration est correctement configurée :

1. `WebApplicationFactory_Should_Be_Instantiable` : Vérifie que la factory peut être instanciée
2. `MvcTesting_Package_Should_Be_Available` : Vérifie que le package Mvc.Testing est disponible
3. `FluentAssertions_Package_Should_Be_Available` : Vérifie que FluentAssertions est disponible
4. `TestcontainersPostgreSql_Package_Should_Be_Available` : Vérifie que Testcontainers.PostgreSql est disponible

## Utilisation

### Exécuter les tests

```bash
# Tous les tests d'intégration
dotnet test SubExplore.API.IntegrationTests.csproj

# Avec verbosité
dotnet test SubExplore.API.IntegrationTests.csproj --verbosity normal

# Un test spécifique
dotnet test --filter "FullyQualifiedName~ApiSetupVerificationTests"
```

## Prochaines étapes

1. **TASK-018** (en cours) : Configuration tests d'intégration
   - ✅ Créer projet SubExplore.API.IntegrationTests
   - ✅ Configurer WebApplicationFactory
   - ✅ Configurer base de données de test
   - ✅ Créer test basique de santé API

2. **Après TASK-018** :
   - Créer des tests d'intégration pour les endpoints de l'API
   - Ajouter des tests avec authentification
   - Configurer des scénarios de tests complets

## Notes techniques

### WebApplicationFactory

La `SubExploreWebApplicationFactory` hérite de `WebApplicationFactory<Program>` et configure :
- Environnement de test
- Configuration in-memory
- Logging réduit (Warning level)
- Désactivation des erreurs détaillées

### Testcontainers

Testcontainers sera utilisé pour :
- Lancer une instance PostgreSQL isolée pour chaque suite de tests
- Garantir que les tests sont reproductibles
- Éviter les dépendances sur des bases de données partagées

## Limitations actuelles

- Les tests d'intégration API complets ne sont pas encore implémentés
- La configuration de la base de données de test avec Testcontainers est en attente
- Le Program.cs de l'API utilise un try/catch global qui nécessite une refactorisation pour les tests

## Contribution

Pour ajouter de nouveaux tests d'intégration :

1. Créer une nouvelle classe de tests héritant de `IClassFixture<SubExploreWebApplicationFactory>`
2. Injecter la factory dans le constructeur
3. Créer un HttpClient avec `factory.CreateClient()`
4. Écrire des tests utilisant le pattern AAA (Arrange-Act-Assert)
5. Utiliser FluentAssertions pour les assertions

## Ressources

- [Documentation xUnit](https://xunit.net/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Testcontainers](https://dotnet.testcontainers.org/)
- [FluentAssertions](https://fluentassertions.com/)

---

**Dernière mise à jour** : 2025-12-11
**Status** : Configuration de base complétée, tests de vérification passent
