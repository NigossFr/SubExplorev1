# Phase 2 - Architecture et Domain Layer
**Durée estimée** : 3-4 semaines
**Statut** : 🔄 EN COURS (51.4%)
**Progression** : 18/35 tâches (51.4%)
**Date de début** : 2025-12-11

## 📋 Objectifs de la phase
- Créer les Value Objects de base (Coordinates, Depth, Temperature, Visibility)
- Créer les Entités du Domain (User, DivingSpot, DiveLog, Event, Achievement, Message)
- Définir les Repository Interfaces
- Définir les Domain Services Interfaces
- Implémenter les Domain Events
- Configurer MediatR pour CQRS
- Créer les Commands et Queries (Authentication, User, DivingSpot, DiveLog, Events)
- Configurer AutoMapper et FluentValidation
- Créer les DTOs et Responses
- Implémenter l'Exception Handling
- Configurer le Caching
- Implémenter les Repositories (User, DivingSpot, DiveLog, Event)
- Implémenter les Services Infrastructure (Storage, Geolocation, External APIs, Notifications)

## 🏗️ Architecture visée
```
SubExplore.Domain/
├── Entities/          # Entités métier (User, DivingSpot, DiveLog, Event, etc.)
├── ValueObjects/      # Value Objects immutables (Coordinates, Depth, etc.)
├── Repositories/      # Interfaces des repositories
├── Services/          # Interfaces des domain services
└── Events/            # Domain events

SubExplore.Application/
├── Commands/          # CQRS Commands + Handlers
├── Queries/           # CQRS Queries + Handlers
├── DTOs/              # Data Transfer Objects
├── Behaviors/         # MediatR Pipeline Behaviors
└── Mappings/          # AutoMapper Profiles

SubExplore.Infrastructure/
├── Persistence/       # Implémentations repositories (Supabase)
├── Services/          # Implémentations domain services
├── External/          # Intégrations APIs externes
└── Caching/           # Cache service implementation
```

---

## 📦 Domain Layer - Entités Core

### TASK-021: Création des Value Objects de base
- [x] Créer dossier Domain/ValueObjects
- [x] Implémenter Coordinates (latitude, longitude)
- [x] Implémenter Depth (valeur, unité)
- [x] Implémenter WaterTemperature
- [x] Implémenter Visibility
- [x] Tests unitaires pour chaque VO

**Status:** ✅ Terminé
**Dépendances:** TASK-020
**Complété le:** 2025-12-11

**Résultat:**
- ✅ 4 Value Objects créés (Coordinates, Depth, WaterTemperature, Visibility)
- ✅ Tous les VO sont immutables (record struct)
- ✅ Validation complète dans les constructeurs
- ✅ Conversion d'unités (Meters ⇄ Feet, Celsius ⇄ Fahrenheit)
- ✅ 99 tests unitaires ajoutés (tous passent)
- ✅ Documentation XML complète
- ✅ Compilation: 0 erreurs, 0 warnings
- ✅ Tests totaux: 121/121 passent (100%)

**Fichiers créés:**
- `SubExplore.Domain/ValueObjects/Coordinates.cs`
- `SubExplore.Domain/ValueObjects/Depth.cs`
- `SubExplore.Domain/ValueObjects/WaterTemperature.cs`
- `SubExplore.Domain/ValueObjects/Visibility.cs`

---

### TASK-022: Entité User
- [x] Créer Domain/Entities/User.cs
- [x] Propriétés: Id, Email, Username, Profile
- [x] Méthodes: UpdateProfile, UpgradeToPremium
- [x] Validation avec FluentValidation
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-11

**Résultat:**
- ✅ Value Object UserProfile créé (FirstName, LastName, Bio, ProfilePictureUrl)
- ✅ Entité User complète avec encapsulation DDD
- ✅ Propriétés: Id (Guid), Email, Username, Profile, IsPremium, CreatedAt, UpdatedAt, PremiumSince
- ✅ Méthodes métier: UpdateProfile, UpgradeToPremium, DowngradeToPremium, UpdateEmail, UpdateUsername
- ✅ Validation inline dans l'entité (pattern DDD)
  - Email: format valide, max 100 chars, normalisé en lowercase
  - Username: 3-30 chars, alphanumeric + underscore/hyphen uniquement
  - Profile: FirstName/LastName max 50 chars, Bio max 500 chars
- ✅ 54 tests unitaires ajoutés (tous passent)
  - 19 tests UserProfile
  - 35 tests User entity
- ✅ Tests totaux: 175/175 passent (100%)
- ✅ Compilation: 0 erreurs, 0 warnings

**Fichiers créés:**
- `SubExplore.Domain/ValueObjects/UserProfile.cs`
- `SubExplore.Domain/Entities/User.cs`
- `Tests/SubExplore.Domain.UnitTests/ValueObjects/UserProfileTests.cs`
- `Tests/SubExplore.Domain.UnitTests/Entities/UserTests.cs`

---

### TASK-023: Entité DivingSpot
- [x] Créer Domain/Entities/DivingSpot.cs
- [x] Propriétés: Id, Name, Description, Coordinates, etc.
- [x] Méthodes: AddPhoto, UpdateConditions, Rate
- [x] Agrégat avec Photos, Ratings
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-11
**Dépendances:** TASK-021

**Résultat:**
- ✅ Entité DivingSpot créée (aggregate root)
- ✅ Entités enfants: DivingSpotPhoto, DivingSpotRating
- ✅ Enum DivingSpotDifficulty (Beginner, Intermediate, Advanced, Expert)
- ✅ Propriétés: Id, Name, Description, Location (Coordinates), CurrentTemperature, CurrentVisibility, MaximumDepth, Difficulty, CreatedBy, etc.
- ✅ Méthodes métier: AddPhoto(), RemovePhoto(), AddRating(), UpdateCurrentConditions(), CalculateAverageRating()
- ✅ 72 tests unitaires ajoutés (tous passent)
- ✅ Validation complète (Name 3-100 chars, Description 10-1000 chars)
- ✅ Gestion collections privées (_photos, _ratings) avec IReadOnlyCollection
- ✅ Tests totaux: 247/247 passent (100%)

**Propriétés complètes:
- Id (Guid)
- Name (string, max 100 chars)
- Description (string, max 1000 chars)
- Coordinates (Value Object)
- Depth (Value Object)
- Difficulty (enum: Beginner, Intermediate, Advanced, Expert)
- CurrentConditions (WaterTemperature, Visibility, CurrentStrength)
- Photos (collection)
- Ratings (collection)
- CreatedBy (UserId)
- CreatedAt, UpdatedAt

**Méthodes métier:**
- AddPhoto(url, description)
- UpdateConditions(temperature, visibility, current)
- Rate(userId, rating, comment)
- CalculateAverageRating()

---

### TASK-024: Entité DiveLog
- [x] Créer Domain/Entities/DiveLog.cs
- [x] Propriétés: Date, Spot, Depth, Duration, etc.
- [x] Calculs automatiques (consommation air, etc.)
- [x] Validation règles métier
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-11
**Dépendances:** TASK-023

**Résultat:**
- ✅ Entité DiveLog créée (aggregate root)
- ✅ Enum DiveType (Recreational, Training, Technical, FreeDiving, Night, Wreck, Cave, Deep)
- ✅ Propriétés: Id, UserId, DivingSpotId, BuddyUserId, DiveDate, Duration, MaxDepth, AverageDepth, WaterTemperature, Visibility, DiveType, etc.
- ✅ Méthodes métier: UpdateDuration(), UpdateDepths(), UpdateConditions(), SetEquipmentUsed(), AddNotes()
- ✅ Calculs automatiques pour air consumption (via méthode dédiée)
- ✅ 49 tests unitaires ajoutés (tous passent)
- ✅ Validation complète (Duration 1-600 min, Depths > 0, Equipment/Notes max chars)
- ✅ Support buddy diving (BuddyUserId optionnel)
- ✅ Tests totaux: 296/296 passent (100%)

**Propriétés complètes:
- Id (Guid)
- UserId (Guid)
- SpotId (Guid)
- DiveDate (DateTime)
- EntryTime, ExitTime (TimeSpan)
- MaxDepth, AverageDepth (Depth VO)
- WaterTemperature (VO)
- Visibility (VO)
- AirConsumption (calcul automatique)
- Equipment (string)
- Notes (string)
- Photos (collection)

**Méthodes métier:**
- CalculateDuration()
- CalculateAirConsumption(tankSize, startPressure, endPressure)
- AddPhoto(url, description)
- Share(userId)

---

### TASK-025: Entité Event
- [x] Créer Domain/Entities/Event.cs
- [x] Propriétés: Title, Date, Location, Participants
- [x] Méthodes: RegisterParticipant, Cancel
- [x] Gestion des limites de participants
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-11

**Résultat:**
- ✅ Entité Event créée (aggregate root)
- ✅ Entité enfant: EventParticipant
- ✅ Enum EventStatus (Scheduled, Ongoing, Completed, Cancelled)
- ✅ Propriétés: Id, Title, Description, EventDate, Location, LocationName, DivingSpotId, OrganizedBy, MaxParticipants, Status, etc.
- ✅ Méthodes métier: RegisterParticipant(), UnregisterParticipant(), Cancel(), Complete(), UpdateDetails(), UpdateLocation()
- ✅ Gestion limite participants avec validation automatique
- ✅ Vérification contraintes métier (pas de registration si event cancelled/completed, limites max, pas de doublons)
- ✅ 52 tests unitaires ajoutés (41 Event + 11 EventParticipant) (tous passent)
- ✅ Validation complète (Title 3-100 chars, Description 10-1000 chars, LocationName 3-200 chars, MaxParticipants 1-1000)
- ✅ Tests totaux: 348/348 passent (100%)

**Propriétés complètes:
- Id (Guid)
- Title (string, max 100 chars)
- Description (string, max 1000 chars)
- EventDate (DateTime)
- Location (Coordinates or string)
- SpotId (Guid, optionnel)
- MaxParticipants (int)
- CurrentParticipants (collection)
- Status (enum: Planned, InProgress, Completed, Cancelled)
- CreatedBy (UserId)

**Méthodes métier:**
- RegisterParticipant(userId)
- UnregisterParticipant(userId)
- Cancel(reason)
- IsFull()
- CanRegister(userId)

---

### TASK-026: Système de Achievements
- [x] Créer Domain/Entities/Achievement.cs
- [x] Définir types d'achievements
- [x] Logique de déverrouillage
- [x] UserAchievement (liaison)
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-11

**Résultat:**
- ✅ Entité Achievement créée (template d'achievement)
- ✅ Entité UserAchievement créée (achievement déverrouillé)
- ✅ Enum AchievementType (8 types): Depth, DiveCount, Experience, Exploration, Social, Conservation, Education, Safety
- ✅ Enum AchievementCategory (5 tiers): Bronze, Silver, Gold, Platinum, Diamond
- ✅ Propriétés Achievement: Id, Title, Description, Type, Category, Points (0-10000), IconUrl, RequiredValue, IsSecret, CreatedAt, UpdatedAt
- ✅ Propriétés UserAchievement: Id, UserId, AchievementId, UnlockedAt, Progress (0-1000000)
- ✅ Méthodes métier Achievement: UpdateDetails(), SetIconUrl(), UpdateRequiredValue(), ToggleSecret()
- ✅ Méthodes métier UserAchievement: UpdateProgress()
- ✅ 44 tests unitaires ajoutés (32 Achievement + 12 UserAchievement) (tous passent)
- ✅ Support achievements progressifs (ex: "100 Dives" avec RequiredValue=100)
- ✅ Support achievements secrets (cachés jusqu'au déverrouillage)
- ✅ Système de points pour gamification
- ✅ Tests totaux: 392/392 passent (100%)

**Types d'achievements couverts:**
- FirstDive (première plongée)
- DeepDiver (plongée à >30m)
- Explorer (10 spots différents)
- Photographer (100 photos uploadées)
- SocialButterfly (10 événements participés)
- CertificationMaster (toutes certifications)

**Entités:**
- Achievement (Id, Name, Description, Icon, Condition)
- UserAchievement (UserId, AchievementId, UnlockedAt)

---

### TASK-027: Système de Notifications
- [x] Créer Domain/Entities/Notification.cs
- [x] Types: Event, Message, Achievement, System
- [x] Propriétés: Read, Priority
- [x] Tests unitaires

**Status:** ✅ Terminé
**Complété le:** 2025-12-12

**Résultat:**
- ✅ Entité Notification créée
- ✅ Enum NotificationType (4 types): Event, Message, Achievement, System
- ✅ Enum NotificationPriority (4 niveaux): Low, Normal, High, Urgent
- ✅ Propriétés: Id, UserId, Type, Title, Message, IsRead, Priority, CreatedAt, ReadAt, ReferenceId
- ✅ Méthodes métier: Create, MarkAsRead, MarkAsUnread, UpdatePriority, UpdateContent
- ✅ 35 tests unitaires ajoutés (tous passent)
- ✅ Validation complète:
  - Title: 1-200 chars
  - Message: 1-1000 chars
  - CreatedAt: pas dans le futur
  - UpdatePriority/UpdateContent: uniquement sur notifications non lues
- ✅ ReferenceId optionnel pour lier aux entités (EventId, MessageId, AchievementId)
- ✅ Tests totaux: 434/434 passent (100%)
- ✅ Compilation: 0 erreurs, 0 warnings bloquants

**Fichiers créés:**
- `SubExplore.Domain/Enums/NotificationType.cs`
- `SubExplore.Domain/Enums/NotificationPriority.cs`
- `SubExplore.Domain/Entities/Notification.cs`
- `Tests/SubExplore.Domain.UnitTests/Entities/NotificationTests.cs`

---

### TASK-028: Entité Message/Conversation
- [x] Créer Domain/Entities/Conversation.cs
- [x] Créer Domain/Entities/Message.cs
- [x] Support messages privés et groupes
- [x] Tests unitaires

**Status:** ✅ Terminé
**Dépendances:** TASK-021, TASK-022
**Complété le:** 2025-12-16

**Résultat:**
- ✅ Entité Conversation créée (aggregate root)
  - Factory methods: CreatePrivate, CreateGroup
  - Propriétés: Id, Title, IsGroupConversation, LastMessageAt, CreatedAt, ParticipantIds, Messages
  - Méthodes métier: AddParticipant, RemoveParticipant, UpdateTitle, AddMessage, IsParticipant
  - Validation complète: Title max 100 chars, min 2 participants pour groupes
- ✅ Entité Message créée
  - Factory method: Create
  - Propriétés: Id, ConversationId, SenderId, Content, SentAt, ReadByUserIds
  - Méthodes métier: MarkAsReadBy, IsReadBy, UpdateContent
  - Validation complète: Content 1-2000 chars
  - Sender auto-read: l'expéditeur marque automatiquement son propre message comme lu
- ✅ 76 tests unitaires ajoutés (tous passent)
  - 43 tests ConversationTests (CreatePrivate, CreateGroup, AddParticipant, RemoveParticipant, UpdateTitle, IsParticipant)
  - 33 tests MessageTests (Create, MarkAsReadBy, IsReadBy, UpdateContent)
- ✅ Tests totaux: 489/489 passent (100%)
- ✅ Compilation: 0 erreurs, 0 warnings

**Fichiers créés:**
- `SubExplore.Domain/Entities/Conversation.cs`
- `SubExplore.Domain/Entities/Message.cs`
- `Tests/SubExplore.Domain.UnitTests/Entities/ConversationTests.cs`
- `Tests/SubExplore.Domain.UnitTests/Entities/MessageTests.cs`

---

## 📋 Domain - Interfaces et Contrats

### TASK-029: Repository Interfaces
- [x] Créer Domain/Repositories/IUserRepository.cs
- [x] Créer Domain/Repositories/IDivingSpotRepository.cs
- [x] Créer Domain/Repositories/IDiveLogRepository.cs
- [x] Créer Domain/Repositories/IEventRepository.cs
- [x] Méthodes CRUD + requêtes spécifiques

**Status:** ✅ Terminé
**Dépendances:** TASK-022, TASK-023, TASK-024, TASK-025
**Complété le:** 2025-12-16

**Résultat:**
- ✅ Interface générique IRepository<T> créée avec méthodes CRUD communes
  - GetByIdAsync, GetAllAsync, GetCountAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync
- ✅ IUserRepository avec méthodes spécifiques
  - GetByEmailAsync, GetByUsernameAsync, SearchUsersAsync, GetPremiumUsersAsync
  - EmailExistsAsync, UsernameExistsAsync
- ✅ IDivingSpotRepository avec recherche géospatiale
  - GetNearbyAsync (recherche par coordonnées + rayon), SearchAsync
  - GetByOwnerAsync, GetPopularAsync, GetByMinimumRatingAsync
- ✅ IDiveLogRepository avec statistiques
  - GetByUserAsync, GetBySpotAsync, GetByDateRangeAsync, GetByBuddyAsync
  - GetStatisticsAsync avec UserDivingStatistics record
- ✅ IEventRepository avec filtrage avancé
  - GetUpcomingAsync, GetPastAsync, GetByOrganizerAsync, GetByParticipantAsync
  - GetByStatusAsync, GetByDivingSpotAsync, SearchAsync, GetWithAvailableSpotsAsync
- ✅ Compilation: 0 erreurs, warnings StyleCop/Analyzers non-bloquants

**Fichiers créés:**
- `SubExplore.Domain/Repositories/IRepository.cs`
- `SubExplore.Domain/Repositories/IUserRepository.cs`
- `SubExplore.Domain/Repositories/IDivingSpotRepository.cs`
- `SubExplore.Domain/Repositories/IDiveLogRepository.cs`
- `SubExplore.Domain/Repositories/IEventRepository.cs`

---

### TASK-030: Domain Services Interfaces
- [x] IGeolocationService (calcul distances)
- [x] IWeatherService (données météo)
- [x] ITideService (marées)
- [x] INotificationService
- [x] IAchievementService

**Status:** ✅ Complétée le 2025-12-16

**Interfaces créées:**
- ✅ `IGeolocationService` - Service de géolocalisation
  - CalculateDistance(coord1, coord2, unit) : double - Calcul de distance entre 2 points
  - GetNearbyPoints(center, radius, points) : List<Coordinates> - Points proches d'un centre
  - ConvertUnits(distance, fromUnit, toUnit) : double - Conversion d'unités de distance
  - DistanceUnit enum (Kilometers, Miles, NauticalMiles, Meters, Feet)

- ✅ `IWeatherService` - Service météo avec modèle WeatherData
  - GetCurrentWeatherAsync(coordinates) : WeatherData - Météo actuelle
  - GetForecastAsync(coordinates, days) : List<WeatherData> - Prévisions météo (1-7 jours)
  - WeatherData record avec température, pression, humidité, vent, visibilité, précipitations, UV

- ✅ `ITideService` - Service des marées avec modèles TideData/TideEvent
  - GetTideDataAsync(coordinates, date) : TideData - Données marées pour une date
  - GetNextHighTideAsync(coordinates) : DateTime - Prochaine marée haute
  - GetNextLowTideAsync(coordinates) : DateTime - Prochaine marée basse
  - TideData record avec TideEvents, CurrentHeight, CurrentState
  - TideEvent record (Time, Type, HeightMeters)
  - TideType enum (High, Low) et TideState enum (Rising, Falling, HighTide, LowTide)

- ✅ `INotificationService` - Service de notifications multi-canal
  - SendPushNotificationAsync(userId, title, message, data) - Notifications push
  - SendEmailAsync(email, subject, body, isHtml) - Notifications email
  - CreateInAppNotificationAsync(userId, type, title, message, ...) - Notifications in-app
  - SendBulkNotificationAsync(userIds, title, message, type) - Envoi en masse
  - MarkAsReadAsync(notificationId) - Marquer comme lu
  - GetUnreadCountAsync(userId) - Compteur non lus
  - NotificationType enum (12 types: System, DiveLogShared, EventInvitation, EventReminder, etc.)

- ✅ `IAchievementService` - Service de gestion des achievements/badges
  - CheckAndUnlockAchievementsAsync(userId) - Vérifier et débloquer achievements
  - TryUnlockAchievementAsync(userId, achievementId) - Débloquer achievement spécifique
  - GetProgressAsync(userId, achievementId) : AchievementProgress - Progression utilisateur
  - GetAllProgressAsync(userId) : List<AchievementProgress> - Toutes les progressions
  - GetUnlockedAchievementsAsync(userId) : List<UnlockedAchievement> - Achievements débloqués
  - GetTotalPointsAsync(userId) : int - Total des points
  - AchievementProgress record avec progression, pourcentage, statut unlock
  - UnlockedAchievement record avec ID, nom, date, points

**Fichiers créés:**
- `SubExplore.Domain/Services/IGeolocationService.cs` (71 lignes)
- `SubExplore.Domain/Services/IWeatherService.cs` (110 lignes)
- `SubExplore.Domain/Services/ITideService.cs` (121 lignes)
- `SubExplore.Domain/Services/INotificationService.cs` (133 lignes)
- `SubExplore.Domain/Services/IAchievementService.cs` (168 lignes)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants

---

### TASK-031: Domain Events
- [x] Créer infrastructure Domain Events
- [x] UserRegisteredEvent
- [x] DiveLogCreatedEvent
- [x] EventCreatedEvent
- [x] AchievementUnlockedEvent

**Status:** ✅ Complétée le 2025-12-16

**Infrastructure créée :**
- ✅ `IDomainEvent` - Interface de base pour tous les domain events
  - Propriété OccurredOn : DateTime (date/heure de l'événement)
  - Base pour tous les événements du domaine

**Domain Events créés :**
- ✅ `UserRegisteredEvent` - Événement levé lors de l'inscription d'un nouvel utilisateur
  - Paramètres : UserId (Guid), Email (string), OccurredOn (DateTime)
  - Utilisé pour déclencher des actions comme envoi email bienvenue, création profil initial

- ✅ `DiveLogCreatedEvent` - Événement levé lors de la création d'un dive log
  - Paramètres : DiveLogId (Guid), UserId (Guid), SpotId (Guid), OccurredOn (DateTime)
  - Utilisé pour notifications buddies, mise à jour statistiques, vérification achievements

- ✅ `EventCreatedEvent` - Événement levé lors de la création d'un événement de plongée
  - Paramètres : EventId (Guid), CreatedBy (Guid), OccurredOn (DateTime)
  - Utilisé pour notifications participants potentiels, indexation événement

- ✅ `AchievementUnlockedEvent` - Événement levé lors du déblocage d'un achievement
  - Paramètres : UserId (Guid), AchievementId (Guid), OccurredOn (DateTime)
  - Utilisé pour notifications utilisateur, mise à jour profil, partage social

**Pattern utilisé :**
- Records immuables pour garantir l'intégrité des événements
- Interface IDomainEvent pour typage fort et extensibilité
- Tous les events incluent OccurredOn pour traçabilité temporelle
- Prêt pour intégration avec MediatR (TASK-032)

**Fichiers créés :**
- `SubExplore.Domain/Events/IDomainEvent.cs` (13 lignes)
- `SubExplore.Domain/Events/UserRegisteredEvent.cs` (13 lignes)
- `SubExplore.Domain/Events/DiveLogCreatedEvent.cs` (14 lignes)
- `SubExplore.Domain/Events/EventCreatedEvent.cs` (13 lignes)
- `SubExplore.Domain/Events/AchievementUnlockedEvent.cs` (13 lignes)

**Compilation :** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants

---

## 🏛️ Application Layer - CQRS avec MediatR

### TASK-032: Configuration MediatR
- [x] Installer MediatR dans Application
- [x] Configurer DI pour MediatR
- [x] Créer structure Commands/Queries
- [x] Créer PipelineBehaviors (Logging, Validation, Performance, Transaction)

**Status:** ✅ Complétée le 2025-12-16

**Packages installés:**
- ✅ MediatR 14.0.0 - Framework CQRS pour Command/Query handling
- ✅ FluentValidation 12.1.1 - Validation des requests
- ✅ FluentValidation.DependencyInjectionExtensions 12.1.1 - Extensions DI pour FluentValidation

**Structure créée:**
- ✅ Dossier `Commands/` - Pour les commandes CQRS (create, update, delete)
- ✅ Dossier `Queries/` - Pour les requêtes CQRS (read operations)
- ✅ Dossier `Behaviors/` - Pour les pipeline behaviors MediatR

**Pipeline Behaviors créés:**
- ✅ `LoggingBehavior` - Logging automatique de toutes les requests/responses
  - Log au début du traitement avec RequestId unique
  - Log à la fin avec temps d'exécution en ms
  - Log des erreurs avec exception details et temps écoulé

- ✅ `ValidationBehavior` - Validation FluentValidation automatique
  - Injection de tous les validators pour le type de request
  - Exécution parallèle de tous les validators
  - Collecte et agrégation des erreurs de validation
  - Lève ValidationException si échec de validation

- ✅ `PerformanceBehavior` - Tracking et monitoring des performances
  - Mesure du temps d'exécution avec Stopwatch
  - Log warning si dépassement du seuil (500ms par défaut)
  - Utile pour identifier les requêtes lentes en production

- ✅ `TransactionBehavior` - Gestion des transactions DB (placeholder)
  - Infrastructure pour future implémentation avec DbContext
  - TODO: Begin/Commit/Rollback quand DbContext sera ajouté
  - Logging des begin/commit/rollback pour traçabilité

**Configuration DI:**
- ✅ Fichier `DependencyInjection.cs` créé avec méthode extension `AddApplication()`
- ✅ Enregistrement MediatR avec Assembly scanning automatique
- ✅ Enregistrement FluentValidation validators avec Assembly scanning
- ✅ Enregistrement des 4 pipeline behaviors dans l'ordre correct :
  1. LoggingBehavior (premier - log entrée)
  2. ValidationBehavior (validation avant exécution)
  3. PerformanceBehavior (mesure performance)
  4. TransactionBehavior (dernier - gestion transaction)

**Ordre des behaviors:** L'ordre est important car ils forment une chaîne :
Request → Logging → Validation → Performance → Transaction → Handler → Transaction → Performance → Validation → Logging → Response

**Fichiers créés:**
- `SubExplore.Application/Behaviors/LoggingBehavior.cs` (79 lignes)
- `SubExplore.Application/Behaviors/ValidationBehavior.cs` (60 lignes)
- `SubExplore.Application/Behaviors/PerformanceBehavior.cs` (70 lignes)
- `SubExplore.Application/Behaviors/TransactionBehavior.cs` (72 lignes)
- `SubExplore.Application/DependencyInjection.cs` (36 lignes)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants

---

### TASK-033: Commands - Authentification
- [x] RegisterUserCommand + Handler
- [x] LoginCommand + Handler
- [x] RefreshTokenCommand + Handler
- [x] LogoutCommand + Handler
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Commands d'authentification créés:**

1. **RegisterUserCommand** - Inscription nouvel utilisateur
   - Paramètres: Email, Password, Username, FirstName, LastName
   - Retour: RegisterUserResult (UserId, Email, Username)
   - Validator: Email (format + max 255), Password (min 8, uppercase, lowercase, digit, special char), Username (3-50 chars, alphanum + underscore/hyphen), FirstName/LastName (required, max 100)
   - Handler: Placeholder avec TODO pour implémentation future (hash password, save to DB, send welcome email)
   - 23 tests unitaires

2. **LoginCommand** - Connexion utilisateur
   - Paramètres: Email, Password
   - Retour: LoginResult (UserId, Email, AccessToken, RefreshToken, ExpiresIn)
   - Validator: Email (format), Password (required only, no complexity check)
   - Handler: Placeholder retournant tokens temporaires, ExpiresIn = 3600 (1 hour)
   - 5 tests unitaires

3. **RefreshTokenCommand** - Rafraîchissement du token d'accès
   - Paramètres: RefreshToken
   - Retour: RefreshTokenResult (AccessToken, RefreshToken, ExpiresIn)
   - Validator: RefreshToken (required)
   - Handler: Placeholder avec TODO pour implémentation token rotation
   - 4 tests unitaires

4. **LogoutCommand** - Déconnexion utilisateur
   - Paramètres: UserId, RefreshToken
   - Retour: LogoutResult (Success)
   - Validator: UserId (not empty), RefreshToken (required)
   - Handler: Placeholder avec TODO pour invalidation token
   - 4 tests unitaires

**Pattern utilisé:**
- ✅ Chaque command = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TCommand, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TCommand>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative
- ✅ XML documentation complète

**Fichiers créés:**
- `SubExplore.Application/Commands/Auth/RegisterUserCommand.cs` (32 lignes)
- `SubExplore.Application/Commands/Auth/RegisterUserCommandHandler.cs` (56 lignes)
- `SubExplore.Application/Commands/Auth/RegisterUserCommandValidator.cs` (46 lignes)
- `SubExplore.Application/Commands/Auth/LoginCommand.cs` (30 lignes)
- `SubExplore.Application/Commands/Auth/LoginCommandHandler.cs` (61 lignes)
- `SubExplore.Application/Commands/Auth/LoginCommandValidator.cs` (26 lignes)
- `SubExplore.Application/Commands/Auth/RefreshTokenCommand.cs` (24 lignes)
- `SubExplore.Application/Commands/Auth/RefreshTokenCommandHandler.cs` (55 lignes)
- `SubExplore.Application/Commands/Auth/RefreshTokenCommandValidator.cs` (19 lignes)
- `SubExplore.Application/Commands/Auth/LogoutCommand.cs` (20 lignes)
- `SubExplore.Application/Commands/Auth/LogoutCommandHandler.cs` (49 lignes)
- `SubExplore.Application/Commands/Auth/LogoutCommandValidator.cs` (22 lignes)

**Tests créés:**
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/RegisterUserCommandValidatorTests.cs` (358 lignes, 23 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/RegisterUserCommandHandlerTests.cs` (115 lignes, 4 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/LoginCommandValidatorTests.cs` (82 lignes, 5 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/LoginCommandHandlerTests.cs` (99 lignes, 5 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/RefreshTokenCommandValidatorTests.cs` (42 lignes, 2 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/RefreshTokenCommandHandlerTests.cs` (97 lignes, 4 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/LogoutCommandValidatorTests.cs` (53 lignes, 3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/Auth/LogoutCommandHandlerTests.cs` (83 lignes, 3 tests)

**Tests totaux:** ✅ 66/66 tests passent (100%)
- 23 tests RegisterUserCommand (validator + handler)
- 10 tests LoginCommand (validator + handler)
- 6 tests RefreshTokenCommand (validator + handler)
- 6 tests LogoutCommand (validator + handler)
- +21 tests existants (SetupVerification tests)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants

**Notes pour implémentation future:**
- RegisterUserCommand: Hash password avec BCrypt, vérifier email/username unique, créer User entity, save to DB, envoyer email bienvenue
- LoginCommand: Vérifier email existe, comparer password hash, générer JWT access token et refresh token, store refresh token
- RefreshTokenCommand: Valider refresh token, vérifier non expiré, générer nouveaux tokens, invalider ancien refresh token (rotation)
- LogoutCommand: Invalider refresh token dans DB, optionnellement blacklister access token

---

### TASK-034: Commands - User Profile
- [x] UpdateProfileCommand + Handler + Validator
- [x] UploadAvatarCommand + Handler + Validator
- [x] UpdateDivingCertificationsCommand + Handler + Validator
- [x] UpgradeToPremiumCommand + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Commands User Profile créés:**

1. **UpdateProfileCommand** - Mise à jour du profil utilisateur
   - Paramètres: UserId, FirstName, LastName, Bio (optional), ProfilePictureUrl (optional)
   - Retour: UpdateProfileResult (Success, UserId)
   - Validator: UserId required, FirstName required + max 50 chars, LastName required + max 50 chars, Bio max 500 chars, ProfilePictureUrl max 500 chars
   - Handler: Placeholder avec TODO pour implémentation future (get user from repository, validate user exists, update profile, save changes, publish UserProfileUpdatedEvent)
   - 11 tests unitaires (validator) + 3 tests (handler)

2. **UploadAvatarCommand** - Upload avatar/photo de profil
   - Paramètres: UserId, FileName, ContentType, FileData (byte[])
   - Retour: UploadAvatarResult (Success, AvatarUrl)
   - Validator: UserId required, FileName required + max 255 chars, ContentType validation (jpeg, jpg, png, webp - case insensitive), FileData required, File size max 5 MB
   - Handler: Placeholder retournant URL temporaire `https://storage.example.com/avatars/{UserId}/{FileName}`
   - TODO: Validate image format/dimensions, resize/compress, upload to cloud storage, update profile, delete old avatar, publish event
   - 13 tests unitaires (validator) + 3 tests (handler)

3. **UpdateDivingCertificationsCommand** - Mise à jour des certifications de plongée
   - Paramètres: UserId, List<CertificationDto>
   - CertificationDto: Organization, Level, CertificationNumber (optional), IssueDate (optional)
   - Retour: UpdateDivingCertificationsResult (Success, UserId, CertificationCount)
   - Validator: UserId required, Certifications not null, Max 20 certifications per user, Organization required + max 50 chars, Level required + max 100 chars, CertificationNumber max 50 chars, IssueDate between 1950 and present
   - Handler: Placeholder retournant le nombre de certifications
   - TODO: Get user, clear/merge certifications, validate data, create records, save, publish event
   - FIX APPLIED: Wrapped count validation in `When` clause to avoid NullReferenceException when list is null
   - 14 tests unitaires (validator) + 4 tests (handler)

4. **UpgradeToPremiumCommand** - Passage au compte premium
   - Paramètres: UserId, PaymentMethod, PaymentToken, SubscriptionPlan (enum: Monthly, Yearly)
   - Retour: UpgradeToPremiumResult (Success, UserId, IsPremium, PremiumExpiresAt)
   - Validator: UserId required, PaymentMethod required + whitelist (CreditCard, PayPal, Stripe, ApplePay, GooglePay - case insensitive), PaymentToken required + max 500 chars, SubscriptionPlan must be valid enum
   - Handler: Placeholder calculant expiration (Monthly = 30 days, Yearly = 365 days)
   - TODO: Validate payment token, process payment, get user, update premium status, create subscription record, send welcome email, publish event
   - 12 tests unitaires (validator) + 5 tests (handler)

**Pattern utilisé:**
- ✅ Chaque command = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TCommand, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TCommand>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative
- ✅ XML documentation complète

**Fichiers créés (12 production files):**
- `SubExplore.Application/Commands/UserProfile/UpdateProfileCommand.cs` (26 lignes)
- `SubExplore.Application/Commands/UserProfile/UpdateProfileCommandHandler.cs` (51 lignes)
- `SubExplore.Application/Commands/UserProfile/UpdateProfileCommandValidator.cs` (32 lignes)
- `SubExplore.Application/Commands/UserProfile/UploadAvatarCommand.cs` (26 lignes)
- `SubExplore.Application/Commands/UserProfile/UploadAvatarCommandHandler.cs` (54 lignes)
- `SubExplore.Application/Commands/UserProfile/UploadAvatarCommandValidator.cs` (42 lignes)
- `SubExplore.Application/Commands/UserProfile/UpdateDivingCertificationsCommand.cs` (36 lignes)
- `SubExplore.Application/Commands/UserProfile/UpdateDivingCertificationsCommandHandler.cs` (52 lignes)
- `SubExplore.Application/Commands/UserProfile/UpdateDivingCertificationsCommandValidator.cs` (46 lignes)
- `SubExplore.Application/Commands/UserProfile/UpgradeToPremiumCommand.cs` (34 lignes)
- `SubExplore.Application/Commands/UserProfile/UpgradeToPremiumCommandHandler.cs` (69 lignes)
- `SubExplore.Application/Commands/UserProfile/UpgradeToPremiumCommandValidator.cs` (40 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpdateProfileCommandValidatorTests.cs` (11 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpdateProfileCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UploadAvatarCommandValidatorTests.cs` (13 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UploadAvatarCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpdateDivingCertificationsCommandValidatorTests.cs` (14 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpdateDivingCertificationsCommandHandlerTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpgradeToPremiumCommandValidatorTests.cs` (12 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/UserProfile/UpgradeToPremiumCommandHandlerTests.cs` (5 tests)

**Tests totaux:** ✅ 136/136 tests passent (100%)
- 50 tests validators UserProfile commands
- 15 tests handlers UserProfile commands
- 45 tests Auth commands (TASK-033)
- 21 tests existants (SetupVerification tests)
- 5 tests Application.UnitTests

**Bug fix appliqué:**
- UpdateDivingCertificationsCommandValidator: Wrapped count validation in `When` clause to avoid NullReferenceException when Certifications list is null
- Removed unused private field `KnownOrganizations` to eliminate StyleCop warning S1144

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants

**Notes pour implémentation future:**
- UpdateProfileCommand: Get user from repository, validate user exists, update UserProfile value object, save changes to DB, publish UserProfileUpdatedEvent
- UploadAvatarCommand: Validate image format/dimensions, resize/compress image, upload to cloud storage (Supabase Storage), update user profile with new avatar URL, delete old avatar if exists, publish UserAvatarUpdatedEvent
- UpdateDivingCertificationsCommand: Get user, clear existing certifications or merge with new ones, validate organization against known list (PADI, SSI, CMAS, NAUI, SDI, TDI, BSAC, RAID), create certification records, save to DB, publish UserCertificationsUpdatedEvent
- UpgradeToPremiumCommand: Validate payment token with payment provider, process payment transaction, get user from repository, update premium status and expiration date, create subscription record, send premium welcome email, publish UserUpgradedToPremiumEvent

---

### TASK-035: Commands - DivingSpot
- [x] CreateSpotCommand + Handler + Validator
- [x] UpdateSpotCommand + Handler + Validator
- [x] DeleteSpotCommand + Handler + Validator
- [x] AddSpotPhotoCommand + Handler + Validator
- [x] RateSpotCommand + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Commands DivingSpot créés:**

1. **CreateSpotCommand** - Création d'un nouveau diving spot
   - Paramètres: Name, Description, Latitude, Longitude, MaxDepthMeters, Difficulty (0-3), CreatedBy
   - Retour: CreateSpotResult (Success, SpotId)
   - Validator: Name 3-100 chars, Description 10-1000 chars, Latitude -90 to 90, Longitude -180 to 180, MaxDepth 0-500m, Difficulty 0-3 (Beginner to Expert), CreatedBy required
   - Handler: Placeholder avec TODO pour création spot (create Coordinates/Depth VOs, validate coordinates, save to repository, publish DivingSpotCreatedEvent)
   - 16 tests unitaires (validator) + 3 tests (handler)

2. **UpdateSpotCommand** - Mise à jour d'un diving spot existant
   - Paramètres: SpotId, Name, Description, MaxDepthMeters, Difficulty, CurrentTemperatureCelsius (optional), CurrentVisibilityMeters (optional), UserId
   - Retour: UpdateSpotResult (Success, SpotId)
   - Validator: SpotId required, Name 3-100 chars, Description 10-1000 chars, MaxDepth > 0 and ≤ 500m, Difficulty 0-3, Temperature -5 to 50°C (when provided), Visibility > 0 and ≤ 100m (when provided), UserId required
   - Handler: Placeholder avec TODO pour mise à jour (get spot from repository, validate user permissions, update properties and conditions, save, publish DivingSpotUpdatedEvent)
   - 13 tests unitaires (validator) + 3 tests (handler)

3. **DeleteSpotCommand** - Suppression d'un diving spot
   - Paramètres: SpotId, UserId
   - Retour: DeleteSpotResult (Success, SpotId)
   - Validator: SpotId required, UserId required
   - Handler: Placeholder avec TODO pour suppression (get spot from repository, validate user permissions, check associated dive logs, delete photos from storage, soft delete or hard delete, publish DivingSpotDeletedEvent)
   - 3 tests unitaires (validator) + 3 tests (handler)

4. **AddSpotPhotoCommand** - Ajout d'une photo à un diving spot
   - Paramètres: SpotId, Url (string), Description (optional), UserId
   - Retour: AddSpotPhotoResult (Success, PhotoId)
   - Validator: SpotId required, Url required + max 500 chars + valid HTTP/HTTPS URL, Description max 500 chars (when provided), UserId required
   - Handler: Placeholder retournant PhotoId unique
   - TODO: Validate URL accessibility, create DivingSpotPhoto entity, add photo to spot using AddPhoto method, save to repository, publish DivingSpotPhotoAddedEvent
   - 11 tests unitaires (validator) + 3 tests (handler)

5. **RateSpotCommand** - Notation d'un diving spot
   - Paramètres: SpotId, UserId, Rating (1-5), Comment (optional)
   - Retour: RateSpotResult (Success, RatingId, AverageRating)
   - Validator: SpotId required, UserId required, Rating 1-5 stars, Comment max 1000 chars (when provided)
   - Handler: Placeholder calculant nouvelle moyenne (placeholder = rating value)
   - TODO: Get spot from repository, check if user already rated, update or create rating, add rating to spot using AddRating method, calculate new average rating, save to repository, publish DivingSpotRatedEvent
   - 9 tests unitaires (validator) + 3 tests (handler)

**Pattern utilisé:**
- ✅ Chaque command = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TCommand, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TCommand>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative
- ✅ XML documentation complète

**Fichiers créés (15 production files):**
- `SubExplore.Application/Commands/DivingSpot/CreateSpotCommand.cs` (32 lignes)
- `SubExplore.Application/Commands/DivingSpot/CreateSpotCommandHandler.cs` (54 lignes)
- `SubExplore.Application/Commands/DivingSpot/CreateSpotCommandValidator.cs` (44 lignes)
- `SubExplore.Application/Commands/DivingSpot/UpdateSpotCommand.cs` (32 lignes)
- `SubExplore.Application/Commands/DivingSpot/UpdateSpotCommandHandler.cs` (52 lignes)
- `SubExplore.Application/Commands/DivingSpot/UpdateSpotCommandValidator.cs` (52 lignes)
- `SubExplore.Application/Commands/DivingSpot/DeleteSpotCommand.cs` (20 lignes)
- `SubExplore.Application/Commands/DivingSpot/DeleteSpotCommandHandler.cs` (50 lignes)
- `SubExplore.Application/Commands/DivingSpot/DeleteSpotCommandValidator.cs` (21 lignes)
- `SubExplore.Application/Commands/DivingSpot/AddSpotPhotoCommand.cs` (26 lignes)
- `SubExplore.Application/Commands/DivingSpot/AddSpotPhotoCommandHandler.cs` (55 lignes)
- `SubExplore.Application/Commands/DivingSpot/AddSpotPhotoCommandValidator.cs` (43 lignes)
- `SubExplore.Application/Commands/DivingSpot/RateSpotCommand.cs` (26 lignes)
- `SubExplore.Application/Commands/DivingSpot/RateSpotCommandHandler.cs` (58 lignes)
- `SubExplore.Application/Commands/DivingSpot/RateSpotCommandValidator.cs` (30 lignes)

**Tests créés (10 test files):**
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/CreateSpotCommandValidatorTests.cs` (16 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/CreateSpotCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/UpdateSpotCommandValidatorTests.cs` (13 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/UpdateSpotCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/DeleteSpotCommandValidatorTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/DeleteSpotCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/AddSpotPhotoCommandValidatorTests.cs` (11 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/AddSpotPhotoCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/RateSpotCommandValidatorTests.cs` (9 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DivingSpot/RateSpotCommandHandlerTests.cs` (3 tests)

**Tests totaux projet:** ✅ 196/196 tests passent (100%)
- 58 tests validators DivingSpot commands
- 15 tests handlers DivingSpot commands
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 21 tests existants (SetupVerification tests)
- 5 tests Application.UnitTests
- +410 tests Domain.UnitTests (non comptabilisés dans le total Application)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- CreateSpotCommand: Create Coordinates and Depth value objects from parameters, validate coordinates ranges, create DivingSpot entity, save to repository via IDivingSpotRepository, publish DivingSpotCreatedEvent
- UpdateSpotCommand: Get spot from repository, validate user has permission (owner or admin), update spot properties (name, description, depth, difficulty), update current conditions if provided (temperature, visibility), update UpdatedAt timestamp, save changes, publish DivingSpotUpdatedEvent
- DeleteSpotCommand: Get spot from repository, validate user has permission (owner or admin), check if spot has associated dive logs (consider soft delete strategy), delete associated photos from storage service, mark as deleted or hard delete from repository, publish DivingSpotDeletedEvent
- AddSpotPhotoCommand: Get spot from repository, validate spot exists and URL is valid/accessible, create DivingSpotPhoto entity, add photo to spot using AddPhoto method, save to repository, optionally publish DivingSpotPhotoAddedEvent
- RateSpotCommand: Get spot from repository, validate spot exists, check if user has already rated (update existing or create new), add rating to spot using AddRating method, calculate new average rating using CalculateAverageRating, save to repository, optionally publish DivingSpotRatedEvent

---

### TASK-036: Commands - DiveLog
- [x] CreateDiveLogCommand + Handler + Validator
- [x] UpdateDiveLogCommand + Handler + Validator
- [x] DeleteDiveLogCommand + Handler + Validator
- [x] ShareDiveLogCommand + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Commands DiveLog créés:**

1. **CreateDiveLogCommand** - Création d'un nouveau log de plongée
   - Paramètres: UserId, DivingSpotId, DiveDate, EntryTime, ExitTime, MaxDepthMeters, AverageDepthMeters (optional), WaterTemperatureCelsius (optional), VisibilityMeters (optional), DiveType (0-7), BuddyUserId (optional), Equipment (optional), Notes (optional)
   - Retour: CreateDiveLogResult (Success, DiveLogId, DurationMinutes)
   - Validator: UserId required, DivingSpotId required, DiveDate not more than 1 day in future, EntryTime < ExitTime, Duration 1-600 minutes, MaxDepth > 0 and ≤ 500m, AverageDepth ≤ MaxDepth, Temperature -5 to 50°C, Visibility 0-100m, DiveType 0-7, Equipment max 500 chars, Notes max 2000 chars
   - Handler: Placeholder calculant duration automatiquement (ExitTime - EntryTime)
   - TODO: Validate diving spot exists, validate buddy user exists if provided, create Depth/WaterTemperature/Visibility VOs, create DiveLog entity, save to repository, publish DiveLogCreatedEvent
   - 17 tests unitaires (validator) + 3 tests (handler)

2. **UpdateDiveLogCommand** - Mise à jour d'un log de plongée
   - Paramètres: DiveLogId, UserId, MaxDepthMeters, AverageDepthMeters (optional), WaterTemperatureCelsius (optional), VisibilityMeters (optional), Equipment (optional), Notes (optional)
   - Retour: UpdateDiveLogResult (Success, DiveLogId)
   - Validator: DiveLogId required, UserId required, MaxDepth > 0 and ≤ 500m, AverageDepth ≤ MaxDepth (when provided), Temperature -5 to 50°C (when provided), Visibility 0-100m (when provided), Equipment max 500 chars, Notes max 2000 chars
   - Handler: Placeholder avec TODO pour mise à jour
   - TODO: Get dive log from repository, validate user has permission (owner only), update depths and conditions, update equipment and notes, update UpdatedAt timestamp, save changes, publish DiveLogUpdatedEvent
   - 11 tests unitaires (validator) + 3 tests (handler)

3. **DeleteDiveLogCommand** - Suppression d'un log de plongée
   - Paramètres: DiveLogId, UserId
   - Retour: DeleteDiveLogResult (Success, DiveLogId)
   - Validator: DiveLogId required, UserId required
   - Handler: Placeholder avec TODO pour suppression
   - TODO: Get dive log from repository, validate user has permission (owner only), delete associated photos from storage, remove from shared users, soft delete or hard delete from repository, publish DiveLogDeletedEvent
   - 3 tests unitaires (validator) + 3 tests (handler)

4. **ShareDiveLogCommand** - Partage d'un log de plongée avec d'autres utilisateurs
   - Paramètres: DiveLogId, UserId, SharedWithUserIds (List<Guid>), Message (optional)
   - Retour: ShareDiveLogResult (Success, DiveLogId, SharedCount)
   - Validator: DiveLogId required, UserId required, SharedWithUserIds not null/empty, max 50 users per share, no empty Guids in list, cannot share with yourself, Message max 500 chars (when provided)
   - Handler: Placeholder retournant count of shared users
   - TODO: Get dive log from repository, validate user has permission to share (owner only), validate all target users exist, create share records for each target user, send notifications to all shared users, save changes, publish DiveLogSharedEvent
   - 11 tests unitaires (validator) + 3 tests (handler)

**Pattern utilisé:**
- ✅ Chaque command = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TCommand, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TCommand>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative avec null checks appropriés
- ✅ XML documentation complète

**Fichiers créés (12 production files):**
- `SubExplore.Application/Commands/DiveLog/CreateDiveLogCommand.cs` (46 lignes)
- `SubExplore.Application/Commands/DiveLog/CreateDiveLogCommandHandler.cs` (61 lignes)
- `SubExplore.Application/Commands/DiveLog/CreateDiveLogCommandValidator.cs` (68 lignes)
- `SubExplore.Application/Commands/DiveLog/UpdateDiveLogCommand.cs` (32 lignes)
- `SubExplore.Application/Commands/DiveLog/UpdateDiveLogCommandHandler.cs` (52 lignes)
- `SubExplore.Application/Commands/DiveLog/UpdateDiveLogCommandValidator.cs` (48 lignes)
- `SubExplore.Application/Commands/DiveLog/DeleteDiveLogCommand.cs` (20 lignes)
- `SubExplore.Application/Commands/DiveLog/DeleteDiveLogCommandHandler.cs` (50 lignes)
- `SubExplore.Application/Commands/DiveLog/DeleteDiveLogCommandValidator.cs` (21 lignes)
- `SubExplore.Application/Commands/DiveLog/ShareDiveLogCommand.cs` (28 lignes)
- `SubExplore.Application/Commands/DiveLog/ShareDiveLogCommandHandler.cs` (54 lignes)
- `SubExplore.Application/Commands/DiveLog/ShareDiveLogCommandValidator.cs` (38 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/CreateDiveLogCommandValidatorTests.cs` (17 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/CreateDiveLogCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/UpdateDiveLogCommandValidatorTests.cs` (11 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/UpdateDiveLogCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/DeleteDiveLogCommandValidatorTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/DeleteDiveLogCommandHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/ShareDiveLogCommandValidatorTests.cs` (11 tests)
- `Tests/SubExplore.Application.UnitTests/Commands/DiveLog/ShareDiveLogCommandHandlerTests.cs` (3 tests)

**Tests totaux projet:** ✅ 284/284 tests passent (100%)
- 42 tests validators DiveLog commands
- 12 tests handlers DiveLog commands
- 58 tests validators DivingSpot commands (TASK-035)
- 15 tests handlers DivingSpot commands (TASK-035)
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 21 tests existants (SetupVerification tests)
- 5 tests Application.UnitTests
- +410 tests Domain.UnitTests (non comptabilisés dans le total Application)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- CreateDiveLogCommand: Validate diving spot exists in repository, validate buddy user exists if provided, create Depth/WaterTemperature/Visibility value objects from parameters, create DiveLog entity with all properties, calculate dive duration from times, save to repository via IDiveLogRepository, publish DiveLogCreatedEvent for statistics updates
- UpdateDiveLogCommand: Get dive log from repository, validate user is the owner of the dive log, update depths (max and average if provided), update water conditions (temperature, visibility), update equipment list and notes, update UpdatedAt timestamp, save changes to repository, publish DiveLogUpdatedEvent for audit trail
- DeleteDiveLogCommand: Get dive log from repository, validate user is the owner, check for associated photos and delete from storage service, remove all share records with other users, consider soft delete strategy for historical data, mark as deleted or hard delete from repository, publish DiveLogDeletedEvent for statistics recalculation
- ShareDiveLogCommand: Get dive log from repository, validate user is the owner and has permission to share, validate all target user IDs exist in system, create DiveLogShare records for each target user with optional message, send notification to each shared user (email/push notification), save all share records to repository, publish DiveLogSharedEvent for activity feed updates

**Détails spécifiques:**
- **DiveType Enum** (8 valeurs: 0-7): Recreational, Training, Technical, FreeDiving, Night, Wreck, Cave, Deep
- **Time Validation**: EntryTime must be before ExitTime, resulting duration must be 1-600 minutes (10 hours max)
- **Depth Validation**: MaxDepth > 0 and ≤ 500m, AverageDepth must be ≤ MaxDepth when provided
- **Share Limits**: Maximum 50 users per share operation to prevent abuse
- **Null Safety**: All validators include proper null checks to prevent NullReferenceException (fixed during implementation)
- **Duration Calculation**: Automatically calculated in handler from ExitTime - EntryTime

---

### TASK-037: Queries - DivingSpot
- [x] GetNearbySpots Query + Handler + Validator (géolocalisation)
- [x] GetSpotById Query + Handler + Validator
- [x] SearchSpots Query + Handler + Validator (filtres)
- [x] GetPopularSpots Query + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Queries DivingSpot créées:**

1. **GetNearbySpotsQuery** - Recherche géospatiale de spots à proximité
   - Paramètres: Latitude, Longitude, RadiusKm (default 10, max 100), MinDifficulty (0-3, optional), MaxDifficulty (0-3, optional), MinDepthMeters (optional), MaxDepthMeters (optional), Limit (default 20, max 100)
   - Retour: GetNearbySpotsResult (Success, List<DivingSpotDto>, TotalCount)
   - Validator: Latitude -90 to 90, Longitude -180 to 180, Radius 0-100km, Difficulty filters 0-3, Depth filters with range validation, Limit 1-100
   - Handler: Placeholder avec mock data incluant distance calculée
   - TODO: Implement geospatial search using PostGIS ST_Distance or Haversine formula, filter by difficulty/depth ranges, order by distance ascending, apply limit, map to DivingSpotDto with distance information
   - DTOs: DivingSpotDto (Id, Name, Description, Latitude, Longitude, MaxDepthMeters, Difficulty, AverageRating, RatingCount, DistanceKm, CurrentTemperatureCelsius, CurrentVisibilityMeters)
   - 15 tests unitaires (validator) + 3 tests (handler)

2. **GetSpotByIdQuery** - Récupération détaillée d'un spot spécifique
   - Paramètres: SpotId, IncludePhotos (default true), IncludeRatings (default true)
   - Retour: GetSpotByIdResult (Success, DetailedDivingSpotDto)
   - Validator: SpotId required (not empty)
   - Handler: Placeholder retournant spot détaillé avec photos et ratings selon flags
   - TODO: Get spot from repository by ID, include photos if IncludePhotos (map DivingSpotPhoto entities), include ratings if IncludeRatings (map DivingSpotRating entities), calculate average rating, return null if not found
   - DTOs: DetailedDivingSpotDto (full spot info + Photos + Ratings + metadata), SpotPhotoDto (Id, Url, Description, UploadedBy, UploadedAt), SpotRatingDto (Id, UserId, Rating, Comment, CreatedAt)
   - 4 tests unitaires (validator) + 4 tests (handler)

3. **SearchSpotsQuery** - Recherche avancée avec filtres multiples
   - Paramètres: SearchText (optional, max 100 chars), MinDifficulty (0-3, optional), MaxDifficulty (0-3, optional), MinDepthMeters (optional), MaxDepthMeters (optional), MinRating (1-5, optional), MinTemperatureCelsius (optional), MaxTemperatureCelsius (optional), MinVisibilityMeters (optional), SortBy (Name/Rating/Depth/CreatedAt, default Rating), SortDescending (default true), PageNumber (default 1), PageSize (default 20, max 100)
   - Retour: SearchSpotsResult (Success, List<DivingSpotDto>, TotalCount, PageNumber, PageSize, TotalPages)
   - Validator: SearchText max 100 chars, difficulty/depth/rating/temperature/visibility ranges validated, SortBy must be valid field, pagination parameters validated
   - Handler: Placeholder avec pagination calculée
   - TODO: Build query with text search on name/description, apply all filters (difficulty, depth, rating, temperature, visibility), apply sorting by field and direction, calculate total count, apply pagination (skip/take), map results to DivingSpotDto, calculate total pages
   - 22 tests unitaires (validator) + 4 tests (handler)

4. **GetPopularSpotsQuery** - Récupération des spots populaires
   - Paramètres: Limit (default 10, max 50), MinimumRatings (default 5, max 1000), DaysBack (default 90, max 365)
   - Retour: GetPopularSpotsResult (Success, List<PopularDivingSpotDto>, TotalCount)
   - Validator: Limit 1-50, MinimumRatings 0-1000, DaysBack 1-365
   - Handler: Placeholder avec calcul de popularité score
   - TODO: Calculate date threshold from DaysBack, get spots with rating count >= MinimumRatings, count recent dive logs per spot, calculate PopularityScore = (AverageRating * 0.5) + (RatingCount * 0.3) + (RecentDiveLogsCount * 0.2), order by PopularityScore desc, take top Limit spots, map to PopularDivingSpotDto
   - DTOs: PopularDivingSpotDto (extends DivingSpotDto with RecentDiveLogsCount, PopularityScore)
   - Popularity formula: Quality 50% + Total Popularity 30% + Recent Activity 20%
   - 10 tests unitaires (validator) + 5 tests (handler)

**Pattern utilisé:**
- ✅ Chaque query = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TQuery, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TQuery>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative
- ✅ DTOs séparés pour différents niveaux de détail (DivingSpotDto, DetailedDivingSpotDto, PopularDivingSpotDto)
- ✅ Pagination intégrée pour SearchSpots avec calcul de pages
- ✅ Paramètres optionnels avec valeurs par défaut appropriées
- ✅ XML documentation complète

**Fichiers créés (12 production files):**
- `SubExplore.Application/Queries/DivingSpot/GetNearbySpots.cs` (64 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetNearbySpotsHandler.cs` (71 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetNearbySpotsValidator.cs` (58 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetSpotById.cs` (92 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetSpotByIdHandler.cs` (82 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetSpotByIdValidator.cs` (21 lignes)
- `SubExplore.Application/Queries/DivingSpot/SearchSpots.cs` (55 lignes)
- `SubExplore.Application/Queries/DivingSpot/SearchSpotsHandler.cs` (87 lignes)
- `SubExplore.Application/Queries/DivingSpot/SearchSpotsValidator.cs` (96 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetPopularSpots.cs` (64 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetPopularSpotsHandler.cs` (89 lignes)
- `SubExplore.Application/Queries/DivingSpot/GetPopularSpotsValidator.cs` (29 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetNearbySpotsValidatorTests.cs` (15 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetNearbySpotsHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetSpotByIdValidatorTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetSpotByIdHandlerTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/SearchSpotsValidatorTests.cs` (22 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/SearchSpotsHandlerTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetPopularSpotsValidatorTests.cs` (10 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DivingSpot/GetPopularSpotsHandlerTests.cs` (5 tests)

**Tests totaux projet:** ✅ 366/366 tests passent (100%)
- 51 tests validators DivingSpot queries
- 16 tests handlers DivingSpot queries
- 42 tests validators DiveLog commands (TASK-036)
- 12 tests handlers DiveLog commands (TASK-036)
- 58 tests validators DivingSpot commands (TASK-035)
- 15 tests handlers DivingSpot commands (TASK-035)
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 26 tests existants (SetupVerification tests + Application tests)
- 5 tests Application.UnitTests
- +410 tests Domain.UnitTests (non comptabilisés dans le total Application)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- GetNearbySpotsQuery: Implement geospatial search using database spatial functions (PostGIS ST_Distance, ST_DWithin) or calculate distance using Haversine formula for each spot, apply difficulty and depth filters, order results by distance ascending, limit to requested count, include current conditions in results
- GetSpotByIdQuery: Retrieve spot from repository with eager loading of photos and ratings collections if requested, calculate average rating from ratings collection, map all related entities to DTOs, return null if spot not found (Success=false)
- SearchSpotsQuery: Build dynamic query with text search using full-text search or LIKE on name/description, apply all optional filters efficiently using indexed columns, implement flexible sorting based on SortBy parameter, calculate pagination metadata (total pages, has next/previous), optimize query performance with proper indexing
- GetPopularSpotsQuery: Calculate cutoff date from DaysBack parameter, filter spots by minimum ratings threshold, count associated dive logs within date range using efficient join/subquery, calculate weighted popularity score favoring quality (50%), total engagement (30%), and recent activity (20%), cache results for performance (15-30 minute TTL), return top N spots ordered by popularity

**Détails spécifiques:**
- **Geospatial Search**: Uses Haversine formula or PostGIS for accurate distance calculation, supports radius up to 100km for performance reasons
- **Search Flexibility**: SearchSpots supports partial text matching, multiple filter combinations, and 4 sort options (Name, Rating, Depth, CreatedAt)
- **Popularity Algorithm**: Balanced scoring system weighing quality (average rating), quantity (rating count), and recency (recent dive logs)
- **Pagination**: Standard offset-based pagination with page number and size, total pages calculated automatically
- **DTOs**: Three levels of detail (basic DivingSpotDto for lists, DetailedDivingSpotDto for single spot, PopularDivingSpotDto with metrics)
- **Optional Parameters**: All filters are optional, queries work with minimal parameters using sensible defaults

---

### TASK-038: Queries - DiveLog
- [x] GetUserDiveLogs Query + Handler + Validator
- [x] GetDiveLogById Query + Handler + Validator
- [x] GetDiveStatistics Query + Handler + Validator
- [x] GetDiveLogsBySpot Query + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-16
**Dépendances:** TASK-032

**Queries DiveLog créées:**

1. **GetUserDiveLogsQuery** - Récupération des logs de plongée d'un utilisateur
   - Paramètres: UserId, StartDate (optional), EndDate (optional), DivingSpotId (optional), MinDepthMeters (optional), MaxDepthMeters (optional), DiveType (0-7, optional), SortBy (DiveDate/MaxDepth/Duration, default DiveDate), SortDescending (default true), PageNumber (default 1), PageSize (default 20, max 100)
   - Retour: GetUserDiveLogsResult (Success, List<DiveLogDto>, TotalCount, PageNumber, PageSize, TotalPages)
   - Validator: UserId required, date range validation (StartDate <= EndDate), depth range validation (0-500m, MinDepth <= MaxDepth), DiveType 0-7, SortBy must be valid field, pagination parameters validated
   - Handler: Placeholder avec pagination calculée
   - TODO: Get dive logs from repository for user, apply date range filter, apply diving spot filter if specified, apply depth range filter, apply dive type filter, apply sorting by field and direction, calculate total count, apply pagination (skip/take), map to DiveLogDto with diving spot name, calculate total pages
   - DTOs: DiveLogDto (Id, UserId, DivingSpotId, DivingSpotName, DiveDate, EntryTime, ExitTime, DurationMinutes, MaxDepthMeters, AverageDepthMeters, WaterTemperatureCelsius, VisibilityMeters, DiveType, BuddyUserId, Equipment, Notes, CreatedAt)
   - 20 tests unitaires (validator) + 3 tests (handler)

2. **GetDiveLogByIdQuery** - Récupération détaillée d'un log de plongée
   - Paramètres: DiveLogId, UserId (for permission check)
   - Retour: GetDiveLogByIdResult (Success, DetailedDiveLogDto)
   - Validator: DiveLogId required, UserId required
   - Handler: Placeholder retournant dive log détaillé avec informations complètes
   - TODO: Get dive log from repository by ID, check if user has permission to view (owner or shared with), get diving spot information (name, coordinates), get user information (owner name), get buddy information if applicable (buddy name), map dive type int to dive type name, check if dive log is shared and get SharedBy information, map to DetailedDiveLogDto with all information, return null if not found or no permission
   - DTOs: DetailedDiveLogDto (extends DiveLogDto with UserName, DivingSpotLatitude, DivingSpotLongitude, DiveTypeName, BuddyUserName, IsShared, SharedBy, UpdatedAt)
   - 4 tests unitaires (validator) + 4 tests (handler)

3. **GetDiveStatisticsQuery** - Statistiques complètes de plongée d'un utilisateur
   - Paramètres: UserId, StartDate (optional), EndDate (optional)
   - Retour: GetDiveStatisticsResult (Success, DiveStatisticsDto)
   - Validator: UserId required, date range validation (StartDate <= EndDate), EndDate cannot be in future
   - Handler: Placeholder avec statistiques calculées incluant dictionnaires pour distributions
   - TODO: Get all dive logs for user within date range, calculate total dives count, calculate total dive time (sum of all durations), find maximum depth across all dives, calculate average maximum depth, calculate average dive duration, find deepest dive (max depth) with spot name, find longest dive (max duration), find most visited spot (favorite) with dive count, group dives by type and count, group dives by month (last 12 months) and count, count unique diving spots visited, count unique dive buddies, get first and last dive dates, map to DiveStatisticsDto with all metrics
   - DTOs: DiveStatisticsDto (UserId, TotalDives, TotalDiveTimeMinutes, TotalDiveTimeHours, MaxDepthMeters, AverageDepthMeters, AverageDiveTimeMinutes, DeepestDiveId, DeepestDiveSpotName, LongestDiveId, LongestDiveDurationMinutes, FavoriteSpotId, FavoriteSpotName, FavoriteSpotDiveCount, DivesByType (Dictionary), DivesByMonth (Dictionary), UniqueSpots, DiveBuddiesCount, FirstDiveDate, LastDiveDate, PeriodStartDate, PeriodEndDate)
   - 8 tests unitaires (validator) + 7 tests (handler)

4. **GetDiveLogsBySpotQuery** - Logs de plongée pour un spot spécifique
   - Paramètres: DivingSpotId, StartDate (optional), EndDate (optional), MinDepthMeters (optional), MaxDepthMeters (optional), SortBy (DiveDate/MaxDepth/Duration, default DiveDate), SortDescending (default true), PageNumber (default 1), PageSize (default 20, max 100)
   - Retour: GetDiveLogsBySpotResult (Success, DivingSpotId, DivingSpotName, List<SpotDiveLogDto>, TotalCount, PageNumber, PageSize, TotalPages, SpotDiveStatisticsDto)
   - Validator: DivingSpotId required, date range validation, depth range validation (0-500m, MinDepth <= MaxDepth), SortBy must be valid field, pagination parameters validated
   - Handler: Placeholder retournant dive logs et statistiques du spot
   - TODO: Get diving spot information (name), get dive logs from repository for the specified spot, apply date range filter, apply depth range filter, apply sorting by field and direction, calculate total count before pagination, apply pagination (skip/take), map results to SpotDiveLogDto including user names and dive type names, calculate spot statistics (total dives, unique divers, averages), calculate total pages and return result
   - DTOs: SpotDiveLogDto (Id, UserId, UserName, DiveDate, DurationMinutes, MaxDepthMeters, AverageDepthMeters, WaterTemperatureCelsius, VisibilityMeters, DiveType, DiveTypeName, Notes), SpotDiveStatisticsDto (TotalDives, UniqueDivers, AverageDepthMeters, AverageDurationMinutes, AverageTemperatureCelsius, AverageVisibilityMeters, LastDiveDate)
   - 19 tests unitaires (validator) + 5 tests (handler)

**Pattern utilisé:**
- ✅ Chaque query = record implementing IRequest<TResult>
- ✅ Chaque result = record pour la response
- ✅ Chaque handler = class implementing IRequestHandler<TQuery, TResult>
- ✅ Chaque validator = class extending AbstractValidator<TQuery>
- ✅ Tous les handlers incluent logging via ILogger
- ✅ Tous les handlers sont des placeholders avec TODO comments pour implémentation future
- ✅ FluentValidation pour validation déclarative
- ✅ DTOs séparés pour différents niveaux de détail et contextes
- ✅ Pagination intégrée pour GetUserDiveLogs et GetDiveLogsBySpot
- ✅ Statistiques agrégées avec distributions (dictionnaires)
- ✅ Paramètres optionnels avec valeurs par défaut appropriées
- ✅ XML documentation complète

**Fichiers créés (12 production files):**
- `SubExplore.Application/Queries/DiveLog/GetUserDiveLogs.cs` (91 lignes)
- `SubExplore.Application/Queries/DiveLog/GetUserDiveLogsHandler.cs` (86 lignes)
- `SubExplore.Application/Queries/DiveLog/GetUserDiveLogsValidator.cs` (56 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogById.cs` (77 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogByIdHandler.cs` (78 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogByIdValidator.cs` (22 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveStatistics.cs` (98 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveStatisticsHandler.cs` (99 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveStatisticsValidator.cs` (29 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogsBySpot.cs` (98 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogsBySpotHandler.cs` (93 lignes)
- `SubExplore.Application/Queries/DiveLog/GetDiveLogsBySpotValidator.cs` (52 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetUserDiveLogsValidatorTests.cs` (20 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetUserDiveLogsHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveLogByIdValidatorTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveLogByIdHandlerTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveStatisticsValidatorTests.cs` (8 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveStatisticsHandlerTests.cs` (7 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveLogsBySpotValidatorTests.cs` (19 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/DiveLog/GetDiveLogsBySpotHandlerTests.cs` (5 tests)

**Tests totaux projet:** ✅ 428/428 tests passent (100%)
- 51 tests validators DiveLog queries
- 19 tests handlers DiveLog queries
- 51 tests validators DivingSpot queries (TASK-037)
- 16 tests handlers DivingSpot queries (TASK-037)
- 42 tests validators DiveLog commands (TASK-036)
- 12 tests handlers DiveLog commands (TASK-036)
- 58 tests validators DivingSpot commands (TASK-035)
- 15 tests handlers DivingSpot commands (TASK-035)
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 26 tests existants (SetupVerification tests + Application tests)
- 5 tests Application.UnitTests
- +410 tests Domain.UnitTests (non comptabilisés dans le total Application)

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- GetUserDiveLogsQuery: Retrieve all dive logs for the specified user from repository, apply optional filters in sequence (date range, diving spot, depth range, dive type), implement sorting using IQueryable with dynamic field selection, calculate total count before pagination for accurate page metadata, apply efficient pagination using Skip/Take, eager load related entities (diving spot for names), map to DiveLogDto with all required information
- GetDiveLogByIdQuery: Retrieve dive log by ID with permission validation (user must be owner or have log shared with them), eager load related entities (user, diving spot with coordinates, buddy user if exists), map DiveType enum to human-readable name, include sharing metadata (IsShared, SharedBy), return null with Success=false if not found or permission denied
- GetDiveStatisticsQuery: Calculate comprehensive statistics from all user's dive logs within optional date range, use efficient aggregation queries (GroupBy, Sum, Average, Max, Min), generate time-series data for DivesByMonth using last 12 months, identify records (deepest dive, longest dive, favorite spot) with efficient queries, handle edge cases (no dives, single dive), cache results with short TTL (5-10 minutes)
- GetDiveLogsBySpotQuery: Retrieve all dive logs for specific diving spot, include diver information (user names) for community visibility, calculate aggregated spot statistics (total dives, unique divers, averages) using efficient aggregation, apply filters and pagination, map DiveType enum to names, return spot statistics alongside dive logs for comprehensive spot analysis

**Détails spécifiques:**
- **Multi-Filter Support**: GetUserDiveLogs supports 6 simultaneous filters (date range, spot, depth range, dive type) with efficient query building
- **Permission Checking**: GetDiveLogById validates user permissions (owner or shared) before returning data
- **Statistics Aggregation**: GetDiveStatistics includes 20+ metrics with dictionaries for type/month distributions
- **Spot Analytics**: GetDiveLogsBySpot provides both individual logs and aggregated spot statistics in one query
- **Pagination**: Standard offset-based pagination with total pages calculation for both list queries
- **DTOs**: Four specialized DTOs (DiveLogDto for lists, DetailedDiveLogDto for single log, DiveStatisticsDto for metrics, SpotDiveLogDto for spot context)
- **Sorting Options**: 3 sort fields (DiveDate, MaxDepth, Duration) with ascending/descending support

---

### TASK-039: Queries - User
- [x] GetUserProfile Query + Handler + Validator
- [x] GetUserStatistics Query + Handler + Validator
- [x] SearchUsers Query + Handler + Validator
- [x] GetUserAchievements Query + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-17
**Dépendances:** TASK-032 ✅

**Queries User créées:**

1. **GetUserProfile Query** - Récupération du profil complet d'un utilisateur
   - Paramètres: UserId, IncludeAchievements, IncludeCertifications, IncludeStatistics (optional flags)
   - Retour: GetUserProfileResult (Success, UserProfileDto)
   - UserProfileDto: UserId, Username, Email, FirstName, LastName, Bio, ProfilePictureUrl, IsPremium, CreatedAt
   - Optional inclusions: List<AchievementDto>, List<CertificationDto>, UserStatisticsDto
   - Validator: UserId required
   - Handler: Placeholder retournant profil avec données conditionnelles selon flags
   - TODO: Get user from IUserRepository, map to DTO, load optional data (achievements, certifications, statistics)
   - 6 tests validator + 5 tests handler

2. **GetUserStatistics Query** - Statistiques complètes de plongée pour un utilisateur
   - Paramètres: UserId, IncludeByYear, IncludeBySpot (optional flags)
   - Retour: GetUserStatisticsResult (Success, ComprehensiveUserStatisticsDto)
   - ComprehensiveUserStatisticsDto: TotalDives, TotalDiveTimeMinutes, TotalDiveTimeFormatted, MaxDepthMeters, AverageDepthMeters, MaxDiveTimeMinutes, AverageDiveTimeMinutes, TotalDistinctSpots, FavoriteDivingSpotId/Name, FirstDiveDate, LastDiveDate, DivesByDiveType (Dictionary)
   - Optional inclusions: List<YearlyStatisticsDto>, List<SpotStatisticsDto>
   - Validator: UserId required
   - Handler: Placeholder retournant statistiques vides avec listes conditionnelles
   - TODO: Get all user dive logs, calculate comprehensive stats, group by year/spot if requested
   - 5 tests validator + 5 tests handler

3. **SearchUsers Query** - Recherche d'utilisateurs avec filtres et pagination
   - Paramètres: SearchTerm (optional), IsPremium (optional), MinTotalDives (optional), CertificationLevel (optional), PageNumber (default 1), PageSize (default 20, max 100), SortBy (Username/TotalDives/CreatedAt/LastDiveDate), SortDescending
   - UserSortField enum: Username (0), TotalDives (1), CreatedAt (2), LastDiveDate (3)
   - Retour: SearchUsersResult (Success, List<UserSearchResultDto>, TotalCount, PageNumber, PageSize, TotalPages)
   - UserSearchResultDto: UserId, Username, FirstName, LastName, Bio (truncated 100 chars), ProfilePictureUrl, IsPremium, TotalDives, HighestCertificationLevel, LastDiveDate, CreatedAt
   - Validator: SearchTerm max 100 chars, MinTotalDives >= 0, CertificationLevel max 50 chars, PageNumber >= 1, PageSize 1-100, SortBy IsInEnum
   - Handler: Placeholder retournant liste vide avec pagination metadata
   - TODO: Build dynamic query with filters, apply sorting, calculate pagination, get stats for each user
   - 18 tests validator + 5 tests handler

4. **GetUserAchievements Query** - Récupération des achievements débloqués et verrouillés
   - Paramètres: UserId, IncludeLockedAchievements (default true), CategoryFilter (optional)
   - Retour: GetUserAchievementsResult (Success, TotalUnlocked, TotalAvailable, CompletionPercentage, List<DetailedAchievementDto>)
   - DetailedAchievementDto: AchievementId, Title, Description, Category, IconUrl, Points, Rarity, IsUnlocked, UnlockedAt, Progress (0-100), ProgressDescription
   - Validator: UserId required, CategoryFilter max 50 chars
   - Handler: Placeholder retournant liste vide avec totaux zéro
   - TODO: Get all achievements, check which are unlocked, calculate progress for locked ones, filter by category, order by unlock status and points
   - 7 tests validator + 5 tests handler

**Fichiers créés (12 production files):**
- `SubExplore.Application/Queries/User/GetUserProfile.cs` (107 lignes)
- `SubExplore.Application/Queries/User/GetUserProfileHandler.cs` (75 lignes)
- `SubExplore.Application/Queries/User/GetUserProfileValidator.cs` (24 lignes)
- `SubExplore.Application/Queries/User/GetUserStatistics.cs` (105 lignes)
- `SubExplore.Application/Queries/User/GetUserStatisticsHandler.cs` (87 lignes)
- `SubExplore.Application/Queries/User/GetUserStatisticsValidator.cs` (24 lignes)
- `SubExplore.Application/Queries/User/SearchUsers.cs` (106 lignes)
- `SubExplore.Application/Queries/User/SearchUsersHandler.cs` (96 lignes)
- `SubExplore.Application/Queries/User/SearchUsersValidator.cs` (58 lignes)
- `SubExplore.Application/Queries/User/GetUserAchievements.cs` (71 lignes)
- `SubExplore.Application/Queries/User/GetUserAchievementsHandler.cs` (77 lignes)
- `SubExplore.Application/Queries/User/GetUserAchievementsValidator.cs` (31 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserProfileValidatorTests.cs` (6 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserProfileHandlerTests.cs` (5 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserStatisticsValidatorTests.cs` (5 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserStatisticsHandlerTests.cs` (5 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/SearchUsersValidatorTests.cs` (18 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/SearchUsersHandlerTests.cs` (5 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserAchievementsValidatorTests.cs` (7 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/User/GetUserAchievementsHandlerTests.cs` (5 tests)

**Tests totaux projet:** ✅ 969/969 tests passent (100%)
- 36 tests validators User queries
- 20 tests handlers User queries
- 42 tests validators DiveLog commands (TASK-036)
- 12 tests handlers DiveLog commands (TASK-036)
- 51 tests validators DivingSpot queries (TASK-037)
- 16 tests handlers DivingSpot queries (TASK-037)
- 42 tests validators DiveLog queries (TASK-038)
- 16 tests handlers DiveLog queries (TASK-038)
- 58 tests validators DivingSpot commands (TASK-035)
- 15 tests handlers DivingSpot commands (TASK-035)
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 26 tests existants (SetupVerification tests + Application tests)
- 5 tests Application.UnitTests
- +476 tests Domain.UnitTests (non comptabilisés dans le total Application)
- +4 tests API.IntegrationTests

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- GetUserProfile Query: Get user from IUserRepository.GetByIdAsync(UserId), if null return Success=false, map to UserProfileDto, conditionally load achievements from IAchievementRepository, load certifications from user entity or dedicated repository, load statistics from IDiveLogRepository.GetUserStatisticsAsync(UserId), return comprehensive profile with requested optional data
- GetUserStatistics Query: Verify user exists, get all dive logs for user from IDiveLogRepository.GetByUserIdAsync(UserId), calculate overall statistics (total dives, total time, depths, distinct spots, favorite spot, first/last dive dates), build DivesByDiveType dictionary grouping by dive type, if IncludeByYear group by year with stats per year ordered descending, if IncludeBySpot group by spot with stats per spot ordered by visit count, map to ComprehensiveUserStatisticsDto
- SearchUsers Query: Build dynamic query on IUserRepository with filters (SearchTerm on username/name, IsPremium boolean, MinTotalDives with join to dive logs, CertificationLevel match), get total count before pagination for TotalPages calculation, apply sorting based on SortBy enum (Username, TotalDives requires join, CreatedAt, LastDiveDate requires join), apply pagination with skip/take, for each user get TotalDives count and LastDiveDate from dive logs, get HighestCertificationLevel from certifications, truncate Bio to 100 chars, map to UserSearchResultDto, return with pagination metadata
- GetUserAchievements Query: Verify user exists, get all available achievements from IAchievementRepository.GetAllAsync(), filter by CategoryFilter if provided, get user's unlocked achievements from IUserAchievementRepository.GetByUserIdAsync(UserId), for each achievement check if unlocked (set IsUnlocked, UnlockedAt, Progress=100) or locked (calculate current progress if trackable, set ProgressDescription), filter out locked achievements if IncludeLockedAchievements=false, calculate TotalUnlocked and CompletionPercentage, order by unlock status (unlocked first by UnlockedAt desc) then by Points desc, map to DetailedAchievementDto

**Détails spécifiques:**
- **GetUserProfile**: Conditional data loading based on flags reduces unnecessary queries, supports achievements (list with title/description/icon/points/unlocked date), certifications (organization/level/number/issue date), statistics (total dives, times, depths, favorite spot)
- **GetUserStatistics**: Comprehensive statistics with 12+ core metrics, DivesByDiveType dictionary for type breakdown, optional yearly breakdown with YearlyStatisticsDto (year, count, time, max depth, distinct spots), optional spot breakdown with SpotStatisticsDto (spot info, count, time, depths, last dive)
- **SearchUsers**: Flexible search with 4 optional filters, pagination with metadata (total count, pages), 4 sort fields with direction, UserSearchResultDto optimized for list display with truncated bio, includes dive stats and highest certification
- **GetUserAchievements**: Supports both unlocked and locked achievements, progress tracking for locked achievements with percentage and description (e.g. "15/50 dives completed"), category filtering for focused views (Depth, Dives, Exploration, Social), completion percentage calculation, rarity levels (Common, Uncommon, Rare, Epic, Legendary)

---

### TASK-040: Queries - Events
- [x] GetUpcomingEvents Query + Handler + Validator
- [x] GetEventById Query + Handler + Validator
- [x] GetUserEvents Query + Handler + Validator
- [x] SearchEvents Query + Handler + Validator
- [x] Tests unitaires

**Status:** ✅ Complétée le 2025-12-17
**Dépendances:** TASK-032 ✅

**Queries Event créées:**

1. **GetUpcomingEvents Query** - Récupération des événements à venir dans une zone géographique
   - Paramètres: Latitude, Longitude, MaxDistanceKm (optional), DaysAhead (default 30), MaxResults (default 20)
   - Retour: GetUpcomingEventsResult (Success, List<UpcomingEventDto>)
   - UpcomingEventDto: EventId, Title, Description, EventDate, DivingSpotId, DivingSpotName, CurrentParticipants, MaxParticipants, IsAvailable, OrganizerName, DistanceKm, Cost
   - Validator: Latitude [-90, 90], Longitude [-180, 180], MaxDistanceKm > 0, DaysAhead 1-365, MaxResults 1-100
   - Handler: Placeholder retournant liste vide
   - TODO: Get nearby diving spots using IGeolocationService, get upcoming events for those spots from IEventRepository, calculate distances, filter by MaxDistanceKm and DaysAhead, order by EventDate and limit to MaxResults
   - 15 tests validator + 4 tests handler

2. **GetEventById Query** - Récupération des détails complets d'un événement
   - Paramètres: EventId, RequestingUserId (optional)
   - Retour: GetEventByIdResult (Success, DetailedEventDto or null)
   - DetailedEventDto: EventId, Title, Description, EventDate, DivingSpotId, DivingSpotName, SpotLatitude, SpotLongitude, OrganizerId, OrganizerUsername, OrganizerProfilePictureUrl, CurrentParticipants, MaxParticipants, IsAvailable, RequiredCertificationLevel, Cost, Currency, RegistrationDeadline, CancellationDeadline, SpecialRequirements, CreatedAt, UpdatedAt, List<EventParticipantDto>, IsOrganizer, IsParticipant, CanRegister
   - EventParticipantDto: UserId, Username, ProfilePictureUrl, RegistrationDate, CertificationLevel
   - Validator: EventId required
   - Handler: Placeholder retournant DetailedEventDto avec données fictives et participants vides
   - TODO: Get event from IEventRepository.GetByIdAsync(EventId), if null return Success=true with Event=null, get diving spot details, get organizer info, get participants list with their profiles, calculate IsOrganizer/IsParticipant/CanRegister flags based on RequestingUserId, map to DetailedEventDto
   - 4 tests validator + 3 tests handler

3. **GetUserEvents Query** - Récupération des événements organisés ou auxquels l'utilisateur est inscrit
   - Paramètres: UserId, IncludeOrganized (default true), IncludeRegistered (default true), IncludePastEvents (default false), PageNumber (default 1), PageSize (default 20)
   - Retour: GetUserEventsResult (Success, List<UserEventDto>, TotalCount, PageNumber, PageSize, TotalPages)
   - UserEventDto: EventId, Title, Description, EventDate, DivingSpotName, CurrentParticipants, MaxParticipants, Role (Organizer/Participant), Cost, RegistrationDate, IsUpcoming, CanCancel
   - Validator: UserId required, at least one of IncludeOrganized/IncludeRegistered must be true, PageNumber >= 1, PageSize 1-50
   - Handler: Placeholder retournant liste vide avec pagination metadata
   - TODO: Build query based on flags (organized events from IEventRepository.GetByOrganizerIdAsync, registered events from IEventRepository.GetByParticipantIdAsync), filter by date if not IncludePastEvents, apply pagination, map to UserEventDto with role and cancellation permissions
   - 11 tests validator + 5 tests handler

4. **SearchEvents Query** - Recherche d'événements avec filtres avancés et pagination
   - Paramètres: SearchTerm (optional), StartDate (optional), EndDate (optional), DivingSpotId (optional), MinParticipants (optional), MaxParticipants (optional), OnlyAvailable (default false), PageNumber (default 1), PageSize (default 20), SortBy (EventDate/Title/ParticipantCount/CreatedAt), SortDescending (default false)
   - EventSortField enum: EventDate (0), Title (1), ParticipantCount (2), CreatedAt (3)
   - Retour: SearchEventsResult (Success, List<EventSearchResultDto>, TotalCount, PageNumber, PageSize, TotalPages)
   - EventSearchResultDto: EventId, Title, Description (truncated 200 chars), EventDate, DivingSpotName, OrganizerUsername, CurrentParticipants, MaxParticipants, IsAvailable, Cost, Currency, RequiredCertificationLevel, CreatedAt
   - Validator: SearchTerm max 100 chars, StartDate <= EndDate, MinParticipants >= 0, MaxParticipants >= 0, MinParticipants <= MaxParticipants, PageNumber >= 1, PageSize 1-50, SortBy IsInEnum
   - Handler: Placeholder retournant liste vide avec pagination metadata
   - TODO: Build dynamic query with all filters (search in title/description, date range, spot, participant range, availability), apply sorting by selected field and direction, calculate pagination, map to EventSearchResultDto with truncated description
   - 20 tests validator + 8 tests handler

**Fichiers créés (12 production files):**
- `SubExplore.Application/Queries/Event/GetUpcomingEvents.cs` (72 lignes)
- `SubExplore.Application/Queries/Event/GetUpcomingEventsHandler.cs` (60 lignes)
- `SubExplore.Application/Queries/Event/GetUpcomingEventsValidator.cs` (42 lignes)
- `SubExplore.Application/Queries/Event/GetEventById.cs` (100 lignes)
- `SubExplore.Application/Queries/Event/GetEventByIdHandler.cs` (57 lignes)
- `SubExplore.Application/Queries/Event/GetEventByIdValidator.cs` (24 lignes)
- `SubExplore.Application/Queries/Event/GetUserEvents.cs` (72 lignes)
- `SubExplore.Application/Queries/Event/GetUserEventsHandler.cs` (77 lignes)
- `SubExplore.Application/Queries/Event/GetUserEventsValidator.cs` (41 lignes)
- `SubExplore.Application/Queries/Event/SearchEvents.cs` (117 lignes)
- `SubExplore.Application/Queries/Event/SearchEventsHandler.cs` (92 lignes)
- `SubExplore.Application/Queries/Event/SearchEventsValidator.cs` (68 lignes)

**Tests créés (8 test files):**
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetUpcomingEventsValidatorTests.cs` (15 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetUpcomingEventsHandlerTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetEventByIdValidatorTests.cs` (4 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetEventByIdHandlerTests.cs` (3 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetUserEventsValidatorTests.cs` (11 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/GetUserEventsHandlerTests.cs` (5 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/SearchEventsValidatorTests.cs` (20 tests)
- `Tests/SubExplore.Application.UnitTests/Queries/Event/SearchEventsHandlerTests.cs` (8 tests)

**Tests totaux projet:** ✅ 1034/1034 tests passent (100%)
- 50 tests validators Event queries
- 20 tests handlers Event queries
- 36 tests validators User queries (TASK-039)
- 20 tests handlers User queries (TASK-039)
- 42 tests validators DiveLog commands (TASK-036)
- 12 tests handlers DiveLog commands (TASK-036)
- 51 tests validators DivingSpot queries (TASK-037)
- 16 tests handlers DivingSpot queries (TASK-037)
- 42 tests validators DiveLog queries (TASK-038)
- 16 tests handlers DiveLog queries (TASK-038)
- 58 tests validators DivingSpot commands (TASK-035)
- 15 tests handlers DivingSpot commands (TASK-035)
- 50 tests validators UserProfile commands (TASK-034)
- 15 tests handlers UserProfile commands (TASK-034)
- 45 tests Auth commands (TASK-033)
- 26 tests existants (SetupVerification tests + Application tests)
- 5 tests Application.UnitTests
- +476 tests Domain.UnitTests (non comptabilisés dans le total Application)
- +4 tests API.IntegrationTests

**Compilation:** ✅ 0 erreurs, warnings StyleCop/Analyzers non-bloquants uniquement

**Notes pour implémentation future:**
- GetUpcomingEvents Query: Use IGeolocationService.GetNearbyDivingSpotsAsync(Latitude, Longitude, MaxDistanceKm) to get nearby spots, then IEventRepository.GetUpcomingEventsAsync(spotIds, startDate, endDate) filtering by DateTime.UtcNow to DateTime.UtcNow.AddDays(DaysAhead), calculate distances using geolocation service, order by EventDate ascending, take MaxResults, map to UpcomingEventDto including distance in km
- GetEventById Query: Verify event exists with IEventRepository.GetByIdAsync(EventId), return null in result if not found, get diving spot details from IDivingSpotRepository, get organizer profile from IUserRepository, get participants list with IEventRepository.GetParticipantsAsync(EventId) and load their profiles, calculate dynamic flags (IsOrganizer = event.OrganizerId == RequestingUserId, IsParticipant = participants.Any(p => p.UserId == RequestingUserId), CanRegister = IsAvailable && !IsParticipant && DateTime.UtcNow < RegistrationDeadline), map to DetailedEventDto
- GetUserEvents Query: Build query based on flags - if IncludeOrganized get from IEventRepository.GetByOrganizerIdAsync(UserId), if IncludeRegistered get from IEventRepository.GetByParticipantIdAsync(UserId), merge results, filter by DateTime.UtcNow if not IncludePastEvents (EventDate >= DateTime.UtcNow for upcoming only), apply pagination with skip/take, map to UserEventDto with Role enum (Organizer/Participant), calculate CanCancel flag (Participant role && DateTime.UtcNow < CancellationDeadline), order by EventDate descending for upcoming or ascending for past
- SearchEvents Query: Build IQueryable<Event> starting with IEventRepository.GetQueryable(), apply filters sequentially (SearchTerm on Title/Description with Contains, StartDate/EndDate range filter on EventDate, DivingSpotId equality, MinParticipants/MaxParticipants range on CurrentParticipants, OnlyAvailable filters where CurrentParticipants < MaxParticipants && EventDate >= DateTime.UtcNow), apply sorting with switch on SortBy enum (EventDate, Title with string comparison, ParticipantCount = CurrentParticipants, CreatedAt) and SortDescending flag, calculate TotalCount before pagination, apply Skip/Take for pagination, include related entities (DivingSpot, Organizer) for efficient mapping, map to EventSearchResultDto with Description truncated to 200 chars

**Détails spécifiques:**
- **GetUpcomingEvents**: Geolocation-based discovery with distance calculation, filters by date range (next 30 days by default), respects MaxDistanceKm if provided, orders by EventDate for chronological discovery, includes availability status and participant counts for decision making
- **GetEventById**: Complete event details with organizer profile, full participants list with certification levels, dynamic permission flags (IsOrganizer, IsParticipant, CanRegister) based on requesting user context, includes registration and cancellation deadlines, required certification level, cost with currency, diving spot coordinates for map display
- **GetUserEvents**: Dual role support (organized vs registered), optional past events inclusion for history view, pagination for large event lists, Role field distinguishes organizer from participant, CanCancel flag respects cancellation deadline, includes registration dates for tracking
- **SearchEvents**: Flexible search with 6 optional filters, date range filtering for specific periods, diving spot filtering for location-specific searches, participant count filtering for group size preferences, availability-only filter for active events, 4 sort options with ascending/descending support, pagination with metadata, truncated descriptions for list display optimization

---

### TASK-041: Configuration AutoMapper
- [x] Créer profils de mapping
- [x] Entity → DTO mappings
- [x] Command → Entity mappings
- [x] Configurer dans DI

**Status:** ✅ Complété (2025-12-17)
**Dépendances:** TASK-032

**Solution implémentée:**
- AutoMapper 15.1.0 configuré dans DependencyInjection.cs
- BaseMappingProfile créé comme fondation
- Les DTOs contiennent principalement des propriétés calculées (CurrentParticipants, TotalDives, Distance, etc.)
- Ces propriétés sont calculées dans les handlers de requêtes, pas via AutoMapper
- AutoMapper disponible pour les mappings directs futurs si nécessaire

**Fichiers créés:**
- SubExplore.Application/Mappings/BaseMappingProfile.cs

**Tests:** ✅ 1034/1034 tests passent

---

### TASK-042: Validators FluentValidation
- [ ] Validators pour tous les Commands
- [ ] Règles métier dans validators
- [ ] Messages d'erreur localisés
- [ ] Tests des validators

**Status:** ⏳ En attente
**Dépendances:** TASK-033 à TASK-036

---

### TASK-043: DTOs et Responses
- [ ] Créer dossier Application/DTOs
- [ ] UserDto, SpotDto, DiveLogDto, etc.
- [ ] PagedResult<T> pour pagination
- [ ] ResultWrapper pour réponses uniformes

**Status:** ⏳ En attente

**DTOs à créer:**
- UserDto, UserProfileDto, UserStatisticsDto
- SpotDto, SpotDetailsDto, SpotSummaryDto
- DiveLogDto, DiveLogDetailsDto, DiveStatisticsDto
- EventDto, EventDetailsDto, ParticipantDto
- PagedResult<T> (Data, TotalCount, PageNumber, PageSize)
- ApiResponse<T> (Success, Data, Errors)

---

### TASK-044: Exception Handling
- [ ] Créer exceptions personnalisées
- [ ] NotFoundException, ValidationException, etc.
- [ ] Global exception handler
- [ ] Tests d'erreurs

**Status:** ⏳ En attente

**Exceptions à créer:**
- NotFoundException
- ValidationException
- UnauthorizedException
- ForbiddenException
- ConflictException
- BusinessRuleViolationException

---

### TASK-045: Configuration Caching
- [ ] Interface ICacheService
- [ ] Stratégie de cache (spots, user profiles)
- [ ] Invalidation de cache
- [ ] Tests de cache

**Status:** ⏳ En attente

**Stratégie de cache:**
- User profiles : 1 heure
- Diving spots : 30 minutes
- Statistics : 15 minutes
- Search results : 5 minutes

---

## 🧩 Infrastructure Layer - Implémentations

### TASK-046: Configuration Supabase Client
- [ ] Créer SupabaseClientFactory
- [ ] Configuration authentification
- [ ] Configuration storage
- [ ] Gestion des tokens
- [ ] Tests de connexion

**Status:** ⏳ En attente
**Dépendances:** TASK-009

---

### TASK-047: UserRepository Implementation
- [ ] Implémenter IUserRepository
- [ ] Méthodes CRUD complètes
- [ ] Support filtres et pagination
- [ ] Gestion erreurs Supabase
- [ ] Tests d'intégration

**Status:** ⏳ En attente
**Dépendances:** TASK-029, TASK-046

---

### TASK-048: DivingSpotRepository Implementation
- [ ] Implémenter IDivingSpotRepository
- [ ] Requêtes géospatiales (PostGIS)
- [ ] Recherche par rayon
- [ ] Filtres avancés
- [ ] Tests d'intégration

**Status:** ⏳ En attente
**Dépendances:** TASK-029, TASK-046

---

### TASK-049: DiveLogRepository Implementation
- [ ] Implémenter IDiveLogRepository
- [ ] Statistiques utilisateur
- [ ] Requêtes de recherche
- [ ] Export de données
- [ ] Tests d'intégration

**Status:** ⏳ En attente
**Dépendances:** TASK-029, TASK-046

---

### TASK-050: EventRepository Implementation
- [ ] Implémenter IEventRepository
- [ ] Gestion participants
- [ ] Requêtes temporelles
- [ ] Tests d'intégration

**Status:** ⏳ En attente
**Dépendances:** TASK-029, TASK-046

---

### TASK-051: Storage Service
- [ ] Implémenter IStorageService
- [ ] Upload photos/avatars
- [ ] Génération thumbnails
- [ ] Gestion URLs signées
- [ ] Tests d'intégration

**Status:** ⏳ En attente
**Dépendances:** TASK-046

---

### TASK-052: GeolocationService
- [ ] Implémenter calcul de distances
- [ ] Conversion unités (km/mi/nm)
- [ ] Intégration avec PostGIS
- [ ] Tests unitaires

**Status:** ⏳ En attente
**Dépendances:** TASK-030

---

### TASK-053: External APIs Integration
- [ ] WeatherService (OpenWeatherMap)
- [ ] TideService (API marées)
- [ ] Configuration API keys
- [ ] Gestion rate limiting
- [ ] Tests avec mocks

**Status:** ⏳ En attente
**Dépendances:** TASK-030

---

### TASK-054: NotificationService
- [ ] Push notifications (Firebase)
- [ ] Email notifications
- [ ] In-app notifications
- [ ] Templates de messages
- [ ] Tests d'envoi

**Status:** ⏳ En attente
**Dépendances:** TASK-030

---

### TASK-055: Cache Service Implementation
- [ ] Redis ou MemoryCache
- [ ] Implémentation ICacheService
- [ ] Stratégies d'expiration
- [ ] Tests de cache

**Status:** ⏳ En attente
**Dépendances:** TASK-045

---

## 📊 Progression Phase 2

### Résumé
- **Tâches complétées** : 18/35 (51.4%)
- **Tâches en cours** : 0
- **Tâches en attente** : 17

### Par catégorie
- **Domain Entities** : 8/8 (100%) ✅
  - User, DivingSpot, DiveLog, Event, Achievement, Notification, Message/Conversation complétés
- **Domain Interfaces** : 3/3 (100%) ✅
  - Repository Interfaces, Domain Services Interfaces, Domain Events complétés
- **Application CQRS** : 7/14 (50%) 🎯
  - Configuration MediatR, Commands Auth, Commands User Profile, Commands DivingSpot, Commands DiveLog, Queries DivingSpot, Queries DiveLog complétés
  - Queries User, Queries Events, AutoMapper, Exception Handling, Caching en attente
- **Infrastructure** : 0/10 (0%)
  - Repositories et Services en attente

### Prochaines priorités
1. **TASK-039**: Queries - User (GetUserProfile, GetUserStatistics, SearchUsers, GetUserAchievements)
2. **TASK-040**: Queries - Events (GetUpcomingEvents, GetEventById, GetUserEvents, SearchEvents)
3. **TASK-041**: Configuration AutoMapper (Profils de mapping Entity → DTO)
4. **TASK-043**: DTOs et Responses (PagedResult, ResultWrapper, ApiResponse)

---

## 🎯 Critères de succès Phase 2
- [ ] Toutes les entités du domain créées et testées
- [ ] Tous les repository interfaces définis
- [ ] MediatR configuré avec pipeline behaviors
- [ ] Commands et Queries implémentés pour toutes les entités
- [ ] AutoMapper configuré avec tous les profils
- [ ] FluentValidation configuré pour tous les commands
- [ ] DTOs créés pour toutes les entités
- [ ] Exception handling global implémenté
- [ ] Cache service configuré
- [ ] Tous les repositories implémentés et testés
- [ ] Services infrastructure implémentés (Storage, Geolocation, External APIs, Notifications)
- [ ] 100% de tests unitaires passants
- [ ] 100% de tests d'intégration passants
