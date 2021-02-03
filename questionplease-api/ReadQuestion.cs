using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using questionplease_api.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace questionplease_api
{
    public static class ReadOneQuestion
    {
        [FunctionName("ReadQuestion")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "question/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.COLLECTION_NAME,
                ConnectionStringSetting = "CONNECTION_STRING",
                SqlQuery ="SELECT * FROM c WHERE c.id={id}")] IEnumerable<Question> question,
            ILogger log,
            string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (question == null)
            {
                log.LogInformation($"Question item not found");
                return new NotFoundResult();
            }

            log.LogInformation($"Found Question item, Description={question.First().FullQuestion}");
            return new OkObjectResult(question.First());
        }
    }

    public static class ReadAllQuestion
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
