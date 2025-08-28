using Fireball.Common.Constants;
using Fireball.Common.Extensions;
using Fireball.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fireball.FunctionApp
{
    public class SetFunction(ILogger<SetFunction> logger, IDistributedCache cache)
    {
        private readonly ILogger<SetFunction> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        [Function("SetFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{key}")] HttpRequest req, string key)
        {
            _logger.LogInformation("POST {0}", key);
            DistributedCacheEntryOptions options = new();

            if (req.Query.TryGetValue(QueryStringKeys.AbsoluteExpiration, out var absoluteExpirationString)
                && long.TryParse(absoluteExpirationString, out long absoluteExpiration))
            {
                options.SetAbsoluteExpiration(TimeSpan.FromTicks(absoluteExpiration));
            }

            if (req.Query.TryGetValue(QueryStringKeys.SlidingExpiration, out var slidingExpirationString)
                && long.TryParse(slidingExpirationString, out long slidingExpiration))
            {
                options.SetSlidingExpiration(TimeSpan.FromTicks(slidingExpiration));
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
                ? [.. uncompressedData.CompressBytes().Prepend(CompressionFlags.Compressed)]
                : [.. uncompressedData.Prepend(CompressionFlags.Uncompressed)];
            await _cache.SetAsync(key, data, options);
        }
    }
}