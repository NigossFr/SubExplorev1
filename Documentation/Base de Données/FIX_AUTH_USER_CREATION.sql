-- =====================================================
-- FIX AUTH USER CREATION - SubExplore
-- =====================================================
-- Version: 1.0
-- Date: 2025-12-10
-- Description: Correction de la fonction handle_new_user() pour inclure first_name et last_name
-- =====================================================
-- PROBLÈME: La fonction handle_new_user() ne renseigne pas first_name et last_name
--           qui sont NOT NULL dans la table public.users
-- SOLUTION: Mettre à jour la fonction pour fournir des valeurs par défaut
-- =====================================================

-- Supprimer l'ancienne fonction et la recréer avec les valeurs par défaut
DROP FUNCTION IF EXISTS handle_new_user() CASCADE;

CREATE OR REPLACE FUNCTION handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.users (
        auth_id,
        email,
        first_name,
        last_name,
        created_at
    )
    VALUES (
        NEW.id,
        NEW.email,
        COALESCE(NEW.raw_user_meta_data->>'first_name', 'User'),
        COALESCE(NEW.raw_user_meta_data->>'last_name', SUBSTRING(NEW.id::TEXT, 1, 8)),
        NOW()
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Recréer le trigger
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;

CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION handle_new_user();

-- Message de confirmation
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'FIX AUTH USER CREATION - SubExplore';
    RAISE NOTICE '========================================';
    RAISE NOTICE '✅ Fonction handle_new_user() mise à jour';
    RAISE NOTICE '✅ Trigger on_auth_user_created recréé';
    RAISE NOTICE '';
    RAISE NOTICE 'CHANGEMENTS:';
    RAISE NOTICE '- first_name: "User" par défaut';
    RAISE NOTICE '- last_name: 8 premiers caractères de l''UUID';
    RAISE NOTICE '';
    RAISE NOTICE 'Les utilisateurs pourront modifier ces valeurs';
    RAISE NOTICE 'dans leur profil après inscription.';
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Vous pouvez maintenant créer un utilisateur test';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;
