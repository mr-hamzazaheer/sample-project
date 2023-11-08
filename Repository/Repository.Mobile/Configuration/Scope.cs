using Microsoft.Extensions.DependencyInjection;

namespace Repository.Mobile.Configuration
{
    public static class ScopeRepository
    {
        public static IServiceCollection AddMobileScope(this IServiceCollection services)
        {
            services.AddScoped<IAppUser, AppUserRepository>();
            return services;
        }
    }
}
