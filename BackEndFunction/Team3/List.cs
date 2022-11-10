using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Team3.Models;

namespace Team3
{
    public class List
    {
        private readonly ILogger<List> _logger;

        public List(ILogger<List> log)
        {
            _logger = log;
        }

        [FunctionName("List")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Get" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<HealthCheck>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB("Team3", "HealthChecks",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from HealthChecks r")]
                IEnumerable<HealthCheck> healthCheckItems)
        {
            if (healthCheckItems.ToList().Count == 0)
            {
                _logger.LogInformation($"No HealthCheck items found");
            }
            else
            {
                _logger.LogInformation($"Found {healthCheckItems.ToList().Count()} HealthCheck items");
            }

            return new OkObjectResult(healthCheckItems.ToList());
        }
    }
}

