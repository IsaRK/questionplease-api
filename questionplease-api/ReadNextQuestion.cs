using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;
using questionplease_api.Items;
using System.Collections.Generic;
using System.Linq;

namespace questionplease_api
{
    public class ReadNextQuestion
    {
        private readonly ILogger _logger;
        private CosmosClient _cosmosClient;

        private Database _database;
        private Container _userContainer;
        private Container _userQuestionLogContainer;
        private Container _questionContainer;

        public ReadNextQuestion(
            ILogger<ReadNextQuestion> logger,
            CosmosClient cosmosClient
            )
        {
            _logger = logger;
            _cosmosClient = cosmosClient;

            _database = _cosmosClient.GetDatabase(Constants.DATABASE_NAME);
            _userContainer = _database.GetContainer(Constants.USERS_COLLECTION_NAME);
            _userQuestionLogContainer = _database.GetContainer(Constants.USERS_QUESTIONS_COLLECTION_NAME);
            _questionContainer = _database.GetContainer(Constants.QUESTIONS_COLLECTION_NAME);
        }

        [FunctionName("ReadNextQuestion")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "question")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string userName = null;
                var userReq = req.HttpContext.User;
                if (userReq == null)
                {
                    log.LogInformation($"User from context is null");
                }
                else
                {
                    userName = userReq.GetDisplayName();
                    log.LogInformation($"searchedValue is {userName}");
                }

                string name = req.Query["name"];

                List<DatabaseUser> usersWithUserName = new List<DatabaseUser>();

                QueryDefinition getUserWithUserName = new QueryDefinition("select * from users u where u.userName = @username")
                    .WithParameter("@username", userName);
                using (FeedIterator<DatabaseUser> feedIterator = _userContainer.GetItemQueryIterator<DatabaseUser>(getUserWithUserName))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var user in await feedIterator.ReadNextAsync())
                        {
                            usersWithUserName.Add(user);
                        }
                    }
                }

                if (usersWithUserName.Count==0 || usersWithUserName.Count > 1)
                {
                    throw new Exception($"Several users found with userName {userName}");
                }

                int lastQuestionAsked = 0;
                QueryDefinition questionAsked = new QueryDefinition("select value max(u.idQuestion) from userQuestionsLog u where u.idUser = @idUser and u.questionDone = true")
                    .WithParameter("@idUser", usersWithUserName[0].Id);

                var selectMax = _userQuestionLogContainer.GetItemQueryIterator<int>(questionAsked);
                lastQuestionAsked = (await selectMax.ReadNextAsync()).Single();

                List<Question> nextQuestionList = new List<Question>();
                QueryDefinition nextQuestion = new QueryDefinition("select * from questions u where u.id = @id").WithParameter("@id", (lastQuestionAsked + 1).ToString());
                using (FeedIterator<Question> feedIterator = _questionContainer.GetItemQueryIterator<Question>(nextQuestion))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var question in await feedIterator.ReadNextAsync())
                        {
                            nextQuestionList.Add(question);
                        }
                    }
                }

                if (nextQuestionList.Count == 0 || nextQuestionList.Count > 1)
                {
                    throw new Exception($"Several questions found with id {lastQuestionAsked + 1}");
                }

                return new OkObjectResult(nextQuestionList.Single());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not read next question. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
