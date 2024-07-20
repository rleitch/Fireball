using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fireball.FunctionApp
{
    public class RefreshFunction(ILogger<RefreshFunction> logger, IDistributedCache cache)
    {
        private readonly ILogger<RefreshFunction> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        [Function("RefreshFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "{key}")] HttpRequest req, string key)
        {
            await _cache.RefreshAsync(key);
            return new AcceptedResult();
        }
    }
}