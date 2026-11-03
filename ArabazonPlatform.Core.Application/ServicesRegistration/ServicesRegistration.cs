

using ArabazonPlatform.Application.Authentication.Interfaces;
using ArabazonPlatform.Application.Authentication.Security;
using ArabazonPlatform.Application.Authentication.Services;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Application.Services.SettingsManagement.Queries;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace ArabazonPlatform.Application.ServicesRegistration
{
    public static class ServicesRegistration
    {
        public static IServiceCollection ConfigureAdamService(this IServiceCollection services, ServiceProvider serviceProvider)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ITokenHandler, TokenHandler>();
            services.AddSignalR();
            Assembly assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));
            IUnitOfWork unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            CachingPlatformSettings cachingPlatformSettings = new CachingPlatformSettings(unitOfWork);
            services.AddSingleton(cachingPlatformSettings);
            CachingDataFromDMS cachingDataFromDMS = new CachingDataFromDMS(unitOfWork);
            services.AddSingleton(cachingDataFromDMS);

            return services;
        }
    }
}
