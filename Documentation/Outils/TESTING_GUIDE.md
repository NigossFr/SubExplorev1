# 🧪 Guide de Configuration Tests Unitaires - SubExplore

## 📋 Table des matières

1. [Présentation](#présentation)
2. [Architecture des Tests](#architecture-des-tests)
3. [Configuration des Projets](#configuration-des-projets)
4. [Frameworks Utilisés](#frameworks-utilisés)
5. [Structure des Tests](#structure-des-tests)
6. [Patterns de Tests](#patterns-de-tests)
7. [Mocking avec Moq](#mocking-avec-moq)
8. [Assertions avec FluentAssertions](#assertions-avec-fluentassertions)
9. [Exécution des Tests](#exécution-des-tests)
10. [Bonnes Pratiques](#bonnes-pratiques)
11. [Dépannage](#dépannage)

---

## 🎯 Présentation

SubExplore utilise **xUnit**, **FluentAssertions** et **Moq** pour les tests unitaires. Cette stack offre :

- **xUnit** : Framework de tests moderne et extensible
- **FluentAssertions** : Assertions expressives et lisibles
- **Moq** : Library de mocking puissante et simple
- **Couverture** : Tests pour Domain et Application layers

### Projets de Tests

**SubExplore.Domain.UnitTests** :
- Tests des entités du Domain Layer
- Tests des Value Objects
- Tests des règles métier
- Tests de validation

**SubExplore.Application.UnitTests** :
- Tests des Use Cases / Command Handlers
- Tests des Query Handlers
- Tests des services applicatifs
- Tests des validateurs

---

## 🏗️ Architecture des Tests

### Vue d'ensemble

```
Tests/
├── SubExplore.Domain.UnitTests/
│   ├── SetupVerificationTests.cs      # Tests de vérification setup
│   ├── Entities/                      # (À venir) Tests entités
│   ├── ValueObjects/                  # (À venir) Tests value objects
│   └── Validators/                    # (À venir) Tests validators
│
└── SubExplore.Application.UnitTests/
    ├── SetupVerificationTests.cs      # Tests de vérification setup
    ├── Commands/                      # (À venir) Tests commands
    ├── Queries/                       # (À venir) Tests queries
    └── Services/                      # (À venir) Tests services
```

### Dépendances

```
SubExplore.Domain.UnitTests
    ├── xUnit.net 2.9.3
    ├── FluentAssertions 8.8.0
    ├── Moq 4.20.72
    └── → SubExplore.Domain

SubExplore.Application.UnitTests
    ├── xUnit.net 2.9.3
    ├── FluentAssertions 8.8.0
    ├── Moq 4.20.72
    └── → SubExplore.Application
```

---

## ⚙️ Configuration des Projets

### Création des Projets

Les projets ont été créés avec la commande dotnet CLI :

```bash
# Domain tests
dotnet new xunit -n SubExplore.Domain.UnitTests -o Tests/SubExplore.Domain.UnitTests

# Application tests
dotnet new xunit -n SubExplore.Application.UnitTests -o Tests/SubExplore.Application.UnitTests

# Ajout à la solution
dotnet sln add Tests/SubExplore.Domain.UnitTests/SubExplore.Domain.UnitTests.csproj
dotnet sln add Tests/SubExplore.Application.UnitTests/SubExplore.Application.UnitTests.csproj
```

### Installation des Packages

```bash
# FluentAssertions (assertions expressives)
dotnet add Tests/SubExplore.Domain.UnitTests package FluentAssertions
dotnet add Tests/SubExplore.Application.UnitTests package FluentAssertions

# Moq (mocking)
dotnet add Tests/SubExplore.Domain.UnitTests package Moq
dotnet add Tests/SubExplore.Application.UnitTests package Moq
```

### Références de Projet

```bash
# Domain tests référence Domain
dotnet add Tests/SubExplore.Domain.UnitTests reference SubExplore.Domain

# Application tests référence Application
dotnet add Tests/SubExplore.Application.UnitTests reference SubExplore.Application
```

### Fichier .csproj

Exemple de configuration (SubExplore.Domain.UnitTests.csproj) :

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.2" />
    <PackageReference Include="FluentAssertions" Version="8.8.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\SubExplore.Domain\SubExplore.Domain.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔧 Frameworks Utilisés

### xUnit.net

**Version** : 2.9.3

**Caractéristiques** :
- Framework de tests moderne et extensible
- Exécution parallèle des tests par défaut
- Support des `[Fact]` et `[Theory]`
- Isolation des tests (nouvelle instance de classe par test)

**Attributs principaux** :
- `[Fact]` : Test simple sans paramètres
- `[Theory]` : Test avec plusieurs jeux de données
- `[InlineData]` : Données inline pour Theory
- `[Skip]` : Ignorer temporairement un test

### FluentAssertions

**Version** : 8.8.0

**Caractéristiques** :
- Assertions expressives et lisibles
- Messages d'erreur clairs et détaillés
- Support complet des types .NET
- Extensions pour collections, exceptions, async

**Exemple** :
```csharp
// Au lieu de Assert.Equal(expected, actual)
actual.Should().Be(expected);

// Messages d'erreur descriptifs automatiques
result.Should().BeGreaterThan(0, "because the calculation should return a positive value");
```

### Moq

**Version** : 4.20.72

**Caractéristiques** :
- Library de mocking simple et puissante
- API fluide et intuitive
- Support complet des interfaces et classes
- Vérification des appels de méthodes

**Exemple** :
```csharp
var mock = new Mock<IRepository>();
mock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(expectedEntity);
```

---

## 📝 Structure des Tests

### Arrangement AAA (Arrange-Act-Assert)

Tous les tests suivent le pattern AAA :

```csharp
[Fact]
public void MyTest()
{
    // Arrange - Préparer les données et mocks
    var expectedValue = 42;
    var sut = new SystemUnderTest();

    // Act - Exécuter l'opération à tester
    var result = sut.DoSomething();

    // Assert - Vérifier le résultat
    result.Should().Be(expectedValue);
}
```

### Naming Convention

**Format** : `MethodName_Scenario_ExpectedBehavior` ou `Subject_Should_Behavior`

```csharp
// ✅ BON
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum() { }

[Fact]
public void User_Should_Be_Created_With_Valid_Data() { }

// ❌ MAUVAIS
[Fact]
public void Test1() { }

[Fact]
public void AddTest() { }
```

### Tests Paramétrés (Theory)

Utiliser `[Theory]` pour tester plusieurs scénarios :

```csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(-1, 1, 0)]
[InlineData(0, 0, 0)]
[InlineData(100, 200, 300)]
public void Add_DifferentInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    // Arrange
    var calculator = new Calculator();

    // Act
    var result = calculator.Add(a, b);

    // Assert
    result.Should().Be(expected);
}
```

---

## 🎭 Patterns de Tests

### Test d'Entité (Domain)

```csharp
public class UserEntityTests
{
    [Fact]
    public void User_Should_Be_Created_With_Valid_Email()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var result = User.Create(email, firstName, lastName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be(email);
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void User_Should_Not_Be_Created_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var result = User.Create(invalidEmail, firstName, lastName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }
}
```

### Test de Value Object

```csharp
public class CoordinatesTests
{
    [Fact]
    public void Coordinates_Should_Be_Created_With_Valid_Values()
    {
        // Arrange
        var latitude = 48.8566;
        var longitude = 2.3522;

        // Act
        var result = Coordinates.Create(latitude, longitude);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Latitude.Should().Be(latitude);
        result.Value.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData(91.0, 0.0)]     // Latitude trop haute
    [InlineData(-91.0, 0.0)]    // Latitude trop basse
    [InlineData(0.0, 181.0)]    // Longitude trop haute
    [InlineData(0.0, -181.0)]   // Longitude trop basse
    public void Coordinates_Should_Not_Be_Created_With_Invalid_Values(
        double latitude, double longitude)
    {
        // Act
        var result = Coordinates.Create(latitude, longitude);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public void Coordinates_Should_Calculate_Distance_Correctly()
    {
        // Arrange
        var paris = Coordinates.Create(48.8566, 2.3522).Value;
        var london = Coordinates.Create(51.5074, -0.1278).Value;

        // Act
        var distance = paris.DistanceTo(london);

        // Assert
        distance.Should().BeApproximately(344, 10); // ~344 km ± 10 km
    }
}
```

### Test de Command Handler (Application)

```csharp
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _sut = new CreateUserCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesUser()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "John", "Doe");
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), default))
                      .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _mockRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == command.Email &&
            u.FirstName == command.FirstName &&
            u.LastName == command.LastName
        ), default), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsError()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "John", "Doe");
        _mockRepository.Setup(r => r.ExistsAsync(command.Email, default))
                      .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Never);
    }
}
```

---

## 🎭 Mocking avec Moq

### Création de Mocks

```csharp
// Mock d'une interface
var mockRepository = new Mock<IUserRepository>();

// Mock d'une classe (doit avoir des membres virtual)
var mockService = new Mock<UserService>();

// Mock avec comportement strict (lève exception si méthode non configurée)
var strictMock = new Mock<IUserRepository>(MockBehavior.Strict);
```

### Setup de Méthodes

```csharp
// Retour simple
mockRepository.Setup(r => r.GetById(userId))
              .Returns(expectedUser);

// Retour async
mockRepository.Setup(r => r.GetByIdAsync(userId, default))
              .ReturnsAsync(expectedUser);

// Retour conditionnel
mockRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
              .Returns<Guid>(id => id == validId ? expectedUser : null);

// Lancer une exception
mockRepository.Setup(r => r.Save(It.IsAny<User>()))
              .Throws<InvalidOperationException>();
```

### Setup de Propriétés

```csharp
// Propriété simple
mockService.SetupGet(s => s.IsReady).Returns(true);

// Propriété get et set
mockService.SetupProperty(s => s.Name, "Initial Value");
```

### Matchers (It)

```csharp
// N'importe quelle valeur du type
mockRepository.Setup(r => r.GetById(It.IsAny<Guid>()));

// Valeur spécifique
mockRepository.Setup(r => r.GetById(It.Is<Guid>(id => id != Guid.Empty)));

// Regex pour strings
mockRepository.Setup(r => r.GetByEmail(It.IsRegex(@".*@example\.com")));

// Intervalle
mockRepository.Setup(r => r.GetByAge(It.IsInRange(18, 65, Range.Inclusive)));
```

### Vérifications

```csharp
// Vérifier qu'une méthode a été appelée
mockRepository.Verify(r => r.Save(It.IsAny<User>()), Times.Once);

// Vérifier qu'une méthode n'a jamais été appelée
mockRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);

// Vérifier avec paramètres spécifiques
mockRepository.Verify(r => r.Save(It.Is<User>(u => u.Email == "test@example.com")));

// Vérifier le nombre d'appels
mockRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Exactly(3));
mockRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.AtLeast(1));
mockRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.AtMost(5));
```

---

## ✅ Assertions avec FluentAssertions

### Assertions de Base

```csharp
// Égalité
result.Should().Be(expected);
result.Should().NotBe(unexpected);

// Null
result.Should().BeNull();
result.Should().NotBeNull();

// Types
result.Should().BeOfType<User>();
result.Should().BeAssignableTo<IEntity>();
```

### Assertions Numériques

```csharp
age.Should().BeGreaterThan(18);
age.Should().BeGreaterThanOrEqualTo(18);
age.Should().BeLessThan(65);
age.Should().BeInRange(18, 65);
price.Should().BeApproximately(42.5, 0.1); // ±0.1
```

### Assertions de Chaînes

```csharp
name.Should().Be("John");
name.Should().NotBeNullOrEmpty();
name.Should().NotBeNullOrWhiteSpace();
name.Should().StartWith("Jo");
name.Should().EndWith("hn");
name.Should().Contain("oh");
name.Should().MatchRegex(@"^[A-Z][a-z]+$");
```

### Assertions de Collections

```csharp
// Taille
users.Should().HaveCount(5);
users.Should().NotBeEmpty();
users.Should().HaveCountGreaterThan(3);

// Contenu
users.Should().Contain(expectedUser);
users.Should().ContainSingle(u => u.Email == "test@example.com");
users.Should().OnlyContain(u => u.Age >= 18);

// Ordre
numbers.Should().BeInAscendingOrder();
numbers.Should().BeInDescendingOrder();

// Équivalence
actualUsers.Should().BeEquivalentTo(expectedUsers);
```

### Assertions d'Exceptions

```csharp
// Exception levée
Action act = () => sut.DoSomething();
act.Should().Throw<InvalidOperationException>();
act.Should().Throw<ArgumentException>()
   .WithMessage("*parameter*")
   .And.ParamName.Should().Be("userId");

// Pas d'exception
Action act = () => sut.SafeOperation();
act.Should().NotThrow();
```

### Assertions Asynchrones

```csharp
// Async Task
Func<Task> act = async () => await sut.DoSomethingAsync();
await act.Should().ThrowAsync<InvalidOperationException>();
await act.Should().NotThrowAsync();

// Async avec résultat
var result = await sut.GetUserAsync(userId);
result.Should().NotBeNull();
result.Email.Should().Be("test@example.com");
```

---

## 🚀 Exécution des Tests

### Commandes dotnet test

```bash
# Exécuter tous les tests
dotnet test

# Exécuter les tests d'un projet spécifique
dotnet test Tests/SubExplore.Domain.UnitTests

# Exécuter avec verbosité détaillée
dotnet test --verbosity detailed

# Exécuter avec logger
dotnet test --logger "console;verbosity=detailed"

# Filtrer par nom de test
dotnet test --filter "FullyQualifiedName~CreateUser"

# Filtrer par catégorie
dotnet test --filter "Category=Integration"

# Exécuter sans rebuild
dotnet test --no-build

# Collecter la couverture de code
dotnet test --collect:"XPlat Code Coverage"
```

### Exécution dans Visual Studio

1. **Test Explorer** : View → Test Explorer
2. **Run All Tests** : Ctrl+R, A
3. **Run Selected Tests** : Ctrl+R, T
4. **Debug Test** : Right-click → Debug Test

### Exécution dans VS Code

1. Installer l'extension **.NET Core Test Explorer**
2. Les tests apparaissent dans la barre latérale
3. Cliquer sur "Run" ou "Debug" à côté de chaque test

### Exécution dans Rider

1. **Test Explorer** : View → Tool Windows → Unit Tests
2. **Run All** : Ctrl+U, L
3. **Run Current** : Ctrl+U, R
4. **Debug Current** : Ctrl+U, D

---

## ✅ Bonnes Pratiques

### 1. Tests Indépendants

```csharp
// ✅ BON : Chaque test crée ses propres données
[Fact]
public void Test1()
{
    var user = new User("test1@example.com");
    // ...
}

[Fact]
public void Test2()
{
    var user = new User("test2@example.com");
    // ...
}

// ❌ MAUVAIS : Tests partagent des données
private User _sharedUser = new User("shared@example.com");

[Fact]
public void Test1()
{
    _sharedUser.Name = "Test1"; // Modifie l'état partagé
}
```

### 2. Un Test = Un Concept

```csharp
// ✅ BON : Test une seule chose
[Fact]
public void User_Should_Be_Created_With_Valid_Email() { }

[Fact]
public void User_Should_Not_Be_Created_With_Invalid_Email() { }

// ❌ MAUVAIS : Test plusieurs choses
[Fact]
public void UserTests()
{
    // Teste création
    // Teste validation
    // Teste mise à jour
    // ...
}
```

### 3. Tests Lisibles

```csharp
// ✅ BON : Nom explicite, AAA clair
[Fact]
public void Calculate_Distance_Between_Paris_And_London_Returns_Approximately_344_Km()
{
    // Arrange
    var paris = Coordinates.Create(48.8566, 2.3522).Value;
    var london = Coordinates.Create(51.5074, -0.1278).Value;

    // Act
    var distance = paris.DistanceTo(london);

    // Assert
    distance.Should().BeApproximately(344, 10);
}

// ❌ MAUVAIS : Nom cryptique, pas de structure
[Fact]
public void Test1()
{
    var c1 = new Coordinates(48.8566, 2.3522);
    var d = c1.DistanceTo(new Coordinates(51.5074, -0.1278));
    Assert.True(d > 340 && d < 350);
}
```

### 4. Ne Pas Tester le Framework

```csharp
// ❌ MAUVAIS : Teste que List.Add fonctionne
[Fact]
public void List_Should_Add_Items()
{
    var list = new List<int>();
    list.Add(1);
    list.Should().Contain(1);
}

// ✅ BON : Teste la logique métier
[Fact]
public void UserCollection_Should_Not_Accept_Duplicate_Emails()
{
    var collection = new UserCollection();
    collection.Add(new User("test@example.com"));

    var act = () => collection.Add(new User("test@example.com"));

    act.Should().Throw<DuplicateEmailException>();
}
```

### 5. Éviter la Logique dans les Tests

```csharp
// ❌ MAUVAIS : Logique conditionnelle
[Fact]
public void TestWithLogic()
{
    var result = sut.DoSomething();
    if (result > 0)
    {
        // Test quelque chose
    }
    else
    {
        // Test autre chose
    }
}

// ✅ BON : Tests séparés pour chaque cas
[Fact]
public void DoSomething_PositiveResult_ReturnsValue() { }

[Fact]
public void DoSomething_NegativeResult_ReturnsZero() { }
```

### 6. Utiliser Theory pour Cas Multiples

```csharp
// ❌ MAUVAIS : Dupliquer le code
[Fact]
public void IsValidEmail_ValidEmail1_ReturnsTrue()
{
    Validator.IsValidEmail("test@example.com").Should().BeTrue();
}

[Fact]
public void IsValidEmail_ValidEmail2_ReturnsTrue()
{
    Validator.IsValidEmail("user@domain.co.uk").Should().BeTrue();
}

// ✅ BON : Utiliser Theory
[Theory]
[InlineData("test@example.com")]
[InlineData("user@domain.co.uk")]
[InlineData("name.surname@company.org")]
public void IsValidEmail_ValidEmails_ReturnsTrue(string email)
{
    Validator.IsValidEmail(email).Should().BeTrue();
}
```

### 7. Messages d'Assertion Clairs

```csharp
// ✅ BON : Message explicite
result.Should().BeGreaterThan(0, "because the calculation should return a positive distance");

// ❌ MAUVAIS : Pas de message
result.Should().BeGreaterThan(0);
```

---

## 🔧 Dépannage

### Problème : Tests ne s'exécutent pas

**Solution 1** : Vérifier que les packages sont installés

```bash
dotnet restore
dotnet build
```

**Solution 2** : Vérifier la version de Microsoft.NET.Test.Sdk

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
```

### Problème : Mocks ne fonctionnent pas

**Solution** : Vérifier que l'interface est bien utilisée

```csharp
// ✅ BON
var mock = new Mock<IUserRepository>();
var sut = new UserService(mock.Object); // Utiliser mock.Object

// ❌ MAUVAIS
var sut = new UserService(mock); // Passe le Mock au lieu de l'Object
```

### Problème : Tests parallèles échouent

**Solution** : Désactiver le parallélisme si nécessaire

```xml
<!-- Dans le fichier .csproj -->
<PropertyGroup>
  <ParallelizeTestCollections>false</ParallelizeTestCollections>
</PropertyGroup>
```

Ou utiliser l'attribut Collection :

```csharp
[Collection("Sequential")]
public class MyTests { }
```

### Problème : FluentAssertions messages cryptiques

**Solution** : Utiliser Because() pour clarifier

```csharp
result.Should().Be(expected, "because we're testing the calculation of X");
```

---

## 📚 Ressources

### Documentation Officielle

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)

### Références SubExplore

- **Tests de vérification** : `SetupVerificationTests.cs` dans chaque projet
- **Exemples de tests** : Voir les tests de vérification pour des patterns de base

---

## ✅ Checklist de Configuration

- [x] Projets de tests créés (Domain.UnitTests, Application.UnitTests)
- [x] Packages installés (xUnit 2.9.3, FluentAssertions 8.8.0, Moq 4.20.72)
- [x] Références de projet configurées
- [x] Tests de vérification créés et passant (18 tests total)
- [x] Structure AAA utilisée dans tous les tests
- [x] Naming convention cohérente
- [ ] Tests de couverture configurés (à venir)
- [ ] Tests d'intégration configurés (TASK-018)

---

## 🌐 Tests d'Intégration (API)

### Présentation

Les tests d'intégration vérifient que l'API fonctionne correctement en testant les endpoints HTTP, l'authentification, les bases de données, etc.

### Projet de Tests d'Intégration

**SubExplore.API.IntegrationTests** :
- Tests des endpoints de l'API
- Tests d'authentification et autorisation
- Tests de validation et erreurs
- Tests avec base de données (Testcontainers)

### Frameworks Utilisés

```bash
# xUnit
dotnet add package xUnit --version 2.9.3

# FluentAssertions
dotnet add package FluentAssertions --version 8.8.0

# WebApplicationFactory
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.0

# Testcontainers PostgreSQL
dotnet add package Testcontainers.PostgreSql --version 4.9.0
```

### Architecture

```
Tests/SubExplore.API.IntegrationTests/
├── SubExploreWebApplicationFactory.cs    # Factory personnalisée
├── ApiSetupVerificationTests.cs          # Tests de vérification
└── README.md                              # Documentation
```

### WebApplicationFactory

La `SubExploreWebApplicationFactory` hérite de `WebApplicationFactory<Program>` :

```csharp
public class SubExploreWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Environment"] = "Test",
                ["Logging:LogLevel:Default"] = "Warning"
            });
        });

        builder.UseEnvironment("Test");
    }
}
```

### Tests de Vérification

4 tests de vérification de la configuration :

1. **WebApplicationFactory_Should_Be_Instantiable** : Vérifie que la factory peut être instanciée
2. **MvcTesting_Package_Should_Be_Available** : Vérifie que Mvc.Testing est disponible
3. **FluentAssertions_Package_Should_Be_Available** : Vérifie que FluentAssertions est disponible
4. **TestcontainersPostgreSql_Package_Should_Be_Available** : Vérifie que Testcontainers est disponible

### Exemple de Test d'Intégration (À venir)

```csharp
public class DiverEndpointsTests : IClassFixture<SubExploreWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DiverEndpointsTests(SubExploreWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllDivers_Should_Return_Ok()
    {
        // Arrange
        var endpoint = "/api/divers";

        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var divers = await response.Content.ReadFromJsonAsync<List<DiverDto>>();
        divers.Should().NotBeNull();
    }
}
```

### Testcontainers (À configurer)

Testcontainers permet de lancer des conteneurs Docker pour les tests :

```csharp
// Exemple de configuration PostgreSQL (à implémenter)
private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
    .WithDatabase("subexplore_test")
    .WithUsername("test")
    .WithPassword("test")
    .Build();

// Démarrage du conteneur
await _postgresContainer.StartAsync();

// Connexion à la base de test
var connectionString = _postgresContainer.GetConnectionString();
```

### Exécution des Tests d'Intégration

```bash
# Tous les tests d'intégration
dotnet test SubExplore.API.IntegrationTests.csproj

# Avec verbosité
dotnet test SubExplore.API.IntegrationTests.csproj --verbosity normal

# Tests spécifiques
dotnet test --filter "FullyQualifiedName~ApiSetupVerificationTests"
```

### État Actuel (TASK-018)

- ✅ Projet créé
- ✅ Packages installés (WebApplicationFactory, Testcontainers)
- ✅ WebApplicationFactory configurée
- ✅ Tests de vérification (4 tests passent)
- 🚧 Configuration base de données de test (en attente)
- 🚧 Tests complets d'endpoints (en attente)

### Prochaines Étapes

1. Configurer Testcontainers avec PostgreSQL
2. Créer des tests pour les endpoints de l'API
3. Ajouter des tests d'authentification
4. Configurer des scénarios de tests complets
5. Intégrer dans le pipeline CI/CD

---

## 📊 Statistiques

**Projets de Tests** : 3
- Domain.UnitTests : 9 tests
- Application.UnitTests : 9 tests
- API.IntegrationTests : 4 tests de vérification

**Frameworks** : xUnit 2.9.3, FluentAssertions 8.8.0, Moq 4.20.72
**Tests Totaux** : 22 (18 unitaires + 4 intégration)
**Taux de Réussite** : 100% ✅

---

**Dernière mise à jour** : 2025-12-11
**Version** : 1.1
**Auteur** : SubExplore Development Team
