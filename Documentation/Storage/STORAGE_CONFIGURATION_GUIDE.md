# GUIDE DE CONFIGURATION STORAGE SUPABASE - SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Durée estimée:** ~30 minutes

---

## 📋 Vue d'ensemble

Ce guide décrit la configuration des buckets de stockage Supabase pour SubExplore:

| Bucket | Type | Usage | Taille max | Formats acceptés |
|--------|------|-------|------------|------------------|
| **avatars** | Public | Photos de profil utilisateurs | 5 MB | jpg, jpeg, png, webp |
| **spot-photos** | Public | Photos de sites de plongée | 10 MB | jpg, jpeg, png, webp |
| **certification-docs** | Private | Certifications de plongée | 5 MB | pdf, jpg, jpeg, png |

---

## 🎯 Objectifs TASK-011

- [x] Créer bucket "avatars" (public)
- [x] Créer bucket "spot-photos" (public)
- [x] Créer bucket "certification-docs" (private)
- [x] Configurer policies de storage
- [x] Tester upload/download

---

## 🚀 PARTIE 1 : Création des Buckets

### Étape 1 : Accéder à Supabase Storage

1. Connectez-vous à **Supabase** : https://supabase.com
2. Sélectionnez votre projet : **SubExplorev1**
3. Dans le menu latéral, cliquez sur **Storage**
4. Cliquez sur **New bucket**

---

### Étape 2 : Créer le bucket "avatars"

**Configuration :**
```yaml
Bucket Name: avatars
Public bucket: ✅ Activé
File size limit: 5242880 (5 MB)
Allowed MIME types: image/jpeg, image/png, image/webp
```

**Instructions :**
1. Cliquez sur **New bucket**
2. Name: `avatars`
3. Cochez **Public bucket**
4. Cliquez sur **Create bucket**

**Résultat attendu :**
```
✅ Bucket "avatars" créé avec succès
📁 URL publique: https://[project-ref].supabase.co/storage/v1/object/public/avatars/
```

---

### Étape 3 : Créer le bucket "spot-photos"

**Configuration :**
```yaml
Bucket Name: spot-photos
Public bucket: ✅ Activé
File size limit: 10485760 (10 MB)
Allowed MIME types: image/jpeg, image/png, image/webp
```

**Instructions :**
1. Cliquez sur **New bucket**
2. Name: `spot-photos`
3. Cochez **Public bucket**
4. Cliquez sur **Create bucket**

**Résultat attendu :**
```
✅ Bucket "spot-photos" créé avec succès
📁 URL publique: https://[project-ref].supabase.co/storage/v1/object/public/spot-photos/
```

---

### Étape 4 : Créer le bucket "certification-docs"

**Configuration :**
```yaml
Bucket Name: certification-docs
Public bucket: ❌ Désactivé (PRIVATE)
File size limit: 5242880 (5 MB)
Allowed MIME types: application/pdf, image/jpeg, image/png
```

**Instructions :**
1. Cliquez sur **New bucket**
2. Name: `certification-docs`
3. **NE PAS cocher** Public bucket (doit rester privé)
4. Cliquez sur **Create bucket**

**Résultat attendu :**
```
✅ Bucket "certification-docs" créé avec succès (PRIVATE)
🔒 Accès uniquement via RLS policies
```

---

## 🔐 PARTIE 2 : Configuration des Storage Policies

Les policies de storage contrôlent qui peut uploader, lire, mettre à jour ou supprimer des fichiers.

### Policy 1 : Avatars - Upload (Authenticated Users)

**Règle :** Un utilisateur authentifié peut uploader son propre avatar

```sql
CREATE POLICY "Users can upload own avatar"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
```

**Explication :**
- Bucket ciblé: `avatars`
- Autorisation: INSERT (upload)
- Condition: Le nom du dossier doit correspondre à l'user ID
- Structure: `avatars/{user_id}/{filename}`

---

### Policy 2 : Avatars - Read (Public Access)

**Règle :** Tout le monde peut voir les avatars (buckets publics)

```sql
CREATE POLICY "Avatars are publicly accessible"
ON storage.objects FOR SELECT
TO public
USING (bucket_id = 'avatars');
```

**Explication :**
- Accès public en lecture (SELECT)
- Tous les avatars sont visibles
- Pas d'authentification requise

---

### Policy 3 : Avatars - Update/Delete (Own Files Only)

**Règle :** Un utilisateur peut modifier/supprimer uniquement son propre avatar

```sql
CREATE POLICY "Users can update own avatar"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

CREATE POLICY "Users can delete own avatar"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'avatars'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
```

---

### Policy 4 : Spot Photos - Upload (Authenticated Users)

**Règle :** Un utilisateur authentifié peut uploader des photos de spots

```sql
CREATE POLICY "Users can upload spot photos"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'spot-photos'
  AND (storage.foldername(name))[1] = 'spots'
);
```

**Explication :**
- Structure: `spot-photos/spots/{spot_id}/{filename}`
- Tout utilisateur authentifié peut uploader
- Validation métier dans l'application (vérifier ownership du spot)

---

### Policy 5 : Spot Photos - Read (Public Access)

**Règle :** Tout le monde peut voir les photos de spots (tourisme, découverte)

```sql
CREATE POLICY "Spot photos are publicly accessible"
ON storage.objects FOR SELECT
TO public
USING (bucket_id = 'spot-photos');
```

---

### Policy 6 : Spot Photos - Update/Delete (Creator Only)

**Règle :** Seul le créateur du spot peut modifier/supprimer ses photos

**Note :** Cette logique nécessite une vérification côté application (via fonction PostgreSQL)

```sql
-- Fonction helper pour vérifier la propriété d'un spot
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

CREATE POLICY "Users can update own spot photos"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'spot-photos'
  AND public.is_spot_owner(
    (regexp_match(name, 'spots/([0-9a-f-]+)/'))[1]::UUID
  )
);

CREATE POLICY "Users can delete own spot photos"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'spot-photos'
  AND public.is_spot_owner(
    (regexp_match(name, 'spots/([0-9a-f-]+)/'))[1]::UUID
  )
);
```

---

### Policy 7 : Certification Docs - Upload (Own Files Only)

**Règle :** Un utilisateur peut uploader uniquement ses propres certifications

```sql
CREATE POLICY "Users can upload own certifications"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
```

**Explication :**
- Bucket privé
- Structure: `certification-docs/{user_id}/{filename}`
- Isolation complète par utilisateur

---

### Policy 8 : Certification Docs - Read (Own Files Only)

**Règle :** Un utilisateur peut lire uniquement ses propres certifications

```sql
CREATE POLICY "Users can view own certifications"
ON storage.objects FOR SELECT
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
```

**Explication :**
- Accès restreint aux propriétaires
- Aucune visibilité publique
- Aucun accès inter-utilisateur

---

### Policy 9 : Certification Docs - Update/Delete (Own Files Only)

**Règle :** Un utilisateur peut modifier/supprimer uniquement ses propres certifications

```sql
CREATE POLICY "Users can update own certifications"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

CREATE POLICY "Users can delete own certifications"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'certification-docs'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
```

---

## 🧪 PARTIE 3 : Tests Manuels

### Test 1 : Vérifier l'existence des buckets (2 min)

**Requête SQL :**
```sql
SELECT
  name as bucket_name,
  public as is_public,
  created_at
FROM storage.buckets
WHERE name IN ('avatars', 'spot-photos', 'certification-docs')
ORDER BY name;
```

**Résultat attendu :**
```
bucket_name         | is_public | created_at
--------------------+-----------+-------------------------
avatars             | true      | 2025-12-10 ...
certification-docs  | false     | 2025-12-10 ...
spot-photos         | true      | 2025-12-10 ...
```

---

### Test 2 : Compter les policies de storage (2 min)

**Requête SQL :**
```sql
SELECT COUNT(*) as policy_count
FROM storage.policies;
```

**Résultat attendu :**
```
policy_count
------------
11
```

---

### Test 3 : Liste des policies par bucket (3 min)

**Requête SQL :**
```sql
SELECT
  bucket_id,
  name as policy_name,
  definition
FROM storage.policies
ORDER BY bucket_id, name;
```

**Résultat attendu :**
```
bucket_id           | policy_name                          | definition
--------------------+--------------------------------------+------------------
avatars             | Avatars are publicly accessible      | SELECT ...
avatars             | Users can delete own avatar          | DELETE ...
avatars             | Users can update own avatar          | UPDATE ...
avatars             | Users can upload own avatar          | INSERT ...
certification-docs  | Users can delete own certifications  | DELETE ...
certification-docs  | Users can update own certifications  | UPDATE ...
certification-docs  | Users can upload own certifications  | INSERT ...
certification-docs  | Users can view own certifications    | SELECT ...
spot-photos         | Spot photos are publicly accessible  | SELECT ...
spot-photos         | Users can delete own spot photos     | DELETE ...
spot-photos         | Users can update own spot photos     | UPDATE ...
spot-photos         | Users can upload spot photos         | INSERT ...
```

---

### Test 4 : Upload d'un fichier test (Interface Supabase)

**Instructions :**
1. Allez dans **Storage** → **avatars**
2. Cliquez sur **Upload file**
3. Créez un dossier avec un UUID fictif (ex: `test-user-id`)
4. Uploadez une image de test
5. Vérifiez que l'URL publique fonctionne

**Résultat attendu :**
```
✅ Upload réussi
✅ URL publique accessible: https://[project].supabase.co/storage/v1/object/public/avatars/test-user-id/image.jpg
```

---

### Test 5 : Test d'isolation (Certification Docs)

**Objectif :** Vérifier qu'un utilisateur ne peut pas accéder aux certifications d'un autre

**Requête SQL (simuler utilisateur A) :**
```sql
-- Se connecter en tant qu'utilisateur A (via Supabase Auth)
-- Tenter de lire les fichiers d'un autre utilisateur
SELECT *
FROM storage.objects
WHERE bucket_id = 'certification-docs'
AND (storage.foldername(name))[1] != auth.uid()::text;
```

**Résultat attendu :**
```
0 rows (aucun accès aux fichiers des autres utilisateurs)
```

---

## 📊 PARTIE 4 : Vérification Finale

### Checklist de validation

- [ ] **Bucket avatars** créé et public
- [ ] **Bucket spot-photos** créé et public
- [ ] **Bucket certification-docs** créé et privé
- [ ] **11 policies** créées et actives
- [ ] **Test upload** réussi sur avatars
- [ ] **Isolation** vérifiée sur certification-docs
- [ ] **URLs publiques** fonctionnelles pour avatars et spot-photos

---

## 📝 Structure des Dossiers Recommandée

### Avatars
```
avatars/
  ├── {user_id}/
  │   └── avatar.jpg (ou .png, .webp)
```

**Exemple :**
```
avatars/550e8400-e29b-41d4-a716-446655440000/avatar.jpg
```

### Spot Photos
```
spot-photos/
  ├── spots/
  │   ├── {spot_id}/
  │   │   ├── photo-1.jpg
  │   │   ├── photo-2.jpg
  │   │   └── photo-3.jpg
```

**Exemple :**
```
spot-photos/spots/a1b2c3d4-e5f6-7890-abcd-ef1234567890/main.jpg
spot-photos/spots/a1b2c3d4-e5f6-7890-abcd-ef1234567890/underwater.jpg
```

### Certification Docs
```
certification-docs/
  ├── {user_id}/
  │   ├── padi-open-water.pdf
  │   ├── advanced-open-water.pdf
  │   └── rescue-diver.jpg
```

**Exemple :**
```
certification-docs/550e8400-e29b-41d4-a716-446655440000/padi-open-water.pdf
certification-docs/550e8400-e29b-41d4-a716-446655440000/rescue-diver.jpg
```

---

## ⚠️ Considérations de Sécurité

### Validation Côté Client (MAUI App)

**Taille des fichiers :**
```csharp
// SubExplore.Infrastructure/Services/StorageService.cs
public class StorageService
{
    private const long MAX_AVATAR_SIZE = 5 * 1024 * 1024; // 5 MB
    private const long MAX_SPOT_PHOTO_SIZE = 10 * 1024 * 1024; // 10 MB
    private const long MAX_CERT_DOC_SIZE = 5 * 1024 * 1024; // 5 MB

    public async Task<bool> ValidateFileSize(string bucketName, long fileSize)
    {
        return bucketName switch
        {
            "avatars" => fileSize <= MAX_AVATAR_SIZE,
            "spot-photos" => fileSize <= MAX_SPOT_PHOTO_SIZE,
            "certification-docs" => fileSize <= MAX_CERT_DOC_SIZE,
            _ => false
        };
    }
}
```

**Formats de fichiers :**
```csharp
private static readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
private static readonly string[] ALLOWED_DOC_EXTENSIONS = { ".pdf", ".jpg", ".jpeg", ".png" };

public bool ValidateFileExtension(string bucketName, string fileName)
{
    var extension = Path.GetExtension(fileName).ToLowerInvariant();

    return bucketName switch
    {
        "avatars" or "spot-photos" => ALLOWED_IMAGE_EXTENSIONS.Contains(extension),
        "certification-docs" => ALLOWED_DOC_EXTENSIONS.Contains(extension),
        _ => false
    };
}
```

---

### Nommage des Fichiers

**Bonnes pratiques :**
- ✅ Utiliser des UUIDs pour éviter les collisions
- ✅ Normaliser les noms (lowercase, sans espaces)
- ✅ Ajouter un timestamp pour le versioning
- ❌ Éviter les caractères spéciaux
- ❌ Ne pas exposer d'informations sensibles dans les noms

**Exemple de génération :**
```csharp
public string GenerateFileName(string originalFileName)
{
    var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    var uniqueId = Guid.NewGuid().ToString("N")[..8];

    return $"{timestamp}-{uniqueId}{extension}";
    // Exemple: 1702234567-a1b2c3d4.jpg
}
```

---

## 🔄 Migration et Nettoyage

### Supprimer des fichiers orphelins

**Objectif :** Supprimer les fichiers dont l'utilisateur ou le spot n'existe plus

```sql
-- Supprimer les avatars d'utilisateurs supprimés
DELETE FROM storage.objects
WHERE bucket_id = 'avatars'
AND (storage.foldername(name))[1]::UUID NOT IN (
  SELECT auth_id::TEXT FROM public.users
);

-- Supprimer les photos de spots supprimés
-- (nécessite une fonction pour extraire le spot_id du chemin)
```

---

## 📚 Ressources Supplémentaires

### Documentation Supabase Storage
- **Official Docs:** https://supabase.com/docs/guides/storage
- **Storage Policies:** https://supabase.com/docs/guides/storage/security/access-control
- **Client Libraries:** https://supabase.com/docs/reference/javascript/storage

### Fichiers de Configuration
- `STORAGE_POLICIES_SETUP.sql` - Script SQL complet pour les policies
- `STORAGE_VERIFICATION_TESTS.sql` - Script de vérification automatisé

---

## ✅ Critères de Succès TASK-011

**TASK-011 est complétée si :**

✅ Les 3 buckets sont créés (avatars, spot-photos, certification-docs)
✅ Les 11 policies de storage sont créées et actives
✅ Les buckets publics (avatars, spot-photos) sont accessibles via URL
✅ Le bucket privé (certification-docs) est protégé par RLS
✅ Les tests d'upload/download fonctionnent
✅ L'isolation des données est vérifiée
✅ La documentation est complète

---

**Dernière mise à jour :** 2025-12-10
**Statut :** ✅ Prêt pour implémentation
