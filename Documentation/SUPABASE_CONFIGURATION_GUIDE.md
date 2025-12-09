# Guide de Configuration Supabase - SubExplore

## 📋 Prérequis
- Compte Supabase (gratuit)
- Email vérifié

## 🚀 Étape 1: Créer le Projet Supabase

1. **Connexion au Dashboard**
   - Allez sur https://app.supabase.com
   - Connectez-vous avec votre compte

2. **Créer un Nouveau Projet**
   - Cliquez sur "New Project"
   - Remplissez les informations:
     - **Name:** `SubExplore`
     - **Database Password:** Choisissez un mot de passe fort (NOTEZ-LE!)
     - **Region:** Choisissez la région la plus proche (ex: Europe West)
     - **Pricing Plan:** Free (suffisant pour le développement)
   - Cliquez sur "Create new project"
   - ⏳ Attendez 1-2 minutes que le projet soit créé

## 🔑 Étape 2: Récupérer les Clés API

Une fois le projet créé:

1. **Accéder aux Settings**
   - Dans le menu latéral, cliquez sur "Project Settings" (icône d'engrenage en bas)
   - Puis cliquez sur "API" dans le sous-menu

2. **Copier les Informations**
   Vous trouverez:

   **Project URL:**
   ```
   https://votre-projet-ref.supabase.co
   ```
   ↪️ C'est votre `SUPABASE_URL`

   **Project API keys:**
   - **anon / public:** `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
     ↪️ C'est votre `SUPABASE_ANON_KEY`
   - **service_role / secret:** `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
     ↪️ C'est votre `SUPABASE_SERVICE_ROLE_KEY` (⚠️ GARDEZ CETTE CLÉ SECRÈTE!)

## ⚙️ Étape 3: Configurer l'Application

### Option A: Fichier .env (Recommandé pour développement)

1. **Créer le fichier .env**
   ```bash
   # À la racine du projet SubExplore/
   cp .env.example .env
   ```

2. **Remplir avec vos valeurs**
   Ouvrez `.env` et remplacez:
   ```env
   SUPABASE_URL=https://votre-projet-ref.supabase.co
   SUPABASE_ANON_KEY=votre-anon-key-ici
   SUPABASE_SERVICE_ROLE_KEY=votre-service-role-key-ici
   ```

### Option B: appsettings.Development.json (Alternative)

Ouvrez `SubExplore.API/appsettings.Development.json` et ajoutez:

```json
{
  "Supabase": {
    "Url": "https://votre-projet-ref.supabase.co",
    "Key": "votre-anon-key-ici"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.votre-projet-ref.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=votre-mot-de-passe-db"
  }
}
```

⚠️ **IMPORTANT:** Ne commitez JAMAIS ce fichier avec vos vraies clés!

## 🔗 Étape 4: Récupérer la Connection String PostgreSQL

1. Dans Project Settings → Database
2. Copiez la **Connection String** en mode **URI**
3. Format: `postgresql://postgres:[VOTRE-PASSWORD]@db.xxx.supabase.co:5432/postgres`
4. Remplacez `[VOTRE-PASSWORD]` par le mot de passe choisi à l'étape 1

## ✅ Étape 5: Vérifier la Configuration

### Test de connexion basique (optionnel)

Vous pouvez tester la connexion dans le SQL Editor de Supabase:

1. Allez dans "SQL Editor" dans le menu
2. Exécutez cette requête simple:
```sql
SELECT version();
```

Si ça fonctionne, votre projet Supabase est prêt!

## 📝 Étape 6: Sécurité

### ⚠️ Règles de sécurité importantes:

1. **Ne JAMAIS committer:**
   - `.env`
   - `appsettings.Development.json` avec de vraies clés
   - Fichiers contenant `SUPABASE_SERVICE_ROLE_KEY`

2. **Vérifier .gitignore:**
   ```gitignore
   .env
   .env.local
   appsettings.Development.json
   **/appsettings.Development.json
   ```

3. **Service Role Key:**
   - Cette clé contourne toutes les RLS policies
   - Utilisez-la UNIQUEMENT côté serveur (API)
   - JAMAIS dans le client mobile

## 🎯 Prochaines Étapes

Maintenant que Supabase est configuré, vous pouvez:
1. ✅ TASK-006: Configuration des secrets et variables d'environnement
2. ✅ TASK-009: Exécuter le script SQL de création de la base de données
3. ✅ TASK-010: Configurer les Entity Models

## 🆘 Troubleshooting

### Erreur: "Invalid API key"
- Vérifiez que vous avez copié la clé complète
- Vérifiez qu'il n'y a pas d'espaces avant/après la clé

### Erreur: "Project not found"
- Vérifiez l'URL du projet
- Assurez-vous que le projet est bien créé et actif

### Erreur de connexion PostgreSQL
- Vérifiez le mot de passe de la base
- Vérifiez que le port 5432 n'est pas bloqué par un firewall

## 📚 Ressources

- [Documentation Supabase](https://supabase.com/docs)
- [Supabase C# Client](https://github.com/supabase-community/supabase-csharp)
- [Supabase Dashboard](https://app.supabase.com)
