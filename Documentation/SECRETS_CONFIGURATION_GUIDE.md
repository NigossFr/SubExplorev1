# 🔐 Guide de Configuration des Secrets - SubExplore

## 📋 Vue d'ensemble

Ce guide explique comment gérer les secrets et variables d'environnement de manière sécurisée dans le projet SubExplore.

**Architecture de configuration :**
- **Fichiers `.env`** : Variables d'environnement pour l'application mobile et Infrastructure
- **User Secrets** : Secrets de développement pour l'API (.NET Core)
- **appsettings.json** : Configuration publique et structure
- **appsettings.Development.json** : Configuration de développement (ignoré par Git)

---

## 🎯 Stratégie de gestion des secrets

### Hiérarchie de configuration

1. **Production** → Variables d'environnement système ou service cloud
2. **Staging** → Variables d'environnement ou Azure Key Vault
3. **Développement** → User Secrets (API) + fichier .env (Mobile/Infra)

### Fichiers protégés par .gitignore

```
✅ Protégés (ne jamais committer) :
- .env
- .env.local
- .env.*.local
- appsettings.Development.json
- appsettings.Staging.json
- appsettings.Production.json
- secrets.json
- *.key, *.p12, *.pfx

✅ À committer (templates sans secrets) :
- .env.example
- appsettings.json
```

---

## 🔧 Configuration pour le développement

### 1. Fichier `.env` (Mobile + Infrastructure)

**Emplacement :** `D:\Developpement\SubExplore V3\SubExplore\.env`

**Création :**
```bash
# Copiez le template
cp .env.example .env

# Éditez avec vos valeurs Supabase
```

**Contenu :**
```env
# Configuration Supabase pour SubExplore
SUPABASE_URL=https://your-project-ref.supabase.co
SUPABASE_ANON_KEY=your-anon-key-here
SUPABASE_SERVICE_ROLE_KEY=your-service-role-key-here

# Database Configuration
DATABASE_URL=postgresql://postgres:[PASSWORD]@db.your-project-ref.supabase.co:5432/postgres

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

**Utilisation dans le code :**
```csharp
// Charger le fichier .env
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
DotNetEnv.Env.Load(envPath);

// Lire les variables
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");
```

---

### 2. User Secrets (API)

**Projet concerné :** `SubExplore.API`

#### Initialisation

```bash
cd SubExplore.API
dotnet user-secrets init
```

Cela ajoute un `UserSecretsId` dans le fichier `.csproj` :
```xml
<PropertyGroup>
  <UserSecretsId>b05fb52f-dc1d-42a1-9e90-1188f2d7bad7</UserSecretsId>
</PropertyGroup>
```

#### Ajouter des secrets

```bash
# Supabase Configuration
dotnet user-secrets set "Supabase:Url" "https://gyhbrmpmbbqjhztyxwpg.supabase.co"
dotnet user-secrets set "Supabase:Key" "your-anon-key-here"

# Connection String
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

#### Lister les secrets

```bash
dotnet user-secrets list
```

#### Supprimer un secret

```bash
dotnet user-secrets remove "Supabase:Url"
```

#### Supprimer tous les secrets

```bash
dotnet user-secrets clear
```

#### Emplacement physique des secrets

**Windows :**
```
%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json
```

**macOS/Linux :**
```
~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
```

---

### 3. appsettings.json (Configuration publique)

**Emplacement :** `SubExplore.API/appsettings.json`

**Contenu :**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Supabase": {
    "Url": "",
    "Key": "",
    "Options": {
      "AutoRefreshToken": true,
      "PersistSession": true,
      "DetectSessionInUrl": true
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

**⚠️ Important :** Les valeurs Url et Key sont vides. Elles seront remplies par :
- User Secrets en développement
- Variables d'environnement en production

---

### 4. appsettings.Development.json (Développement)

**Emplacement :** `SubExplore.API/appsettings.Development.json`

**Contenu :**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "DetailedErrors": true,
  "Supabase": {
    "Url": "",
    "Key": "",
    "Options": {
      "AutoRefreshToken": true,
      "PersistSession": false,
      "DetectSessionInUrl": false
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "AllowedHosts": "*",
  "Cors": {
    "Origins": [
      "http://localhost:5000",
      "https://localhost:5001"
    ]
  }
}
```

**⚠️ Ce fichier est ignoré par Git** (ligne 161 de .gitignore)

---

## 🚀 Configuration par environnement

### Développement (local)

**API :**
- User Secrets pour Supabase:Url et Supabase:Key
- appsettings.Development.json pour les options

**Mobile/Infrastructure :**
- Fichier `.env` avec les valeurs Supabase
- DotNetEnv pour charger les variables

### Staging (test)

**Variables d'environnement système :**
```bash
export SUPABASE_URL="https://staging-project.supabase.co"
export SUPABASE_ANON_KEY="staging-anon-key"
export ConnectionStrings__DefaultConnection="staging-connection-string"
```

**appsettings.Staging.json :**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Production

**Variables d'environnement (Azure App Service, Docker, etc.) :**
```bash
SUPABASE_URL=https://production-project.supabase.co
SUPABASE_ANON_KEY=production-anon-key
ConnectionStrings__DefaultConnection=production-connection-string
ASPNETCORE_ENVIRONMENT=Production
```

**appsettings.Production.json :**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "DetailedErrors": false
}
```

---

## 🔍 Vérification de la configuration

### Vérifier les User Secrets (API)

```bash
cd SubExplore.API
dotnet user-secrets list
```

**Sortie attendue :**
```
Supabase:Url = https://gyhbrmpmbbqjhztyxwpg.supabase.co
Supabase:Key = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Vérifier le fichier .env

```bash
cat .env
```

**Sortie attendue :**
```env
SUPABASE_URL=https://gyhbrmpmbbqjhztyxwpg.supabase.co
SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Vérifier la protection Git

```bash
git status
```

**Fichiers qui ne doivent PAS apparaître :**
- `.env`
- `appsettings.Development.json`
- `secrets.json`

---

## 📖 Utilisation dans le code

### API (.NET Core)

```csharp
// Startup.cs ou Program.cs
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Lire la configuration Supabase
        var supabaseUrl = Configuration["Supabase:Url"];
        var supabaseKey = Configuration["Supabase:Key"];

        // Configurer le client Supabase
        services.AddSingleton<Supabase.Client>(sp =>
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = Configuration.GetValue<bool>("Supabase:Options:AutoRefreshToken"),
                PersistSession = Configuration.GetValue<bool>("Supabase:Options:PersistSession")
            };
            return new Supabase.Client(supabaseUrl, supabaseKey, options);
        });
    }
}
```

### Infrastructure (.NET)

```csharp
// Charger .env
var envPath = Path.Combine(
    Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "",
    ".env"
);

if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
}

// Lire les variables
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");

if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("SUPABASE_URL ou SUPABASE_ANON_KEY non défini");
}

// Créer le client
var client = new Supabase.Client(url, key, options);
```

---

## 🛡️ Bonnes pratiques de sécurité

### ✅ À faire

1. **Toujours utiliser User Secrets en développement**
   ```bash
   dotnet user-secrets set "SecretKey" "value"
   ```

2. **Ne jamais committer de secrets**
   - Vérifier avec `git status` avant chaque commit
   - Utiliser `.env.example` pour les templates

3. **Rotation régulière des clés**
   - Supabase : Régénérer les clés tous les 6 mois
   - API Keys : Rotation mensuelle recommandée

4. **Principe du moindre privilège**
   - Utiliser `ANON_KEY` pour le client
   - `SERVICE_ROLE_KEY` uniquement côté serveur

5. **Variables d'environnement en production**
   - Azure App Service : Configuration → Application Settings
   - Docker : fichier `.env` monté en volume (hors image)
   - Kubernetes : Secrets et ConfigMaps

### ❌ À éviter

1. **Hardcoder des secrets dans le code**
   ```csharp
   // ❌ JAMAIS FAIRE ÇA
   var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
   ```

2. **Committer des fichiers avec secrets**
   - `.env`
   - `appsettings.Development.json`
   - `secrets.json`

3. **Partager des secrets par email/chat**
   - Utiliser un gestionnaire de mots de passe d'équipe (1Password, LastPass)
   - Ou partage sécurisé temporaire (ex: onetimesecret.com)

4. **Utiliser les mêmes secrets en dev et prod**
   - Toujours avoir des projets Supabase séparés
   - Dev : `subexplore-dev`
   - Prod : `subexplore-prod`

---

## 🔧 Dépannage

### User Secrets ne fonctionne pas

**Problème :** Les valeurs des User Secrets ne sont pas chargées

**Solutions :**
1. Vérifier l'environnement :
   ```bash
   echo $ASPNETCORE_ENVIRONMENT
   # Doit être "Development"
   ```

2. Vérifier le UserSecretsId dans `.csproj` :
   ```xml
   <UserSecretsId>b05fb52f-dc1d-42a1-9e90-1188f2d7bad7</UserSecretsId>
   ```

3. Lister les secrets :
   ```bash
   dotnet user-secrets list
   ```

4. Réinitialiser si nécessaire :
   ```bash
   dotnet user-secrets clear
   dotnet user-secrets set "Key" "Value"
   ```

### Fichier .env non trouvé

**Problème :** `FileNotFoundException: .env not found`

**Solutions :**
1. Vérifier l'emplacement du fichier :
   ```bash
   ls -la .env
   ```

2. Vérifier le chemin dans le code :
   ```csharp
   var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
   Console.WriteLine($"Looking for .env at: {envPath}");
   ```

3. Créer à partir du template :
   ```bash
   cp .env.example .env
   ```

### Variables d'environnement non définies

**Problème :** `SUPABASE_URL` ou `SUPABASE_ANON_KEY` est null

**Solutions :**
1. Vérifier que le fichier .env est chargé :
   ```csharp
   DotNetEnv.Env.Load(envPath);
   ```

2. Vérifier les variables :
   ```bash
   # Windows PowerShell
   $env:SUPABASE_URL

   # Linux/macOS
   echo $SUPABASE_URL
   ```

3. Définir manuellement (temporaire) :
   ```bash
   # Windows
   set SUPABASE_URL=https://project.supabase.co

   # Linux/macOS
   export SUPABASE_URL=https://project.supabase.co
   ```

---

## 📚 Références

- [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/)
- [DotNetEnv Library](https://github.com/tonerdo/dotnet-env)
- [Supabase Configuration](https://supabase.com/docs/guides/getting-started)
- [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/)

---

## ✅ Checklist de sécurité

- [ ] User Secrets initialisé pour SubExplore.API
- [ ] Fichier .env créé et rempli (ne pas committer)
- [ ] .env.example à jour avec tous les champs nécessaires
- [ ] .gitignore protège tous les fichiers sensibles
- [ ] Pas de secrets hardcodés dans le code
- [ ] appsettings.Development.json ignoré par Git
- [ ] Variables d'environnement configurées en production
- [ ] Rotation des clés planifiée (calendrier 6 mois)
- [ ] Accès aux secrets limité à l'équipe nécessaire
- [ ] Documentation à jour

---

**Date de création :** 2025-12-09
**Dernière mise à jour :** 2025-12-09
**Version :** 1.0.0
