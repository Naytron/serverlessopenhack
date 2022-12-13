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
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string ratingId = req.Query["ratingid"];
            CosmosClient client = new CosmosClient(endpoint, key);

            Database database = await client.CreateDatabaseIfNotExistsAsync("bfyoc");
            Container container = await database.CreateContainerIfNotExistsAsync("rating", "/id");

            string partId = "9603ca6c-9e28-4a02-9194-51cdb7fea816";
            PartitionKey partitionKey = new (partId);

            Rating rating = await container.ReadItemAsync<Rating>(ratingId, partitionKey); 
  /*          string productURL = "https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={name}"; 
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(productURL);
            if (response.IsSuccessStatusCode)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new BadRequestObjectResult(response);
            }
            */

        }
    }
}

