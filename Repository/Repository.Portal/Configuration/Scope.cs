using Microsoft.Extensions.DependencyInjection;

namespace Repository.Portal.Configuration
{
    public static class ScopeRepository
    {
        public static IServiceCollection AddPortalScope(this IServiceCollection services)
        {
            services.AddScoped<IAppUser, AppUserRepository>();
            return services;
        }
    }
}
