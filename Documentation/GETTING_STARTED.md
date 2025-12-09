# 🚀 Guide de Premier Lancement - SubExplore

Bienvenue sur SubExplore ! Ce guide vous accompagne pas à pas pour lancer le projet sur votre machine de développement.

---

## 📋 Table des matières

1. [Prérequis](#prérequis)
2. [Installation](#installation)
3. [Configuration Supabase](#configuration-supabase)
4. [Configuration des Secrets](#configuration-des-secrets)
5. [Premier Build](#premier-build)
6. [Lancer l'Application](#lancer-lapplication)
7. [Vérifications](#vérifications)
8. [Dépannage](#dépannage)

---

## 📦 Prérequis

### Obligatoires

**1. .NET SDK 9.0+**
```bash
# Télécharger depuis https://dotnet.microsoft.com/download/dotnet/9.0

# Vérifier l'installation
dotnet --version
# Devrait afficher: 9.0.xxx
```

**2. Visual Studio 2022 (version 17.14+)** ou **VS Code**

Pour Visual Studio 2022, installer les workloads:
- ✅ Développement mobile avec .NET (MAUI)
- ✅ Développement ASP.NET et web
- ✅ Développement Android
- ✅ (Mac uniquement) Développement iOS avec Xamarin

**3. Android SDK**
- API Level 24 (Android 7.0) minimum
- API Level 34 (Android 14) recommandé

**4. Git**
```bash
# Vérifier l'installation
git --version
```

### Optionnels mais recommandés

- **Xcode** (Mac uniquement, pour iOS)
- **Android Studio** (pour gérer les émulateurs Android)
- **GitHub CLI** (`gh`) pour faciliter les interactions GitHub

---

## 🛠️ Installation

### Étape 1 : Cloner le repository

```bash
# Cloner le projet
git clone https://github.com/NigossFr/SubExplorev1.git

# Aller dans le dossier
cd SubExplorev1
```

### Étape 2 : Vérifier les workloads .NET MAUI

```bash
# Vérifier les workloads installés
dotnet workload list

# Vous devez voir:
# - android
# - ios (Mac uniquement)
# - maccatalyst (Mac uniquement)
# - maui
```

**Si des workloads manquent :**
```bash
# Installer les workloads MAUI
dotnet workload install maui

# Ou spécifiquement
dotnet workload install android
dotnet workload install ios        # Mac uniquement
dotnet workload install maccatalyst # Mac uniquement
```

⚠️ **Note :** L'installation des workloads peut prendre 10-30 minutes.

### Étape 3 : Restaurer les packages NuGet

```bash
# Restaurer tous les packages du projet
dotnet restore SubExplore.sln
```

**Packages installés :**
- `CommunityToolkit.Mvvm` 8.4.0
- `CommunityToolkit.Maui` 9.1.1
- `supabase-csharp` 0.16.2
- `DotNetEnv` 3.1.1
- `MediatR` 13.1.0
- `AutoMapper` 15.1.0
- `FluentValidation` 12.1.0

---

## 🔧 Configuration Supabase

### Étape 1 : Créer un compte Supabase

1. Allez sur https://supabase.com
2. Cliquez sur "Start your project"
3. Créez un compte (GitHub OAuth recommandé)

### Étape 2 : Créer un projet

1. Cliquez sur "New Project"
2. Remplissez les informations :
   - **Name:** SubExplorev1 (ou votre propre nom)
   - **Database Password:** ⚠️ **NOTEZ CE MOT DE PASSE**
   - **Region:** Choisir la région la plus proche
   - **Pricing Plan:** Free (suffisant pour le développement)
3. Cliquez sur "Create new project"
4. ⏳ Attendez 2-5 minutes que le projet soit créé

### Étape 3 : Récupérer les clés API

1. Une fois le projet créé, allez dans **Settings** > **API**
2. Notez les informations suivantes :
   - **Project URL** : `https://xxxxxxxxxx.supabase.co`
   - **anon public key** : `eyJhbGciOiJI...` (clé longue)
   - **service_role key** : `eyJhbGciOiJI...` (optionnel pour l'instant)

---

## 🔐 Configuration des Secrets

### Étape 1 : Créer le fichier .env

```bash
# À la racine du projet SubExplore/
cp .env.example .env
```

### Étape 2 : Remplir le fichier .env

Ouvrez le fichier `.env` avec votre éditeur et remplissez :

```env
# Configuration Supabase pour SubExplore
SUPABASE_URL=https://xxxxxxxxxx.supabase.co
SUPABASE_ANON_KEY=eyJhbGciOiJI...votre-clé-anon
SUPABASE_SERVICE_ROLE_KEY=

# Database Configuration
DATABASE_URL=

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

⚠️ **Important :**
- Remplacez `xxxxxxxxxx` par votre project ref
- Collez votre `anon public key`
- **Ne commitez JAMAIS ce fichier** (déjà protégé par .gitignore)

### Étape 3 : Configurer User Secrets (API)

```bash
# Aller dans le projet API
cd SubExplore.API

# Initialiser User Secrets
dotnet user-secrets init

# Ajouter les secrets Supabase
dotnet user-secrets set "Supabase:Url" "https://xxxxxxxxxx.supabase.co"
dotnet user-secrets set "Supabase:Key" "eyJhbGciOiJI...votre-clé-anon"

# Vérifier
dotnet user-secrets list

# Retourner à la racine
cd ..
```

**Guides détaillés :**
- 📖 [SUPABASE_CONFIGURATION_GUIDE.md](./SUPABASE_CONFIGURATION_GUIDE.md)
- 📖 [SECRETS_CONFIGURATION_GUIDE.md](./SECRETS_CONFIGURATION_GUIDE.md)

---

## 🔨 Premier Build

### Étape 1 : Build complet de la solution

```bash
# Build de tous les projets
dotnet build SubExplore.sln
```

**Résultat attendu :**
```
Build succeeded.
    2 Warning(s)
    0 Error(s)
```

⚠️ **Warnings normaux :**
- `MVVMTK0045` : Avertissements AOT pour Windows (non bloquants)

### Étape 2 : Tester la connexion Supabase

```bash
# Aller dans le projet de test
cd Tests/SupabaseConnectionTest

# Exécuter le test
dotnet run

# Résultat attendu:
# ✅ RÉSULTAT: Test réussi!
#    Vous pouvez maintenant utiliser Supabase dans votre application.
```

---

## 🚀 Lancer l'Application

### Option 1 : Android (Windows/Mac)

**Prérequis :**
- Émulateur Android démarré OU appareil Android connecté en USB

```bash
# Lancer sur Android
dotnet build -t:Run -f net9.0-android
```

**Première fois :**
- ⏳ Peut prendre 5-10 minutes (installation de l'app sur l'émulateur)
- Le déploiement sera plus rapide les fois suivantes

### Option 2 : Windows (Windows uniquement)

```bash
# Lancer sur Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Option 3 : iOS (Mac uniquement)

**Prérequis :**
- Xcode installé
- Simulateur iOS configuré

```bash
# Lancer sur iOS Simulator
dotnet build -t:Run -f net9.0-ios
```

### Option 4 : Via Visual Studio

1. Ouvrir `SubExplore.sln` dans Visual Studio 2022
2. Sélectionner la plateforme cible dans la barre d'outils :
   - **Android Emulator** / **Android Device**
   - **Windows Machine**
   - **iOS Simulator** (Mac uniquement)
3. Appuyer sur **F5** ou cliquer sur ▶️ **Run**

---

## ✅ Vérifications

### Checklist de démarrage

- [ ] .NET SDK 9.0+ installé (`dotnet --version`)
- [ ] Workloads MAUI installés (`dotnet workload list`)
- [ ] Repository cloné
- [ ] Packages NuGet restaurés (`dotnet restore`)
- [ ] Fichier `.env` créé et rempli
- [ ] User Secrets configurés pour l'API
- [ ] Build réussi (`dotnet build`)
- [ ] Test de connexion Supabase réussi
- [ ] Application lancée sur une plateforme

### Vérifier l'état du projet

```bash
# État Git
git status

# Vérifier que .env n'est PAS listé (doit être ignoré)

# Branches
git branch

# Commits
git log --oneline -5
```

---

## 🔧 Dépannage

### Erreur : "Workload not found"

**Problème :** Les workloads MAUI ne sont pas installés.

**Solution :**
```bash
dotnet workload install maui
```

### Erreur : "Unable to connect to Supabase"

**Problème :** Configuration Supabase incorrecte.

**Vérifications :**
1. Vérifier que `.env` existe et est rempli
2. Vérifier l'URL Supabase (doit se terminer par `.supabase.co`)
3. Vérifier que la clé `anon public` est complète
4. Vérifier que le projet Supabase est actif sur https://supabase.com

**Test manuel :**
```bash
cd Tests/SupabaseConnectionTest
dotnet run
```

### Erreur : "MAUI workload installation failed"

**Problème :** Échec de l'installation des workloads.

**Solutions :**
```bash
# Nettoyer et réinstaller
dotnet workload clean
dotnet workload restore

# Si ça ne fonctionne pas, installer manuellement
dotnet workload install android
dotnet workload install maui
```

### Erreur : "Android SDK not found"

**Problème :** Android SDK non configuré.

**Solution (Windows) :**
1. Installer Android Studio
2. Dans Android Studio : Tools > SDK Manager
3. Installer Android SDK 34 (Android 14)
4. Définir la variable d'environnement `ANDROID_HOME`

```bash
# Windows PowerShell
$env:ANDROID_HOME = "C:\Users\<USER>\AppData\Local\Android\Sdk"

# Linux/Mac
export ANDROID_HOME=$HOME/Android/Sdk
```

### Build lent / bloqué

**Problème :** Premier build MAUI très long.

**Explication :**
- Le premier build MAUI peut prendre 10-30 minutes
- Installation des runtimes Android, iOS, etc.
- Normal pour .NET MAUI

**Solution :** Patience ☕

---

## 📖 Documentation Complémentaire

### Guides de configuration
- [SUPABASE_CONFIGURATION_GUIDE.md](./SUPABASE_CONFIGURATION_GUIDE.md) - Configuration Supabase détaillée
- [SECRETS_CONFIGURATION_GUIDE.md](./SECRETS_CONFIGURATION_GUIDE.md) - Gestion des secrets

### Architecture
- [DESIGN_PATTERNS_ARCHITECTURE_AVANCEE.md](./Architecture%20et%20Développement/DESIGN_PATTERNS_ARCHITECTURE_AVANCEE.md) - Patterns utilisés
- [API_REST_DOCUMENTATION.md](./Architecture%20et%20Développement/API_REST_DOCUMENTATION.md) - Documentation API

### Base de données
- [SUPABASE_DATABASE_SETUP.sql](./Base%20de%20Données/SUPABASE_DATABASE_SETUP.sql) - Script SQL complet

### Contribution
- [GUIDE_CONTRIBUTION_EQUIPE.md](./Tests%20et%20Déploiement/GUIDE_CONTRIBUTION_EQUIPE.md) - Standards de code

---

## 🎯 Prochaines Étapes

Maintenant que votre environnement est configuré :

1. **Exécuter le script SQL Supabase** (TASK-009)
   - Créer les tables de la base de données
   - Configurer PostGIS et les extensions

2. **Explorer le code**
   - Parcourir les projets Domain, Application, Infrastructure
   - Comprendre l'architecture Clean Architecture

3. **Lire la documentation**
   - [TASK_TRACKER_SUBEXPLORE.md](./TASK_TRACKER_SUBEXPLORE.md) - Voir la progression
   - [cahier-des-charges-final.md](./Documents%20Fondamentaux/cahier-des-charges-final.md) - Comprendre le projet

4. **Commencer à développer !**
   - Suivre les conventions de code
   - Créer des branches pour vos features
   - Faire des commits descriptifs

---

## 💡 Conseils Pratiques

### Organisation du travail

1. **Toujours créer une branche** pour une nouvelle feature :
   ```bash
   git checkout -b feature/nom-de-la-feature
   ```

2. **Commits fréquents** avec messages clairs :
   ```bash
   git commit -m "feat(spots): add nearby spots search"
   ```

3. **Tester avant de committer** :
   ```bash
   dotnet test
   dotnet build
   ```

### Raccourcis utiles

```bash
# Build rapide
dotnet build

# Build + Run Android
dotnet build -t:Run -f net9.0-android

# Clean complet
dotnet clean

# Restaurer les packages
dotnet restore

# Lister les secrets API
cd SubExplore.API && dotnet user-secrets list
```

### Ressources utiles

- **Documentation .NET MAUI** : https://learn.microsoft.com/dotnet/maui/
- **Documentation Supabase** : https://supabase.com/docs
- **Community Toolkit** : https://learn.microsoft.com/dotnet/communitytoolkit/
- **Clean Architecture** : https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

---

## 🆘 Besoin d'aide ?

- 🐛 **Issues** : [GitHub Issues](https://github.com/NigossFr/SubExplorev1/issues)
- 📖 **Documentation** : [Documentation complète](https://github.com/NigossFr/SubExplorev1/tree/main/Documentation)
- 📝 **TASK_TRACKER** : [Suivi des tâches](./TASK_TRACKER_SUBEXPLORE.md)

---

**Dernière mise à jour :** 2025-12-09
**Version du guide :** 1.0.0

Bon développement ! 🚀🤿
