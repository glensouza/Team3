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
    public class GetStatus
    {
        private readonly ILogger<GetStatus> _logger;

        public GetStatus(ILogger<GetStatus> log)
        {
            _logger = log;
        }

        [FunctionName("GetStatus")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetStatus/{id}")] HttpRequest req,
            [CosmosDB("Team3", "HealthChecks",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from HealthChecks r where r.id = {id}")]
                IEnumerable<HealthCheck> healthCheckItems)
        {
            if (healthCheckItems.ToList().Count == 0)
            {
                _logger.LogInformation($"No HealthCheck items found");
                return new OkResult();
            }
            else
            {
                _logger.LogInformation($"Found HealthCheck item, HealthStatus={healthCheckItems.FirstOrDefault()?.HealthStatus}");
                return new OkObjectResult(healthCheckItems.FirstOrDefault().HealthStatus);
            }
        }
    }
}

