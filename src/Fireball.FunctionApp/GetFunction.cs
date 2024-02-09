using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using Fireball.Common.Extensions;
using Fireball.FunctionApp.Constants;
using System.Threading.Tasks;
using System.Linq;

namespace Fireball.FunctionApp
{
    public class GetFunction(ILogger<GetFunction> logger, IDistributedCache cache)
    {
        private readonly ILogger<GetFunction> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        [Function("GetFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{key}")] HttpRequest req, string key)
        {
            var cachedData = await _cache.GetAsync(key);
            if (cachedData == null || cachedData.Length == 0)
            {
                return new NotFoundResult();
            }

            byte flag = cachedData[0];
            byte[] dataBuffer = cachedData.Skip(1).ToArray();
            return new OkObjectResult(flag == CompressionFlags.Compressed
                ? dataBuffer.DecompressString()
                : Encoding.UTF8.GetString(dataBuffer));
        }
    }
}