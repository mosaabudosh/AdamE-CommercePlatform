using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Infrastructure.HttpServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArabazonPlatform.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAdamInfrastructureService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IHttpAccessor, HttpAccessor>();

            return services;
        }
    }
}
