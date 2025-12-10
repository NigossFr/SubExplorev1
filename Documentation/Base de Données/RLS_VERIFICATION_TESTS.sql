-- =====================================================
-- RLS VERIFICATION TESTS - SubExplore
-- =====================================================
-- Version: 1.1 (corrigé)
-- Date: 2025-12-10
-- Description: Tests de vérification des Row Level Security policies
-- =====================================================

-- =====================================================
-- PARTIE 1: VÉRIFICATION DE L'ACTIVATION RLS
-- =====================================================

DO $$
DECLARE
    table_count INTEGER;
    expected_count INTEGER := 13;
BEGIN
    -- Compter le nombre de tables avec RLS activé
    SELECT COUNT(*)
    INTO table_count
    FROM pg_tables
    WHERE schemaname = 'public'
    AND rowsecurity = true;

    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 1: Vérification activation RLS';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'Tables avec RLS activé: % / % attendues', table_count, expected_count;

    IF table_count = expected_count THEN
        RAISE NOTICE '✅ SUCCÈS: Toutes les tables ont RLS activé';
    ELSE
        RAISE WARNING '⚠️  ATTENTION: Certaines tables n''ont pas RLS activé';
    END IF;
    RAISE NOTICE '';
END $$;

-- Liste des tables avec RLS activé
SELECT
    tablename as "Table",
    CASE WHEN rowsecurity THEN '✅ Activé' ELSE '❌ Désactivé' END as "RLS Status"
FROM pg_tables
WHERE schemaname = 'public'
AND tablename IN (
    'users', 'spots', 'structures', 'shops', 'community_posts',
    'buddy_profiles', 'buddy_matches', 'conversations', 'messages',
    'bookings', 'reviews', 'favorites', 'notifications'
)
ORDER BY tablename;

-- =====================================================
-- PARTIE 2: LISTE DES POLICIES CRÉÉES
-- =====================================================

DO $$
DECLARE
    policy_count INTEGER;
    expected_count INTEGER := 19;
BEGIN
    -- Compter le nombre total de policies
    SELECT COUNT(*)
    INTO policy_count
    FROM pg_policies
    WHERE schemaname = 'public';

    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 2: Vérification des policies RLS';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'Policies créées: % / % attendues', policy_count, expected_count;

    IF policy_count >= expected_count THEN
        RAISE NOTICE '✅ SUCCÈS: Toutes les policies sont créées';
    ELSE
        RAISE WARNING '⚠️  ATTENTION: Certaines policies manquent';
    END IF;
    RAISE NOTICE '';
END $$;

-- Liste détaillée des policies par table
SELECT
    tablename as "Table",
    policyname as "Policy Name",
    cmd as "Command"
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;

-- =====================================================
-- PARTIE 3: STATISTIQUES PAR TABLE
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 3: Statistiques des policies par table';
    RAISE NOTICE '===========================================';
END $$;

SELECT
    tablename as "Table",
    COUNT(*) as "Nombre de Policies",
    STRING_AGG(DISTINCT cmd::text, ', ' ORDER BY cmd::text) as "Commandes Protégées"
FROM pg_policies
WHERE schemaname = 'public'
GROUP BY tablename
ORDER BY tablename;

-- =====================================================
-- PARTIE 4: VÉRIFICATION DES POLICIES CRITIQUES
-- =====================================================

DO $$
DECLARE
    users_policies INTEGER;
    spots_policies INTEGER;
    messages_policies INTEGER;
    bookings_policies INTEGER;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 4: Vérification policies critiques';
    RAISE NOTICE '===========================================';

    -- Vérifier Users
    SELECT COUNT(*) INTO users_policies
    FROM pg_policies
    WHERE schemaname = 'public' AND tablename = 'users';

    IF users_policies >= 3 THEN
        RAISE NOTICE '✅ Table users: % policies (OK)', users_policies;
    ELSE
        RAISE WARNING '⚠️  Table users: % policies (Attendu: 3)', users_policies;
    END IF;

    -- Vérifier Spots
    SELECT COUNT(*) INTO spots_policies
    FROM pg_policies
    WHERE schemaname = 'public' AND tablename = 'spots';

    IF spots_policies >= 3 THEN
        RAISE NOTICE '✅ Table spots: % policies (OK)', spots_policies;
    ELSE
        RAISE WARNING '⚠️  Table spots: % policies (Attendu: 3)', spots_policies;
    END IF;

    -- Vérifier Messages
    SELECT COUNT(*) INTO messages_policies
    FROM pg_policies
    WHERE schemaname = 'public' AND tablename = 'messages';

    IF messages_policies >= 2 THEN
        RAISE NOTICE '✅ Table messages: % policies (OK)', messages_policies;
    ELSE
        RAISE WARNING '⚠️  Table messages: % policies (Attendu: 2)', messages_policies;
    END IF;

    -- Vérifier Bookings
    SELECT COUNT(*) INTO bookings_policies
    FROM pg_policies
    WHERE schemaname = 'public' AND tablename = 'bookings';

    IF bookings_policies >= 2 THEN
        RAISE NOTICE '✅ Table bookings: % policies (OK)', bookings_policies;
    ELSE
        RAISE WARNING '⚠️  Table bookings: % policies (Attendu: 2)', bookings_policies;
    END IF;

    RAISE NOTICE '';
END $$;

-- =====================================================
-- PARTIE 5: VÉRIFICATION DES PERMISSIONS PAR RÔLE
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 5: Vérification permissions par rôle';
    RAISE NOTICE '===========================================';
END $$;

-- Permissions pour anon (utilisateur anonyme)
SELECT
    'anon' as "Rôle",
    table_name as "Table",
    privilege_type as "Permission"
FROM information_schema.table_privileges
WHERE grantee = 'anon'
AND table_schema = 'public'
ORDER BY table_name, privilege_type
LIMIT 10;

-- Permissions pour authenticated (utilisateur connecté)
SELECT
    'authenticated' as "Rôle",
    table_name as "Table",
    privilege_type as "Permission"
FROM information_schema.table_privileges
WHERE grantee = 'authenticated'
AND table_schema = 'public'
ORDER BY table_name, privilege_type
LIMIT 10;

-- =====================================================
-- PARTIE 6: TESTS DE POLICIES SPÉCIFIQUES
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'TEST 6: Détails des policies spécifiques';
    RAISE NOTICE '===========================================';
END $$;

-- Policy Users - View active profiles
SELECT
    'users' as "Table",
    policyname as "Policy",
    cmd as "Command"
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'users'
AND policyname = 'Users can view active profiles';

-- Policy Spots - View approved spots
SELECT
    'spots' as "Table",
    policyname as "Policy",
    cmd as "Command"
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'spots'
AND policyname = 'View approved spots or own spots';

-- Policy Messages - View in conversations
SELECT
    'messages' as "Table",
    policyname as "Policy",
    cmd as "Command"
FROM pg_policies
WHERE schemaname = 'public'
AND tablename = 'messages'
AND policyname = 'Users can view messages in their conversations';

-- =====================================================
-- PARTIE 7: RÉSUMÉ FINAL
-- =====================================================

DO $$
DECLARE
    total_tables INTEGER;
    total_policies INTEGER;
    tables_with_policies INTEGER;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'RÉSUMÉ FINAL - TESTS RLS';
    RAISE NOTICE '===========================================';

    -- Tables avec RLS
    SELECT COUNT(*) INTO total_tables
    FROM pg_tables
    WHERE schemaname = 'public' AND rowsecurity = true;

    -- Total policies
    SELECT COUNT(*) INTO total_policies
    FROM pg_policies
    WHERE schemaname = 'public';

    -- Tables avec au moins une policy
    SELECT COUNT(DISTINCT tablename) INTO tables_with_policies
    FROM pg_policies
    WHERE schemaname = 'public';

    RAISE NOTICE 'Tables avec RLS activé: %', total_tables;
    RAISE NOTICE 'Total de policies créées: %', total_policies;
    RAISE NOTICE 'Tables avec policies: %', tables_with_policies;
    RAISE NOTICE '';

    IF total_tables >= 13 AND total_policies >= 19 THEN
        RAISE NOTICE '✅ STATUT GLOBAL: RLS CORRECTEMENT CONFIGURÉ';
    ELSE
        RAISE WARNING '⚠️  STATUT GLOBAL: VÉRIFICATION NÉCESSAIRE';
    END IF;

    RAISE NOTICE '';
    RAISE NOTICE '===========================================';
    RAISE NOTICE 'Tests de vérification terminés';
    RAISE NOTICE '===========================================';
    RAISE NOTICE '';
    RAISE NOTICE '📝 RECOMMANDATIONS POUR TESTS MANUELS:';
    RAISE NOTICE '';
    RAISE NOTICE '1. Tester la lecture des spots en tant qu''utilisateur anonyme';
    RAISE NOTICE '   → Devrait voir uniquement les spots approuvés';
    RAISE NOTICE '';
    RAISE NOTICE '2. Tester la création de spots en tant qu''utilisateur authentifié';
    RAISE NOTICE '   → Devrait pouvoir créer un spot pour soi-même';
    RAISE NOTICE '   → Ne devrait PAS pouvoir créer un spot pour un autre utilisateur';
    RAISE NOTICE '';
    RAISE NOTICE '3. Tester l''isolation des messages privés';
    RAISE NOTICE '   → Un utilisateur ne devrait voir que ses propres conversations';
    RAISE NOTICE '';
    RAISE NOTICE '4. Tester l''isolation des favoris';
    RAISE NOTICE '   → Un utilisateur ne devrait voir que ses propres favoris';
    RAISE NOTICE '';
    RAISE NOTICE '5. Tester les restrictions d''âge pour buddy_profiles';
    RAISE NOTICE '   → Un utilisateur de moins de 18 ans ne devrait pas pouvoir créer un profil buddy';
    RAISE NOTICE '';
    RAISE NOTICE 'Pour les tests manuels détaillés, consultez:';
    RAISE NOTICE 'Documentation/Sécurité/RLS_POLICIES_DOCUMENTATION.md';
    RAISE NOTICE '';
END $$;
