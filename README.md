# SubExplore 🤿

Application mobile communautaire pour les sports sous-marins (plongée, apnée, snorkeling).

## 📱 À propos

SubExplore permet aux passionnés de sports sous-marins de :
- Découvrir et partager des spots de plongée
- Tenir un carnet de plongée numérique
- Organiser des événements et sorties
- Suivre leurs statistiques et achievements
- Rejoindre une communauté de plongeurs

## 🏗️ Architecture Technique

### Technologies
- **Mobile:** .NET MAUI 9.0 (iOS, Android, Windows)
- **Backend:** ASP.NET Core 9.0 Web API
- **Base de données:** Supabase (PostgreSQL + PostGIS)
- **Architecture:** Clean Architecture + CQRS + MVVM
- **Authentification:** JWT avec Supabase Auth

### Structure de la Solution
```
SubExplore/
├── SubExplore.sln              # Solution principale
├── SubExplore.csproj           # Application mobile .NET MAUI
├── Documentation/              # Documentation complète du projet
│   ├── TASK_TRACKER_SUBEXPLORE.md  # Suivi des 234+ tâches
│   ├── Documents Fondamentaux/
│   ├── Architecture et Développement/
│   ├── Base de Données/
│   ├── Tests et Déploiement/
│   ├── Sécurité/
│   └── Opérations/
└── (À venir: Domain, Application, Infrastructure, API)
```

## 🚀 Prérequis

### Développement Mobile
- **.NET SDK 9.0+** ([Télécharger](https://dotnet.microsoft.com/download/dotnet/9.0))
- **Visual Studio 2022 17.14+** avec workloads:
  - Développement mobile avec .NET (MAUI)
  - Développement Android
  - Développement iOS (Mac uniquement)
- **Android SDK** (API 24+)
- **Xcode** (Mac uniquement, pour iOS)

### Vérifier l'installation
```bash
dotnet --version                    # Devrait afficher 9.0.xxx
dotnet workload list                # Vérifier android, ios, maccatalyst
```

## 🛠️ Installation

### 1. Cloner le repository
```bash
git clone https://github.com/NigossFr/SubExplorev1.git
cd SubExplorev1
```

### 2. Restaurer les packages NuGet
```bash
dotnet restore
```

### 3. Compiler la solution
```bash
dotnet build
```

### 4. Lancer l'application

#### Windows (émulateur Android)
```bash
dotnet build -t:Run -f net9.0-android
```

#### Windows (Windows natif)
```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

#### Mac (émulateur iOS)
```bash
dotnet build -t:Run -f net9.0-ios
```

#### Mac (émulateur Android)
```bash
dotnet build -t:Run -f net9.0-android
```

## 📋 État du Projet

### Phase 1: Configuration Initiale (35% - 7/20 tâches)
- [x] **TASK-001:** Structure de solution .NET MAUI ✅
  - Solution créée avec .NET 9.0
  - Support Android API 24+ (Android 7.0)
  - Support iOS 14.0+
  - Support Windows 10.0.19041.0+
  - Compilation sans erreurs ni warnings

- [x] **TASK-002:** Configuration Clean Architecture ✅
  - 4 projets créés (Domain, Application, Infrastructure, API)
  - Structure de dossiers logique
  - Références entre projets configurées

- [x] **TASK-003:** Installation des packages NuGet ✅
  - Domain: FluentValidation 12.1.0, ErrorOr 2.0.1
  - Application: MediatR 13.1.0, AutoMapper 15.1.0
  - Infrastructure: supabase-csharp 0.16.2, Npgsql 10.0.0
  - Mobile: CommunityToolkit.Mvvm 8.4.0, CommunityToolkit.Maui 9.1.1

- [x] **TASK-004:** Configuration MVVM ✅
  - BaseViewModel avec CommunityToolkit.Mvvm
  - Services (INavigationService, IDialogService)
  - Dependency Injection configurée

- [x] **TASK-005:** Configuration Supabase ✅
  - Projet Supabase créé (SubExplorev1)
  - Connexion testée et validée
  - Variables d'environnement configurées

- [x] **TASK-006:** Configuration des secrets ✅
  - User Secrets configurés pour API
  - appsettings.Development.json créé
  - Secrets protégés par .gitignore

- [x] **TASK-007:** Configuration Git ✅
  - Repository Git initialisé
  - Premier commit créé
  - Synchronisé sur GitHub

**Progression globale:** 7/234 tâches (3.0%)

Voir le fichier [TASK_TRACKER_SUBEXPLORE.md](./Documentation/TASK_TRACKER_SUBEXPLORE.md) pour le suivi détaillé.

## 📚 Documentation

Toute la documentation est disponible dans le dossier `Documentation/`:

### Documents Essentiels
- **[TASK_TRACKER_SUBEXPLORE.md](./Documentation/TASK_TRACKER_SUBEXPLORE.md)** - Suivi des 234+ tâches
- **[GETTING_STARTED.md](./Documentation/GETTING_STARTED.md)** - Guide de premier lancement
- **cahier-des-charges-final.md** - Spécifications complètes
- **ROADMAP_VISION_FUTURE.md** - Vision et planning 24 mois
- **GUIDE_IMPLEMENTATION_SUBEXPLORE.md** - Guide pratique

### Configuration
- **[SUPABASE_CONFIGURATION_GUIDE.md](./Documentation/SUPABASE_CONFIGURATION_GUIDE.md)** - Configuration Supabase détaillée
- **[SECRETS_CONFIGURATION_GUIDE.md](./Documentation/SECRETS_CONFIGURATION_GUIDE.md)** - Gestion des secrets et variables d'environnement

### Architecture
- **DESIGN_PATTERNS_ARCHITECTURE_AVANCEE.md** - Clean Architecture/MVVM
- **API_REST_DOCUMENTATION.md** - Documentation API complète
- **GUIDE_OPTIMISATION_PERFORMANCE.md** - Stratégies de performance

### Base de Données
- **SUPABASE_DATABASE_SETUP.sql** - Script SQL complet (1000+ lignes)

### Qualité et Sécurité
- **GUIDE_TESTS_DEPLOYMENT_CICD.md** - Standards de tests et CI/CD
- **GUIDE_SECURITE_RGPD.md** - Sécurité et conformité RGPD
- **GUIDE_CONTRIBUTION_EQUIPE.md** - Standards de code

## 🎯 Prochaines Étapes

1. **TASK-008:** Documentation de configuration ⏳ (En cours)
   - ✅ README.md mis à jour
   - ⏳ Guide de premier lancement (GETTING_STARTED.md)

2. **TASK-009:** Exécution du script SQL Supabase
   - Créer les extensions PostgreSQL (PostGIS)
   - Créer les tables principales
   - Configurer Row Level Security (RLS)
   - Créer les indexes et contraintes

3. **TASK-010:** Configuration Row Level Security
   - Vérifier activation RLS sur toutes les tables
   - Tester les policies de lecture publique
   - Vérifier isolation des données utilisateurs

4. **TASK-011:** Configuration Storage Supabase
   - Créer buckets (avatars, spot-photos, certification-docs)
   - Configurer les policies de storage
   - Tester upload/download

## 🤝 Contribution

Ce projet suit les standards de code définis dans `GUIDE_CONTRIBUTION_EQUIPE.md`.

### Conventions de Code
- **Classes:** PascalCase (ex: `SpotService`)
- **Interfaces:** IPascalCase (ex: `ISpotRepository`)
- **Méthodes:** PascalCase (ex: `GetNearbySpots`)
- **Variables:** camelCase (ex: `spotList`)
- **Constantes:** UPPER_CASE (ex: `MAX_RADIUS`)

### Commits
Format: `type(scope): description`
- `feat:` nouvelle fonctionnalité
- `fix:` correction de bug
- `docs:` documentation
- `refactor:` refactoring
- `test:` ajout de tests

Exemple: `feat(spots): add nearby spots search functionality`

## 📊 Métriques Qualité (Objectifs)

- ✅ Code coverage: >80%
- ✅ Code duplication: <5%
- ✅ Technical debt ratio: <5%
- ✅ Maintainability index: >70
- ✅ API response time: <200ms (p95)
- ✅ Mobile app start: <3s
- ✅ Crash-free rate: >99.5%

## 📞 Support

Pour toute question ou problème :
- 🐛 Issues: [GitHub Issues](https://github.com/NigossFr/SubExplorev1/issues)
- 📖 Documentation: [Documentation complète](https://github.com/NigossFr/SubExplorev1/tree/main/Documentation)

## 📄 Licence

[À définir]

---

**Version actuelle:** 0.1.0-alpha
**Dernière mise à jour:** 2025-12-09
**Statut:** 🟡 En développement actif
**Repository:** https://github.com/NigossFr/SubExplorev1
