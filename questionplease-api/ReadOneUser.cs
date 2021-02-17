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

            var searchValue = claimsPrincipal.GetObjectId();
            var homeObjectValue = claimsPrincipal.GetHomeObjectId();
            var msalAccountValue = claimsPrincipal.GetMsalAccountId();
            var identityName = claimsPrincipal.Identity.Name;
            var identityString = claimsPrincipal.Identity.ToString();
            var NameIdentifierId = claimsPrincipal.GetNameIdentifierId();
            //var currentUserName = ClaimsPrincipal.Current.Identity.Name;
            //var otherIdentity = ClaimsPrincipal.Current.Identity.ToString();
            //var currentHomeObjectValue = ClaimsPrincipal.Current.GetHomeObjectId();
            //var currentMsalAccountId = ClaimsPrincipal.Current.GetMsalAccountId();

            if (req.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-NAME", out var principalName))
            {
                log.LogInformation($"principalName Id: {principalName}");
            }
            else
            {
                log.LogInformation($"No principalName");
            }

            if (req.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-ID", out var principalId))
            {
                log.LogInformation($"principalId Id: {principalId}");
            }
            else
            {
                log.LogInformation($"No principalId");
            }

            if (ClaimsPrincipal.Current == null)
            {
                log.LogInformation($"ClaimsPrincipal.Current is null");
            }
            else if (ClaimsPrincipal.Current.Identity == null)
            {
                log.LogInformation($"ClaimsPrincipal.Current.Identity is null");
            }
            else
            {
                log.LogInformation($"ClaimsPrincipal.Current.Identity is {ClaimsPrincipal.Current.Identity.ToString()}");
            }

            var userReq = req.HttpContext.User;
            if (userReq == null)
            {
                log.LogInformation($"User from context is null");
            }
            else
            {
                log.LogInformation($"User from context: {userReq.GetDisplayName()}");
            }


            log.LogInformation($"Searching for: {searchValue}");
            log.LogInformation($"Home Object ID: {homeObjectValue}");
            log.LogInformation($"MSAL Account Value ID: {msalAccountValue}");
            log.LogInformation($"IdentityName: {identityName}");
            log.LogInformation($"Name Identifier Id: {NameIdentifierId}");
            log.LogInformation($"IdentityString Id: {identityString}");
            //log.LogInformation($"currentUserName Id: {currentUserName}");
            //log.LogInformation($"otherIdentity Id: {otherIdentity}");
            //log.LogInformation($"currentHomeObjectValue Id: {currentHomeObjectValue}");
            //log.LogInformation($"currentMsalAccountId Id: {currentMsalAccountId}");
            string name = req.Query["name"];

            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<User> query = client.CreateDocumentQuery<User>(collectionUri, option)
                .Where(p => p.HomeAccountId == searchValue)
                .AsDocumentQuery();

            List<User> result = new List<User>();

            while (query.HasMoreResults)
            {
                foreach (User user in await query.ExecuteNextAsync())
                {
                    result.Add(user);
                }
            }

            return new OkObjectResult(result);
        }
    }
}
