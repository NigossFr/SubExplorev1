# API REST DOCUMENTATION - SUBEXPLORE
## Guide Complet de l'API Backend

---

## TABLE DES MATIÈRES

1. [Architecture API](#1-architecture-api)
2. [Authentication & Authorization](#2-authentication--authorization)
3. [Endpoints Documentation](#3-endpoints-documentation)
4. [Error Handling](#4-error-handling)
5. [Rate Limiting & Quotas](#5-rate-limiting--quotas)
6. [Webhooks & Events](#6-webhooks--events)
7. [GraphQL Alternative](#7-graphql-alternative)
8. [API Testing](#8-api-testing)

---

## 1. ARCHITECTURE API

### 1.1 Vue d'Ensemble

```yaml
API Architecture:
  Base URL: https://api.subexplore.app
  Version: v1
  Protocol: HTTPS only
  Format: JSON (application/json)
  
  Structure:
    /v1
      /auth         # Authentication endpoints
      /users        # User management
      /spots        # Diving spots
      /structures   # Clubs and centers
      /shops        # Dive shops
      /bookings     # Reservations
      /reviews      # Reviews and ratings
      /messages     # Messaging
      /buddy        # Buddy finder
      /community    # Community features
      /admin        # Administration
```

### 1.2 Configuration ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-API-Version")
    );
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SubExplore API",
        Version = "v1",
        Description = "API for SubExplore diving community platform",
        Contact = new OpenApiContact
        {
            Name = "SubExplore Team",
            Email = "api@subexplore.app",
            Url = new Uri("https://subexplore.app")
        }
    });
    
    // Security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("SubExplorePolicy", builder =>
    {
        builder
            .WithOrigins(
                "https://subexplore.app",
                "https://app.subexplore.app",
                "http://localhost:8081" // Dev only
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromHours(24));
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Specific endpoints limits
    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
        factory: partition => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(15)
        }));
});

// Compression
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("SubExplorePolicy");
app.UseResponseCompression();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.MapControllers();
app.Run();
```

### 1.3 Base Controller

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;
    protected readonly IMediator _mediator;
    
    protected BaseApiController(ILogger logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    protected Guid CurrentUserId => 
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
    
    protected string CurrentUserEmail => 
        User.FindFirst(ClaimTypes.Email)?.Value;
    
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value == null ? NotFound() : Ok(result.Value);
        }
        
        if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(result.Error);
        
        if (result.Error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
            return Unauthorized(result.Error);
        
        if (result.Error.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
            return Forbid(result.Error);
        
        return BadRequest(result.Error);
    }
    
    protected async Task<IActionResult> HandleCommand<T>(IRequest<Result<T>> command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
    
    protected async Task<IActionResult> HandleQuery<T>(IRequest<Result<T>> query)
    {
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}
```

---

## 2. AUTHENTICATION & AUTHORIZATION

### 2.1 Authentication Endpoints

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(UsersController.GetProfile),
                "Users",
                new { id = result.Value.UserId },
                result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [RateLimitPolicy("auth")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result.IsSuccess)
        {
            // Set refresh token in HTTP-only cookie
            Response.Cookies.Append("refresh_token", 
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            
            return Ok(new
            {
                result.Value.AccessToken,
                result.Value.ExpiresAt,
                result.Value.User
            });
        }
        
        return Unauthorized(result.Error);
    }
    
    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("No refresh token provided");
        
        var result = await _authService.RefreshTokenAsync(refreshToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return Unauthorized(result.Error);
    }
    
    /// <summary>
    /// Logout and invalidate tokens
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();
        
        await _authService.LogoutAsync(token);
        
        Response.Cookies.Delete("refresh_token");
        
        return NoContent();
    }
    
    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [RateLimitPolicy("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.RequestPasswordResetAsync(request.Email);
        
        // Always return success to prevent user enumeration
        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }
    
    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(
            request.Token, 
            request.NewPassword);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Password reset successfully" });
        }
        
        return BadRequest(result.Error);
    }
}

// Request/Response Models
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
    public string Password { get; set; }
    
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; }
    
    [Required]
    [MinLength(2)]
    public string LastName { get; set; }
    
    [Required]
    public DateTime BirthDate { get; set; }
}

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; }
}
```

### 2.2 JWT Configuration

```csharp
// JWT Service
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    
    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("account_type", user.AccountType.ToString()),
            new("permissions", user.Permissions.ToString())
        };
        
        // Add role claims
        foreach (var role in GetUserRoles(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
        return principal;
    }
}

// Authorization Policies
public static class AuthorizationPolicies
{
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        options.AddPolicy("RequireVerifiedEmail", policy =>
            policy.RequireClaim("email_verified", "true"));
        
        options.AddPolicy("RequireAdministrator", policy =>
            policy.RequireClaim("account_type", "Administrator"));
        
        options.AddPolicy("RequireModerator", policy =>
            policy.RequireClaim("account_type", "ExpertModerator", "Administrator"));
        
        options.AddPolicy("RequireProfessional", policy =>
            policy.RequireClaim("account_type", "VerifiedProfessional"));
        
        options.AddPolicy("RequireBuddyFinderAccess", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(18)));
        
        options.AddPolicy("CanValidateSpots", policy =>
            policy.Requirements.Add(new PermissionRequirement(Permission.ValidateSpots)));
    }
}

// Custom Authorization Handler
public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission RequiredPermission { get; }
    
    public PermissionRequirement(Permission permission)
    {
        RequiredPermission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissionsClaim = context.User.FindFirst("permissions");
        
        if (permissionsClaim != null && 
            int.TryParse(permissionsClaim.Value, out var permissions))
        {
            if ((permissions & (int)requirement.RequiredPermission) != 0)
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}
```

---

## 3. ENDPOINTS DOCUMENTATION

### 3.1 Spots Endpoints

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/spots")]
[Authorize]
public class SpotsController : BaseApiController
{
    /// <summary>
    /// Get nearby spots
    /// </summary>
    /// <param name="latitude">Center latitude</param>
    /// <param name="longitude">Center longitude</param>
    /// <param name="radius">Radius in kilometers (default: 50)</param>
    /// <param name="filters">Additional filters</param>
    /// <returns>List of spots within radius</returns>
    [HttpGet("nearby")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<SpotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearbySpots(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radius = 50,
        [FromQuery] SpotFilters? filters = null)
    {
        var query = new GetNearbySpotsQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radius,
            Filters = filters ?? new SpotFilters()
        };
        
        return await HandleQuery(query);
    }
    
    /// <summary>
    /// Get spot by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SpotDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpot(Guid id)
    {
        var query = new GetSpotByIdQuery { SpotId = id };
        return await HandleQuery(query);
    }
    
    /// <summary>
    /// Create a new spot
    /// </summary>
    [HttpPost]
    [RequirePermission(Permission.CreateSpots)]
    [ProducesResponseType(typeof(SpotDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSpot([FromBody] CreateSpotRequest request)
    {
        var command = _mapper.Map<CreateSpotCommand>(request);
        command.CreatorId = CurrentUserId;
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetSpot),
                new { id = result.Value.Id },
                result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Update a spot
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SpotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSpot(
        Guid id, 
        [FromBody] UpdateSpotRequest request)
    {
        var command = _mapper.Map<UpdateSpotCommand>(request);
        command.SpotId = id;
        command.UserId = CurrentUserId;
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Delete a spot
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSpot(Guid id)
    {
        var command = new DeleteSpotCommand
        {
            SpotId = id,
            UserId = CurrentUserId
        };
        
        await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>
    /// Upload images for a spot
    /// </summary>
    [HttpPost("{id:guid}/images")]
    [RequestSizeLimit(10_485_760)] // 10 MB
    [ProducesResponseType(typeof(List<ImageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImages(
        Guid id,
        [FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded");
        
        if (files.Count > 10)
            return BadRequest("Maximum 10 images allowed");
        
        var command = new UploadSpotImagesCommand
        {
            SpotId = id,
            UserId = CurrentUserId,
            Files = files
        };
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Validate a spot (moderators only)
    /// </summary>
    [HttpPost("{id:guid}/validate")]
    [Authorize(Policy = "RequireModerator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateSpot(
        Guid id,
        [FromBody] ValidateSpotRequest request)
    {
        var command = new ValidateSpotCommand
        {
            SpotId = id,
            ValidatorId = CurrentUserId,
            Status = request.Status,
            Notes = request.Notes
        };
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Add spot to favorites
    /// </summary>
    [HttpPost("{id:guid}/favorite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddToFavorites(Guid id)
    {
        var command = new AddToFavoritesCommand
        {
            UserId = CurrentUserId,
            EntityType = "spot",
            EntityId = id
        };
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Remove spot from favorites
    /// </summary>
    [HttpDelete("{id:guid}/favorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveFromFavorites(Guid id)
    {
        var command = new RemoveFromFavoritesCommand
        {
            UserId = CurrentUserId,
            EntityType = "spot",
            EntityId = id
        };
        
        await _mediator.Send(command);
        return NoContent();
    }
}

// Request/Response DTOs
public class SpotFilters
{
    public List<DifficultyLevel>? Difficulty { get; set; }
    public List<string>? Activities { get; set; }
    public List<WaterType>? WaterType { get; set; }
    public decimal? MaxDepth { get; set; }
    public decimal? MinVisibility { get; set; }
    public bool? HasEquipmentRental { get; set; }
    public AccessType? AccessType { get; set; }
}

public class CreateSpotRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(5000, MinimumLength = 50)]
    public string Description { get; set; }
    
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }
    
    [Required]
    public DifficultyLevel Difficulty { get; set; }
    
    [Required]
    public WaterType WaterType { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<string> Activities { get; set; }
    
    public SpotCharacteristics? Characteristics { get; set; }
    public SpotSafety? Safety { get; set; }
}

public class SpotDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public LocationDto Location { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public WaterType WaterType { get; set; }
    public List<string> Activities { get; set; }
    public double? Distance { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string CoverImageUrl { get; set; }
    public bool IsFavorite { get; set; }
    public SpotValidationStatus ValidationStatus { get; set; }
}
```

### 3.2 Booking Endpoints

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bookings")]
[Authorize]
public class BookingsController : BaseApiController
{
    /// <summary>
    /// Get user's bookings
    /// </summary>
    [HttpGet("my-bookings")]
    [ProducesResponseType(typeof(PagedResult<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBookings(
        [FromQuery] BookingStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetUserBookingsQuery
        {
            UserId = CurrentUserId,
            Status = status,
            Page = page,
            PageSize = pageSize
        };
        
        return await HandleQuery(query);
    }
    
    /// <summary>
    /// Create a booking
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var command = new CreateBookingCommand
        {
            CustomerId = CurrentUserId,
            StructureId = request.StructureId,
            ServiceType = request.ServiceType,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            ParticipantsCount = request.ParticipantsCount,
            CustomerNotes = request.CustomerNotes
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            // Send confirmation email
            _ = Task.Run(() => SendBookingConfirmationEmail(result.Value));
            
            return CreatedAtAction(
                nameof(GetBooking),
                new { id = result.Value.Id },
                result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Cancel a booking
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        [FromBody] CancelBookingRequest request)
    {
        var command = new CancelBookingCommand
        {
            BookingId = id,
            UserId = CurrentUserId,
            Reason = request.Reason
        };
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Get available slots for a service
    /// </summary>
    [HttpGet("availability")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AvailableSlotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] Guid structureId,
        [FromQuery] string serviceType,
        [FromQuery] DateTime date)
    {
        var query = new GetAvailableSlotsQuery
        {
            StructureId = structureId,
            ServiceType = serviceType,
            Date = date
        };
        
        return await HandleQuery(query);
    }
}
```

### 3.3 Messaging Endpoints

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/messages")]
[Authorize]
public class MessagesController : BaseApiController
{
    /// <summary>
    /// Get user's conversations
    /// </summary>
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations()
    {
        var query = new GetUserConversationsQuery
        {
            UserId = CurrentUserId
        };
        
        return await HandleQuery(query);
    }
    
    /// <summary>
    /// Get messages in a conversation
    /// </summary>
    [HttpGet("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(PagedResult<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetConversationMessagesQuery
        {
            ConversationId = conversationId,
            UserId = CurrentUserId,
            Page = page,
            PageSize = pageSize
        };
        
        return await HandleQuery(query);
    }
    
    /// <summary>
    /// Send a message
    /// </summary>
    [HttpPost("send")]
    [RateLimitPolicy("messaging")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        // Validate content
        if (ContainsInappropriateContent(request.Content))
        {
            return BadRequest("Message contains inappropriate content");
        }
        
        var command = new SendMessageCommand
        {
            SenderId = CurrentUserId,
            ConversationId = request.ConversationId,
            Content = SanitizeMessage(request.Content)
        };
        
        return await HandleCommand(command);
    }
    
    /// <summary>
    /// Start a new conversation
    /// </summary>
    [HttpPost("conversations")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var command = new StartConversationCommand
        {
            InitiatorId = CurrentUserId,
            ParticipantIds = request.ParticipantIds,
            InitialMessage = request.InitialMessage,
            ContextType = request.ContextType,
            ContextId = request.ContextId
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetMessages),
                new { conversationId = result.Value.Id },
                result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Mark messages as read
    /// </summary>
    [HttpPost("mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
    {
        var command = new MarkMessagesAsReadCommand
        {
            UserId = CurrentUserId,
            MessageIds = request.MessageIds
        };
        
        return await HandleCommand(command);
    }
}
```

---

## 4. ERROR HANDLING

### 4.1 Global Error Handler

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");
        
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse
        {
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
            Message = "An error occurred while processing your request"
        };
        
        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Validation failed";
                response.Errors = validationEx.Errors;
                break;
                
            case NotFoundException notFoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = notFoundEx.Message;
                break;
                
            case UnauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Unauthorized";
                break;
                
            case ForbiddenException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Forbidden";
                break;
                
            case ConflictException conflictEx:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = conflictEx.Message;
                break;
                
            case TooManyRequestsException:
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                response.Message = "Too many requests. Please try again later.";
                break;
                
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                
                if (_environment.IsDevelopment())
                {
                    response.Message = exception.Message;
                    response.Details = exception.StackTrace;
                }
                break;
        }
        
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string TraceId { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

### 4.2 Custom Exceptions

```csharp
public abstract class ApiException : Exception
{
    public int StatusCode { get; }
    
    protected ApiException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class ValidationException : ApiException
{
    public Dictionary<string, string[]> Errors { get; }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Validation failed", 400)
    {
        Errors = errors;
    }
    
    public ValidationException(string field, string error) 
        : base("Validation failed", 400)
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = new[] { error }
        };
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message = "Resource not found") 
        : base(message, 404) { }
    
    public NotFoundException(string resource, object key)
        : base($"{resource} with ID {key} not found", 404) { }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message = "Unauthorized") 
        : base(message, 401) { }
}

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message = "Forbidden") 
        : base(message, 403) { }
}

public class ConflictException : ApiException
{
    public ConflictException(string message) 
        : base(message, 409) { }
}

public class TooManyRequestsException : ApiException
{
    public TooManyRequestsException(string message = "Too many requests") 
        : base(message, 429) { }
}
```

---

## 5. RATE LIMITING & QUOTAS

### 5.1 Rate Limiting Configuration

```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly RateLimitOptions _options;
    
    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        IOptions<RateLimitOptions> options)
    {
        _next = next;
        _cache = cache;
        _options = options.Value;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var rateLimitAttribute = endpoint?.Metadata
            .GetMetadata<RateLimitAttribute>();
        
        if (rateLimitAttribute != null)
        {
            var key = GenerateKey(context, rateLimitAttribute);
            var limit = rateLimitAttribute.Limit;
            var period = rateLimitAttribute.Period;
            
            var requestCount = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.Add(period);
                return 0;
            });
            
            if (requestCount >= limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                
                // Add Retry-After header
                var retryAfter = _cache.Get<DateTime>($"{key}_reset");
                context.Response.Headers.Add("Retry-After", 
                    retryAfter.Subtract(DateTime.UtcNow).TotalSeconds.ToString());
                
                // Add rate limit headers
                context.Response.Headers.Add("X-RateLimit-Limit", limit.ToString());
                context.Response.Headers.Add("X-RateLimit-Remaining", "0");
                context.Response.Headers.Add("X-RateLimit-Reset", 
                    new DateTimeOffset(retryAfter).ToUnixTimeSeconds().ToString());
                
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }
            
            // Increment counter
            _cache.Set(key, requestCount + 1, period);
            
            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit", limit.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", 
                (limit - requestCount - 1).ToString());
        }
        
        await _next(context);
    }
    
    private string GenerateKey(HttpContext context, RateLimitAttribute attribute)
    {
        var identity = context.User?.Identity?.Name 
                      ?? context.Connection.RemoteIpAddress?.ToString() 
                      ?? "anonymous";
        
        return $"rate_limit:{attribute.Name}:{identity}";
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public string Name { get; set; }
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
    
    public RateLimitAttribute(string name, int limit, int periodSeconds)
    {
        Name = name;
        Limit = limit;
        Period = TimeSpan.FromSeconds(periodSeconds);
    }
}

// Usage
[HttpPost("login")]
[RateLimit("auth_login", 5, 900)] // 5 requests per 15 minutes
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // ...
}
```

### 5.2 API Quotas

```csharp
public class ApiQuotaService
{
    private readonly IQuotaRepository _quotaRepository;
    
    public async Task<bool> CheckQuotaAsync(Guid userId, QuotaType type)
    {
        var quota = await _quotaRepository.GetUserQuotaAsync(userId, type);
        
        if (quota == null)
        {
            // Create default quota
            quota = await CreateDefaultQuotaAsync(userId, type);
        }
        
        return quota.Used < quota.Limit;
    }
    
    public async Task<QuotaUsage> IncrementUsageAsync(Guid userId, QuotaType type)
    {
        var quota = await _quotaRepository.GetUserQuotaAsync(userId, type);
        
        if (quota.Used >= quota.Limit)
        {
            throw new QuotaExceededException(type);
        }
        
        quota.Used++;
        await _quotaRepository.UpdateAsync(quota);
        
        return new QuotaUsage
        {
            Type = type,
            Used = quota.Used,
            Limit = quota.Limit,
            ResetsAt = quota.ResetsAt
        };
    }
    
    private async Task<UserQuota> CreateDefaultQuotaAsync(Guid userId, QuotaType type)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        var limits = GetQuotaLimits(user.AccountType);
        
        return new UserQuota
        {
            UserId = userId,
            Type = type,
            Limit = limits[type],
            Used = 0,
            ResetsAt = GetResetTime(type)
        };
    }
    
    private Dictionary<QuotaType, int> GetQuotaLimits(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Standard => new Dictionary<QuotaType, int>
            {
                [QuotaType.SpotsPerMonth] = 5,
                [QuotaType.ImagesPerSpot] = 5,
                [QuotaType.MessagesPerDay] = 100,
                [QuotaType.BookingsPerMonth] = 10
            },
            AccountType.Premium => new Dictionary<QuotaType, int>
            {
                [QuotaType.SpotsPerMonth] = 20,
                [QuotaType.ImagesPerSpot] = 10,
                [QuotaType.MessagesPerDay] = 500,
                [QuotaType.BookingsPerMonth] = 50
            },
            AccountType.Professional => new Dictionary<QuotaType, int>
            {
                [QuotaType.SpotsPerMonth] = int.MaxValue,
                [QuotaType.ImagesPerSpot] = 20,
                [QuotaType.MessagesPerDay] = int.MaxValue,
                [QuotaType.BookingsPerMonth] = int.MaxValue
            },
            _ => throw new ArgumentException($"Unknown account type: {accountType}")
        };
    }
}
```

---

## 6. WEBHOOKS & EVENTS

### 6.1 Webhook System

```csharp
public class WebhookService
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookService> _logger;
    
    public async Task TriggerWebhooksAsync(WebhookEvent webhookEvent)
    {
        var webhooks = await _webhookRepository
            .GetActiveWebhooksAsync(webhookEvent.EventType);
        
        var tasks = webhooks.Select(webhook => 
            SendWebhookAsync(webhook, webhookEvent));
        
        await Task.WhenAll(tasks);
    }
    
    private async Task SendWebhookAsync(Webhook webhook, WebhookEvent webhookEvent)
    {
        try
        {
            var payload = new WebhookPayload
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Event = webhookEvent.EventType,
                Data = webhookEvent.Data
            };
            
            // Sign the payload
            var signature = GenerateSignature(payload, webhook.Secret);
            
            var request = new HttpRequestMessage(HttpMethod.Post, webhook.Url)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };
            
            request.Headers.Add("X-Webhook-Signature", signature);
            request.Headers.Add("X-Webhook-Event", webhookEvent.EventType);
            
            // Retry logic with exponential backoff
            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Webhook retry {RetryCount} after {Timespan}s for {Url}",
                            retryCount, timespan.TotalSeconds, webhook.Url);
                    });
            
            var response = await policy.ExecuteAsync(async () =>
                await _httpClient.SendAsync(request));
            
            // Log delivery
            await LogWebhookDeliveryAsync(webhook.Id, payload.Id, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send webhook to {Url}", webhook.Url);
            await MarkWebhookAsFailedAsync(webhook.Id);
        }
    }
    
    private string GenerateSignature(WebhookPayload payload, string secret)
    {
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(bytes);
        
        return Convert.ToBase64String(hash);
    }
}

// Webhook Events
public enum WebhookEventType
{
    // Spots
    SpotCreated,
    SpotUpdated,
    SpotValidated,
    SpotDeleted,
    
    // Bookings
    BookingCreated,
    BookingConfirmed,
    BookingCancelled,
    
    // Reviews
    ReviewSubmitted,
    ReviewApproved,
    
    // Users
    UserRegistered,
    UserVerified,
    UserDeleted,
    
    // Messaging
    MessageSent,
    ConversationStarted
}

// Webhook Registration Endpoint
[ApiController]
[Route("api/v1/webhooks")]
[Authorize(Policy = "RequireDeveloper")]
public class WebhooksController : BaseApiController
{
    /// <summary>
    /// Register a webhook
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterWebhook([FromBody] RegisterWebhookRequest request)
    {
        var command = new RegisterWebhookCommand
        {
            UserId = CurrentUserId,
            Url = request.Url,
            Events = request.Events,
            Secret = GenerateWebhookSecret()
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetWebhook),
                new { id = result.Value.Id },
                result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Test a webhook
    /// </summary>
    [HttpPost("{id:guid}/test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TestWebhook(Guid id)
    {
        await _webhookService.SendTestWebhookAsync(id);
        return Ok(new { message = "Test webhook sent" });
    }
}
```

---

## 7. GRAPHQL ALTERNATIVE

### 7.1 GraphQL Configuration

```csharp
// GraphQL Schema
public class SubExploreSchema : Schema
{
    public SubExploreSchema(IServiceProvider provider) : base(provider)
    {
        Query = provider.GetRequiredService<SubExploreQuery>();
        Mutation = provider.GetRequiredService<SubExploreMutation>();
        Subscription = provider.GetRequiredService<SubExploreSubscription>();
    }
}

// Query Type
public class SubExploreQuery : ObjectGraphType
{
    public SubExploreQuery(ISpotService spotService, IUserService userService)
    {
        Field<ListGraphType<SpotType>>(
            "nearbySpots",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "latitude" },
                new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "longitude" },
                new QueryArgument<FloatGraphType> { Name = "radius", DefaultValue = 50.0 }
            ),
            resolve: context =>
            {
                var lat = context.GetArgument<double>("latitude");
                var lon = context.GetArgument<double>("longitude");
                var radius = context.GetArgument<double>("radius");
                
                return spotService.GetNearbyAsync(lat, lon, radius);
            }
        );
        
        Field<SpotType>(
            "spot",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
            ),
            resolve: context =>
            {
                var id = context.GetArgument<Guid>("id");
                return spotService.GetByIdAsync(id);
            }
        );
        
        Field<UserType>(
            "me",
            resolve: context =>
            {
                var userId = context.User?.GetUserId();
                return userId.HasValue ? userService.GetByIdAsync(userId.Value) : null;
            }
        ).AuthorizeWith("Authenticated");
    }
}

// Mutation Type
public class SubExploreMutation : ObjectGraphType
{
    public SubExploreMutation(ISpotService spotService)
    {
        Field<SpotType>(
            "createSpot",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<SpotInputType>> { Name = "input" }
            ),
            resolve: context =>
            {
                var input = context.GetArgument<CreateSpotInput>("input");
                var userId = context.User.GetUserId();
                
                return spotService.CreateAsync(input, userId.Value);
            }
        ).AuthorizeWith("Authenticated");
    }
}

// Subscription Type
public class SubExploreSubscription : ObjectGraphType
{
    public SubExploreSubscription(IMessageService messageService)
    {
        AddField(new EventStreamFieldType
        {
            Name = "messageReceived",
            Type = typeof(MessageType),
            Resolver = new FuncFieldResolver<Message>(context => context.Source as Message),
            Subscriber = new EventStreamResolver<Message>(context =>
            {
                var conversationId = context.GetArgument<Guid>("conversationId");
                return messageService.SubscribeToMessages(conversationId);
            }),
            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "conversationId" }
            )
        });
    }
}

// GraphQL Controller
[ApiController]
[Route("api/graphql")]
public class GraphQLController : Controller
{
    private readonly IDocumentExecuter _documentExecuter;
    private readonly ISchema _schema;
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
    {
        var inputs = query.Variables?.ToInputs();
        
        var executionOptions = new ExecutionOptions
        {
            Schema = _schema,
            Query = query.Query,
            Inputs = inputs,
            User = User,
            ValidationRules = new[] { new AuthorizationValidationRule() }
        };
        
        var result = await _documentExecuter.ExecuteAsync(executionOptions);
        
        if (result.Errors?.Count > 0)
        {
            return BadRequest(result);
        }
        
        return Ok(result);
    }
}
```

---

## 8. API TESTING

### 8.1 Integration Tests

```csharp
public class SpotsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public SpotsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override database with in-memory
                services.RemoveAll<DbContext>();
                services.AddDbContext<SubExploreContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task GetNearbySpots_ReturnsSuccessStatusCode()
    {
        // Arrange
        var url = "/api/v1/spots/nearby?latitude=48.8566&longitude=2.3522&radius=50";
        
        // Act
        var response = await _client.GetAsync(url);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<SpotDto>>(content);
        
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
    }
    
    [Fact]
    public async Task CreateSpot_RequiresAuthentication()
    {
        // Arrange
        var request = new CreateSpotRequest
        {
            Name = "Test Spot",
            Description = "Test description",
            Latitude = 48.8566,
            Longitude = 2.3522
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/spots", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateSpot_WithValidData_ReturnsCreated()
    {
        // Arrange
        await AuthenticateAsync();
        
        var request = new CreateSpotRequest
        {
            Name = "Beautiful Diving Spot",
            Description = "A wonderful place for diving with crystal clear water and abundant marine life",
            Latitude = 48.8566,
            Longitude = 2.3522,
            Difficulty = DifficultyLevel.Intermediate,
            WaterType = WaterType.Sea,
            Activities = new List<string> { "Scuba", "Snorkeling" }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/spots", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var spot = JsonSerializer.Deserialize<SpotDto>(content);
        
        Assert.NotNull(spot);
        Assert.Equal(request.Name, spot.Name);
        Assert.NotEqual(Guid.Empty, spot.Id);
    }
    
    private async Task AuthenticateAsync()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Test123!"
        };
        
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content);
        
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);
    }
}
```

### 8.2 Load Testing

```csharp
// NBomber Load Test
public class ApiLoadTests
{
    [Fact]
    public void Api_Should_Handle_Load()
    {
        var scenario = Scenario.Create("api_load_test", async context =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.subexplore.app")
            };
            
            // Test different endpoints
            var endpoints = new[]
            {
                "/api/v1/spots/nearby?latitude=48.8566&longitude=2.3522",
                "/api/v1/structures",
                "/api/v1/community/posts"
            };
            
            var endpoint = endpoints[Random.Next(endpoints.Length)];
            
            var response = await client.GetAsync(endpoint);
            
            return response.IsSuccessStatusCode 
                ? Response.Ok(statusCode: (int)response.StatusCode) 
                : Response.Fail(statusCode: (int)response.StatusCode);
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Assertions
        Assert.True(stats.AllOkCount > 0);
        Assert.True(stats.ScenarioStats[0].Ok.Latency.Mean < 1000); // < 1s average
        Assert.True(stats.ScenarioStats[0].Ok.Latency.P99 < 3000); // < 3s for 99%
        Assert.True(stats.ScenarioStats[0].Fail.Count < stats.AllOkCount * 0.01); // < 1% errors
    }
}
```

---

## CONCLUSION

Cette documentation API complète fournit:

- **Architecture robuste** avec ASP.NET Core
- **Sécurité complète** avec JWT et politiques d'autorisation
- **Documentation OpenAPI** pour tous les endpoints
- **Gestion des erreurs** cohérente
- **Rate limiting** et quotas
- **Tests complets** pour assurer la qualité
- **Support GraphQL** optionnel

### Points Clés

1. **Versioning** systématique de l'API
2. **Authentication** sécurisée avec refresh tokens
3. **Validation** robuste des entrées
4. **Monitoring** et logging complets
5. **Performance** optimisée avec cache et pagination

---

**Document créé le**: {{DATE}}
**Version**: 1.0
**Statut**: Documentation API de référence
**OpenAPI Spec**: Available at `/swagger`

*Cette documentation doit être maintenue à jour avec chaque modification de l'API.*