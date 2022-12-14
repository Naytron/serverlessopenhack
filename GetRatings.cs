using System.Collections.Generic;
using System.IO;
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

namespace Openhack.Team2
{
    
    public class GetRatings
    {
        private readonly ILogger<GetRatings> _logger;

        public GetRatings(ILogger<GetRatings> log)
        {
            _logger = log;
        }

        string endpoint = "https://cosmosdb-team2.documents.azure.com:443/";
        string key = "V06ef1oxmSxY3K2WjTGcWAdmMW5D4o0fOuXV0yZAbTXLdCpDfip9iQMEzqfyrptthgoL7XCn8yOsACDbqyIycA==";

        [FunctionName("GetRatings")]
        [OpenApiOperation(operationId: "getratings", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getRatings/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "bfyoc",
                containerName: "rating",
                Connection = @"ConnectionString",
                SqlQuery = "select * from rating r where r.userId = {userId}")]
                IEnumerable<Rating> ratings,               
            ILogger log)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(ratings);
        }
    }
}

