using Fireball.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fireball.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFireballClient(this IServiceCollection services, IFireballClientSettings fireballClientSettings)
        {
            services.AddHttpClient<IFireballClient, FireballClient>(client =>
            {
                client.BaseAddress = fireballClientSettings.BaseAddress;
            });
            return services;
        }
    }
}