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
using System.Collections.Generic;

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

            //Also works : req.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-NAME", out var principalName) = Isabelle Riverain
            //req.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-ID", out var principalId) = AccountInfo.LocalAccountId

            string searchValue = null;
            var userReq = req.HttpContext.User;
            if (userReq == null)
            {
                log.LogInformation($"User from context is null");
            }
            else
            {
                searchValue = userReq.GetDisplayName();
                log.LogInformation($"searchedValue is {searchValue}");
            }

            string name = req.Query["name"];

            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<DatabaseUser> query = client.CreateDocumentQuery<DatabaseUser>(collectionUri, option)
                .Where(p => p.UserName == searchValue)
                .AsDocumentQuery();

            List<DatabaseUser> result = new List<DatabaseUser>();

            while (query.HasMoreResults)
            {
                foreach (DatabaseUser user in await query.ExecuteNextAsync())
                {
                    result.Add(user);
                }
            }

            if (result.Count > 1)
            {
                return new ObjectResult($"Found several users with userName {searchValue}");
            }

            List<ReturnedUser> okResult = new List<ReturnedUser>();
            foreach(var u in result)
            {
                var returnUser = new ReturnedUser { Id = u.Id, Login = u.Login };
            }

            return new OkObjectResult(result);
        }
    }
}
