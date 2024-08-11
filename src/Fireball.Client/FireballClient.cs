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
    public class FireballClient(HttpClient httpClient) : IFireballClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(Uri.EscapeDataString(key), cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetStringAsync(Uri.EscapeDataString(key), cancellationToken);
        }

        public async Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonSerializerOptions = null, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(Uri.EscapeDataString(key), jsonSerializerOptions, cancellationToken);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync(Uri.EscapeDataString(key), null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetAsync<T>(
            string key, 
            T value, 
            TimeSpan? absoluteExpiration = null, 
            TimeSpan? slidingExpiration = null,
            JsonSerializerOptions jsonSerializerOptions = null, 
            CancellationToken cancellationToken = default)
        {
            key = Uri.EscapeDataString(key);
            var queryStringParameters = new HashSet<string>();
            if (absoluteExpiration.HasValue)
            {
                queryStringParameters.Add(absoluteExpiration.Value.ToQueryString(QueryStringKeys.AbsoluteExpiration));
            }

            if (slidingExpiration.HasValue)
            {
                queryStringParameters.Add(slidingExpiration.Value.ToQueryString(QueryStringKeys.SlidingExpiration));
            }

            if(queryStringParameters.Count > 0)
            {
                key = $"{key}?{string.Join("&", queryStringParameters)}";
            }

            var response = await _httpClient.PostAsJsonAsync(key, value, jsonSerializerOptions, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
        {
            key = Uri.EscapeDataString(key);
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

            using var content = new StringContent(value);
            var response = await _httpClient.PostAsync(key, content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}