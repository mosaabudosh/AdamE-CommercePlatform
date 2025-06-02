

using Adam.Generic.Models;
using Adam.Logging;
using E_Commerce.API;
using E_Commerce.API.Middleware;
using E_Commerce.Application.Authentication.Security;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.AppSettings;
using E_Commerce.Application.ServicesRegistration;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using E_Commerce.Infrastructure.Data.ServicesRegistration;
using E_Commerce.Infrastructure.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PdfSharp.Charting;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers(p =>
    {
        p.ValueProviderFactories.Insert(0, new CustomQueryStringParserFactory());
        AuthorizationPolicy puthorizationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        p.Filters.Add(new AuthorizeFilter(puthorizationPolicy));
    }).AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    AppSettings appSettings = builder.Configuration.Get<AppSettings>();
    string[] origins = appSettings.origins.Split(',');
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Policy", builder => builder
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
    });

    builder.Services.AddAdamLoggingService(builder.Configuration);
    builder.Services.ConfigureInfrastructureDataService(builder.Configuration);
    builder.Services.AddAdamInfrastructureService();

    ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
    IUserAccessor userAccessor = serviceProvider.GetRequiredService<IUserAccessor>();
    builder.Services.ConfigureAdamService(serviceProvider);

    builder.Services.Configure<TokenOptions>(options => builder.Configuration.GetSection("TokenOptions").Bind(options));

    SigningCredentialsConfiguration signingConfigurations = new SigningCredentialsConfiguration();
    builder.Services.AddSingleton(signingConfigurations);

    TokenOptions tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                //IUserService userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                //ClaimsIdentity userPrincipal = (ClaimsIdentity)context.Principal.Identity;
                //if (userPrincipal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                //{
                //    //int userId = int.Parse(userPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                //    //BaseResponse<User> user = userService.GetUserById(userId).Result;
                //    //if (user == null || (user != null && user.IsSuccess == false) || (user != null && user.DataObject != null && user.DataObject.IsActive == false))
                //    //{
                //    //    context.Fail("Unauthorized");
                //    //}
                //}
                return Task.CompletedTask;
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

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseCors("Policy");

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseMiddleware<Interceptor>(builder.Configuration, userAccessor, appSettings);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    while (ex.InnerException != null) ex = ex.InnerException;
    Startup.Log("Program File", ex);
}
