# GUIDE DE CONFIGURATION AUTHENTIFICATION SUPABASE - SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Durée estimée:** ~20-30 minutes

---

## 📋 Vue d'ensemble

Ce guide décrit la configuration de l'authentification Supabase pour SubExplore.

**Méthodes d'authentification configurées :**
- ✅ **Email/Password** (obligatoire)
- 🔒 **OAuth Google** (optionnel - recommandé)
- 🔒 **OAuth Apple** (optionnel - iOS uniquement)

**Fonctionnalités :**
- Inscription avec email + mot de passe
- Confirmation d'email obligatoire
- Réinitialisation de mot de passe
- Connexion sécurisée
- Gestion des sessions

---

## 🎯 Objectifs TASK-012

- [x] Activer Email/Password provider
- [x] Configurer paramètres de sécurité
- [x] Personnaliser templates d'emails
- [x] Configurer URLs de redirection
- [x] Tester inscription utilisateur
- [x] Tester connexion

---

## 🚀 PARTIE 1 : Configuration Email/Password

### Étape 1 : Accéder aux paramètres Auth

1. Connectez-vous à **Supabase** : https://supabase.com
2. Sélectionnez votre projet : **SubExplorev1**
3. Dans le menu latéral, cliquez sur **Authentication**
4. Cliquez sur **Providers** dans le sous-menu

---

### Étape 2 : Activer Email Provider

**Instructions :**
1. Dans la liste des providers, localisez **Email**
2. Cliquez sur **Email** pour ouvrir les paramètres
3. Assurez-vous que **Enable Email provider** est ✅ **activé**
4. Vérifiez les paramètres suivants :

**Configuration recommandée :**
```yaml
Enable Email provider: ✅ Activé
Confirm email: ✅ Activé (IMPORTANT pour sécurité)
Secure email change: ✅ Activé
```

**Explication :**
- **Confirm email** : Force l'utilisateur à confirmer son email avant de pouvoir se connecter
- **Secure email change** : Envoie un email de confirmation lors du changement d'email

5. Cliquez sur **Save** pour enregistrer

---

### Étape 3 : Configurer les paramètres de sécurité du mot de passe

**Accès :**
1. Restez dans **Authentication** → **Providers**
2. Cliquez sur **Email** si ce n'est pas déjà ouvert
3. Descendez jusqu'à la section **Password Settings**

**Configuration recommandée :**
```yaml
Minimum password length: 8 caractères
Require uppercase: ✅ Recommandé
Require lowercase: ✅ Recommandé
Require numbers: ✅ Recommandé
Require special characters: ⚠️ Optionnel (peut être contraignant)
```

**Configuration SubExplore (équilibrée) :**
```yaml
Minimum password length: 8
Require uppercase: ✅ Activé
Require lowercase: ✅ Activé
Require numbers: ✅ Activé
Require special characters: ❌ Désactivé (pour faciliter l'UX)
```

**Exemple de mot de passe valide :**
- ✅ `Plongee2024`
- ✅ `SubExplore99`
- ❌ `plongee` (pas de majuscule, pas de chiffre)
- ❌ `PLONGEE` (pas de minuscule, pas de chiffre)

---

## 🔐 PARTIE 2 : Configuration des URLs de redirection

### Étape 4 : Configurer les Redirect URLs

**Accès :**
1. Cliquez sur **Authentication** → **URL Configuration**
2. Section **Redirect URLs**

**URLs à ajouter pour SubExplore :**

```
# Développement local
http://localhost:8081/auth/callback
http://127.0.0.1:8081/auth/callback

# Production (à ajouter plus tard)
https://subexplore.app/auth/callback
https://www.subexplore.app/auth/callback

# Deep Links Mobile (.NET MAUI)
subexplore://auth/callback
subexplore://reset-password
subexplore://verify-email
```

**Instructions :**
1. Cliquez sur **Add URL** pour chaque URL
2. Collez l'URL dans le champ
3. Cliquez sur **Save**
4. Répétez pour toutes les URLs

**Important :** Les deep links `subexplore://` sont utilisés par l'application mobile .NET MAUI.

---

### Étape 5 : Configurer Site URL

**Accès :**
1. Toujours dans **URL Configuration**
2. Section **Site URL**

**Configuration :**
```
Site URL: https://subexplore.app (production)
OU
Site URL: http://localhost:8081 (développement)
```

**Recommandation :** Utilisez `http://localhost:8081` pendant le développement, puis changez pour l'URL de production lors du déploiement.

---

## 📧 PARTIE 3 : Personnalisation des Templates d'Emails

### Étape 6 : Configurer les Email Templates

**Accès :**
1. Cliquez sur **Authentication** → **Email Templates**
2. Vous verrez 3 templates principaux :
   - **Confirm signup** (Confirmation d'inscription)
   - **Magic Link** (Connexion sans mot de passe)
   - **Reset Password** (Réinitialisation de mot de passe)

---

### Template 1 : Confirmation d'inscription (Confirm signup)

**Objectif :** Email envoyé après inscription pour confirmer l'adresse email

**Template personnalisé SubExplore :**

**Subject :**
```
Bienvenue sur SubExplore - Confirmez votre email
```

**Body (HTML) :**
```html
<h2>Bienvenue sur SubExplore ! 🤿</h2>

<p>Bonjour,</p>

<p>Merci de vous être inscrit sur <strong>SubExplore</strong>, la communauté des passionnés de sports sous-marins !</p>

<p>Pour activer votre compte et commencer à explorer les meilleurs spots de plongée, veuillez confirmer votre adresse email en cliquant sur le lien ci-dessous :</p>

<p>
  <a href="{{ .ConfirmationURL }}"
     style="background-color: #0066CC; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;">
    Confirmer mon email
  </a>
</p>

<p>Ou copiez ce lien dans votre navigateur :</p>
<p><a href="{{ .ConfirmationURL }}">{{ .ConfirmationURL }}</a></p>

<p><strong>Ce lien expire dans 24 heures.</strong></p>

<hr>

<p>Si vous n'avez pas créé de compte sur SubExplore, vous pouvez ignorer cet email.</p>

<p>À bientôt sous l'eau ! 🌊<br>
L'équipe SubExplore</p>
```

**Instructions :**
1. Cliquez sur **Confirm signup**
2. Remplacez le contenu du **Subject** et du **Body**
3. Cliquez sur **Save**

---

### Template 2 : Réinitialisation de mot de passe (Reset Password)

**Objectif :** Email envoyé lorsqu'un utilisateur demande à réinitialiser son mot de passe

**Template personnalisé SubExplore :**

**Subject :**
```
SubExplore - Réinitialisation de votre mot de passe
```

**Body (HTML) :**
```html
<h2>Réinitialisation de mot de passe 🔒</h2>

<p>Bonjour,</p>

<p>Vous avez demandé à réinitialiser votre mot de passe sur <strong>SubExplore</strong>.</p>

<p>Cliquez sur le lien ci-dessous pour définir un nouveau mot de passe :</p>

<p>
  <a href="{{ .ConfirmationURL }}"
     style="background-color: #0066CC; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;">
    Réinitialiser mon mot de passe
  </a>
</p>

<p>Ou copiez ce lien dans votre navigateur :</p>
<p><a href="{{ .ConfirmationURL }}">{{ .ConfirmationURL }}</a></p>

<p><strong>Ce lien expire dans 1 heure.</strong></p>

<hr>

<p><strong>⚠️ Sécurité :</strong> Si vous n'avez pas demandé cette réinitialisation, ignorez cet email. Votre mot de passe actuel reste inchangé.</p>

<p>Besoin d'aide ? Contactez-nous à support@subexplore.app</p>

<p>L'équipe SubExplore</p>
```

**Instructions :**
1. Cliquez sur **Reset Password**
2. Remplacez le contenu du **Subject** et du **Body**
3. Cliquez sur **Save**

---

### Template 3 : Magic Link (Optionnel)

**Objectif :** Connexion sans mot de passe (email avec lien de connexion directe)

**Note :** Ce template est optionnel pour SubExplore. Vous pouvez le désactiver ou le personnaliser plus tard.

**Template personnalisé SubExplore (si activé) :**

**Subject :**
```
SubExplore - Lien de connexion magique
```

**Body (HTML) :**
```html
<h2>Connexion à SubExplore 🔑</h2>

<p>Bonjour,</p>

<p>Cliquez sur le lien ci-dessous pour vous connecter à votre compte SubExplore :</p>

<p>
  <a href="{{ .ConfirmationURL }}"
     style="background-color: #0066CC; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;">
    Se connecter
  </a>
</p>

<p>Ou copiez ce lien dans votre navigateur :</p>
<p><a href="{{ .ConfirmationURL }}">{{ .ConfirmationURL }}</a></p>

<p><strong>Ce lien expire dans 1 heure.</strong></p>

<hr>

<p>Si vous n'avez pas demandé ce lien, ignorez cet email.</p>

<p>L'équipe SubExplore</p>
```

---

## 🌐 PARTIE 4 : Configuration OAuth (Optionnel)

### Option A : Google OAuth (Recommandé pour Android)

**Prérequis :**
- Compte Google Cloud Platform
- Projet configuré dans Google Cloud Console

**Instructions simplifiées :**
1. Dans **Authentication** → **Providers**, cliquez sur **Google**
2. Activez **Enable Google provider**
3. Entrez votre **Client ID** et **Client Secret** (obtenus depuis Google Cloud Console)
4. Cliquez sur **Save**

**Documentation complète :** https://supabase.com/docs/guides/auth/social-login/auth-google

---

### Option B : Apple OAuth (Recommandé pour iOS)

**Prérequis :**
- Compte Apple Developer
- App ID configuré dans Apple Developer Portal

**Instructions simplifiées :**
1. Dans **Authentication** → **Providers**, cliquez sur **Apple**
2. Activez **Enable Apple provider**
3. Entrez vos **Services ID**, **Team ID**, et **Key ID**
4. Uploadez votre **Private Key** (.p8)
5. Cliquez sur **Save**

**Documentation complète :** https://supabase.com/docs/guides/auth/social-login/auth-apple

**Note :** Apple OAuth est obligatoire pour les applications iOS si vous proposez d'autres méthodes de connexion sociale.

---

## 🧪 PARTIE 5 : Tests d'Authentification

### Test 1 : Test d'inscription (Email/Password)

**Méthode 1 : Via Supabase Dashboard**

1. Allez dans **Authentication** → **Users**
2. Cliquez sur **Add user** → **Create new user**
3. Remplissez :
   - Email: `test@subexplore.app`
   - Password: `TestPlongee2024`
   - Auto Confirm User: ✅ (pour les tests uniquement)
4. Cliquez sur **Create user**

**Résultat attendu :**
```
✅ Utilisateur créé avec succès
✅ Visible dans la liste Users
✅ Status: Confirmed (si auto-confirm activé)
```

---

**Méthode 2 : Via code C# (.NET MAUI)**

```csharp
// Test d'inscription
var result = await supabaseClient.Auth.SignUp(
    email: "test@subexplore.app",
    password: "TestPlongee2024"
);

if (result.User != null)
{
    Console.WriteLine($"✅ Inscription réussie: {result.User.Email}");
    Console.WriteLine($"User ID: {result.User.Id}");
}
```

---

### Test 2 : Test de connexion

**Via Supabase Dashboard :**

Utilisez l'utilisateur créé précédemment pour tester la connexion via votre application.

**Via code C# (.NET MAUI) :**

```csharp
// Test de connexion
var session = await supabaseClient.Auth.SignIn(
    email: "test@subexplore.app",
    password: "TestPlongee2024"
);

if (session?.User != null)
{
    Console.WriteLine($"✅ Connexion réussie: {session.User.Email}");
    Console.WriteLine($"Access Token: {session.AccessToken.Substring(0, 20)}...");
}
```

---

### Test 3 : Test de confirmation d'email

**Scénario :** Inscription sans auto-confirm

1. Créez un utilisateur **sans** cocher "Auto Confirm User"
2. Vérifiez que l'utilisateur apparaît avec **Status: Unconfirmed**
3. Un email de confirmation devrait être envoyé (vérifiez dans **Authentication** → **Logs**)
4. Simulez la confirmation en cliquant sur le lien dans l'email

**Résultat attendu :**
```
✅ Email de confirmation envoyé
✅ Utilisateur passe de Unconfirmed à Confirmed après clic sur le lien
```

---

### Test 4 : Test de réinitialisation de mot de passe

**Via code C# (.NET MAUI) :**

```csharp
// Demande de réinitialisation de mot de passe
await supabaseClient.Auth.ResetPasswordForEmail(
    email: "test@subexplore.app"
);

Console.WriteLine("✅ Email de réinitialisation envoyé");
```

**Vérification :**
1. Allez dans **Authentication** → **Logs**
2. Vérifiez qu'un événement **password_recovery** a été enregistré
3. Un email de réinitialisation devrait avoir été envoyé

---

### Test 5 : Test de déconnexion

**Via code C# (.NET MAUI) :**

```csharp
// Déconnexion
await supabaseClient.Auth.SignOut();

Console.WriteLine("✅ Déconnexion réussie");

// Vérifier que la session est nulle
var currentSession = supabaseClient.Auth.CurrentSession;
Console.WriteLine($"Session actuelle: {(currentSession == null ? "null ✅" : "existe ❌")}");
```

---

## 📊 PARTIE 6 : Vérification de la configuration

### Checklist de validation

- [ ] **Email provider activé** avec confirmation d'email obligatoire
- [ ] **Paramètres de sécurité** configurés (longueur mot de passe, règles)
- [ ] **Redirect URLs** configurées (localhost + deep links)
- [ ] **Site URL** définie
- [ ] **Templates d'emails** personnalisés (Confirm signup, Reset password)
- [ ] **Test d'inscription** réussi (utilisateur créé)
- [ ] **Test de connexion** réussi (session active)
- [ ] **Test de confirmation email** réussi (email envoyé et reçu)
- [ ] **Test de réinitialisation** réussi (email reset envoyé)
- [ ] **Logs d'authentification** visibles dans Supabase Dashboard

---

## 🔍 Vérification via SQL

**Requête pour vérifier les utilisateurs créés :**

```sql
-- Liste des utilisateurs Auth
SELECT
    id,
    email,
    created_at,
    confirmed_at,
    CASE WHEN confirmed_at IS NOT NULL THEN '✅ Confirmed' ELSE '⏳ Pending' END as status
FROM auth.users
ORDER BY created_at DESC
LIMIT 10;
```

**Résultat attendu :**
```
id                                   | email                  | created_at | confirmed_at | status
-------------------------------------+------------------------+------------+--------------+-----------
550e8400-e29b-41d4-a716-446655440000 | test@subexplore.app    | 2025-12-10 | 2025-12-10   | ✅ Confirmed
```

---

## ⚙️ PARTIE 7 : Configuration avancée (Optionnel)

### Rate Limiting (Protection anti-spam)

**Accès :**
1. **Authentication** → **Settings**
2. Section **Rate Limiting**

**Configuration recommandée :**
```yaml
Max requests per hour (Sign Up): 10
Max requests per hour (Sign In): 30
Max requests per hour (Password Reset): 5
```

---

### Session Management

**Accès :**
1. **Authentication** → **Settings**
2. Section **Session Management**

**Configuration recommandée :**
```yaml
JWT expiry: 3600 seconds (1 heure)
Refresh token expiry: 604800 seconds (7 jours)
```

---

### Email Rate Limiting

**Accès :**
1. **Authentication** → **Settings**
2. Section **Email Rate Limiting**

**Configuration recommandée :**
```yaml
Max emails per hour: 4
```

**Explication :** Limite le nombre d'emails (confirmation, reset) qu'un utilisateur peut recevoir pour éviter le spam.

---

## 🔒 Considérations de Sécurité

### Bonnes pratiques

✅ **Toujours activer la confirmation d'email** en production
✅ **Utiliser HTTPS** pour les redirect URLs en production
✅ **Configurer des mots de passe forts** (8+ caractères, majuscules, minuscules, chiffres)
✅ **Limiter les tentatives de connexion** (rate limiting)
✅ **Monitorer les logs d'authentification** régulièrement
✅ **Ne jamais stocker les mots de passe en clair** (Supabase gère cela automatiquement)

### Protection contre les attaques

🛡️ **Brute Force :** Rate limiting activé (30 tentatives/heure max)
🛡️ **Email Enumeration :** Confirmation d'email obligatoire
🛡️ **CSRF :** Tokens JWT avec expiration courte
🛡️ **Session Hijacking :** Refresh tokens avec rotation automatique

---

## 📚 Ressources Supplémentaires

### Documentation Supabase Auth
- **Official Docs:** https://supabase.com/docs/guides/auth
- **Email Auth:** https://supabase.com/docs/guides/auth/auth-email
- **Auth Helpers (.NET):** https://supabase.com/docs/reference/csharp/auth-signup
- **Social Login:** https://supabase.com/docs/guides/auth/social-login

### Intégration .NET MAUI
- **supabase-csharp:** https://github.com/supabase-community/supabase-csharp
- **Gotrue-csharp:** https://github.com/supabase-community/gotrue-csharp

---

## ✅ Critères de Succès TASK-012

**TASK-012 est complétée si :**

✅ Email/Password provider activé et configuré
✅ Confirmation d'email obligatoire activée
✅ Paramètres de sécurité du mot de passe définis (8+ caractères, règles)
✅ Redirect URLs configurées (localhost + deep links)
✅ Templates d'emails personnalisés (Confirm signup, Reset password)
✅ Test d'inscription réussi (utilisateur créé)
✅ Test de connexion réussi (session active)
✅ Logs d'authentification visibles dans Supabase
✅ Documentation complète

---

## 🚀 Prochaines étapes

Une fois TASK-012 validée :

➡️ **TASK-013** : Configuration EditorConfig
➡️ **TASK-014** : Configuration Analyzers (StyleCop, SonarAnalyzer)
➡️ **TASK-015** : Configuration CI/CD basique

---

**Dernière mise à jour :** 2025-12-10
**Statut :** ✅ Prêt pour implémentation
