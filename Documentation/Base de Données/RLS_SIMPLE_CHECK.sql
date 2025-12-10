-- =====================================================
-- RLS SIMPLE CHECK - SubExplore
-- =====================================================
-- Version: 1.0
-- Date: 2025-12-10
-- Description: Vérification rapide et simple du RLS
-- =====================================================

-- TEST 1: Combien de tables ont RLS activé ?
SELECT COUNT(*) as tables_with_rls
FROM pg_tables
WHERE schemaname = 'public'
AND rowsecurity = true;
-- Résultat attendu: 13

-- TEST 2: Combien de policies existent ?
SELECT COUNT(*) as total_policies
FROM pg_policies
WHERE schemaname = 'public';
-- Résultat attendu: 19

-- TEST 3: Policies par table (résumé)
SELECT
    tablename as table_name,
    COUNT(*) as policy_count
FROM pg_policies
WHERE schemaname = 'public'
GROUP BY tablename
ORDER BY tablename;

-- TEST 4: Vérification des tables critiques
SELECT
    tablename as table_name,
    rowsecurity as rls_enabled
FROM pg_tables
WHERE schemaname = 'public'
AND tablename IN ('users', 'spots', 'messages', 'bookings', 'favorites')
ORDER BY tablename;

-- TEST 5: Résumé final
DO $$
DECLARE
    total_tables INTEGER;
    total_policies INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_tables
    FROM pg_tables
    WHERE schemaname = 'public' AND rowsecurity = true;

    SELECT COUNT(*) INTO total_policies
    FROM pg_policies
    WHERE schemaname = 'public';

    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'RÉSUMÉ RLS - SubExplore';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Tables avec RLS: % (attendu: 13)', total_tables;
    RAISE NOTICE 'Policies créées: % (attendu: 19)', total_policies;
    RAISE NOTICE '';

    IF total_tables >= 13 AND total_policies >= 19 THEN
        RAISE NOTICE '✅ STATUT: RLS CORRECTEMENT CONFIGURÉ';
    ELSE
        RAISE NOTICE '⚠️  STATUT: VÉRIFICATION NÉCESSAIRE';
    END IF;
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;
