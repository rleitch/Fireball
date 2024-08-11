using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fireball.FunctionApp
{
    public class DeleteFunction(ILogger<DeleteFunction> logger, IDistributedCache cache)
    {
        private readonly ILogger<DeleteFunction> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        [Function("DeleteFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "{key}")] HttpRequest req, string key)
        {
            key = Uri.UnescapeDataString(key);
            _logger.LogInformation("DELETE {0}", key);
            await _cache.RemoveAsync(key);
            return new AcceptedResult();
        }
    }
}