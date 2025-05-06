

using E_Commerce.Domain.RepositoryInterfaces;
using E_Commerce.Infrastructure.Data.Context;
using E_Commerce.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Infrastructure.Data.ServicesRegistration
{
    public static class ServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureDataService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<E_CommerceContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
