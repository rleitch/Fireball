using Fireball.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fireball.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFireballClient(this IServiceCollection services, IFireballClientSettings settings)
        {
            services.TryAddSingleton(settings);
            services.AddHttpClient<IFireballClient, FireballClient>(client =>
            {
                client.BaseAddress = settings.BaseAddress;
                client.DefaultRequestHeaders.Add("x-functions-key", settings.ApiKey);
            }).AddStandardResilienceHandler();
            return services;
        }
    }
}