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
    public class GetById
    {
        private readonly ILogger<GetById> _logger;

        public GetById(ILogger<GetById> log)
        {
            _logger = log;
        }

        [FunctionName("GetById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "id" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(HealthCheck), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetById/{id}")] HttpRequest req,
            [CosmosDB("Team3", "HealthChecks",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from HealthChecks r where r.id = {id}")]
                IEnumerable<HealthCheck> healthCheckItems)
        {
            if (healthCheckItems.ToList().Count == 0)
            {
                _logger.LogInformation($"HealthCheck item not found");
                return new OkResult();
            }
            else
            {
                _logger.LogInformation($"Found HealthCheck item, HealthStatus={healthCheckItems.FirstOrDefault()?.HealthStatus}");
                return new OkObjectResult(healthCheckItems.FirstOrDefault());
            }
        }
    }
}