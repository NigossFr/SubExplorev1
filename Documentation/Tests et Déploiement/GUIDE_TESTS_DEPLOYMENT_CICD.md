# GUIDE DE TESTS ET DÉPLOIEMENT - SUBEXPLORE
## Stratégies de Test, CI/CD et Mise en Production

---

## TABLE DES MATIÈRES

1. [Stratégie de Tests](#1-stratégie-de-tests)
2. [Tests Unitaires](#2-tests-unitaires)
3. [Tests d'Intégration](#3-tests-dintégration)
4. [Tests d'Interface (UI)](#4-tests-dinterface-ui)
5. [Tests de Performance](#5-tests-de-performance)
6. [Pipeline CI/CD](#6-pipeline-cicd)
7. [Déploiement Mobile](#7-déploiement-mobile)
8. [Monitoring et Analytics](#8-monitoring-et-analytics)
9. [Plan de Rollback](#9-plan-de-rollback)
10. [Checklist de Production](#10-checklist-de-production)

---

## 1. STRATÉGIE DE TESTS

### 1.1 Pyramide de Tests

```
         /\
        /UI\        5% - Tests UI automatisés
       /____\
      /      \
     /  E2E   \     10% - Tests End-to-End
    /__________\
   /            \
  / Integration  \  25% - Tests d'intégration
 /________________\
/                  \
/   Unit Tests      \ 60% - Tests unitaires
/____________________\
```

### 1.2 Coverage Cible

```yaml
Objectifs de Couverture:
  Global: 80%
  Logique Métier (Core): 95%
  Services: 85%
  ViewModels: 80%
  Repositories: 75%
  UI: 50%
```

### 1.3 Environnements de Test

```yaml
Environnements:
  Local:
    Database: SQLite In-Memory
    API: Mocked
    Storage: Local FileSystem
    
  Dev:
    Database: Supabase Dev Project
    API: Dev Endpoints
    Storage: Dev Bucket
    
  Staging:
    Database: Supabase Staging (Copie Prod)
    API: Staging Endpoints
    Storage: Staging Bucket
    
  Production:
    Database: Supabase Production
    API: Production Endpoints
    Storage: Production Bucket
```

---

## 2. TESTS UNITAIRES

### 2.1 Configuration xUnit

```xml
<!-- SubExplore.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Bogus" Version="35.0.1" />
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
  </ItemGroup>
</Project>
```

### 2.2 Tests de Services

```csharp
public class SpotServiceTests : IDisposable
{
    private readonly Mock<ISpotRepository> _mockRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<SpotService>> _mockLogger;
    private readonly SpotService _sut; // System Under Test
    private readonly Faker<Spot> _spotFaker;

    public SpotServiceTests()
    {
        _mockRepository = new Mock<ISpotRepository>();
        _mockCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<SpotService>>();
        
        _sut = new SpotService(
            _mockRepository.Object,
            _mockCache.Object,
            _mockLogger.Object);
        
        // Configuration Bogus pour données de test
        _spotFaker = new Faker<Spot>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.Name, f => f.Lorem.Sentence(3))
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph())
            .RuleFor(s => s.Latitude, f => f.Address.Latitude())
            .RuleFor(s => s.Longitude, f => f.Address.Longitude())
            .RuleFor(s => s.DifficultyLevel, f => f.PickRandom<DifficultyLevel>())
            .RuleFor(s => s.ValidationStatus, f => f.PickRandom<SpotValidationStatus>());
    }

    [Fact]
    public async Task GetByIdAsync_WhenSpotExists_ReturnsSpot()
    {
        // Arrange
        var expectedSpot = _spotFaker.Generate();
        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedSpot);
        
        object cached = null;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cached))
            .Returns(false);

        // Act
        var result = await _sut.GetByIdAsync(expectedSpot.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSpot);
        
        _mockRepository.Verify(x => x.GetByIdAsync(expectedSpot.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCached_ReturnsCachedValue()
    {
        // Arrange
        var cachedSpot = _spotFaker.Generate();
        object cached = cachedSpot;
        
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cached))
            .Returns(true);

        // Act
        var result = await _sut.GetByIdAsync(cachedSpot.Id);

        // Assert
        result.Should().BeEquivalentTo(cachedSpot);
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Theory]
    [InlineData(0, 0, 10)]
    [InlineData(48.8566, 2.3522, 50)]
    [InlineData(-33.8688, 151.2093, 100)]
    public async Task GetNearbyAsync_ValidCoordinates_ReturnsSpots(
        double lat, double lon, double radius)
    {
        // Arrange
        var spots = _spotFaker.Generate(5);
        _mockRepository
            .Setup(x => x.GetNearbyAsync(lat, lon, radius))
            .ReturnsAsync(spots);

        // Act
        var result = await _sut.GetNearbyAsync(
            new Location(lat, lon), radius);

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeInAscendingOrder(s => s.Distance);
    }

    [Fact]
    public async Task CreateSpotAsync_WithInvalidData_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = new CreateSpotRequest
        {
            Name = "A", // Trop court
            Description = "Short", // Trop court
            Latitude = 200, // Invalide
            Longitude = -200 // Invalide
        };

        // Act & Assert
        await _sut.Invoking(s => s.CreateSpotAsync(invalidRequest, Guid.NewGuid()))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("*validation failed*");
    }

    public void Dispose()
    {
        // Cleanup si nécessaire
    }
}
```

### 2.3 Tests de ViewModels

```csharp
public class MapViewModelTests
{
    private readonly MapViewModel _sut;
    private readonly Mock<ISpotService> _mockSpotService;
    private readonly Mock<INavigationService> _mockNavigation;
    private readonly Mock<IDialogService> _mockDialog;
    
    public MapViewModelTests()
    {
        _mockSpotService = new Mock<ISpotService>();
        _mockNavigation = new Mock<INavigationService>();
        _mockDialog = new Mock<IDialogService>();
        
        _sut = new MapViewModel(
            _mockSpotService.Object,
            _mockNavigation.Object,
            _mockDialog.Object);
    }

    [Fact]
    public async Task InitializeCommand_LoadsSpots_Successfully()
    {
        // Arrange
        var spots = new List<Spot>
        {
            new SpotBuilder().WithName("Spot 1").Build(),
            new SpotBuilder().WithName("Spot 2").Build()
        };
        
        _mockSpotService
            .Setup(x => x.GetNearbyAsync(It.IsAny<Location>(), It.IsAny<double>()))
            .ReturnsAsync(spots);

        // Act
        await _sut.InitializeCommand.ExecuteAsync(null);

        // Assert
        _sut.Spots.Should().HaveCount(2);
        _sut.IsBusy.Should().BeFalse();
        _mockSpotService.Verify(x => x.GetNearbyAsync(
            It.IsAny<Location>(), 
            It.IsAny<double>()), 
            Times.Once);
    }

    [Fact]
    public async Task NavigateToSpotCommand_NavigatesToDetail()
    {
        // Arrange
        var spot = new SpotBuilder().Build();

        // Act
        await _sut.NavigateToSpotCommand.ExecuteAsync(spot);

        // Assert
        _mockNavigation.Verify(
            x => x.NavigateToAsync($"SpotDetail?spotId={spot.Id}"),
            Times.Once);
    }

    [Fact]
    public void FilterCommand_FiltersSpots_Correctly()
    {
        // Arrange
        _sut.Spots.Add(new SpotPin { Difficulty = DifficultyLevel.Beginner });
        _sut.Spots.Add(new SpotPin { Difficulty = DifficultyLevel.Expert });
        _sut.Spots.Add(new SpotPin { Difficulty = DifficultyLevel.Intermediate });

        // Act
        _sut.SelectedDifficulty = DifficultyLevel.Expert;
        _sut.ApplyFilterCommand.Execute(null);

        // Assert
        _sut.FilteredSpots.Should().HaveCount(1);
        _sut.FilteredSpots.First().Difficulty.Should().Be(DifficultyLevel.Expert);
    }
}
```

### 2.4 Tests de Validation

```csharp
public class SpotValidatorTests
{
    private readonly SpotValidator _validator;

    public SpotValidatorTests()
    {
        _validator = new SpotValidator();
    }

    [Theory]
    [InlineData("", false)] // Vide
    [InlineData("AB", false)] // Trop court
    [InlineData("Spot de plongée magnifique", true)] // Valide
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat test test test.", false)] // Trop long
    public void Name_Validation(string name, bool expectedValid)
    {
        // Arrange
        var spot = new CreateSpotRequest { Name = name };

        // Act
        var result = _validator.TestValidate(spot);

        // Assert
        if (expectedValid)
            result.ShouldNotHaveValidationErrorFor(s => s.Name);
        else
            result.ShouldHaveValidationErrorFor(s => s.Name);
    }

    [Theory]
    [InlineData(-91, 0, false)]
    [InlineData(91, 0, false)]
    [InlineData(0, -181, false)]
    [InlineData(0, 181, false)]
    [InlineData(48.8566, 2.3522, true)] // Paris
    [InlineData(-33.8688, 151.2093, true)] // Sydney
    public void Location_Validation(double lat, double lon, bool expectedValid)
    {
        // Arrange
        var spot = new CreateSpotRequest 
        { 
            Latitude = lat,
            Longitude = lon
        };

        // Act
        var result = _validator.TestValidate(spot);

        // Assert
        if (expectedValid)
        {
            result.ShouldNotHaveValidationErrorFor(s => s.Latitude);
            result.ShouldNotHaveValidationErrorFor(s => s.Longitude);
        }
        else
        {
            result.ShouldHaveAnyValidationError();
        }
    }
}
```

---

## 3. TESTS D'INTÉGRATION

### 3.1 Configuration Test Database

```csharp
public class IntegrationTestBase : IAsyncLifetime
{
    protected Supabase.Client TestSupabaseClient { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    
    public async Task InitializeAsync()
    {
        // Configuration test
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .AddEnvironmentVariables()
            .Build();
        
        // Services
        var services = new ServiceCollection();
        
        // Supabase test instance
        var supabaseUrl = configuration["Supabase:TestUrl"];
        var supabaseKey = configuration["Supabase:TestAnonKey"];
        
        TestSupabaseClient = new Supabase.Client(
            supabaseUrl, 
            supabaseKey,
            new SupabaseOptions
            {
                AutoRefreshToken = false,
                AutoConnectRealtime = false
            });
        
        services.AddSingleton(TestSupabaseClient);
        services.AddTransient<ISpotRepository, SpotRepository>();
        services.AddTransient<ISpotService, SpotService>();
        
        ServiceProvider = services.BuildServiceProvider();
        
        // Seed test data
        await SeedTestDataAsync();
    }
    
    private async Task SeedTestDataAsync()
    {
        // Créer un utilisateur de test
        var testUser = new User
        {
            Email = $"test_{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        
        await TestSupabaseClient
            .From<User>()
            .Insert(testUser);
    }
    
    public async Task DisposeAsync()
    {
        // Nettoyer les données de test
        await CleanupTestDataAsync();
    }
    
    private async Task CleanupTestDataAsync()
    {
        // Supprimer toutes les données créées pendant les tests
        await TestSupabaseClient.Rpc("cleanup_test_data", null);
    }
}
```

### 3.2 Tests d'Intégration Repository

```csharp
public class SpotRepositoryIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateAndRetrieveSpot_RoundTrip_Success()
    {
        // Arrange
        var repository = ServiceProvider.GetRequiredService<ISpotRepository>();
        
        var spot = new Spot
        {
            Name = $"Test Spot {Guid.NewGuid()}",
            Description = "Integration test spot",
            Location = new Location(48.8566, 2.3522),
            DifficultyLevel = DifficultyLevel.Intermediate,
            CreatorId = TestUserId
        };
        
        // Act - Create
        var created = await repository.CreateAsync(spot);
        
        // Act - Retrieve
        var retrieved = await repository.GetByIdAsync(created.Id);
        
        // Assert
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be(spot.Name);
        retrieved.Location.Should().BeEquivalentTo(spot.Location);
    }
    
    [Fact]
    public async Task GetNearbySpots_WithPostGIS_ReturnsCorrectDistance()
    {
        // Arrange
        var repository = ServiceProvider.GetRequiredService<ISpotRepository>();
        
        // Créer des spots à différentes distances
        var parisSpot = await repository.CreateAsync(new Spot
        {
            Name = "Paris Spot",
            Location = new Location(48.8566, 2.3522), // Paris
            CreatorId = TestUserId
        });
        
        var londonSpot = await repository.CreateAsync(new Spot
        {
            Name = "London Spot",
            Location = new Location(51.5074, -0.1278), // Londres
            CreatorId = TestUserId
        });
        
        // Act - Recherche depuis Paris avec rayon 50km
        var nearbySpots = await repository.GetNearbyAsync(48.8566, 2.3522, 50);
        
        // Assert
        nearbySpots.Should().ContainSingle();
        nearbySpots.First().Name.Should().Be("Paris Spot");
    }
}
```

### 3.3 Tests End-to-End API

```csharp
public class ApiEndToEndTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;
    
    public ApiEndToEndTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(TestSupabaseClient.Url)
        };
    }
    
    [Fact]
    public async Task CompleteSpotWorkflow_FromCreationToApproval()
    {
        // 1. Login
        var loginResponse = await AuthenticateAsync("user@example.com", "password");
        loginResponse.Should().BeSuccessful();
        
        var token = loginResponse.AccessToken;
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        // 2. Create Spot
        var createRequest = new
        {
            name = "E2E Test Spot",
            description = "Created during E2E test",
            location = new { lat = 48.8566, lon = 2.3522 },
            difficulty = "Intermediate"
        };
        
        var createResponse = await _httpClient.PostAsJsonAsync("/spots", createRequest);
        createResponse.Should().BeSuccessful();
        
        var spotId = (await createResponse.Content.ReadFromJsonAsync<dynamic>()).id;
        
        // 3. Upload Image
        var imageContent = new MultipartFormDataContent();
        imageContent.Add(new ByteArrayContent(GetTestImage()), "file", "test.jpg");
        
        var uploadResponse = await _httpClient.PostAsync(
            $"/spots/{spotId}/images", 
            imageContent);
        uploadResponse.Should().BeSuccessful();
        
        // 4. Submit for Validation
        var submitResponse = await _httpClient.PostAsync(
            $"/spots/{spotId}/submit", 
            null);
        submitResponse.Should().BeSuccessful();
        
        // 5. Login as Moderator
        var modResponse = await AuthenticateAsync("moderator@example.com", "password");
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", 
                modResponse.AccessToken);
        
        // 6. Approve Spot
        var approveRequest = new
        {
            status = "Approved",
            notes = "E2E test approval"
        };
        
        var approveResponse = await _httpClient.PutAsJsonAsync(
            $"/spots/{spotId}/validate", 
            approveRequest);
        approveResponse.Should().BeSuccessful();
        
        // 7. Verify Public Visibility
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        var publicResponse = await _httpClient.GetAsync($"/spots/{spotId}");
        publicResponse.Should().BeSuccessful();
        
        var publicSpot = await publicResponse.Content.ReadFromJsonAsync<dynamic>();
        publicSpot.validation_status.Should().Be("Approved");
    }
}
```

---

## 4. TESTS D'INTERFACE (UI)

### 4.1 Configuration UI Tests

```csharp
// SubExplore.UITests.csproj
public class BaseUITest : IDisposable
{
    protected IApp App { get; private set; }
    
    public BaseUITest(Platform platform)
    {
        App = AppInitializer.StartApp(platform);
    }
    
    protected void TakeScreenshot(string name)
    {
        var screenshot = App.Screenshot(name);
        
        // Sauvegarder pour les rapports
        var path = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "Screenshots",
            $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            
        screenshot.CopyTo(path);
    }
    
    public void Dispose()
    {
        App?.Dispose();
    }
}
```

### 4.2 Tests UI avec Appium

```csharp
public class LoginUITests : BaseUITest
{
    public LoginUITests() : base(Platform.Android) { }
    
    [Fact]
    public void Login_ValidCredentials_NavigatesToHome()
    {
        // Arrange
        App.WaitForElement("EmailEntry");
        TakeScreenshot("Login_Screen");
        
        // Act
        App.EnterText("EmailEntry", "test@example.com");
        App.EnterText("PasswordEntry", "password123");
        App.Tap("LoginButton");
        
        // Assert
        App.WaitForElement("MapView", timeout: TimeSpan.FromSeconds(10));
        App.Query("MapView").Should().HaveCount(1);
        TakeScreenshot("Home_After_Login");
    }
    
    [Fact]
    public void Login_InvalidCredentials_ShowsError()
    {
        // Act
        App.EnterText("EmailEntry", "invalid@example.com");
        App.EnterText("PasswordEntry", "wrongpassword");
        App.Tap("LoginButton");
        
        // Assert
        App.WaitForElement("ErrorLabel");
        var errorText = App.Query("ErrorLabel").First().Text;
        errorText.Should().Contain("Invalid credentials");
        TakeScreenshot("Login_Error");
    }
}

public class SpotCreationUITests : BaseUITest
{
    [Fact]
    public void CreateSpot_CompleteFlow_Success()
    {
        // Login first
        LoginAsTestUser();
        
        // Navigate to creation
        App.Tap("AddSpotFAB");
        App.WaitForElement("SpotNameEntry");
        
        // Step 1: Basic Info
        App.EnterText("SpotNameEntry", "UI Test Spot");
        App.EnterText("DescriptionEditor", "Created via UI test");
        App.Tap("NextButton");
        
        // Step 2: Location
        App.WaitForElement("MapView");
        App.TapCoordinates(100, 200); // Tap on map
        App.Tap("NextButton");
        
        // Step 3: Characteristics
        App.Tap("DifficultyPicker");
        App.Tap("Intermediate");
        App.EnterText("MaxDepthEntry", "30");
        App.Tap("NextButton");
        
        // Step 4: Safety
        App.EnterText("SafetyNotesEditor", "Test safety notes");
        App.Tap("NextButton");
        
        // Step 5: Photos (skip)
        App.Tap("SkipButton");
        
        // Step 6: Review & Submit
        App.WaitForElement("SubmitButton");
        App.ScrollDownTo("SubmitButton");
        App.Tap("SubmitButton");
        
        // Assert
        App.WaitForElement("SuccessDialog");
        TakeScreenshot("Spot_Created_Success");
    }
}
```

---

## 5. TESTS DE PERFORMANCE

### 5.1 Load Testing avec NBomber

```csharp
public class LoadTests
{
    [Fact]
    public void SpotSearch_LoadTest()
    {
        var scenario = Scenario.Create("spot_search", async context =>
        {
            var client = new HttpClient();
            
            var response = await client.GetAsync(
                $"{TestConfig.ApiUrl}/spots/nearby?lat=48.8566&lon=2.3522&radius=50");
            
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Assertions
        stats.AllOkCount.Should().BeGreaterThan(0);
        stats.AllFailCount.Should().Be(0);
        stats.ScenarioStats[0].Ok.Latency.Mean.Should().BeLessThan(1000); // < 1s
        stats.ScenarioStats[0].Ok.DataTransfer.MinKb.Should().BeGreaterThan(0);
    }
}
```

### 5.2 Memory Leak Tests

```csharp
public class MemoryLeakTests
{
    [Fact]
    public void NavigationCycle_NoMemoryLeak()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        
        // Act - Cycle through pages
        for (int i = 0; i < 100; i++)
        {
            var mapPage = new MapPage();
            var mapViewModel = new MapViewModel();
            mapPage.BindingContext = mapViewModel;
            
            // Simulate navigation
            Application.Current.MainPage = mapPage;
            
            // Navigate away
            Application.Current.MainPage = new ContentPage();
            
            // Cleanup
            mapViewModel.Cleanup();
            mapPage = null;
            mapViewModel = null;
        }
        
        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(true);
        
        // Assert - Memory should not grow significantly
        var memoryIncrease = finalMemory - initialMemory;
        memoryIncrease.Should().BeLessThan(1_000_000); // < 1MB
    }
}
```

---

## 6. PIPELINE CI/CD

### 6.1 GitHub Actions Workflow

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]
  release:
    types: [published]

env:
  DOTNET_VERSION: '8.0.x'
  MAUI_VERSION: '8.0.3'

jobs:
  # Job 1: Tests Unitaires
  unit-tests:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run Unit Tests
      run: |
        dotnet test SubExplore.Tests/SubExplore.Tests.csproj \
          --no-build \
          --configuration Release \
          --logger "trx;LogFileName=test-results.trx" \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults
    
    - name: Upload Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-results
        path: TestResults
    
    - name: Code Coverage Report
      uses: codecov/codecov-action@v3
      with:
        files: ./TestResults/**/coverage.cobertura.xml
        fail_ci_if_error: true

  # Job 2: Build Android
  build-android:
    runs-on: windows-latest
    needs: unit-tests
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET MAUI
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Install MAUI Workload
      run: dotnet workload install maui
    
    - name: Restore dependencies
      run: dotnet restore SubExplore.Mobile/SubExplore.Mobile.csproj
    
    - name: Build Android
      run: |
        dotnet publish SubExplore.Mobile/SubExplore.Mobile.csproj \
          -c Release \
          -f net8.0-android \
          -o ./artifacts/android
    
    - name: Sign APK
      if: github.event_name == 'release'
      run: |
        jarsigner -verbose \
          -sigalg SHA256withRSA \
          -digestalg SHA-256 \
          -keystore ${{ secrets.ANDROID_KEYSTORE }} \
          -storepass ${{ secrets.ANDROID_KEYSTORE_PASSWORD }} \
          ./artifacts/android/com.subexplore.app.apk \
          ${{ secrets.ANDROID_KEY_ALIAS }}
    
    - name: Upload Android Artifact
      uses: actions/upload-artifact@v3
      with:
        name: android-app
        path: artifacts/android/*.apk

  # Job 3: Build iOS
  build-ios:
    runs-on: macos-latest
    needs: unit-tests
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET MAUI
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Install MAUI Workload
      run: dotnet workload install maui
    
    - name: Install Apple Certificate
      if: github.event_name == 'release'
      env:
        BUILD_CERTIFICATE_BASE64: ${{ secrets.IOS_CERTIFICATE_BASE64 }}
        P12_PASSWORD: ${{ secrets.IOS_CERTIFICATE_PASSWORD }}
        KEYCHAIN_PASSWORD: ${{ secrets.KEYCHAIN_PASSWORD }}
      run: |
        # Create variables
        CERTIFICATE_PATH=$RUNNER_TEMP/build_certificate.p12
        KEYCHAIN_PATH=$RUNNER_TEMP/app-signing.keychain-db
        
        # Import certificate
        echo -n "$BUILD_CERTIFICATE_BASE64" | base64 --decode --output $CERTIFICATE_PATH
        
        # Create keychain
        security create-keychain -p "$KEYCHAIN_PASSWORD" $KEYCHAIN_PATH
        security set-keychain-settings -lut 21600 $KEYCHAIN_PATH
        security unlock-keychain -p "$KEYCHAIN_PASSWORD" $KEYCHAIN_PATH
        
        # Import certificate to keychain
        security import $CERTIFICATE_PATH -P "$P12_PASSWORD" \
          -A -t cert -f pkcs12 -k $KEYCHAIN_PATH
        security list-keychain -d user -s $KEYCHAIN_PATH
    
    - name: Build iOS
      run: |
        dotnet publish SubExplore.Mobile/SubExplore.Mobile.csproj \
          -c Release \
          -f net8.0-ios \
          -o ./artifacts/ios
    
    - name: Upload iOS Artifact
      uses: actions/upload-artifact@v3
      with:
        name: ios-app
        path: artifacts/ios/*.ipa

  # Job 4: Deploy to Staging
  deploy-staging:
    runs-on: ubuntu-latest
    needs: [build-android, build-ios]
    if: github.ref == 'refs/heads/develop'
    
    steps:
    - name: Download Artifacts
      uses: actions/download-artifact@v3
    
    - name: Deploy to App Center (Android)
      run: |
        npm install -g appcenter-cli
        appcenter login --token ${{ secrets.APPCENTER_TOKEN }}
        appcenter distribute release \
          --app SubExplore/Android-Staging \
          --file android-app/*.apk \
          --group "Internal Testers"
    
    - name: Deploy to TestFlight (iOS)
      run: |
        xcrun altool --upload-app \
          --type ios \
          --file ios-app/*.ipa \
          --username ${{ secrets.APPLE_ID }} \
          --password ${{ secrets.APPLE_APP_PASSWORD }}

  # Job 5: Deploy to Production
  deploy-production:
    runs-on: ubuntu-latest
    needs: [build-android, build-ios]
    if: github.event_name == 'release'
    
    steps:
    - name: Download Artifacts
      uses: actions/download-artifact@v3
    
    - name: Deploy to Google Play
      uses: r0adkll/upload-google-play@v1
      with:
        serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
        packageName: com.subexplore.app
        releaseFiles: android-app/*.apk
        track: production
    
    - name: Deploy to App Store
      run: |
        xcrun altool --upload-app \
          --type ios \
          --file ios-app/*.ipa \
          --username ${{ secrets.APPLE_ID }} \
          --password ${{ secrets.APPLE_APP_PASSWORD }}
    
    - name: Create GitHub Release
      uses: softprops/action-gh-release@v1
      with:
        files: |
          android-app/*.apk
          ios-app/*.ipa
        body: |
          ## Release ${{ github.event.release.tag_name }}
          
          ### Changes
          ${{ github.event.release.body }}
          
          ### Downloads
          - [Android APK](android-app/*.apk)
          - [iOS IPA](ios-app/*.ipa)
```

### 6.2 Azure DevOps Pipeline

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop
  tags:
    include:
      - v*

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'

stages:
- stage: Build
  jobs:
  - job: BuildAndTest
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotnetVersion)
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'
    
    - task: DotNetCoreCLI@2
      displayName: 'Test'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --collect "Code coverage"'
    
    - task: PublishTestResults@2
      inputs:
        testResultsFormat: 'VSTest'
        testResultsFiles: '**/*.trx'

- stage: Package
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: PackageApps
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Package Android'
      inputs:
        command: 'publish'
        projects: 'SubExplore.Mobile/SubExplore.Mobile.csproj'
        arguments: '-c Release -f net8.0-android'
        zipAfterPublish: false
    
    - task: CopyFiles@2
      inputs:
        sourceFolder: '$(Build.SourcesDirectory)'
        contents: '**/*.apk'
        targetFolder: '$(Build.ArtifactStagingDirectory)'
    
    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'drop'
```

---

## 7. DÉPLOIEMENT MOBILE

### 7.1 Configuration Android

```xml
<!-- Platforms/Android/AndroidManifest.xml -->
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
          package="com.subexplore.app"
          android:versionCode="1"
          android:versionName="1.0.0">
    
    <uses-sdk android:minSdkVersion="24" android:targetSdkVersion="34" />
    
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    
    <application android:label="SubExplore"
                 android:icon="@mipmap/appicon"
                 android:theme="@style/Maui.SplashTheme"
                 android:allowBackup="true"
                 android:supportsRtl="true">
        
        <!-- Google Maps API Key -->
        <meta-data android:name="com.google.android.geo.API_KEY"
                   android:value="${MAPS_API_KEY}" />
    </application>
</manifest>
```

### 7.2 Configuration iOS

```xml
<!-- Platforms/iOS/Info.plist -->
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" 
         "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDisplayName</key>
    <string>SubExplore</string>
    <key>CFBundleIdentifier</key>
    <string>com.subexplore.app</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    
    <!-- Permissions -->
    <key>NSLocationWhenInUseUsageDescription</key>
    <string>SubExplore utilise votre position pour afficher les spots à proximité</string>
    <key>NSLocationAlwaysUsageDescription</key>
    <string>SubExplore utilise votre position pour vous notifier des spots intéressants</string>
    <key>NSCameraUsageDescription</key>
    <string>SubExplore a besoin d'accéder à votre caméra pour prendre des photos</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>SubExplore a besoin d'accéder à vos photos pour les partager</string>
    
    <!-- App Transport Security -->
    <key>NSAppTransportSecurity</key>
    <dict>
        <key>NSExceptionDomains</key>
        <dict>
            <key>supabase.co</key>
            <dict>
                <key>NSExceptionAllowsInsecureHTTPLoads</key>
                <false/>
                <key>NSIncludesSubdomains</key>
                <true/>
            </dict>
        </dict>
    </dict>
</dict>
</plist>
```

### 7.3 Script de Déploiement

```bash
#!/bin/bash
# deploy.sh - Script de déploiement automatisé

set -e

VERSION=$1
ENVIRONMENT=$2

if [ -z "$VERSION" ] || [ -z "$ENVIRONMENT" ]; then
    echo "Usage: ./deploy.sh <version> <environment>"
    echo "Example: ./deploy.sh 1.2.0 staging"
    exit 1
fi

echo "🚀 Deploying SubExplore v$VERSION to $ENVIRONMENT"

# Build Android
echo "📱 Building Android..."
dotnet publish SubExplore.Mobile/SubExplore.Mobile.csproj \
    -c Release \
    -f net8.0-android \
    -p:AndroidPackageFormat=apk \
    -p:AndroidKeyStore=true \
    -p:AndroidSigningKeyStore=$ANDROID_KEYSTORE \
    -p:AndroidSigningKeyAlias=$ANDROID_KEY_ALIAS \
    -p:AndroidSigningKeyPass=$ANDROID_KEY_PASSWORD \
    -p:AndroidSigningStorePass=$ANDROID_KEYSTORE_PASSWORD

# Build iOS
echo "🍎 Building iOS..."
dotnet publish SubExplore.Mobile/SubExplore.Mobile.csproj \
    -c Release \
    -f net8.0-ios \
    -p:ArchiveOnBuild=true \
    -p:CodesignKey="$IOS_CODESIGN_KEY" \
    -p:CodesignProvision="$IOS_PROVISION_PROFILE"

# Deploy based on environment
case $ENVIRONMENT in
    staging)
        echo "📤 Deploying to App Center..."
        appcenter distribute release \
            --app SubExplore/Android-Staging \
            --file artifacts/android/*.apk \
            --group "Beta Testers"
        
        echo "📤 Deploying to TestFlight..."
        xcrun altool --upload-app \
            --type ios \
            --file artifacts/ios/*.ipa \
            --username $APPLE_ID \
            --password $APPLE_APP_PASSWORD
        ;;
    
    production)
        echo "📤 Deploying to Google Play..."
        fastlane supply \
            --apk artifacts/android/*.apk \
            --track production \
            --json_key $GOOGLE_PLAY_JSON_KEY
        
        echo "📤 Deploying to App Store..."
        fastlane deliver \
            --ipa artifacts/ios/*.ipa \
            --submit_for_review \
            --automatic_release
        ;;
    
    *)
        echo "Unknown environment: $ENVIRONMENT"
        exit 1
        ;;
esac

echo "✅ Deployment complete!"

# Send notification
curl -X POST $SLACK_WEBHOOK_URL \
    -H 'Content-Type: application/json' \
    -d "{\"text\":\"🎉 SubExplore v$VERSION deployed to $ENVIRONMENT\"}"
```

---

## 8. MONITORING ET ANALYTICS

### 8.1 Application Insights

```csharp
// Configuration AppInsights
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // Application Insights
        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = AppSettings.AppInsightsConnectionString;
            options.EnableAdaptiveSampling = true;
            options.EnableDependencyTrackingTelemetryModule = true;
        });
        
        // Custom Telemetry
        builder.Services.AddSingleton<ITelemetryService, TelemetryService>();
        
        return builder.Build();
    }
}

// Service de Télémétrie
public class TelemetryService : ITelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    
    public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
    {
        _telemetryClient.TrackEvent(eventName, properties);
    }
    
    public void TrackException(Exception exception, Dictionary<string, string> properties = null)
    {
        _telemetryClient.TrackException(exception, properties);
    }
    
    public void TrackPageView(string pageName, TimeSpan duration)
    {
        _telemetryClient.TrackPageView(pageName);
        _telemetryClient.TrackMetric($"{pageName}_Duration", duration.TotalSeconds);
    }
    
    public IDisposable StartOperation(string operationName)
    {
        var operation = _telemetryClient.StartOperation<RequestTelemetry>(operationName);
        return new OperationHolder(operation);
    }
}
```

### 8.2 Crashlytics

```csharp
// Configuration Firebase Crashlytics
public class CrashReportingService
{
    public static void Initialize()
    {
#if ANDROID
        Firebase.FirebaseApp.InitializeApp(Platform.CurrentActivity);
        Firebase.Crashlytics.FirebaseCrashlytics.Instance.SetCrashlyticsCollectionEnabled(true);
#elif IOS
        Firebase.Core.App.Configure();
#endif
    }
    
    public static void LogException(Exception exception)
    {
#if ANDROID || IOS
        Firebase.Crashlytics.FirebaseCrashlytics.Instance.RecordException(
            Java.Lang.Throwable.FromException(exception));
#endif
    }
    
    public static void SetUserIdentifier(string userId)
    {
#if ANDROID || IOS
        Firebase.Crashlytics.FirebaseCrashlytics.Instance.SetUserId(userId);
#endif
    }
    
    public static void Log(string message)
    {
#if ANDROID || IOS
        Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log(message);
#endif
    }
}
```

---

## 9. PLAN DE ROLLBACK

### 9.1 Procédure de Rollback

```yaml
rollback_procedure:
  detection:
    - Monitor error rates (>5% = critical)
    - Monitor crash reports
    - Monitor user complaints
    - Monitor API errors
  
  decision_matrix:
    - Severity: Critical
      Impact: >30% users
      Action: Immediate rollback
      Time: <15 minutes
    
    - Severity: High
      Impact: 10-30% users
      Action: Rollback after fix attempt
      Time: <1 hour
    
    - Severity: Medium
      Impact: <10% users
      Action: Hotfix deployment
      Time: <4 hours
  
  steps:
    1. Alert team via Slack/PagerDuty
    2. Confirm issue severity
    3. Execute rollback script
    4. Verify rollback success
    5. Communicate with users
    6. Post-mortem analysis
```

### 9.2 Script de Rollback

```bash
#!/bin/bash
# rollback.sh

PREVIOUS_VERSION=$1

echo "🔄 Rolling back to version $PREVIOUS_VERSION"

# Rollback Android
echo "Rolling back Android..."
gcloud app versions migrate $PREVIOUS_VERSION --service=android

# Rollback iOS
echo "Rolling back iOS..."
# iOS rollback via App Store Connect API
curl -X POST "https://api.appstoreconnect.apple.com/v1/apps/{APP_ID}/appStoreVersions" \
     -H "Authorization: Bearer $APP_STORE_CONNECT_TOKEN" \
     -d '{"data":{"type":"appStoreVersions","attributes":{"versionString":"'$PREVIOUS_VERSION'"}}}'

# Rollback API/Database if needed
echo "Rolling back Supabase..."
supabase db reset --version $PREVIOUS_VERSION

echo "✅ Rollback complete"
```

---

## 10. CHECKLIST DE PRODUCTION

### 10.1 Pre-Deployment Checklist

```markdown
## 📋 Pre-Deployment Checklist

### Code Quality
- [ ] All tests passing (>80% coverage)
- [ ] No critical SonarQube issues
- [ ] Code review completed
- [ ] Documentation updated

### Security
- [ ] Security scan passed
- [ ] API keys secured
- [ ] SSL certificates valid
- [ ] OWASP Top 10 verified

### Performance
- [ ] Load testing completed
- [ ] Memory leaks checked
- [ ] Bundle size optimized
- [ ] Images optimized

### Configuration
- [ ] Production config verified
- [ ] Feature flags set
- [ ] Analytics configured
- [ ] Error tracking enabled

### Mobile Specific
- [ ] App icons present (all sizes)
- [ ] Splash screens configured
- [ ] Store listings updated
- [ ] Screenshots current
- [ ] Privacy policy updated
- [ ] Terms of service updated

### Backend
- [ ] Database migrations tested
- [ ] Backup verified
- [ ] Monitoring alerts configured
- [ ] Rate limiting configured

### Legal
- [ ] GDPR compliance verified
- [ ] Age restrictions implemented
- [ ] Content moderation active
- [ ] Copyright checks done
```

### 10.2 Post-Deployment Verification

```bash
#!/bin/bash
# post-deployment-check.sh

echo "🔍 Running post-deployment checks..."

# Check API health
API_STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://api.subexplore.app/health)
if [ $API_STATUS -ne 200 ]; then
    echo "❌ API health check failed"
    exit 1
fi

# Check database connectivity
DB_STATUS=$(curl -s https://api.subexplore.app/db-status | jq -r '.status')
if [ "$DB_STATUS" != "healthy" ]; then
    echo "❌ Database check failed"
    exit 1
fi

# Check critical endpoints
ENDPOINTS=(
    "/spots/nearby?lat=0&lon=0"
    "/auth/login"
    "/users/profile"
)

for endpoint in "${ENDPOINTS[@]}"; do
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://api.subexplore.app$endpoint)
    if [ $STATUS -ge 500 ]; then
        echo "❌ Endpoint $endpoint returned $STATUS"
        exit 1
    fi
done

# Check mobile app availability
ANDROID_STATUS=$(curl -s "https://play.google.com/store/apps/details?id=com.subexplore.app" | grep -c "SubExplore")
if [ $ANDROID_STATUS -eq 0 ]; then
    echo "⚠️ Android app not found in store"
fi

echo "✅ All post-deployment checks passed!"
```

---

## CONCLUSION

Ce guide complet de tests et déploiement assure:

- **Qualité**: Tests exhaustifs à tous les niveaux
- **Automatisation**: CI/CD complet
- **Fiabilité**: Monitoring et rollback
- **Performance**: Tests de charge et optimisation
- **Sécurité**: Scans et validations

### Points Clés

1. **Tests First**: Jamais de déploiement sans tests
2. **Automation**: Tout doit être automatisé
3. **Monitoring**: Surveillance constante
4. **Rollback Ready**: Toujours pouvoir revenir en arrière
5. **Documentation**: Tout est documenté

### KPIs de Déploiement

- Temps de build: < 10 minutes
- Temps de déploiement: < 30 minutes
- Taux de succès: > 95%
- MTTR (Mean Time To Recovery): < 1 heure
- Couverture de tests: > 80%

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Statut**: Guide de déploiement production
**Mise à jour**: À chaque release majeure

*Ce document est essentiel pour garantir des déploiements sûrs et efficaces de SubExplore.*