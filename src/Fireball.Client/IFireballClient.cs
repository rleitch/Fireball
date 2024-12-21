using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fireball.Client
{
    public interface IFireballClient
    {
        Task DeleteAsync(CancellationToken cancellationToken, params string[] keyParts);

        Task<string> GetAsync(CancellationToken cancellationToken, params string[] keyParts);

        Task<T> GetAsync<T>(CancellationToken cancellationToken, params string[] keyParts);

        Task<T> GetAsync<T>(JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken, params string[] keyParts);

        Task RefreshAsync(CancellationToken cancellationToken, params string[] keyParts);

        Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts);

        Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts);

        Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken,
            params string[] keyParts);

        Task SetStringAsync(
            string value,
            TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts);

        Task SetStringAsync(
            string value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts);
    }
}