﻿using Fireball.Common;
using Fireball.Common.Extensions;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fireball.Client
{
    public class FireballClient : IFireballClient
    {
        private readonly HttpClient _httpClient;

        public FireballClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            await _httpClient.DeleteAsync(key, cancellationToken);
        }

        public async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetStringAsync(key, cancellationToken);
        }

        public async Task<T?> GetFromJsonAsync<T>(string key, JsonSerializerOptions jsonSerializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<T>(key, jsonSerializerOptions, cancellationToken);
        }

        public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            await _httpClient.PutAsync(key, null, cancellationToken);
        }

        public async Task SetAsync<T>(
            string key, 
            T value, 
            TimeSpan? absoluteExpiration = null, 
            TimeSpan? slidingExpiration = null,
            JsonSerializerOptions jsonSerializerOptions = null, 
            CancellationToken cancellationToken = default)
        {
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

            await _httpClient.PostAsJsonAsync(key, value, jsonSerializerOptions, cancellationToken);
        }
    }
}