using Adam.Generic.Models;
using Adam.Logging;
using E_Commerce.API;
using E_Commerce.API.Middleware;
using E_Commerce.API.SwaggerConfiguration;
using E_Commerce.Application.Authentication.Security;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.AppSettings;
using E_Commerce.Application.Services.NotificationManagement;
using E_Commerce.Application.ServicesRegistration;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using E_Commerce.Infrastructure.Data.ServicesRegistration;
using E_Commerce.Infrastructure.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers(p =>
    {
        p.ValueProviderFactories.Insert(0, new CustomQueryStringParserFactory());
        AuthorizationPolicy authorizationPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        p.Filters.Add(new AuthorizeFilter(authorizationPolicy));
    }).AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

    AppSettings appSettings = builder.Configuration.Get<AppSettings>();
    string[] origins = appSettings.origins.Split(',');

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Policy", corsBuilder => corsBuilder
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
    });

    // ─── Rate Limiting ───────────────────────────────────────────────────────────
    // Protects auth-sensitive endpoints (login, OTP) from brute-force attacks.
    builder.Services.AddRateLimiter(options =>
    {
        // Sliding window: max 10 requests per minute per IP on auth endpoints
        options.AddSlidingWindowLimiter("AuthPolicy", limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.SegmentsPerWindow = 4;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 0;
        });

        // General API limiter: 200 req/min per IP
        options.AddFixedWindowLimiter("GeneralPolicy", limiterOptions =>
        {
            limiterOptions.PermitLimit = 200;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueLimit = 0;
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    builder.Services.AddAdamLoggingService(builder.Configuration);
    builder.Services.ConfigureInfrastructureDataService(builder.Configuration);
    builder.Services.AddAdamInfrastructureService();

    ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
    IUserAccessor userAccessor = serviceProvider.GetRequiredService<IUserAccessor>();
    builder.Services.ConfigureAdamService(serviceProvider);
    IServiceScope serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

    builder.Services.Configure<TokenOptions>(options =>
        builder.Configuration.GetSection("TokenOptions").Bind(options));

    SigningCredentialsConfiguration signingConfigurations = new SigningCredentialsConfiguration();
    builder.Services.AddSingleton(signingConfigurations);

    TokenOptions tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.Events = new JwtBearerEvents
        {
            // ─── FIXED: Validate user is still active on every authenticated request ──
            OnTokenValidated = async context =>
            {
                try
                {
                    ClaimsIdentity? identity = context.Principal?.Identity as ClaimsIdentity;

                    if (identity is null || !identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    {
                        context.Fail("Unauthorized: missing identity claims.");
                        return;
                    }

                    string? userIdStr = identity.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (!Guid.TryParse(userIdStr, out Guid userId))
                    {
                        context.Fail("Unauthorized: invalid user identifier.");
                        return;
                    }

                    // Resolve a scoped UnitOfWork — never use the root provider for
                    // scoped services inside an event callback.
                    IServiceScope scope = context.HttpContext.RequestServices
                        .GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();

                    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Pull the user — IsActive check prevents soft-deleted / locked accounts.
                    var userQuery = unitOfWork.GetRepository<E_Commerce.Domain.Domains.Authorisation.User>()
                        .GetByExpressionAsNoTracking(u => u.UserId == userId);

                    var user = await unitOfWork.GetRepository<E_Commerce.Domain.Domains.Authorisation.User>()
                        .FirstOrDefaultAsync(userQuery);

                    if (user is null || !user.IsActive)
                    {
                        context.Fail("Unauthorized: account is inactive or does not exist.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Never swallow silently — log and reject.
                    Startup.Log("OnTokenValidated", ex);
                    context.Fail("Unauthorized: token validation failed.");
                }
            },

            OnAuthenticationFailed = context =>
            {
                context.Fail("Unauthorized");
                return Task.CompletedTask;
            },
        };

        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            RequireExpirationTime = true,
            IssuerSigningKey = signingConfigurations.Key,
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ARABAZON Platform APIs", Version = "v1" });
        c.DocumentFilter<SpecificControllerFilter>();
        c.OperationFilter<AddRequiredHeaderParameter>();
        c.SchemaFilter<IgnoreRecursiveSchemaFilter>();
        c.IgnoreObsoleteActions();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Format: 'Bearer YOUR_TOKEN'",
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                },
                new List<string>()
            }
        });
    });

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1));
    }

    app.UseCors("Policy");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseRateLimiter();

    Startup.UpdateDatabase(serviceScope);

    app.UseMiddleware<Interceptor>(builder.Configuration, userAccessor, appSettings);

    app.MapControllers();
    app.MapHub<NotificationHub>("/hubs/notification");

    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();

    app.Run();
}
catch (Exception ex)
{
    while (ex.InnerException != null) ex = ex.InnerException;
    Startup.Log("Program File", ex);
}
