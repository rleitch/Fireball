using Fireball.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            var empty = await fireballClient.GetAsync(randomCacheKey);

            // Set string
            await fireballClient.SetStringAsync(
                randomCacheKey, 
                "When you make it to the top, turn and reach down for the person behind you. - Abraham Lincoln");

            // Cache not empty anymore
            var quote = await fireballClient.GetAsync(randomCacheKey);

            // Touch the record to reset the sliding expiration window
            await fireballClient.RefreshAsync(randomCacheKey);

            // Delete from cache
            await fireballClient.DeleteAsync(randomCacheKey);


        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddFireballClient(new Uri("https://fireballfunctionapp.azurewebsites.net/api/"));
            });
    }
}