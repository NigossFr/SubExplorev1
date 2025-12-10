# GUIDE RAPIDE - Tests RLS SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Durée estimée:** ~15-20 minutes

---

## 🚀 DÉMARRAGE RAPIDE

### Étape 1 : Exécuter le Script de Vérification

1. Connectez-vous à **Supabase** : https://supabase.com
2. Sélectionnez votre projet : **SubExplorev1**
3. Accédez au **SQL Editor**
4. Copiez le contenu du fichier `RLS_VERIFICATION_TESTS.sql`
5. Exécutez le script (bouton **Run** ou `Ctrl+Enter`)

**Résultats attendus** :
```
✅ Tables avec RLS activé: 13 / 13 attendues
✅ Policies créées: 19 / 19 attendues
✅ STATUT GLOBAL: RLS CORRECTEMENT CONFIGURÉ
```

---

## 🧪 TESTS MANUELS ESSENTIELS

### Test 1 : Vérification RLS Activé (2 min)

**Requête SQL** :
```sql
SELECT
    tablename,
    rowsecurity as rls_enabled
FROM pg_tables
WHERE schemaname = 'public'
AND tablename IN ('users', 'spots', 'messages', 'bookings')
ORDER BY tablename;
```

**Résultat attendu** :
```
tablename  | rls_enabled
-----------+-------------
bookings   | true
messages   | true
spots      | true
users      | true
```

**✅ Critère de réussite** : Toutes les tables doivent avoir `rls_enabled = true`

---

### Test 2 : Comptage des Policies (2 min)

**Requête SQL** :
```sql
SELECT
    tablename,
    COUNT(*) as policy_count
FROM pg_policies
WHERE schemaname = 'public'
GROUP BY tablename
ORDER BY tablename;
```

**Résultats attendus** :
```
tablename       | policy_count
----------------+-------------
bookings        | 2
buddy_profiles  | 2
favorites       | 2
messages        | 2
notifications   | 2
reviews         | 3
spots           | 3
users           | 3
```

**✅ Critère de réussite** : Total ≥ 19 policies

---

### Test 3 : Test de Lecture Publique - Spots (3 min)

**Objectif** : Vérifier que les utilisateurs anonymes voient uniquement les spots approuvés

**Requête SQL (à exécuter avec le rôle `anon`)** :
```sql
-- Cette requête simule un utilisateur non connecté
SET ROLE anon;

-- Devrait voir uniquement les spots approuvés
SELECT COUNT(*) as spots_publics
FROM public.spots
WHERE validation_status = 'Approved';

-- Devrait retourner 0 (les spots en attente ne sont pas visibles)
SELECT COUNT(*) as spots_pending
FROM public.spots
WHERE validation_status = 'Pending';

-- Revenir au rôle normal
RESET ROLE;
```

**✅ Critère de réussite** :
- Les spots approuvés sont visibles
- Les spots en attente (`Pending`) ne sont PAS visibles (COUNT = 0 ou erreur de permission)

---

### Test 4 : Test d'Isolation - Favoris (3 min)

**Objectif** : Vérifier qu'un utilisateur ne voit que ses propres favoris

**Requête SQL** :
```sql
-- Vérifier que la table favorites a RLS et policies
SELECT
    policyname,
    cmd
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'favorites'
ORDER BY policyname;
```

**Résultat attendu** :
```
policyname                       | cmd
---------------------------------+--------
Users can manage own favorites   | ALL
Users can view own favorites     | SELECT
```

**✅ Critère de réussite** : 2 policies présentes sur la table `favorites`

---

### Test 5 : Test de Sécurité - Messages (3 min)

**Objectif** : Vérifier l'isolation des messages privés

**Requête SQL** :
```sql
-- Vérifier les policies sur messages
SELECT
    policyname,
    cmd,
    LEFT(qual::text, 50) as condition_preview
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'messages'
ORDER BY policyname;
```

**Résultat attendu** :
```
policyname                                      | cmd    | condition_preview
------------------------------------------------+--------+------------------
Users can send messages in their conversations  | INSERT | ...
Users can view messages in their conversations  | SELECT | (EXISTS ( SELECT 1...
```

**✅ Critère de réussite** :
- Policy SELECT : Vérifie l'appartenance à la conversation
- Policy INSERT : Vérifie que l'utilisateur est le sender ET participant

---

### Test 6 : Test de Protection - Buddy Profiles 18+ (3 min)

**Objectif** : Vérifier la restriction d'âge pour le buddy finder

**Requête SQL** :
```sql
-- Vérifier la policy de restriction d'âge
SELECT
    policyname,
    cmd,
    LEFT(qual::text, 80) as age_check_preview
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'buddy_profiles'
AND policyname LIKE '%18+%'
ORDER BY policyname;
```

**Résultat attendu** :
```
policyname                               | cmd | age_check_preview
-----------------------------------------+-----+-------------------
Users 18+ can manage own buddy profile   | ALL | ((user_id = ( SELECT users.id...
```

**✅ Critère de réussite** :
- Policy vérifie `birth_date <= CURRENT_DATE - INTERVAL '18 years'`
- Empêche les mineurs de créer un profil buddy

---

## 📊 CHECKLIST DE VALIDATION

Cochez chaque test réussi :

- [ ] **Test 1** : RLS activé sur toutes les tables critiques
- [ ] **Test 2** : 19+ policies créées et actives
- [ ] **Test 3** : Lecture publique des spots fonctionne correctement
- [ ] **Test 4** : Isolation des favoris vérifiée
- [ ] **Test 5** : Sécurité des messages privés confirmée
- [ ] **Test 6** : Restriction d'âge 18+ pour buddy profiles opérationnelle

---

## ✅ CRITÈRES DE SUCCÈS GLOBAUX

**TASK-010 est complétée si** :

✅ Toutes les tables ont RLS activé (13/13)
✅ Toutes les policies sont créées (19/19)
✅ Les 6 tests essentiels passent avec succès
✅ Aucune erreur critique détectée
✅ Documentation RLS complète créée

---

## 🔍 EN CAS DE PROBLÈME

### Problème : RLS non activé sur une table

**Solution** :
```sql
ALTER TABLE public.[table_name] ENABLE ROW LEVEL SECURITY;
```

### Problème : Policy manquante

**Solution** :
- Consultez `SUPABASE_DATABASE_SETUP.sql` (lignes 1317-1451)
- Ré-exécutez la section de la policy manquante

### Problème : Erreur de permission

**Solution** :
- Vérifiez que vous êtes connecté avec le bon rôle
- Utilisez le SQL Editor de Supabase (a les permissions nécessaires)

---

## 📚 DOCUMENTATION COMPLÈTE

Pour des tests plus approfondis et la documentation complète des policies :

📖 **RLS_POLICIES_DOCUMENTATION.md** - Documentation complète (19 policies détaillées)
📄 **RLS_VERIFICATION_TESTS.sql** - Script de vérification automatisé
🗄️ **SUPABASE_DATABASE_SETUP.sql** - Script SQL initial (lignes 1317-1451)

---

## 🎯 PROCHAINES ÉTAPES

Une fois TASK-010 validée :

➡️ **TASK-011** : Configuration Storage Supabase (buckets photos/avatars)
➡️ **TASK-012** : Configuration Auth Supabase (Email/Password)
➡️ **TASK-013** : Configuration EditorConfig

---

**Dernière mise à jour** : 2025-12-10
**Durée totale estimée** : 15-20 minutes
**Statut** : ✅ Prêt pour les tests
