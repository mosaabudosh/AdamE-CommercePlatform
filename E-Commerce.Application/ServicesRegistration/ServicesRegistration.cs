

using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Authentication.Security;
using E_Commerce.Application.Authentication.Services;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace E_Commerce.Application.ServicesRegistration
{
    public static class ServicesRegistration
    {
        public static IServiceCollection ConfigureAdamService(this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ITokenHandler, TokenHandler>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddScoped<IActionManagementService, ActionManagementService>();

            return services;
        }
    }
}
