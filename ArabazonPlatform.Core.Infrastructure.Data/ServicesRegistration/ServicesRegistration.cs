

using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Infrastructure.Data.Context;
using ArabazonPlatform.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArabazonPlatform.Infrastructure.Data.ServicesRegistration
{
    public static class ServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureDataService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ArabazonPlatformContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
