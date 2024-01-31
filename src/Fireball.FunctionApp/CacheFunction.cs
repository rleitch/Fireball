using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Fireball.FunctionApp.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;

namespace Fireball.FunctionApp
{
    public class CacheFunction(IOptions<Settings> settings, IDistributedCache cache)
    {
        private readonly Settings _settings = settings.Value;
        private readonly IDistributedCache _cache = cache;

        [FunctionName("get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            var cachedString = await _cache.GetStringAsync(key);
            return cachedString == null 
                ? new NotFoundResult() 
                : new OkObjectResult(cachedString);
        }

        [FunctionName("post")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            using var sr = new StreamReader(req.Body);
            var requestBody = await sr.ReadToEndAsync();
            await _cache.SetStringAsync(key, requestBody, new DistributedCacheEntryOptions());
            return new AcceptedResult();
        }
    }
}
