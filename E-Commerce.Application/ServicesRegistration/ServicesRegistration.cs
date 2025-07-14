

using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Authentication.Security;
using E_Commerce.Application.Authentication.Services;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Application.Services.SettingsManagement.Queries;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace E_Commerce.Application.ServicesRegistration
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

            return services;
        }
    }
}
