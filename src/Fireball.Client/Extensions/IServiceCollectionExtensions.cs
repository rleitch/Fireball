using Fireball.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fireball.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFireballClient(this IServiceCollection services, Uri baseAddress, string functionKey)
        {
            services.AddHttpClient<IFireballClient, FireballClient>(client =>
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Add("x-functions-key", functionKey);
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddStandardResilienceHandler();
            return services;
        }
    }
}