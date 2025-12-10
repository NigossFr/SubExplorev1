# GUIDE RAPIDE - Tests Auth SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Durée estimée:** ~10 minutes

---

## 🚀 DÉMARRAGE RAPIDE

### Prérequis

✅ TASK-012 configuration effectuée (Email provider activé, templates configurés)
✅ Accès au Dashboard Supabase
✅ Projet SubExplorev1 sélectionné

---

## 🧪 TESTS ESSENTIELS

### Test 1 : Vérifier Email Provider activé (1 min)

**Étape :**
1. Supabase Dashboard → **Authentication** → **Providers**
2. Vérifier que **Email** est ✅ activé

**Résultat attendu :**
```
✅ Email provider: Enabled
✅ Confirm email: Enabled
```

---

### Test 2 : Créer un utilisateur test (2 min)

**Étape :**
1. **Authentication** → **Users**
2. Cliquez sur **Add user** → **Create new user**
3. Remplissez :
   - Email: `test@subexplore.app`
   - Password: `TestPlongee2024`
   - Auto Confirm User: ✅ **Coché** (pour test uniquement)
4. Cliquez sur **Create user**

**Résultat attendu :**
```
✅ User created successfully
✅ Email: test@subexplore.app
✅ Status: Confirmed
```

---

### Test 3 : Vérifier dans la table users (1 min)

**Requête SQL :**
```sql
-- Vérifier utilisateur créé
SELECT
    id,
    email,
    created_at,
    confirmed_at,
    CASE
        WHEN confirmed_at IS NOT NULL THEN 'Confirmed ✅'
        ELSE 'Pending ⏳'
    END as status
FROM auth.users
WHERE email = 'test@subexplore.app';
```

**Résultat attendu :**
```
id                                   | email                | created_at | confirmed_at | status
-------------------------------------+----------------------+------------+--------------+-----------
[UUID]                               | test@subexplore.app  | 2025-12-10 | 2025-12-10   | Confirmed ✅
```

---

### Test 4 : Vérifier Redirect URLs (1 min)

**Étape :**
1. **Authentication** → **URL Configuration**
2. Section **Redirect URLs**

**URLs requises :**
```
✅ http://localhost:8081/auth/callback
✅ subexplore://auth/callback
✅ subexplore://reset-password
✅ subexplore://verify-email
```

---

### Test 5 : Vérifier Templates d'emails (2 min)

**Étape :**
1. **Authentication** → **Email Templates**
2. Vérifier que **Confirm signup** et **Reset Password** sont personnalisés

**Vérification rapide :**
```
✅ Confirm signup: Subject contient "SubExplore"
✅ Reset Password: Subject contient "SubExplore"
```

---

### Test 6 : Test de connexion via code (3 min)

**Code C# à tester :**

```csharp
using Supabase;

// Test de connexion
var session = await supabaseClient.Auth.SignIn(
    email: "test@subexplore.app",
    password: "TestPlongee2024"
);

if (session?.User != null)
{
    Console.WriteLine($"✅ Connexion réussie");
    Console.WriteLine($"Email: {session.User.Email}");
    Console.WriteLine($"User ID: {session.User.Id}");
    Console.WriteLine($"Token: {session.AccessToken.Substring(0, 20)}...");
}
else
{
    Console.WriteLine("❌ Connexion échouée");
}
```

**Résultat attendu :**
```
✅ Connexion réussie
Email: test@subexplore.app
User ID: [UUID]
Token: eyJhbGciOiJIUzI1NiIsI...
```

---

## 📊 CHECKLIST DE VALIDATION

Cochez chaque test réussi :

- [ ] **Test 1** : Email provider activé ✅
- [ ] **Test 2** : Utilisateur test créé ✅
- [ ] **Test 3** : Utilisateur visible dans auth.users ✅
- [ ] **Test 4** : Redirect URLs configurées ✅
- [ ] **Test 5** : Templates d'emails personnalisés ✅
- [ ] **Test 6** : Connexion via code réussie ✅

---

## ✅ CRITÈRES DE SUCCÈS

**TASK-012 est validée si :**

✅ Les 6 tests passent avec succès
✅ Aucune erreur dans les logs d'authentification
✅ Utilisateur test peut se connecter
✅ Templates d'emails personnalisés affichent "SubExplore"

---

## 🔍 EN CAS DE PROBLÈME

### Problème : Email provider désactivé

**Solution :**
1. **Authentication** → **Providers** → **Email**
2. Activez **Enable Email provider**
3. Cliquez sur **Save**

---

### Problème : Utilisateur non créé

**Solution :**
1. Vérifiez que le mot de passe respecte les règles (8+ caractères, majuscules, minuscules, chiffres)
2. Vérifiez dans **Authentication** → **Logs** pour voir l'erreur

---

### Problème : Connexion échoue

**Solutions possibles :**
1. Vérifiez que l'utilisateur est **Confirmed** (status dans auth.users)
2. Vérifiez que le mot de passe est correct
3. Vérifiez les clés Supabase dans `.env` ou User Secrets

---

### Problème : Templates d'emails non sauvegardés

**Solution :**
1. Vérifiez que vous avez cliqué sur **Save** après chaque modification
2. Actualisez la page et revérifiez

---

## 📚 DOCUMENTATION COMPLÈTE

Pour une configuration détaillée et des tests avancés, consultez :

📖 **AUTH_CONFIGURATION_GUIDE.md** - Guide complet de configuration Auth
🔐 **Supabase Auth Docs** - https://supabase.com/docs/guides/auth

---

## 🎯 PROCHAINES ÉTAPES

Une fois TASK-012 validée :

➡️ **TASK-013** : Configuration EditorConfig
➡️ **TASK-014** : Configuration Analyzers (StyleCop, SonarAnalyzer)
➡️ **TASK-015** : Configuration CI/CD basique

---

**Dernière mise à jour :** 2025-12-10
**Durée totale estimée :** 10 minutes
**Statut :** ✅ Prêt pour les tests
