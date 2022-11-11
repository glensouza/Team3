using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Team3.Models;

namespace Team3
{
    public class Create
    {
        private readonly ILogger<Create> _logger;

        public Create(ILogger<Create> log)
        {
            _logger = log;
        }

        [FunctionName("Create")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Create" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "healthCheck", In = ParameterLocation.Header, Required = true, Type = typeof(HealthCheck), Description = "The **HealthCheck** parameter")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
        public void Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB("Team3", "HealthChecks", ConnectionStringSetting = "CosmosDBConnection")]out HealthCheck healthCheckItem)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string newHealthCheckItem = req.Query["healthCheck"];

            if (string.IsNullOrEmpty(newHealthCheckItem))
            {
                newHealthCheckItem = new StreamReader(req.Body).ReadToEnd();
            }

            if (!string.IsNullOrEmpty(newHealthCheckItem))
            {
                try
                {
                    healthCheckItem = JsonConvert.DeserializeObject<HealthCheck>(newHealthCheckItem);
                    healthCheckItem.Id = Guid.NewGuid().ToString();
                }
                catch
                {
                    throw new Exception("Invalid HealthCheck");
                }
            }
            else
            {
                throw new Exception("HealthCheck required");
            }
        }
    }
}
