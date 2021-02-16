using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using System.Security.Claims;
using Microsoft.Identity.Web;
using Microsoft.Azure.Documents.Linq;
using questionplease_api.Items;
using System.Linq;

namespace questionplease_api
{
    public static class ReadOneUser
    {
        [FunctionName("ReadOneUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req,
            [CosmosDB(databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.USERS_COLLECTION_NAME,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] DocumentClient client,
            ILogger log,
            ClaimsPrincipal claimsPrincipal)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.USERS_COLLECTION_NAME);

            var searchValue = claimsPrincipal.GetObjectId();
            var homeObjectValue = claimsPrincipal.GetHomeObjectId();
            var msalAccountValue = claimsPrincipal.GetMsalAccountId();

            log.LogInformation($"Searching for: {searchValue}");
            log.LogInformation($"Home Object ID: {homeObjectValue}");
            log.LogInformation($"Home Object ID: {msalAccountValue}");
            string name = req.Query["name"];

            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<User> query = client.CreateDocumentQuery<User>(collectionUri, option)
                .Where(p => p.HomeAccountId == searchValue)
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (User result in await query.ExecuteNextAsync())
                {
                    log.LogInformation(result.Login);
                }
            }
            return new OkResult();
            //return new OkObjectResult(responseMessage);
        }
    }
}
