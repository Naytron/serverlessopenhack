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
using Microsoft.Azure.Cosmos;
using System.Net.Http;
using System.Collections.Generic;
using System;
using Microsoft.Azure.WebJobs.Host;


namespace Openhack.Team2
{
    public class CreateRating
    {
        private readonly ILogger<CreateRating> _logger;

        public CreateRating(ILogger<CreateRating> log)
        {
            _logger = log;
        }

        private const string GetUserURL = "https://serverlessohapi.azurewebsites.net/api/GetUser";

        [FunctionName("CreateRating")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(Rating), Description = "Ratings model", Example = typeof(Rating))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "bfyoc", containerName: "rating", Connection = @"ConnectionString")] 
            IAsyncCollector<Rating> icecreamRatingOut, ILogger log)
        {
            string productId = req.Query["productId"];
            string userId = req.Query["userId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic rating = JsonConvert.DeserializeObject(requestBody);

            productId = productId ?? rating.productId;
            userId = userId ?? rating.userId;
            
            string productURL = $"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={productId}";
            string userURL = $"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={userId}";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(productURL).Result;

            string responseMessage = response.IsSuccessStatusCode 
            ? "This HTTP triggered function executed successfully."
            : $"Product {productId} not found.";

            var responseJSON = new Rating();
            responseJSON.productId = rating.productId;
            responseJSON.userId = rating.userId;
            responseJSON.locationName = rating.locationName;
            responseJSON.rating = rating.rating;
            responseJSON.userNotes = rating.userNotes;
            responseJSON.id = Guid.NewGuid().ToString();
            responseJSON.timestamp = DateTime.Now.ToString();

            await icecreamRatingOut.AddAsync(responseJSON);       
            return new OkObjectResult(responseJSON);

        }
    }
}

