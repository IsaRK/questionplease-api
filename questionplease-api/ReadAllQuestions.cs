using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using questionplease_api.Items;
using System.Security.Claims;

namespace questionplease_api
{
    /*
    public static class ReadAllQuestions
    {
        [FunctionName("ReadAllQuestions")]
        public static IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "question")] HttpRequest req,
                [CosmosDB(
                databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.QUESTIONS_COLLECTION_NAME,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                SqlQuery ="SELECT * FROM c")] IEnumerable<Question> question,
                ILogger log)
        {
            //https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=csharp#working-with-client-identities
            //ClaimsPrincipal identities = req.HttpContext.User;

            //string owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(question);
        }
    }
    */
}
