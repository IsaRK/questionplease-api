using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using questionplease_api.Items;

namespace questionplease_api
{
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{login}")] HttpRequest req,
            [CosmosDB(databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.USERS_COLLECTION_NAME,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] IAsyncCollector<object> users,
            string login,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string name = req.Query["name"];

                string userName = null;
                var userReq = req.HttpContext.User;
                if (userReq == null)
                {
                    log.LogInformation($"User from context is null");
                }
                else
                {
                    userName = userReq.GetDisplayName();
                    log.LogInformation($"added UserName is {userName}");
                }

                var newUser = new DatabaseUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Login = login
                };

                await users.AddAsync(newUser);

                return new OkObjectResult(new ReturnedUser { Id = newUser.Id, Login = newUser.Login});
            }
            catch (Exception ex)
            {
                log.LogError($"Couldn't insert item. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
