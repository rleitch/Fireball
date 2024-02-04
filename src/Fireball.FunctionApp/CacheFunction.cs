using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System;
using System.Linq;
using System.Text;
using Fireball.FunctionApp.Extensions;
using Fireball.Common;

namespace Fireball.FunctionApp
{
    public class CacheFunction(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;

        private const byte CompressedFlag = 0x01;
        private const byte UncompressedFlag = 0x00;

        [FunctionName("get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            var cachedData = await GetStringAsync(key);
            return cachedData == null 
                ? new NotFoundResult() 
                : new OkObjectResult(cachedData);
        }

        [FunctionName("post")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            DistributedCacheEntryOptions options = new();

            if (TimeSpan.TryParse(Uri.UnescapeDataString(req.Query[QueryStringKeys.AbsoluteExpiration]), out TimeSpan absoluteExpiration))
            {
                options.SetAbsoluteExpiration(absoluteExpiration);
            }

            if (TimeSpan.TryParse(Uri.UnescapeDataString(req.Query[QueryStringKeys.SlidingExpiration]), out TimeSpan slidingExpiration))
            {
                options.SetAbsoluteExpiration(absoluteExpiration);
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

        [FunctionName("put")]
        public async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            await _cache.RefreshAsync(key);
            return new AcceptedResult();
        }

        [FunctionName("delete")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            await _cache.RemoveAsync(key);
            return new AcceptedResult();
        }

        public async Task SetAsync(string key, byte[] uncompressedData, DistributedCacheEntryOptions options)
        {
            byte[] data = uncompressedData.Length > 1024 
                ? ([CompressedFlag, .. uncompressedData.CompressBytes()]) 
                : ([UncompressedFlag, .. uncompressedData]);
            await _cache.SetAsync(key, data, options);
        }
        public async Task<string> GetStringAsync(string key)
        {
            var cachedData = await _cache.GetAsync(key);
            if(cachedData == null || cachedData.Length == 0)
            {
                return null;
            }

            byte flag = cachedData[0];
            byte[] dataBuffer = cachedData.Skip(1).ToArray();
            return flag == CompressedFlag ? dataBuffer.DecompressString() : Encoding.UTF8.GetString(dataBuffer);
        }
    }
}