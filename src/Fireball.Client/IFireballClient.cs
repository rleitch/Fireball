using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fireball.Client
{
    public interface IFireballClient
    {
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        Task<string> GetAsync(string key, CancellationToken cancellationToken = default);

        Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonSerializerOptions = null, CancellationToken cancellationToken = default);

        Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null,
            JsonSerializerOptions jsonSerializerOptions = null,
            CancellationToken cancellationToken = default);

        Task SetStringAsync(
            string key,
            string value,
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null,
            CancellationToken cancellationToken = default);
    }
}