using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using questionplease_api.Items;

namespace questionplease_api
{
    public static class AbandonQuestion
    {
        [FunctionName("AbandonQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "question/abandon")] HttpRequest req,
            [CosmosDB(databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.USERS_QUESTIONS_COLLECTION_NAME,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] IAsyncCollector<object> userQuestionsLog,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string userId = data?.userId;
                int questionId = data?.questionId;

                var newUserQuestionsLog = new UserQuestionsLog
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = userId,
                    IdQuestion = questionId.ToString(),
                    QuestionDone = false,
                    QuestionPoint = 0
                };

                await userQuestionsLog.AddAsync(newUserQuestionsLog);

                return new OkObjectResult(newUserQuestionsLog);
            }
            catch (Exception ex)
            {
                log.LogError($"Couldn't insert item in userQuestionsLog. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
