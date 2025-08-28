using Fireball.Client.Configuration;
using Fireball.Client.Services;
using Fireball.Common;
using Fireball.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fireball.Client
{
    public class FireballClient(HttpClient httpClient, IFireballClientSettings settings) : IFireballClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task DeleteAsync(CancellationToken cancellationToken, params string[] keyParts)
        {
            var key = KeyService.BuildKey([settings.ApiKey, ..keyParts]);

            var response = await _httpClient.DeleteAsync(key, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetAsync(CancellationToken cancellationToken, params string[] keyParts)
        {
            var key = KeyService.BuildKey([settings.ApiKey, .. keyParts]);

            return await _httpClient.GetStringAsync(key, cancellationToken);
        }

        public async Task<T> GetAsync<T>(CancellationToken cancellationToken, params string[] keyParts)
            => await GetAsync<T>(null, cancellationToken, keyParts);

        public async Task<T> GetAsync<T>(JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken, params string[] keyParts)
        {
            try
            {
                var key = KeyService.BuildKey([settings.ApiKey, .. keyParts]);

                return await _httpClient.GetFromJsonAsync<T>(key, jsonSerializerOptions, cancellationToken);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        public async Task RefreshAsync(CancellationToken cancellationToken, params string[] keyParts)
        {
            var key = KeyService.BuildKey([settings.ApiKey, .. keyParts]);

            var response = await _httpClient.PutAsync(key, null, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts)
        {
            await SetAsync(value, absoluteExpiration, null, cancellationToken, keyParts);
        }

        public async Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts)
        {
            await SetAsync(value, absoluteExpiration, slidingExpiration, null, cancellationToken, keyParts);
        }

        public async Task SetAsync<T>(
            T value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken,
            params string[] keyParts)
        {
            var key = BuildSetKey(absoluteExpiration, slidingExpiration, keyParts);

            var response = await _httpClient.PostAsJsonAsync(key, value, jsonSerializerOptions, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task SetStringAsync(
            string value,
            TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts)
            => await SetStringAsync(value, absoluteExpiration, null, cancellationToken, keyParts);

        public async Task SetStringAsync(
            string value,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            CancellationToken cancellationToken,
            params string[] keyParts)
        {
            var key = BuildSetKey(absoluteExpiration, slidingExpiration, keyParts);

            using var content = new StringContent(value);

            var response = await _httpClient.PostAsync(key, content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        private string BuildSetKey(
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            params string[] keyParts)
        {
            var key = KeyService.BuildKey([settings.ApiKey, .. keyParts]);

            var queryStringParameters = new HashSet<string>();

            if (absoluteExpiration.HasValue)
            {
                queryStringParameters.Add(absoluteExpiration.Value.ToQueryString(QueryStringKeys.AbsoluteExpiration));
            }

            if (slidingExpiration.HasValue)
            {
                queryStringParameters.Add(slidingExpiration.Value.ToQueryString(QueryStringKeys.SlidingExpiration));
            }

            if (queryStringParameters.Count > 0)
            {
                key = $"{key}?{string.Join("&", queryStringParameters)}";
            }

            return key;
        }
    }
}