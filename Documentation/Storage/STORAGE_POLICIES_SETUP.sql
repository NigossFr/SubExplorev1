-- =====================================================
-- STORAGE POLICIES SETUP - SubExplore
-- =====================================================
-- Version: 1.0
-- Date: 2025-12-10
-- Description: Configuration des policies de storage pour les buckets Supabase
-- =====================================================
-- IMPORTANT: Les buckets doivent être créés via l'interface Supabase AVANT d'exécuter ce script
-- Buckets requis: avatars (public), spot-photos (public), certification-docs (private)
-- =====================================================

-- =====================================================
-- PARTIE 1: FONCTION HELPER - Vérification de propriété des spots
-- =====================================================

-- Fonction pour vérifier si l'utilisateur est le créateur d'un spot
CREATE OR REPLACE FUNCTION public.is_spot_owner(spot_id_param UUID)
RETURNS BOOLEAN AS $$
BEGIN
  RETURN EXISTS (
    SELECT 1 FROM public.spots
    WHERE id = spot_id_param
    AND creator_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
  );
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- =====================================================
-- PARTIE 2: POLICIES POUR BUCKET "avatars" (PUBLIC)
-- =====================================================

-- Policy 1: Upload d'avatar (utilisateur authentifié uniquement)
-- Structure: avatars/{user_id}/avatar.jpg
CREATE POLICY "Users can upload own avatar"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Policy 2: Lecture publique des avatars
-- Tous les utilisateurs (même anonymes) peuvent voir les avatars
CREATE POLICY "Avatars are publicly accessible"
ON storage.objects FOR SELECT
TO public
USING (bucket_id = 'avatars');

-- Policy 3: Mise à jour d'avatar (propriétaire uniquement)
CREATE POLICY "Users can update own avatar"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Policy 4: Suppression d'avatar (propriétaire uniquement)
CREATE POLICY "Users can delete own avatar"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- =====================================================
-- PARTIE 3: POLICIES POUR BUCKET "spot-photos" (PUBLIC)
-- =====================================================

-- Policy 5: Upload de photos de spots (utilisateur authentifié)
-- Structure: spot-photos/spots/{spot_id}/photo.jpg
CREATE POLICY "Users can upload spot photos"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'spot-photos'
  AND (storage.foldername(name))[1] = 'spots'
);

-- Policy 6: Lecture publique des photos de spots
-- Tous les utilisateurs peuvent voir les photos de spots
CREATE POLICY "Spot photos are publicly accessible"
ON storage.objects FOR SELECT
TO public
USING (bucket_id = 'spot-photos');

-- Policy 7: Mise à jour des photos de spots (créateur du spot uniquement)
-- Utilise la fonction is_spot_owner pour vérifier la propriété
CREATE POLICY "Users can update own spot photos"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'spot-photos'
  AND public.is_spot_owner(
    (regexp_match(name, 'spots/([0-9a-f-]+)/'))[1]::UUID
  )
);

-- Policy 8: Suppression des photos de spots (créateur du spot uniquement)
CREATE POLICY "Users can delete own spot photos"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'spot-photos'
  AND public.is_spot_owner(
    (regexp_match(name, 'spots/([0-9a-f-]+)/'))[1]::UUID
  )
);

-- =====================================================
-- PARTIE 4: POLICIES POUR BUCKET "certification-docs" (PRIVATE)
-- =====================================================

-- Policy 9: Upload de certifications (utilisateur authentifié, ses propres docs uniquement)
-- Structure: certification-docs/{user_id}/certification.pdf
CREATE POLICY "Users can upload own certifications"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Policy 10: Lecture de certifications (propriétaire uniquement, pas d'accès public)
CREATE POLICY "Users can view own certifications"
ON storage.objects FOR SELECT
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Policy 11: Mise à jour de certifications (propriétaire uniquement)
CREATE POLICY "Users can update own certifications"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Policy 12: Suppression de certifications (propriétaire uniquement)
CREATE POLICY "Users can delete own certifications"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- =====================================================
-- PARTIE 5: RÉSUMÉ FINAL
-- =====================================================

DO $$
DECLARE
    policy_count INTEGER;
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'CONFIGURATION STORAGE - SubExplore';
    RAISE NOTICE '========================================';

    -- Compter les policies créées sur storage.objects
    SELECT COUNT(*) INTO policy_count
    FROM pg_policies
    WHERE schemaname = 'storage' AND tablename = 'objects';

    RAISE NOTICE 'Policies créées: % (attendu: 12)', policy_count;
    RAISE NOTICE '';

    IF policy_count >= 12 THEN
        RAISE NOTICE '✅ STATUT: STORAGE CORRECTEMENT CONFIGURÉ';
    ELSE
        RAISE NOTICE '⚠️  STATUT: VÉRIFICATION NÉCESSAIRE';
    END IF;

    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'BUCKETS CONFIGURÉS:';
    RAISE NOTICE '========================================';
    RAISE NOTICE '1. avatars (public) - Photos de profil';
    RAISE NOTICE '2. spot-photos (public) - Photos de sites';
    RAISE NOTICE '3. certification-docs (private) - Certifications';
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'STRUCTURE DES DOSSIERS:';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'avatars/{user_id}/avatar.jpg';
    RAISE NOTICE 'spot-photos/spots/{spot_id}/photo.jpg';
    RAISE NOTICE 'certification-docs/{user_id}/cert.pdf';
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'PROCHAINES ÉTAPES:';
    RAISE NOTICE '========================================';
    RAISE NOTICE '1. Exécuter STORAGE_VERIFICATION_TESTS.sql';
    RAISE NOTICE '2. Tester upload/download via interface';
    RAISE NOTICE '3. Vérifier isolation des données';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;
