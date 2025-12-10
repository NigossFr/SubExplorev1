-- =====================================================
-- STORAGE VERIFICATION TESTS - SubExplore
-- =====================================================
-- Version: 1.0
-- Date: 2025-12-10
-- Description: Tests de vérification de la configuration Storage
-- =====================================================

-- =====================================================
-- TEST 1: Vérification de l'existence des buckets
-- =====================================================

DO $$
DECLARE
    avatars_exists BOOLEAN;
    spot_photos_exists BOOLEAN;
    cert_docs_exists BOOLEAN;
    avatars_public BOOLEAN;
    spot_photos_public BOOLEAN;
    cert_docs_public BOOLEAN;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'TEST 1: Vérification des buckets';
    RAISE NOTICE '=========================================';

    -- Vérifier l'existence des buckets
    SELECT EXISTS(SELECT 1 FROM storage.buckets WHERE name = 'avatars') INTO avatars_exists;
    SELECT EXISTS(SELECT 1 FROM storage.buckets WHERE name = 'spot-photos') INTO spot_photos_exists;
    SELECT EXISTS(SELECT 1 FROM storage.buckets WHERE name = 'certification-docs') INTO cert_docs_exists;

    -- Vérifier la visibilité publique/privée
    SELECT public FROM storage.buckets WHERE name = 'avatars' INTO avatars_public;
    SELECT public FROM storage.buckets WHERE name = 'spot-photos' INTO spot_photos_public;
    SELECT public FROM storage.buckets WHERE name = 'certification-docs' INTO cert_docs_public;

    -- Résultats
    IF avatars_exists AND avatars_public THEN
        RAISE NOTICE '✅ Bucket "avatars": existe et PUBLIC';
    ELSE
        RAISE WARNING '⚠️  Bucket "avatars": problème détecté';
    END IF;

    IF spot_photos_exists AND spot_photos_public THEN
        RAISE NOTICE '✅ Bucket "spot-photos": existe et PUBLIC';
    ELSE
        RAISE WARNING '⚠️  Bucket "spot-photos": problème détecté';
    END IF;

    IF cert_docs_exists AND NOT cert_docs_public THEN
        RAISE NOTICE '✅ Bucket "certification-docs": existe et PRIVATE';
    ELSE
        RAISE WARNING '⚠️  Bucket "certification-docs": problème détecté';
    END IF;

    RAISE NOTICE '';
END $$;

-- Liste des buckets avec détails
SELECT
    name as "Bucket Name",
    CASE WHEN public THEN 'Public ✅' ELSE 'Private 🔒' END as "Visibility",
    created_at as "Created At"
FROM storage.buckets
WHERE name IN ('avatars', 'spot-photos', 'certification-docs')
ORDER BY name;

-- =====================================================
-- TEST 2: Comptage des policies de storage
-- =====================================================

DO $$
DECLARE
    policy_count INTEGER;
    expected_count INTEGER := 12;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'TEST 2: Vérification des policies';
    RAISE NOTICE '=========================================';

    -- Compter les policies sur storage.objects
    SELECT COUNT(*) INTO policy_count
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects';

    RAISE NOTICE 'Policies créées: % / % attendues', policy_count, expected_count;

    IF policy_count >= expected_count THEN
        RAISE NOTICE '✅ SUCCÈS: Toutes les policies sont créées';
    ELSE
        RAISE WARNING '⚠️  ATTENTION: Certaines policies manquent';
    END IF;

    RAISE NOTICE '';
END $$;

-- Liste des policies par bucket
SELECT
    'storage' as "Schema",
    COUNT(*) as "Policy Count"
FROM pg_policies
WHERE schemaname = 'storage' AND tablename = 'objects'
GROUP BY schemaname;

-- =====================================================
-- TEST 3: Détails des policies par bucket
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'TEST 3: Détails des policies par bucket';
    RAISE NOTICE '=========================================';
END $$;

-- Policies du bucket "avatars"
SELECT
    'avatars' as "Bucket",
    policyname as "Policy Name",
    cmd as "Operation"
FROM pg_policies
WHERE schemaname = 'storage'
AND tablename = 'objects'
AND policyname LIKE '%avatar%'
ORDER BY policyname;

-- Policies du bucket "spot-photos"
SELECT
    'spot-photos' as "Bucket",
    policyname as "Policy Name",
    cmd as "Operation"
FROM pg_policies
WHERE schemaname = 'storage'
AND tablename = 'objects'
AND policyname LIKE '%spot%'
ORDER BY policyname;

-- Policies du bucket "certification-docs"
SELECT
    'certification-docs' as "Bucket",
    policyname as "Policy Name",
    cmd as "Operation"
FROM pg_policies
WHERE schemaname = 'storage'
AND tablename = 'objects'
AND policyname LIKE '%certification%'
ORDER BY policyname;

-- =====================================================
-- TEST 4: Vérification des policies critiques
-- =====================================================

DO $$
DECLARE
    avatars_policies INTEGER;
    spot_photos_policies INTEGER;
    cert_docs_policies INTEGER;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'TEST 4: Vérification policies critiques';
    RAISE NOTICE '=========================================';

    -- Compter les policies par bucket
    SELECT COUNT(*) INTO avatars_policies
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects'
    AND policyname LIKE '%avatar%';

    SELECT COUNT(*) INTO spot_photos_policies
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects'
    AND policyname LIKE '%spot%';

    SELECT COUNT(*) INTO cert_docs_policies
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects'
    AND policyname LIKE '%certification%';

    -- Vérifier avatars
    IF avatars_policies >= 4 THEN
        RAISE NOTICE '✅ Bucket "avatars": % policies (OK)', avatars_policies;
    ELSE
        RAISE WARNING '⚠️  Bucket "avatars": % policies (Attendu: 4)', avatars_policies;
    END IF;

    -- Vérifier spot-photos
    IF spot_photos_policies >= 4 THEN
        RAISE NOTICE '✅ Bucket "spot-photos": % policies (OK)', spot_photos_policies;
    ELSE
        RAISE WARNING '⚠️  Bucket "spot-photos": % policies (Attendu: 4)', spot_photos_policies;
    END IF;

    -- Vérifier certification-docs
    IF cert_docs_policies >= 4 THEN
        RAISE NOTICE '✅ Bucket "certification-docs": % policies (OK)', cert_docs_policies;
    ELSE
        RAISE WARNING '⚠️  Bucket "certification-docs": % policies (Attendu: 4)', cert_docs_policies;
    END IF;

    RAISE NOTICE '';
END $$;

-- =====================================================
-- TEST 5: Vérification de la fonction helper
-- =====================================================

DO $$
DECLARE
    function_exists BOOLEAN;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'TEST 5: Vérification fonction helper';
    RAISE NOTICE '=========================================';

    -- Vérifier l'existence de la fonction is_spot_owner
    SELECT EXISTS(
        SELECT 1
        FROM pg_proc
        WHERE proname = 'is_spot_owner'
        AND pg_catalog.pg_function_is_visible(oid)
    ) INTO function_exists;

    IF function_exists THEN
        RAISE NOTICE '✅ Fonction "is_spot_owner" existe';
    ELSE
        RAISE WARNING '⚠️  Fonction "is_spot_owner" manquante';
    END IF;

    RAISE NOTICE '';
END $$;

-- Détails de la fonction
SELECT
    proname as "Function Name",
    pg_catalog.pg_get_function_arguments(oid) as "Arguments",
    pg_catalog.pg_get_function_result(oid) as "Return Type"
FROM pg_proc
WHERE proname = 'is_spot_owner'
AND pg_catalog.pg_function_is_visible(oid);

-- =====================================================
-- TEST 6: Résumé final
-- =====================================================

DO $$
DECLARE
    bucket_count INTEGER;
    policy_count INTEGER;
    function_exists BOOLEAN;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'RÉSUMÉ FINAL - TESTS STORAGE';
    RAISE NOTICE '=========================================';

    -- Compter les buckets
    SELECT COUNT(*) INTO bucket_count
    FROM storage.buckets
    WHERE name IN ('avatars', 'spot-photos', 'certification-docs');

    -- Compter les policies sur storage.objects
    SELECT COUNT(*) INTO policy_count
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects';

    -- Vérifier la fonction
    SELECT EXISTS(
        SELECT 1 FROM pg_proc
        WHERE proname = 'is_spot_owner'
    ) INTO function_exists;

    RAISE NOTICE 'Buckets créés: % / 3 attendus', bucket_count;
    RAISE NOTICE 'Policies créées: % / 12 attendues', policy_count;
    RAISE NOTICE 'Fonction helper: %', CASE WHEN function_exists THEN 'OK' ELSE 'MANQUANTE' END;
    RAISE NOTICE '';

    IF bucket_count = 3 AND policy_count >= 12 AND function_exists THEN
        RAISE NOTICE '✅ STATUT GLOBAL: STORAGE CORRECTEMENT CONFIGURÉ';
    ELSE
        RAISE WARNING '⚠️  STATUT GLOBAL: VÉRIFICATION NÉCESSAIRE';
    END IF;

    RAISE NOTICE '';
    RAISE NOTICE '=========================================';
    RAISE NOTICE 'Tests de vérification terminés';
    RAISE NOTICE '=========================================';
    RAISE NOTICE '';
    RAISE NOTICE '📝 RECOMMANDATIONS POUR TESTS MANUELS:';
    RAISE NOTICE '';
    RAISE NOTICE '1. Tester upload d''un avatar via interface Supabase';
    RAISE NOTICE '   → Créer un dossier {user_id} et uploader une image';
    RAISE NOTICE '';
    RAISE NOTICE '2. Vérifier URL publique des avatars';
    RAISE NOTICE '   → URL: https://[project].supabase.co/storage/v1/object/public/avatars/...';
    RAISE NOTICE '';
    RAISE NOTICE '3. Tester isolation certification-docs';
    RAISE NOTICE '   → Vérifier qu''un utilisateur ne voit pas les docs d''un autre';
    RAISE NOTICE '';
    RAISE NOTICE '4. Tester upload photo de spot';
    RAISE NOTICE '   → Structure: spot-photos/spots/{spot_id}/photo.jpg';
    RAISE NOTICE '';
    RAISE NOTICE '5. Vérifier taille max des fichiers';
    RAISE NOTICE '   → avatars: 5 MB, spot-photos: 10 MB, cert-docs: 5 MB';
    RAISE NOTICE '';
    RAISE NOTICE 'Pour les tests détaillés, consultez:';
    RAISE NOTICE 'Documentation/Storage/STORAGE_CONFIGURATION_GUIDE.md';
    RAISE NOTICE '';
END $$;
