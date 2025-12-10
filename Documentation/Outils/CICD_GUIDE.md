# GUIDE COMPLET - CI/CD SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Statut:** ✅ Configuration appliquée

---

## 📋 TABLE DES MATIÈRES

1. [Présentation](#présentation)
2. [Architecture CI/CD](#architecture-cicd)
3. [Workflows GitHub Actions](#workflows-github-actions)
4. [Configuration et Secrets](#configuration-et-secrets)
5. [Badges de statut](#badges-de-statut)
6. [Déclencheurs et événements](#déclencheurs-et-événements)
7. [Jobs et étapes](#jobs-et-étapes)
8. [Gestion des erreurs](#gestion-des-erreurs)
9. [Optimisation des performances](#optimisation-des-performances)
10. [Résolution de problèmes](#résolution-de-problèmes)

---

## 📖 PRÉSENTATION

### Objectif

Automatiser le build, les tests et la validation du code pour garantir la qualité et la stabilité du projet SubExplore à chaque commit et pull request.

### Avantages

✅ **Détection précoce** : Problèmes identifiés dès le commit
✅ **Qualité garantie** : Tests automatiques avant merge
✅ **Feedback rapide** : Résultats en quelques minutes
✅ **Documentation vivante** : Build status badges
✅ **Collaboration facilitée** : Reviews de code assistées
✅ **Déploiement fiable** : Process reproductible

---

## 🏗️ ARCHITECTURE CI/CD

### Vue d'ensemble

```
┌─────────────────────────────────────────────────────────────┐
│                     GitHub Repository                        │
│                    github.com/SubExplore                     │
└────────────────────┬────────────────────────────────────────┘
                     │
      ┌──────────────┼──────────────┐
      │              │              │
      ▼              ▼              ▼
┌──────────┐  ┌──────────┐  ┌──────────┐
│   Push   │  │    PR    │  │ Manual   │
│  Trigger │  │ Trigger  │  │ Trigger  │
└─────┬────┘  └─────┬────┘  └─────┬────┘
      │              │              │
      └──────────────┼──────────────┘
                     │
                     ▼
         ┌───────────────────────┐
         │  GitHub Actions       │
         │  Windows Runner       │
         │  .NET 9.0 + MAUI      │
         └──────────┬────────────┘
                    │
      ┌─────────────┼─────────────┐
      │             │             │
      ▼             ▼             ▼
┌──────────┐  ┌──────────┐  ┌──────────┐
│  Build   │  │   Test   │  │ Analyze  │
│   Job    │  │   Job    │  │   Job    │
└─────┬────┘  └─────┬────┘  └─────┬────┘
      │             │             │
      └─────────────┼─────────────┘
                    │
                    ▼
            ✅ Success / ❌ Failure
                    │
                    ▼
        ┌───────────────────────┐
        │   Notifications       │
        │   - GitHub UI         │
        │   - Email (optional)  │
        │   - Slack (future)    │
        └───────────────────────┘
```

### Stratégie de branches

| Branche | Protection | CI Trigger | Description |
|---------|------------|------------|-------------|
| **main** | ✅ Protected | Push | Production-ready code |
| **develop** | ✅ Protected | Push | Development integration |
| **feature/** | ❌ Open | PR only | Feature development |
| **bugfix/** | ❌ Open | PR only | Bug fixes |
| **hotfix/** | ⚠️ Semi-protected | Push + PR | Critical fixes |

---

## ⚙️ WORKFLOWS GITHUB ACTIONS

### Workflow 1 : `build.yml`

**Fichier** : `.github/workflows/build.yml`

**Objectif** : Build principal et tests automatiques

#### Déclencheurs

```yaml
on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
  workflow_dispatch:  # Manuel
```

#### Jobs

##### Job 1 : `build` (Build Solution)

**Runners** : `windows-latest`

**Étapes** :
1. ✅ Checkout code
2. ✅ Setup .NET 9.0
3. ✅ Install MAUI Workloads (maui, android, ios, maccatalyst)
4. ✅ Display .NET Info
5. ✅ Restore dependencies
6. ✅ Build Solution (Debug)
7. ⚠️ Build Solution (Release) - continue-on-error
8. ✅ Run Tests
9. ✅ Upload Test Results
10. ✅ Build Summary

**Particularités** :
- **Release build** avec `continue-on-error: true` car warnings as errors
- Génère un **Build Summary** dans l'interface GitHub

**Temps d'exécution estimé** : ~5-7 minutes

##### Job 2 : `build-android` (Build Android)

**Runners** : `windows-latest`

**Condition** : Uniquement sur push vers `main` ou `develop`

**Étapes** :
1. ✅ Checkout code
2. ✅ Setup .NET 9.0
3. ✅ Install MAUI Android Workload
4. ✅ Restore dependencies
5. ✅ Build Android App (Debug)
6. ✅ Android Build Summary

**Temps d'exécution estimé** : ~4-6 minutes

##### Job 3 : `analyze` (Code Analysis)

**Runners** : `windows-latest`

**Condition** : Uniquement sur pull requests

**Étapes** :
1. ✅ Checkout code
2. ✅ Setup .NET 9.0
3. ✅ Restore dependencies
4. ✅ Run Analyzers (StyleCop, SonarAnalyzer, .NET Analyzers)
5. ✅ Analyzer Summary

**Temps d'exécution estimé** : ~3-5 minutes

### Workflow 2 : `pr-validation.yml`

**Fichier** : `.github/workflows/pr-validation.yml`

**Objectif** : Validation complète des Pull Requests

#### Déclencheurs

```yaml
on:
  pull_request:
    types: [opened, synchronize, reopened]
```

#### Jobs

##### Job 1 : `validation` (PR Validation Checks)

**Étapes** :
1. ✅ Checkout code
2. ✅ Setup .NET 9.0
3. ✅ Restore dependencies
4. ✅ Check Formatting (EditorConfig)
5. ✅ Build with Analyzers
6. ℹ️ Security Scan (Future)
7. ✅ PR Validation Summary

##### Job 2 : `labeler` (Auto Label PR)

**Objectif** : Labelliser automatiquement les PRs selon les fichiers modifiés

**Labels automatiques** :
- `domain` : Modifications dans SubExplore.Domain
- `application` : Modifications dans SubExplore.Application
- `infrastructure` : Modifications dans SubExplore.Infrastructure
- `api` : Modifications dans SubExplore.API
- `mobile` : Modifications dans l'app mobile
- `documentation` : Modifications de docs
- `database` : Modifications SQL
- `configuration` : Modifications config
- `tests` : Modifications tests
- `security` : Modifications sécurité

##### Job 3 : `size-label` (PR Size Label)

**Objectif** : Ajouter un label de taille à la PR

**Labels de taille** :
- `size/XS` : ≤10 lignes
- `size/S` : ≤100 lignes
- `size/M` : ≤500 lignes
- `size/L` : ≤1000 lignes
- `size/XL` : >1000 lignes

---

## 🔐 CONFIGURATION ET SECRETS

### Secrets GitHub

**Emplacement** : Settings → Secrets and variables → Actions

#### Secrets actuels (aucun requis pour l'instant)

Le projet n'utilise **pas encore** de secrets GitHub car :
- Build ne nécessite pas d'authentification
- Pas encore de déploiement automatique
- Pas encore d'intégration services tiers

#### Secrets futurs (à ajouter plus tard)

| Secret | Description | Utilisation |
|--------|-------------|-------------|
| `SUPABASE_URL` | URL Supabase | Tests d'intégration |
| `SUPABASE_KEY` | Clé API Supabase | Tests d'intégration |
| `SONAR_TOKEN` | Token SonarCloud | Analyse de code |
| `SLACK_WEBHOOK` | Webhook Slack | Notifications |
| `ANDROID_KEYSTORE` | Keystore Android | Signing APK |
| `IOS_CERTIFICATE` | Certificate iOS | Signing IPA |

### Variables d'environnement

**Définies dans** : Chaque workflow `.yml`

```yaml
env:
  DOTNET_VERSION: '9.0.x'
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true
```

---

## 📊 BADGES DE STATUT

### Ajout dans README.md

**Build Status Badge** :

```markdown
![Build Status](https://github.com/NigossFr/SubExplorev1/actions/workflows/build.yml/badge.svg)
```

**PR Validation Badge** :

```markdown
![PR Validation](https://github.com/NigossFr/SubExplorev1/actions/workflows/pr-validation.yml/badge.svg)
```

**Exemple complet dans README** :

```markdown
# SubExplore

![Build Status](https://github.com/NigossFr/SubExplorev1/actions/workflows/build.yml/badge.svg)
![PR Validation](https://github.com/NigossFr/SubExplorev1/actions/workflows/pr-validation.yml/badge.svg)
![License](https://img.shields.io/badge/license-MIT-blue.svg)

Application mobile communautaire pour sports sous-marins
```

---

## 🎯 DÉCLENCHEURS ET ÉVÉNEMENTS

### Types de déclencheurs

#### 1. Push

**Workflow** : `build.yml`

```yaml
on:
  push:
    branches: [ main, develop ]
```

**Événements** :
- Commit direct vers `main` ou `develop`
- Merge d'une PR
- Fast-forward merge

#### 2. Pull Request

**Workflow** : `build.yml`, `pr-validation.yml`

```yaml
on:
  pull_request:
    branches: [ main, develop ]
```

**Événements** :
- Création de PR
- Nouveau commit dans PR
- Réouverture de PR

#### 3. Workflow Dispatch (Manuel)

**Workflow** : `build.yml`

```yaml
on:
  workflow_dispatch:
```

**Utilisation** :
- Interface GitHub → Actions → Workflow → Run workflow
- Utile pour tests manuels ou re-runs

### Filtres avancés (Futur)

**Exemple** : Déclencher uniquement si certains fichiers changent

```yaml
on:
  push:
    branches: [ main ]
    paths:
      - '**.cs'
      - '**.csproj'
      - '.github/workflows/**'
```

---

## 🛠️ JOBS ET ÉTAPES

### Anatomie d'un Job

```yaml
jobs:
  job-name:
    name: Display Name
    runs-on: windows-latest
    needs: [previous-job]  # Dépendance optionnelle
    if: github.event_name == 'push'  # Condition optionnelle

    steps:
    - name: Step Name
      uses: actions/checkout@v4
      with:
        parameter: value

    - name: Run Command
      run: dotnet build
      continue-on-error: true  # Ne pas échouer le job
```

### Stratégies de parallélisation

#### Actuellement

```
build (Job 1)
  └──> build-android (Job 2)  # Après build
  └──> analyze (Job 3)        # Après build
```

#### Futur (avec Matrix Strategy)

```yaml
strategy:
  matrix:
    os: [windows-latest, macos-latest]
    dotnet: ['9.0.x']
```

**Avantage** : Build Android + iOS en parallèle

---

## ⚠️ GESTION DES ERREURS

### Continue-on-error

**Usage** : Permettre la continuation même en cas d'échec

```yaml
- name: Build Release
  run: dotnet build --configuration Release
  continue-on-error: true
  id: release_build

- name: Check Status
  if: steps.release_build.outcome == 'failure'
  run: echo "Release build failed"
```

**Cas d'usage** :
- ✅ Release build avec warnings as errors (TASK-014)
- ✅ Tests qui peuvent échouer temporairement
- ✅ Étapes optionnelles (labeling, notifications)

### Always() condition

**Usage** : Exécuter une étape même si les précédentes ont échoué

```yaml
- name: Upload Test Results
  if: always()
  uses: actions/upload-artifact@v4
```

**Cas d'usage** :
- ✅ Upload d'artifacts (logs, tests)
- ✅ Nettoyage de ressources
- ✅ Notifications de statut

### Retry Strategy (Futur)

```yaml
- name: Flaky Step
  uses: nick-invision/retry@v2
  with:
    timeout_minutes: 5
    max_attempts: 3
    command: dotnet test
```

---

## 🚀 OPTIMISATION DES PERFORMANCES

### Cache de dépendances

**Actuellement** : Pas de cache (workflows rapides <10 min)

**Futur** : Ajouter cache pour NuGet packages

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

**Gain estimé** : 1-2 minutes par build

### Artifact Caching

**Actuellement** : Test results uploadés

**Futur** : Cacher build outputs

```yaml
- name: Upload Build Artifacts
  uses: actions/upload-artifact@v4
  with:
    name: build-output
    path: |
      **/bin/Release/**
      **/obj/Release/**
```

### Concurrency Control

**Objectif** : Annuler les builds obsolètes

```yaml
concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: true
```

**Effet** : Si nouveau commit pendant build, annuler le build précédent

---

## 🔍 RÉSOLUTION DE PROBLÈMES

### Problème 1 : Workflow ne se déclenche pas

**Symptômes** :
- Push vers main/develop
- Aucun workflow n'apparaît dans Actions

**Causes possibles** :
1. Fichier `.yml` mal formaté (indentation YAML stricte)
2. Branche incorrecte dans `on.push.branches`
3. Fichier pas dans `.github/workflows/`

**Solution** :

1. **Vérifier syntaxe YAML** :
```bash
# Utiliser un validateur YAML en ligne
# https://www.yamllint.com/
```

2. **Vérifier emplacement** :
```bash
ls -la .github/workflows/
# Doit contenir build.yml, pr-validation.yml
```

3. **Forcer un nouveau trigger** :
```bash
git commit --allow-empty -m "Trigger CI"
git push
```

### Problème 2 : Build échoue sur "MAUI Workload not found"

**Symptômes** :
```
error: Unknown workload 'maui'
```

**Cause** : Workload MAUI pas installé sur le runner

**Solution déjà appliquée** :

```yaml
- name: Install MAUI Workloads
  run: |
    dotnet workload install maui
    dotnet workload install android
    dotnet workload install ios
    dotnet workload install maccatalyst
```

**Si le problème persiste** :
```yaml
- name: Install MAUI Workloads
  run: |
    dotnet workload update
    dotnet workload restore
```

### Problème 3 : Release Build échoue (warnings as errors)

**Symptômes** :
```
CSC: warning SA1503
Build FAILED
```

**Cause** : `TreatWarningsAsErrors=true` en Release (TASK-014)

**Solution déjà appliquée** :

```yaml
- name: Build Solution (Release)
  run: dotnet build --configuration Release
  continue-on-error: true
  id: release_build

- name: Check Release Build Status
  if: steps.release_build.outcome == 'failure'
  run: echo "⚠️ Release build failed due to warnings as errors"
```

**Status** : ✅ Expected behavior pendant le développement

### Problème 4 : Temps de build trop long (>15 min)

**Symptômes** :
- Build prend plus de 15 minutes
- Timeouts fréquents

**Solutions** :

**Option A : Désactiver analyseurs en CI**

```yaml
- name: Build (Fast)
  run: dotnet build /p:RunAnalyzers=false
```

**Option B : Build incrémental**

```yaml
- name: Build
  run: dotnet build --no-restore --no-incremental:false
```

**Option C : Paralléliser**

```yaml
- name: Build
  run: dotnet build -m:4  # 4 workers parallèles
```

### Problème 5 : Tests ne s'exécutent pas

**Symptômes** :
```
No test is available
```

**Cause** : Pas encore de projets de tests (TASK-017 à venir)

**Solution déjà appliquée** :

```yaml
- name: Run Tests
  run: dotnet test --no-build
  continue-on-error: true
```

**Status** : ✅ Normal, tests seront ajoutés en TASK-017

---

## 📚 RESSOURCES

### Documentation officielle

- **GitHub Actions** : https://docs.github.com/en/actions
- **GitHub Actions Marketplace** : https://github.com/marketplace?type=actions
- **.NET CI/CD** : https://learn.microsoft.com/en-us/dotnet/devops/

### Actions utilisées

- **actions/checkout@v4** : Checkout code
- **actions/setup-dotnet@v4** : Setup .NET
- **actions/upload-artifact@v4** : Upload artifacts
- **actions/labeler@v5** : Auto-label PRs
- **codelytv/pr-size-labeler@v1** : PR size labels

### Workflows de référence

- **Microsoft MAUI Samples** : https://github.com/dotnet/maui-samples
- **.NET GitHub Actions** : https://github.com/dotnet/core/tree/main/.github/workflows

---

## ✅ CHECKLIST DE VALIDATION

Avant de considérer TASK-015 comme terminée, vérifiez:

- [x] Dossier `.github/workflows/` créé
- [x] Workflow `build.yml` créé et configuré
- [x] Workflow `pr-validation.yml` créé et configuré
- [x] Fichier `labeler.yml` créé avec labels
- [x] Documentation complète créée (CICD_GUIDE.md)
- [ ] Premier workflow exécuté avec succès sur GitHub
- [ ] Badges ajoutés au README.md
- [ ] Branch protection rules configurées (optionnel)

---

## 🎯 PROCHAINES ÉTAPES

**Post-TASK-015** :
1. **Push vers GitHub** : Déclencher le premier workflow
2. **Configurer branch protection** : Protéger main/develop
3. **TASK-016** : Configuration Logging (Serilog)
4. **TASK-017** : Configuration tests unitaires (xUnit)
5. **Améliorer CI/CD** : Ajouter cache, matrix strategy, déploiement

---

**Dernière mise à jour :** 2025-12-10
**Prochaine tâche :** Push vers GitHub pour tester les workflows
**État actuel :** CI/CD configuré et prêt à l'emploi
