

using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Authentication.Security;
using E_Commerce.Application.Authentication.Services;
using E_Commerce.Application.Caching;
using MediatR;
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

            Assembly assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));

            IMediator mediator = new Mediator(serviceProvider);
            CachingPlatformSettings cachingPlatformSettings = new CachingPlatformSettings(mediator);
            services.AddSingleton(cachingPlatformSettings);

            return services;
        }
    }
}
