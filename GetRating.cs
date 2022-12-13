using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Openhack.Team2
{
    public class GetRating
    {
        private readonly ILogger<GetRating> _logger;

        public GetRating(ILogger<GetRating> log)
        {
            _logger = log;
        }

        string endpoint = "https://cosmosdb-team2.documents.azure.com:443/";
        string key = "V06ef1oxmSxY3K2WjTGcWAdmMW5D4o0fOuXV0yZAbTXLdCpDfip9iQMEzqfyrptthgoL7XCn8yOsACDbqyIycA==";


        [FunctionName("GetRating")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "ratingid" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "ratingid", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ratingid** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string ratingId = req.Query["ratingid"];
            CosmosClient client = new CosmosClient(endpoint, key);

            Database database = await client.CreateDatabaseIfNotExistsAsync("bfyoc");
            Container container = await database.CreateContainerIfNotExistsAsync("rating", "/id");

            string partId = "/id";
            PartitionKey partitionKey = new (partId);
            try
            {
                ItemResponse<Rating> rating = await container.ReadItemAsync<Rating>(ratingId, partitionKey);
                return new OkObjectResult(rating);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new NotFoundResult();
            }

        }
    }
}

