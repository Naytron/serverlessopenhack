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

namespace Openhack.Team2
{
    public class CreateRating
    {
        private readonly ILogger<CreateRating> _logger;

        public CreateRating(ILogger<CreateRating> log)
        {
            _logger = log;
        }

        private const string GetProductURL = "https://serverlessohapi.azurewebsites.net/api/GetProduct";
        private const string GetUserURL = "https://serverlessohapi.azurewebsites.net/api/GetUser";

        [FunctionName("CreateRating")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(Rating), Description = "Ratings model", Example = typeof(Rating))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
           
            // string productId = req.Query["productId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic rating = JsonConvert.DeserializeObject(requestBody);
            // productId = productId ?? rating.productId;


            HttpClient client = new HttpClient();
            
            HttpResponseMessage response = client.GetAsync(GetProductURL).Result;

            if(response.IsSuccessStatusCode)
            {
                rating = response.Content.ReadAsAsync<IEnumerable<Rating>>().Result;
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

