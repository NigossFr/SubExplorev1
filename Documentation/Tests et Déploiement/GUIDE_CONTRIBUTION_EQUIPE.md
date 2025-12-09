# GUIDE DE CONTRIBUTION ET BONNES PRATIQUES - SUBEXPLORE
## Manuel de l'Équipe de Développement

---

## TABLE DES MATIÈRES

1. [Onboarding Développeur](#1-onboarding-développeur)
2. [Standards de Code](#2-standards-de-code)
3. [Workflow Git](#3-workflow-git)
4. [Processus de Développement](#4-processus-de-développement)
5. [Code Review Guidelines](#5-code-review-guidelines)
6. [Documentation](#6-documentation)
7. [Testing Standards](#7-testing-standards)
8. [Communication d'Équipe](#8-communication-déquipe)

---

## 1. ONBOARDING DÉVELOPPEUR

### 1.1 Checklist Jour 1

```yaml
Accès_Requis:
  Comptes:
    ✅ GitHub (organisation SubExplore)
    ✅ Supabase (dev environment)
    ✅ Azure DevOps
    ✅ Slack workspace
    ✅ Figma (designs)
    ✅ Jira/Linear
    ✅ 1Password (team vault)
    
  Outils_Développement:
    ✅ Visual Studio 2022
    ✅ VS Code
    ✅ .NET 8 SDK
    ✅ MAUI workloads
    ✅ Git
    ✅ Docker Desktop
    ✅ Postman/Insomnia
    
  Documentation:
    ✅ Architecture overview
    ✅ Cahier des charges
    ✅ API documentation
    ✅ Database schema
    ✅ Style guide
```

### 1.2 Setup Environnement Local

```bash
# 1. Cloner le repository
git clone https://github.com/subexplore/subexplore-app.git
cd subexplore-app

# 2. Installer les dépendances
dotnet restore

# 3. Configuration locale
cp .env.example .env.local
# Éditer .env.local avec les credentials dev

# 4. Base de données locale
docker-compose up -d postgres
dotnet ef database update

# 5. Lancer l'API
cd src/SubExplore.Api
dotnet run

# 6. Lancer l'app mobile (Android)
cd src/SubExplore.Mobile
dotnet build -t:Run -f net8.0-android

# 7. Vérifier que tout fonctionne
dotnet test
```

### 1.3 Architecture Overview

```csharp
// Structure de la solution
SubExplore/
├── src/
│   ├── SubExplore.Domain/          // Entités, Value Objects, Interfaces
│   ├── SubExplore.Application/     // Use Cases, DTOs, Services
│   ├── SubExplore.Infrastructure/  // Repositories, External Services
│   ├── SubExplore.Api/            // Controllers, Middleware
│   ├── SubExplore.Mobile/         // MAUI App
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   ├── Services/
│   │   └── Resources/
│   └── SubExplore.Shared/         // Code partagé
├── tests/
│   ├── SubExplore.UnitTests/
│   ├── SubExplore.IntegrationTests/
│   └── SubExplore.UITests/
├── docs/
├── scripts/
└── .github/workflows/
```

---

## 2. STANDARDS DE CODE

### 2.1 C# Coding Standards

```csharp
// ✅ GOOD: Nommage clair et conventions C#
namespace SubExplore.Application.Spots.Commands
{
    public sealed class CreateSpotCommand : IRequest<Result<SpotDto>>
    {
        public string Name { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
    
    public sealed class CreateSpotCommandHandler 
        : IRequestHandler<CreateSpotCommand, Result<SpotDto>>
    {
        private readonly ISpotRepository _spotRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSpotCommandHandler> _logger;
        
        public CreateSpotCommandHandler(
            ISpotRepository spotRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateSpotCommandHandler> logger)
        {
            _spotRepository = spotRepository ?? throw new ArgumentNullException(nameof(spotRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Result<SpotDto>> Handle(
            CreateSpotCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validation
                var validationResult = await ValidateAsync(request, cancellationToken);
                if (!validationResult.IsSuccess)
                {
                    return Result<SpotDto>.Failure(validationResult.Error);
                }
                
                // Business logic
                var spot = Spot.Create(
                    request.Name,
                    new Location(request.Latitude, request.Longitude));
                
                // Persistence
                await _spotRepository.AddAsync(spot, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "Spot created successfully. Id: {SpotId}, Name: {SpotName}",
                    spot.Id, spot.Name);
                
                return Result<SpotDto>.Success(SpotDto.FromEntity(spot));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error creating spot with name: {SpotName}", 
                    request.Name);
                    
                return Result<SpotDto>.Failure("An error occurred while creating the spot");
            }
        }
        
        private async Task<Result> ValidateAsync(
            CreateSpotCommand request,
            CancellationToken cancellationToken)
        {
            // Validation logic
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure("Spot name is required");
            }
            
            if (request.Name.Length < 3 || request.Name.Length > 200)
            {
                return Result.Failure("Spot name must be between 3 and 200 characters");
            }
            
            if (!Location.IsValidCoordinates(request.Latitude, request.Longitude))
            {
                return Result.Failure("Invalid coordinates");
            }
            
            // Check for duplicates
            var exists = await _spotRepository.ExistsAsync(
                request.Name, 
                request.Latitude, 
                request.Longitude,
                cancellationToken);
                
            if (exists)
            {
                return Result.Failure("A spot already exists at this location");
            }
            
            return Result.Success();
        }
    }
}

// ❌ BAD: Mauvaises pratiques à éviter
public class spot_service  // Mauvais nommage
{
    private SpotRepository sr;  // Noms abrégés
    
    public void CreateSpot(string n, double lat, double lng)  // Paramètres abrégés
    {
        try
        {
            var s = new Spot();  // Variable peu claire
            s.Name = n;
            s.Lat = lat;  // Propriétés abrégées
            s.Lng = lng;
            sr.Add(s);
        }
        catch
        {
            // Swallow exception - JAMAIS!
        }
    }
}
```

### 2.2 XAML Standards

```xml
<!-- ✅ GOOD: XAML bien structuré -->
<ContentPage x:Class="SubExplore.Mobile.Views.SpotDetailPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SubExplore.Mobile.ViewModels"
             xmlns:controls="clr-namespace:SubExplore.Mobile.Controls"
             x:DataType="vm:SpotDetailViewModel"
             Title="{Binding Spot.Name}">
    
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderLabelStyle" TargetType="Label">
                <Setter Property="FontSize" Value="20"/>
                <Setter Property="FontAttributes" Value="Bold"/>
                <Setter Property="TextColor" Value="{StaticResource PrimaryColor}"/>
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    
    <ScrollView>
        <VerticalStackLayout Spacing="10" Padding="15">
            <!-- Header Section -->
            <Frame BackgroundColor="{StaticResource SecondaryColor}"
                   CornerRadius="10"
                   Padding="15"
                   HasShadow="True">
                <Grid ColumnDefinitions="*, Auto"
                      RowDefinitions="Auto, Auto">
                    <Label Grid.Row="0" Grid.Column="0"
                           Text="{Binding Spot.Name}"
                           Style="{StaticResource HeaderLabelStyle}"/>
                    
                    <controls:RatingView Grid.Row="0" Grid.Column="1"
                                       Rating="{Binding Spot.AverageRating}"
                                       IsReadOnly="True"/>
                    
                    <Label Grid.Row="1" Grid.Column="0"
                           Grid.ColumnSpan="2"
                           Text="{Binding Spot.Description}"
                           LineBreakMode="WordWrap"/>
                </Grid>
            </Frame>
            
            <!-- Actions -->
            <HorizontalStackLayout Spacing="10">
                <Button Text="Navigate"
                        Command="{Binding NavigateCommand}"
                        Style="{StaticResource PrimaryButtonStyle}"/>
                
                <Button Text="Add to Favorites"
                        Command="{Binding AddToFavoritesCommand}"
                        Style="{StaticResource SecondaryButtonStyle}"/>
            </HorizontalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>

<!-- ❌ BAD: XAML mal structuré -->
<ContentPage>
    <StackLayout>
        <Label Text="{Binding Name}" FontSize="20" TextColor="Blue" FontAttributes="Bold"/>
        <Label Text="{Binding Description}" FontSize="14" TextColor="Gray"/>
        <Button Text="Navigate" Clicked="OnNavigateClicked" BackgroundColor="Blue" TextColor="White"/>
    </StackLayout>
</ContentPage>
```

### 2.3 Async/Await Best Practices

```csharp
// ✅ GOOD: Utilisation correcte async/await
public class SpotService : ISpotService
{
    public async Task<SpotDto> GetSpotAsync(Guid id, CancellationToken ct = default)
    {
        // Toujours propager CancellationToken
        var spot = await _repository.GetByIdAsync(id, ct);
        
        if (spot == null)
        {
            throw new NotFoundException($"Spot with ID {id} not found");
        }
        
        // ConfigureAwait(false) pour library code
        var images = await _imageService
            .GetSpotImagesAsync(id, ct)
            .ConfigureAwait(false);
        
        return new SpotDto
        {
            Id = spot.Id,
            Name = spot.Name,
            Images = images
        };
    }
    
    // Parallel async operations
    public async Task<DashboardData> GetDashboardDataAsync(Guid userId, CancellationToken ct)
    {
        // Lancer toutes les tâches en parallèle
        var spotsTask = GetUserSpotsAsync(userId, ct);
        var bookingsTask = GetUserBookingsAsync(userId, ct);
        var reviewsTask = GetUserReviewsAsync(userId, ct);
        
        // Attendre toutes les tâches
        await Task.WhenAll(spotsTask, bookingsTask, reviewsTask);
        
        return new DashboardData
        {
            Spots = await spotsTask,
            Bookings = await bookingsTask,
            Reviews = await reviewsTask
        };
    }
}

// ❌ BAD: Mauvaise utilisation async/await
public class BadService
{
    // Async void = DANGER
    public async void LoadData()  // ❌ Never async void except event handlers
    {
        var data = await GetDataAsync();
    }
    
    // Blocking async code
    public Data GetData()
    {
        return GetDataAsync().Result;  // ❌ Deadlock risk!
    }
    
    // Not using CancellationToken
    public async Task<Data> GetDataAsync()  // ❌ No CancellationToken
    {
        return await _repository.GetAsync();
    }
}
```

---

## 3. WORKFLOW GIT

### 3.1 Branch Strategy

```yaml
Branch_Strategy:
  main:
    - Production-ready code
    - Protected branch
    - Requires PR + reviews
    
  develop:
    - Integration branch
    - Next release preparation
    - Daily merges from features
    
  feature/*:
    - New features
    - Named: feature/SUB-123-spot-creation
    - Branched from: develop
    - Merges to: develop
    
  bugfix/*:
    - Bug fixes
    - Named: bugfix/SUB-456-fix-login
    - Branched from: develop
    - Merges to: develop
    
  hotfix/*:
    - Production urgences
    - Named: hotfix/SUB-789-critical-fix
    - Branched from: main
    - Merges to: main + develop
    
  release/*:
    - Release preparation
    - Named: release/1.2.0
    - Branched from: develop
    - Merges to: main + develop
```

### 3.2 Commit Messages

```bash
# Format: <type>(<scope>): <subject>
#
# Types:
# - feat: Nouvelle fonctionnalité
# - fix: Correction de bug
# - docs: Documentation
# - style: Formatage (pas de changement de code)
# - refactor: Refactoring
# - perf: Amélioration performance
# - test: Ajout de tests
# - chore: Maintenance

# ✅ GOOD Examples:
feat(spots): add spot creation workflow
fix(auth): resolve token refresh issue
docs(api): update endpoint documentation
perf(map): optimize marker clustering
test(booking): add integration tests

# Commit avec description détaillée
git commit -m "feat(buddy): implement buddy matching algorithm

- Add compatibility scoring based on preferences
- Implement swipe gesture handling
- Create match notification system
- Add real-time chat for matches

Closes #123"

# ❌ BAD Examples:
"fix bug"           # Trop vague
"WIP"              # Non descriptif
"update code"      # Aucune info utile
"asdfgh"           # ???
```

### 3.3 Pull Request Process

```markdown
## PR Template

### 🎯 Objectif
Brief description of what this PR does

### 🔗 Issue
Closes #123

### ✅ Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] All tests passing
- [ ] No console.logs or debug code

### 📝 Type de changement
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

### 🧪 Tests effectués
- Unit tests: ✅
- Integration tests: ✅
- Manual testing: ✅
- Device tested: iPhone 14, Pixel 7

### 📸 Screenshots
[Si applicable]

### 🔍 Points d'attention pour la review
- Complex logic in `SpotService.cs` line 145-200
- New dependency added: `SkiaSharp`
```

---

## 4. PROCESSUS DE DÉVELOPPEMENT

### 4.1 Sprint Planning

```yaml
Sprint_Cycle: 2 semaines

Ceremonies:
  Sprint_Planning:
    When: Lundi matin (2h)
    Attendees: Toute l'équipe
    Output: Sprint backlog
    
  Daily_Standup:
    When: 9h30 (15min max)
    Format: Yesterday/Today/Blockers
    Location: Slack huddle ou présentiel
    
  Sprint_Review:
    When: Vendredi après-midi (1h)
    Attendees: Équipe + stakeholders
    Output: Demo + feedback
    
  Retrospective:
    When: Vendredi après-midi (1h)
    Format: Start/Stop/Continue
    Output: Action items

Estimation:
  Method: Story points (Fibonacci)
  Velocity: ~40 points/sprint (équipe de 4)
  
Story_Points:
  1: Trivial (< 2h)
  2: Simple (2-4h)
  3: Medium (1 jour)
  5: Complex (2-3 jours)
  8: Very complex (1 semaine)
  13: Epic (à découper)
```

### 4.2 Definition of Done

```yaml
Definition_of_Done:
  Code:
    ✅ Feature complete
    ✅ Code reviewed
    ✅ Follows coding standards
    ✅ No compiler warnings
    
  Tests:
    ✅ Unit tests written (>80% coverage)
    ✅ Integration tests if needed
    ✅ All tests passing
    ✅ Manual testing completed
    
  Documentation:
    ✅ Code commented
    ✅ API documented
    ✅ README updated if needed
    ✅ Changelog updated
    
  Quality:
    ✅ No critical SonarQube issues
    ✅ Performance validated
    ✅ Security reviewed
    ✅ Accessibility checked
    
  Deployment:
    ✅ Deployed to staging
    ✅ Smoke tests passed
    ✅ Product Owner approved
```

---

## 5. CODE REVIEW GUIDELINES

### 5.1 Review Checklist

```yaml
Code_Review_Checklist:
  Functionality:
    - ✅ Code does what it's supposed to do
    - ✅ Edge cases handled
    - ✅ Error handling appropriate
    - ✅ No regression introduced
    
  Design:
    - ✅ Follows architecture patterns
    - ✅ SOLID principles respected
    - ✅ DRY (Don't Repeat Yourself)
    - ✅ KISS (Keep It Simple)
    
  Performance:
    - ✅ No N+1 queries
    - ✅ Async/await used properly
    - ✅ No unnecessary allocations
    - ✅ Caching used where appropriate
    
  Security:
    - ✅ Input validation
    - ✅ SQL injection prevention
    - ✅ XSS prevention
    - ✅ Authentication/Authorization correct
    
  Maintainability:
    - ✅ Code is readable
    - ✅ Names are meaningful
    - ✅ Complex logic documented
    - ✅ Tests are clear
```

### 5.2 Review Comments

```csharp
// 🔴 Blocking - Must fix
// SECURITY: SQL injection vulnerability
var query = $"SELECT * FROM spots WHERE name = '{userInput}'"; // ❌ NEVER!

// 🟡 Major - Should fix
// PERFORMANCE: This will cause N+1 queries
foreach (var spot in spots)
{
    spot.Reviews = await _reviewRepository.GetBySpotIdAsync(spot.Id);
}
// Suggestion: Use Include() or load all reviews in one query

// 🟢 Minor - Consider fixing
// NAMING: Method name could be more descriptive
public List<Spot> GetData() { }
// Suggestion: GetActiveSpotsByUser() would be clearer

// 💭 Thought - Discussion point
// DESIGN: Consider extracting this to a service
// This could be reusable across multiple controllers

// ✨ Praise - Good work!
// NICE: Excellent error handling and logging here!
```

---

## 6. DOCUMENTATION

### 6.1 Code Documentation

```csharp
/// <summary>
/// Searches for diving spots within a specified radius from given coordinates.
/// </summary>
/// <param name="latitude">The latitude of the search center point.</param>
/// <param name="longitude">The longitude of the search center point.</param>
/// <param name="radiusKm">The search radius in kilometers. Default is 50km.</param>
/// <param name="filters">Optional filters to apply to the search results.</param>
/// <param name="cancellationToken">Cancellation token for the operation.</param>
/// <returns>A list of spots within the specified radius, ordered by distance.</returns>
/// <exception cref="ArgumentException">
/// Thrown when coordinates are invalid or radius is negative.
/// </exception>
/// <example>
/// <code>
/// var spots = await spotService.SearchNearbyAsync(
///     latitude: 48.8566,
///     longitude: 2.3522,
///     radiusKm: 25,
///     filters: new SpotFilters { Difficulty = DifficultyLevel.Beginner }
/// );
/// </code>
/// </example>
public async Task<List<SpotDto>> SearchNearbyAsync(
    double latitude,
    double longitude,
    double radiusKm = 50,
    SpotFilters filters = null,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### 6.2 API Documentation

```yaml
openapi: 3.0.0
info:
  title: SubExplore API
  version: 1.0.0
  description: API for SubExplore diving platform

paths:
  /api/v1/spots/nearby:
    get:
      summary: Search nearby diving spots
      tags:
        - Spots
      parameters:
        - name: latitude
          in: query
          required: true
          schema:
            type: number
            format: double
          description: Latitude of search center
          example: 48.8566
          
        - name: longitude
          in: query
          required: true
          schema:
            type: number
            format: double
          description: Longitude of search center
          example: 2.3522
          
        - name: radius
          in: query
          schema:
            type: number
            default: 50
          description: Search radius in kilometers
          
      responses:
        200:
          description: List of nearby spots
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/SpotDto'
```

---

## 7. TESTING STANDARDS

### 7.1 Unit Test Structure

```csharp
public class SpotServiceTests
{
    private readonly Mock<ISpotRepository> _mockRepository;
    private readonly Mock<ILogger<SpotService>> _mockLogger;
    private readonly SpotService _sut; // System Under Test
    
    public SpotServiceTests()
    {
        _mockRepository = new Mock<ISpotRepository>();
        _mockLogger = new Mock<ILogger<SpotService>>();
        _sut = new SpotService(_mockRepository.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetSpotAsync_WhenSpotExists_ShouldReturnSpot()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        var expectedSpot = new Spot
        {
            Id = spotId,
            Name = "Test Spot",
            Location = new Location(48.8566, 2.3522)
        };
        
        _mockRepository
            .Setup(x => x.GetByIdAsync(spotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpot);
        
        // Act
        var result = await _sut.GetSpotAsync(spotId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSpot.Name, result.Name);
        Assert.Equal(expectedSpot.Id, result.Id);
        
        _mockRepository.Verify(
            x => x.GetByIdAsync(spotId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetSpotAsync_WhenSpotDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        
        _mockRepository
            .Setup(x => x.GetByIdAsync(spotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Spot)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetSpotAsync(spotId));
    }
    
    [Theory]
    [InlineData(48.8566, 2.3522, 10)]
    [InlineData(-33.8688, 151.2093, 25)]
    [InlineData(40.7128, -74.0060, 50)]
    public async Task SearchNearbyAsync_WithValidCoordinates_ShouldReturnSpots(
        double latitude, double longitude, double radius)
    {
        // Arrange
        var expectedSpots = GenerateTestSpots(5);
        
        _mockRepository
            .Setup(x => x.SearchNearbyAsync(
                latitude, longitude, radius, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpots);
        
        // Act
        var result = await _sut.SearchNearbyAsync(latitude, longitude, radius);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSpots.Count, result.Count);
    }
}
```

### 7.2 Integration Tests

```csharp
public class SpotApiIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;
    
    public SpotApiIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateSpot_EndToEnd_ShouldCreateAndRetrieve()
    {
        // Arrange
        await AuthenticateAsync();
        
        var createRequest = new CreateSpotRequest
        {
            Name = $"Test Spot {Guid.NewGuid()}",
            Description = "Integration test spot",
            Latitude = 48.8566,
            Longitude = 2.3522,
            Difficulty = DifficultyLevel.Intermediate
        };
        
        // Act - Create
        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/spots", createRequest);
        
        // Assert - Creation
        createResponse.EnsureSuccessStatusCode();
        var createdSpot = await createResponse.Content
            .ReadFromJsonAsync<SpotDto>();
        
        Assert.NotNull(createdSpot);
        Assert.NotEqual(Guid.Empty, createdSpot.Id);
        
        // Act - Retrieve
        var getResponse = await _client.GetAsync(
            $"/api/v1/spots/{createdSpot.Id}");
        
        // Assert - Retrieval
        getResponse.EnsureSuccessStatusCode();
        var retrievedSpot = await getResponse.Content
            .ReadFromJsonAsync<SpotDto>();
        
        Assert.Equal(createdSpot.Id, retrievedSpot.Id);
        Assert.Equal(createRequest.Name, retrievedSpot.Name);
    }
}
```

---

## 8. COMMUNICATION D'ÉQUIPE

### 8.1 Canaux de Communication

```yaml
Communication_Channels:
  Slack:
    #general: Annonces équipe
    #dev: Discussions techniques
    #random: Off-topic
    #releases: Notes de version
    #monitoring: Alertes production
    #standup: Daily updates
    
  Meetings:
    Daily: Slack huddle ou présentiel
    Sprint: Zoom avec recording
    1-on-1: Présentiel préféré
    
  Documentation:
    Notion: Documentation équipe
    GitHub Wiki: Documentation technique
    Confluence: Documentation projet
    
  Emergency:
    PagerDuty: Incidents critiques
    WhatsApp: Groupe urgences
    Phone: Liste contacts équipe
```

### 8.2 Communication Guidelines

```yaml
Response_Times:
  Slack:
    Working_hours: < 2h
    After_hours: Next business day
    Urgent: Use @channel or phone
    
  Email:
    Internal: < 24h
    External: < 48h
    
  PR_Reviews:
    Small: < 4h
    Medium: < 1 day
    Large: < 2 days
    
Meeting_Etiquette:
  - Agenda envoyée 24h avant
  - Starts/ends on time
  - Camera on si remote
  - Mute quand pas en train de parler
  - Actions items documentés
  - Recording pour absents
  
Status_Updates:
  Format: |
    🟢 Yesterday: Completed user authentication
    🔵 Today: Working on spot creation UI
    🔴 Blockers: Need design for error states
    
  Emojis:
    ✅ Completed
    🚧 In progress
    🔴 Blocked
    🤔 Need help
    🎉 Shipped
    🐛 Bug found
    ♻️ Refactoring
```

### 8.3 Knowledge Sharing

```yaml
Knowledge_Sharing:
  Tech_Talks:
    Frequency: Bi-weekly
    Duration: 30-45 min
    Format: Présentation + Q&A
    Topics: New tech, lessons learned, deep dives
    
  Pair_Programming:
    When: Complex features, onboarding
    Tools: VS Code Live Share, Tuple
    
  Documentation_Days:
    Frequency: Monthly
    Focus: Update docs, write guides
    
  Post_Mortems:
    When: After incidents
    Format: Blameless
    Output: Action items
    
  Learning_Budget:
    Courses: 500€/year/person
    Conferences: 1-2/year
    Books: Unlimited
    Time: 10% for learning
```

---

## CONCLUSION

### Quick Reference Card

```yaml
Daily_Checklist:
  Morning:
    ✅ Check Slack
    ✅ Review assigned PRs
    ✅ Update Jira tickets
    ✅ Daily standup
    
  Before_Commit:
    ✅ Run tests locally
    ✅ Check linting
    ✅ Review own changes
    ✅ Update documentation
    
  Before_PR:
    ✅ Rebase on latest develop
    ✅ Squash WIP commits
    ✅ Fill PR template
    ✅ Assign reviewers
    
  End_of_Day:
    ✅ Push all changes
    ✅ Update ticket status
    ✅ Plan tomorrow
    ✅ Check CI/CD status
```

### Useful Commands

```bash
# Commandes fréquentes
alias gs='git status'
alias gp='git pull'
alias gc='git commit -m'
alias gco='git checkout'
alias gbr='git branch'

# Build & Test
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"

# Database
dotnet ef migrations add MigrationName
dotnet ef database update

# Mobile
dotnet build -t:Run -f net8.0-android
dotnet build -t:Run -f net8.0-ios
```

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Dernière mise à jour**: À chaque sprint
**Propriétaire**: Tech Lead

*"Code is read much more often than it is written" - Guido van Rossum*

*Bienvenue dans l'équipe SubExplore! 🤿*