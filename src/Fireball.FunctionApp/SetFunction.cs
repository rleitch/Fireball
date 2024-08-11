using Fireball.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using Fireball.Common.Extensions;
using Fireball.FunctionApp.Constants;

namespace Fireball.FunctionApp
{
    public class SetFunction(ILogger<SetFunction> logger, IDistributedCache cache)
    {
        private readonly ILogger<SetFunction> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        [Function("SetFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{key}")] HttpRequest req, string key)
        {
            key = Uri.UnescapeDataString(key);
            _logger.LogInformation("POST {0}", key);
            DistributedCacheEntryOptions options = new();

            if (req.Query.TryGetValue(QueryStringKeys.AbsoluteExpiration, out var absoluteExpirationValue)
                && TimeSpan.TryParse(Uri.UnescapeDataString(absoluteExpirationValue), out TimeSpan absoluteExpiration))
            {
                options.SetAbsoluteExpiration(absoluteExpiration);
            }

            if (req.Query.TryGetValue(QueryStringKeys.SlidingExpiration, out var slidingExpirationValue)
                && TimeSpan.TryParse(Uri.UnescapeDataString(slidingExpirationValue), out TimeSpan slidingExpiration))
            {
                options.SetSlidingExpiration(slidingExpiration);
            }

            byte[] requestBody;
            using (var ms = new MemoryStream())
            {
                await req.Body.CopyToAsync(ms);
                requestBody = ms.ToArray();
            }

            await SetAsync(key, requestBody, options);
            return new AcceptedResult();
        }

        public async Task SetAsync(string key, byte[] uncompressedData, DistributedCacheEntryOptions options)
        {
            byte[] data = uncompressedData.Length > 1024
                ? uncompressedData.CompressBytes().Prepend(CompressionFlags.Compressed).ToArray()
                : uncompressedData.Prepend(CompressionFlags.Uncompressed).ToArray();
            await _cache.SetAsync(key, data, options);
        }
    }
}
