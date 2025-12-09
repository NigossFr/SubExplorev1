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
git clone https://github.com/votre-repo/subexplore.git
cd subexplore
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

### Phase 1: Configuration Initiale (En cours)
- [x] **TASK-001:** Structure de solution .NET MAUI ✅
  - Solution créée avec .NET 9.0
  - Support Android API 24+ (Android 7.0)
  - Support iOS 14.0+
  - Support Windows 10.0.19041.0+
  - Compilation sans erreurs ni warnings
- [ ] **TASK-002:** Configuration Clean Architecture
- [ ] **TASK-003:** Installation des packages NuGet
- [ ] **TASK-004:** Configuration MVVM
- [ ] **TASK-005:** Configuration Supabase

**Progression globale:** 1/234 tâches (0.4%)

Voir le fichier [TASK_TRACKER_SUBEXPLORE.md](./Documentation/TASK_TRACKER_SUBEXPLORE.md) pour le suivi détaillé.

## 📚 Documentation

Toute la documentation est disponible dans le dossier `Documentation/`:

### Documents Essentiels
- **[TASK_TRACKER_SUBEXPLORE.md](./Documentation/TASK_TRACKER_SUBEXPLORE.md)** - Suivi des 234+ tâches
- **cahier-des-charges-final.md** - Spécifications complètes
- **ROADMAP_VISION_FUTURE.md** - Vision et planning 24 mois
- **GUIDE_IMPLEMENTATION_SUBEXPLORE.md** - Guide pratique

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

1. **TASK-002:** Créer la structure Clean Architecture
   - Projet Domain (entités, interfaces)
   - Projet Application (use cases, CQRS)
   - Projet Infrastructure (repositories, services)
   - Projet API (ASP.NET Core Web API)

2. **TASK-003:** Installer les packages NuGet essentiels
   - MediatR, FluentValidation, AutoMapper
   - Supabase SDK
   - CommunityToolkit.Mvvm

3. **TASK-005:** Configurer Supabase
   - Créer projet Supabase
   - Exécuter script SQL (1000+ lignes)
   - Configurer Auth et Storage

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
- 📧 Email: support@subexplore.app
- 🐛 Issues: [GitHub Issues](https://github.com/votre-repo/subexplore/issues)
- 📖 Wiki: [Documentation Wiki](https://github.com/votre-repo/subexplore/wiki)

## 📄 Licence

[À définir]

---

**Version actuelle:** 0.1.0-alpha
**Dernière mise à jour:** 2025-11-28
**Statut:** 🟡 En développement actif
