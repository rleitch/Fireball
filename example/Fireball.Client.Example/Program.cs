using Fireball.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fireball.Client.Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var fireballClient = host.Services.GetRequiredService<IFireballClient>();

            var randomCacheKey = Path.GetRandomFileName();

            // Cache empty
            var empty = await fireballClient.GetAsync(CancellationToken.None, randomCacheKey);

            // Set string

            await fireballClient.SetStringAsync(
                "When you make it to the top, turn and reach down for the person behind you. - Abraham Lincoln",
                null,
                CancellationToken.None,
                randomCacheKey);

            // Cache not empty anymore
            var quote = await fireballClient.GetAsync(CancellationToken.None, randomCacheKey);

            // Touch the record to reset the sliding expiration window
            await fireballClient.RefreshAsync(CancellationToken.None, randomCacheKey);

            // Delete from cache
            await fireballClient.DeleteAsync(CancellationToken.None, randomCacheKey);

        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddFireballClient(new Uri("https://fireballfunctionapp.azurewebsites.net/api/"), string.Empty);
            });
    }
}