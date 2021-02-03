using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using questionplease_api.Items;

namespace questionplease_api
{
    public static class ReadAllQuestions
    {
        [FunctionName("ReadAllQuestions")]
        public static IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "question")] HttpRequest req,
                [CosmosDB(
                databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.COLLECTION_NAME,
                ConnectionStringSetting = "CONNECTION_STRING",
                SqlQuery ="SELECT * FROM c")] IEnumerable<Question> question,
                ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(question);
        }
    }
}
