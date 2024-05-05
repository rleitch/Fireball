using Fireball.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fireball.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFireballClient(this IServiceCollection services, Uri baseAddress)
        {
            services.AddHttpClient<IFireballClient, FireballClient>(client =>
            {
                client.BaseAddress = baseAddress;
            }).AddStandardResilienceHandler();
            return services;
        }
    }
}