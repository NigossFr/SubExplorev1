# GUIDE COMPLET - Configuration EditorConfig SubExplore

**Version:** 1.0
**Date:** 2025-12-10
**Statut:** ✅ Configuration appliquée

---

## 📋 TABLE DES MATIÈRES

1. [Présentation](#présentation)
2. [Qu'est-ce qu'EditorConfig ?](#quest-ce-queditorconfig)
3. [Conventions de nommage C#](#conventions-de-nommage-c)
4. [Règles de formatage](#règles-de-formatage)
5. [Styles de code](#styles-de-code)
6. [Utilisation dans les IDEs](#utilisation-dans-les-ides)
7. [Vérification de la configuration](#vérification-de-la-configuration)
8. [Exemples pratiques](#exemples-pratiques)
9. [Résolution de problèmes](#résolution-de-problèmes)

---

## 📖 PRÉSENTATION

### Objectif

Garantir une cohérence de code dans tout le projet SubExplore en définissant des conventions de nommage, de formatage et de style automatiquement appliquées par les IDEs.

### Avantages

✅ **Cohérence** : Même style de code pour toute l'équipe
✅ **Automatisation** : Formatage automatique lors de l'enregistrement
✅ **Moins de conflits Git** : Style uniforme réduit les différences de formatage
✅ **Qualité** : Application des best practices C# automatiquement
✅ **Productivité** : Pas besoin de discuter du style, c'est défini

---

## ❓ QU'EST-CE QU'EDITORCONFIG ?

### Définition

EditorConfig est un **standard cross-IDE** pour définir et maintenir des styles de code cohérents. Un fichier `.editorconfig` à la racine du projet contient toutes les règles de formatage et de style.

### Compatibilité

- ✅ **Visual Studio** 2017+ (support natif)
- ✅ **Visual Studio Code** (extension EditorConfig)
- ✅ **JetBrains Rider** (support natif)
- ✅ **Visual Studio pour Mac** (support natif)

### Emplacement

```
D:\Developpement\SubExplore V3\SubExplore\.editorconfig
```

Le fichier est à la **racine de la solution**, ce qui signifie que toutes les règles s'appliquent à tous les projets de la solution.

---

## 🏷️ CONVENTIONS DE NOMMAGE C#

### Résumé des conventions

| Type | Convention | Exemple | Sévérité |
|------|-----------|---------|----------|
| **Interface** | `IPascalCase` | `ISpotService` | ⚠️ Warning |
| **Classe** | `PascalCase` | `SpotService` | ⚠️ Warning |
| **Méthode** | `PascalCase` | `GetSpotById` | ⚠️ Warning |
| **Propriété** | `PascalCase` | `UserId` | ⚠️ Warning |
| **Champ privé** | `_camelCase` | `_spotRepository` | ⚠️ Warning |
| **Paramètre** | `camelCase` | `spotId` | ⚠️ Warning |
| **Variable locale** | `camelCase` | `result` | 💡 Suggestion |
| **Constante** | `PascalCase` | `MaxSpotDepth` | ⚠️ Warning |
| **Champ static readonly** | `PascalCase` | `DefaultTimeout` | ⚠️ Warning |

### Détail des règles

#### Interfaces (begins_with_i)

```csharp
// ✅ CORRECT
public interface ISpotService { }
public interface IUserRepository { }
public interface IDivingLogService { }

// ❌ INCORRECT
public interface SpotService { }      // Manque le préfixe I
public interface iUserRepository { }  // i minuscule
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning
dotnet_naming_style.begins_with_i.required_prefix = I
dotnet_naming_style.begins_with_i.capitalization = pascal_case
```

#### Classes, structs, enums (pascal_case)

```csharp
// ✅ CORRECT
public class SpotService { }
public struct GpsCoordinates { }
public enum DiveType { }

// ❌ INCORRECT
public class spotService { }      // Commence par une minuscule
public class spot_service { }     // Utilise des underscores
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.types_should_be_pascal_case.severity = warning
dotnet_naming_style.pascal_case.capitalization = pascal_case
```

#### Méthodes et propriétés (pascal_case)

```csharp
// ✅ CORRECT
public async Task<Spot> GetSpotByIdAsync(Guid spotId) { }
public string UserName { get; set; }
public int MaxDepth { get; private set; }

// ❌ INCORRECT
public async Task<Spot> getSpotById(Guid spotId) { }  // minuscule
public string userName { get; set; }                   // minuscule
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.non_field_members_should_be_pascal_case.severity = warning
```

#### Champs privés (begins_with_underscore)

```csharp
// ✅ CORRECT
private readonly ISpotRepository _spotRepository;
private string _userName;
private int _maxDepth;

// ❌ INCORRECT
private readonly ISpotRepository spotRepository;   // Manque underscore
private string UserName;                            // PascalCase au lieu de _camelCase
private int m_maxDepth;                             // Préfixe m_ non standard
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.private_field_should_be_begins_with_underscore.severity = warning
dotnet_naming_style.begins_with_underscore.required_prefix = _
dotnet_naming_style.begins_with_underscore.capitalization = camel_case
```

#### Paramètres et variables locales (camelCase)

```csharp
// ✅ CORRECT
public Spot GetSpotById(Guid spotId)
{
    var result = _spotRepository.Find(spotId);
    var userName = result.Owner.Name;
    return result;
}

// ❌ INCORRECT
public Spot GetSpotById(Guid SpotId)        // PascalCase
{
    var Result = _spotRepository.Find(SpotId);  // PascalCase
    var UserName = Result.Owner.Name;           // PascalCase
    return Result;
}
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.parameter_should_be_camel_case.severity = warning
dotnet_naming_rule.local_variable_should_be_camel_case.severity = suggestion
dotnet_naming_style.camel_case.capitalization = camel_case
```

#### Constantes et champs static readonly (PascalCase)

```csharp
// ✅ CORRECT
public const int MaxSpotDepth = 200;
public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

// ❌ INCORRECT
public const int MAX_SPOT_DEPTH = 200;                    // Screaming case
public static readonly TimeSpan default_timeout = ...;    // snake_case
```

**Règle EditorConfig:**
```ini
dotnet_naming_rule.constant_should_be_pascal_case.severity = warning
dotnet_naming_rule.static_readonly_should_be_pascal_case.severity = warning
```

---

## 📐 RÈGLES DE FORMATAGE

### Indentation

```ini
[*.cs]
indent_size = 4
indent_style = space
tab_width = 4
```

**Exemple:**
```csharp
// ✅ CORRECT (4 espaces)
public class SpotService
{
    public async Task<Spot> GetSpotAsync(Guid id)
    {
        var spot = await _repository.FindAsync(id);
        return spot;
    }
}
```

### Fin de ligne

```ini
[*.cs]
end_of_line = crlf
insert_final_newline = true
```

- **Windows:** CRLF (`\r\n`)
- **Toujours** une ligne vide à la fin du fichier

### Espaces et accolades

```ini
# Nouvelle ligne avant accolade ouvrante (Allman style)
csharp_new_line_before_open_brace = all

# Toujours utiliser des accolades
csharp_prefer_braces = true:suggestion
```

**Exemple (Allman style):**
```csharp
// ✅ CORRECT
public class SpotService
{
    public void DoSomething()
    {
        if (condition)
        {
            // Code
        }
    }
}

// ❌ INCORRECT (K&R style)
public class SpotService {
    public void DoSomething() {
        if (condition) {
            // Code
        }
    }
}
```

### Organisation des usings

```ini
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false
```

**Exemple:**
```csharp
// ✅ CORRECT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SubExplore.Core.Interfaces;
using SubExplore.Core.Models;

// ❌ INCORRECT
using SubExplore.Core.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using SubExplore.Core.Interfaces;
```

### Espaces autour des opérateurs

```ini
csharp_space_around_binary_operators = before_and_after
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_after_cast = false
```

**Exemple:**
```csharp
// ✅ CORRECT
var result = a + b * c;
if (condition)
var casted = (int)value;

// ❌ INCORRECT
var result=a+b*c;
if(condition)
var casted = (int) value;
```

---

## 🎨 STYLES DE CODE

### Utilisation de `var`

```ini
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion
```

**Exemple:**
```csharp
// ✅ CORRECT
var count = 10;
var userName = "test";
var spot = new Spot();
var spots = _repository.GetAll();

// ❌ INCORRECT (mais toléré)
int count = 10;
string userName = "test";
Spot spot = new Spot();
```

### Expression-bodied members

```ini
csharp_style_expression_bodied_properties = true:suggestion
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
```

**Exemple:**
```csharp
// ✅ CORRECT
public string FullName => $"{FirstName} {LastName}";
public int GetCount() => _items.Count;

// ✅ AUSSI CORRECT (multi-lignes)
public async Task<Spot> GetSpotAsync(Guid id)
{
    var spot = await _repository.FindAsync(id);
    return spot;
}
```

### Pattern matching

```ini
csharp_style_pattern_matching_over_is_with_cast_check = true:suggestion
csharp_style_pattern_matching_over_as_with_null_check = true:suggestion
```

**Exemple:**
```csharp
// ✅ CORRECT
if (obj is Spot spot)
{
    Console.WriteLine(spot.Name);
}

// ❌ INCORRECT (mais fonctionne)
if (obj is Spot)
{
    var spot = (Spot)obj;
    Console.WriteLine(spot.Name);
}
```

### Null checking

```ini
csharp_style_null_propagation = true:suggestion
csharp_style_coalesce_expression = true:suggestion
```

**Exemple:**
```csharp
// ✅ CORRECT
var name = user?.Name ?? "Unknown";
var count = spots?.Count() ?? 0;

// ❌ INCORRECT (mais fonctionne)
var name = user != null ? user.Name : "Unknown";
var count = spots != null ? spots.Count() : 0;
```

---

## 🔧 UTILISATION DANS LES IDES

### Visual Studio 2022

#### Installation

✅ **Support natif** - Aucune installation nécessaire

#### Vérification

1. Ouvrez la solution SubExplore
2. Menu **Tools** → **Options**
3. **Text Editor** → **C#** → **Code Style**
4. Cliquez sur **Generate .editorconfig file from settings**
5. Comparez avec le fichier existant

#### Application automatique

- **Ctrl + K, Ctrl + D** : Formater tout le document
- **Ctrl + K, Ctrl + F** : Formater la sélection
- **Enregistrement** : Formatage automatique (si activé dans Options)

#### Activer le formatage à l'enregistrement

1. **Tools** → **Options**
2. **Text Editor** → **C#** → **Advanced**
3. Cochez **Format document on save**

### Visual Studio Code

#### Installation extension

```bash
# Rechercher dans Extensions (Ctrl+Shift+X)
EditorConfig for VS Code
```

**Extension ID:** `EditorConfig.EditorConfig`

#### Vérification

1. Ouvrez un fichier `.cs`
2. En bas à droite, vous devriez voir **EditorConfig** dans la barre d'état
3. Si configuré, le fichier est automatiquement formaté à l'enregistrement

#### Configuration recommandée (`settings.json`)

```json
{
    "editor.formatOnSave": true,
    "editor.formatOnType": false,
    "omnisharp.enableEditorConfigSupport": true,
    "omnisharp.enableRoslynAnalyzers": true
}
```

### JetBrains Rider

#### Installation

✅ **Support natif** - Aucune installation nécessaire

#### Vérification

1. **Settings** (Ctrl+Alt+S)
2. **Editor** → **Code Style**
3. Vérifiez que **Enable EditorConfig support** est coché

#### Application automatique

- **Ctrl + Alt + L** : Reformater le code
- **Enregistrement** : Formatage automatique (configurable)

#### Activer le formatage à l'enregistrement

1. **Settings** → **Tools** → **Actions on Save**
2. Cochez **Reformat code**

---

## ✅ VÉRIFICATION DE LA CONFIGURATION

### Test 1 : Vérifier que le fichier existe

```bash
# Dans le terminal, à la racine de la solution
dir .editorconfig

# Ou avec PowerShell
Test-Path .editorconfig
```

**Résultat attendu:**
```
✅ .editorconfig existe
```

### Test 2 : Créer une classe test

Créez un fichier `TestEditorConfig.cs` dans le projet API :

```csharp
namespace SubExplore.API
{
public class testeditorconfig{
private string UserName;
private readonly int maxdepth;
public void dosomething(int SpotId){
if(SpotId>0){var Result=GetData(SpotId);
}}}
}
```

### Test 3 : Appliquer le formatage

- **Visual Studio:** Ctrl + K, Ctrl + D
- **VS Code:** Shift + Alt + F
- **Rider:** Ctrl + Alt + L

**Résultat attendu après formatage:**

```csharp
namespace SubExplore.API
{
    public class TestEditorConfig
    {
        private string _userName;
        private readonly int _maxDepth;

        public void DoSomething(int spotId)
        {
            if (spotId > 0)
            {
                var result = GetData(spotId);
            }
        }
    }
}
```

### Test 4 : Vérifier les warnings

Après formatage, vous devriez voir des **warnings** dans la liste d'erreurs:

```
⚠️ IDE1006: Naming rule violation: These words must begin with upper case characters: testeditorconfig
⚠️ IDE1006: Naming rule violation: Private field 'UserName' must begin with underscore
⚠️ IDE1006: Naming rule violation: Parameter 'SpotId' must begin with lower case character
```

---

## 🧪 EXEMPLES PRATIQUES

### Exemple 1 : Service complet avec conventions

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SubExplore.Core.Interfaces;
using SubExplore.Core.Models;

namespace SubExplore.Core.Services
{
    public class SpotService : ISpotService
    {
        private readonly ISpotRepository _spotRepository;
        private readonly ILogger<SpotService> _logger;
        private const int MaxDepthMeters = 200;

        public SpotService(
            ISpotRepository spotRepository,
            ILogger<SpotService> logger)
        {
            _spotRepository = spotRepository;
            _logger = logger;
        }

        public async Task<Spot?> GetSpotByIdAsync(Guid spotId)
        {
            _logger.LogInformation("Fetching spot with ID: {SpotId}", spotId);

            if (spotId == Guid.Empty)
            {
                _logger.LogWarning("Invalid spot ID provided");
                return null;
            }

            var spot = await _spotRepository.FindByIdAsync(spotId);

            if (spot is null)
            {
                _logger.LogWarning("Spot not found: {SpotId}", spotId);
            }

            return spot;
        }

        public async Task<IEnumerable<Spot>> GetSpotsByDepthAsync(int minDepth, int maxDepth)
        {
            if (maxDepth > MaxDepthMeters)
            {
                throw new ArgumentException(
                    $"Max depth cannot exceed {MaxDepthMeters}m",
                    nameof(maxDepth));
            }

            var spots = await _spotRepository.FindByDepthRangeAsync(minDepth, maxDepth);
            return spots ?? Array.Empty<Spot>();
        }
    }
}
```

### Exemple 2 : Modèle avec propriétés

```csharp
using System;

namespace SubExplore.Core.Models
{
    public class Spot
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? MaxDepth { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public User? Owner { get; set; }

        // Expression-bodied property
        public string Coordinates => $"{Latitude}, {Longitude}";

        // Method
        public bool IsDeepDive() => MaxDepth > 40;
    }
}
```

### Exemple 3 : Interface

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubExplore.Core.Models;

namespace SubExplore.Core.Interfaces
{
    public interface ISpotService
    {
        Task<Spot?> GetSpotByIdAsync(Guid spotId);
        Task<IEnumerable<Spot>> GetSpotsByDepthAsync(int minDepth, int maxDepth);
        Task<IEnumerable<Spot>> GetNearbySpots(double latitude, double longitude, int radiusKm);
        Task<Spot> CreateSpotAsync(Spot spot);
        Task UpdateSpotAsync(Spot spot);
        Task DeleteSpotAsync(Guid spotId);
    }
}
```

---

## 🔍 RÉSOLUTION DE PROBLÈMES

### Problème 1 : EditorConfig ne semble pas appliqué

**Symptômes:**
- Les règles de formatage ne s'appliquent pas
- Pas de warnings pour les violations de nommage

**Solutions:**

#### Visual Studio
1. Fermez et rouvrez Visual Studio
2. Menu **Tools** → **Options** → **Text Editor** → **C#** → **Code Style**
3. Vérifiez que **Enable EditorConfig support** est coché
4. Rechargez la solution (Ctrl+Shift+B)

#### VS Code
1. Vérifiez que l'extension EditorConfig est installée et activée
2. Ouvrez la Command Palette (Ctrl+Shift+P)
3. Tapez **"Reload Window"** et exécutez
4. Vérifiez `omnisharp.enableEditorConfigSupport: true` dans settings.json

#### Rider
1. **Settings** → **Editor** → **Code Style**
2. Vérifiez **Enable EditorConfig support**
3. **File** → **Invalidate Caches / Restart**

### Problème 2 : Conflits avec les paramètres existants

**Symptômes:**
- Formatage incohérent
- Certaines règles appliquées, d'autres non

**Solution:**

EditorConfig a la **priorité la plus haute**. Si vous avez des paramètres personnalisés dans votre IDE :

1. **Visual Studio:** Les paramètres EditorConfig écrasent les paramètres de l'IDE
2. **VS Code:** Vérifiez qu'il n'y a pas de `.vscode/settings.json` avec des règles conflictuelles
3. **Rider:** Les paramètres EditorConfig écrasent les paramètres Rider

**Pour réinitialiser:**
- Supprimez les fichiers de configuration personnalisés (`.vs/`, `.vscode/`, `.idea/`)
- Laissez EditorConfig gérer le style

### Problème 3 : Warnings trop nombreux dans le code existant

**Symptômes:**
- Des centaines de warnings après avoir ajouté .editorconfig
- Code existant ne respecte pas les conventions

**Solution:**

**Option A : Formatage automatique de la solution entière**

1. **Visual Studio:**
   - Extensions → Manage Extensions
   - Installer **Code Cleanup On Save**
   - Ou utiliser **Analyze → Code Cleanup → Run Code Cleanup on Solution**

2. **Rider:**
   - **Code → Reformat Code**
   - Cochez **Whole solution**
   - Cliquez **OK**

**Option B : Mise à jour progressive**
- Corrigez uniquement les fichiers que vous modifiez
- Utilisez la sévérité `suggestion` au lieu de `warning` temporairement

### Problème 4 : Fichier .editorconfig ignoré

**Symptômes:**
- Le fichier existe mais aucun effet visible

**Vérifications:**

1. **Emplacement correct ?**
   ```
   SubExplore/
   ├── .editorconfig    ← Doit être ICI (racine de la solution)
   ├── SubExplore.sln
   ├── SubExplore.API/
   └── SubExplore.Core/
   ```

2. **Syntaxe correcte ?**
   - Pas de caractères spéciaux dans le fichier
   - UTF-8 encoding sans BOM
   - Fins de ligne CRLF (Windows)

3. **Directive root ?**
   ```ini
   # La première ligne du fichier doit être:
   root = true
   ```

---

## 📚 RESSOURCES

### Documentation officielle

- **EditorConfig:** https://editorconfig.org/
- **C# Coding Conventions:** https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- **C# Naming Conventions:** https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names

### Extensions recommandées

**Visual Studio:**
- Code Cleanup On Save
- SonarLint for Visual Studio

**VS Code:**
- EditorConfig for VS Code
- C# Dev Kit
- SonarLint

**Rider:**
- Support natif complet

---

## ✅ CHECKLIST DE VALIDATION

Avant de considérer TASK-013 comme terminée, vérifiez:

- [x] `.editorconfig` existe à la racine de la solution
- [ ] Fichier contient les règles de nommage C# (interfaces, classes, méthodes, champs)
- [ ] Fichier contient les règles de formatage (indentation, accolades, espaces)
- [ ] IDE reconnaît le fichier EditorConfig (vérifier dans les settings)
- [ ] Test de formatage fonctionne (Ctrl+K, Ctrl+D)
- [ ] Warnings de nommage s'affichent pour le code non conforme
- [ ] Documentation créée et complète

---

**Dernière mise à jour:** 2025-12-10
**Prochaine étape:** TASK-014 - Configuration Analyzers (StyleCop, SonarAnalyzer)
