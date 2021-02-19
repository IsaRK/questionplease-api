using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using questionplease_api.Items;
using System.Collections.Generic;
using System.Linq;

namespace questionplease_api
{
    public class ValidateQuestion
    {
        private readonly ILogger _logger;
        private CosmosClient _cosmosClient;

        private Database _database;
        private Container _userContainer;
        private Container _userQuestionLogContainer;
        private Container _questionContainer;

        public ValidateQuestion(
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

        [FunctionName("ValidateQuestion")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "question/validate")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string userId = data?.userId;
                int questionId = data?.questionId;
                string userAnswer = data?.answer;

                string correctAnswer = await GetCorrectAnswer(questionId, log);

                bool isValid = IsAnswerValid(userAnswer, correctAnswer, out int points);
                var user = await GetUser(userId, log);
                var currentScore = user.Score;

                if (isValid)
                {
                    await UpdateUserScore(user, currentScore + points, log);
                    await InsertUserQuestionsLog(userId, questionId, points, log);
                }
                else
                {
                    await InsertUserQuestionsLog(userId, questionId, 0, log);
                }

                var result = new ValidateQuestionReply
                {
                    IsValid = isValid,
                    Points = points,
                    NewScore = currentScore + points
                };

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError($"Error during question validation. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<string> GetCorrectAnswer(int questionId, ILogger log)
        {
            List<Question> questionList = new List<Question>();
            QueryDefinition questionById = new QueryDefinition("select * from questions u where u.id = @id").WithParameter("@id", questionId.ToString());
            using (FeedIterator<Question> feedIterator = _questionContainer.GetItemQueryIterator<Question>(questionById))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        questionList.Add(item);
                    }
                }
            }

            if (questionList.Count == 0 || questionList.Count > 1)
            {
                throw new Exception($"Several questions found with id {questionId}");
            }

            return questionList.Single().CorrectAnswer;
        }

        private bool IsAnswerValid(string userAnswer, string correctAnswer, out int points)
        {
            points = 0;

            if (userAnswer.ToLower() == correctAnswer.ToLower())
            {
                points = 1;
                return true;
            }
            return false;
        }

        private async Task<DatabaseUser> GetUser(string userId, ILogger log)
        {
            List<DatabaseUser> usersWithId = new List<DatabaseUser>();
            QueryDefinition getUserWithId = new QueryDefinition("select * from users u where u.id = @id").WithParameter("@id", userId);
            using (FeedIterator<DatabaseUser> feedIterator = _userContainer.GetItemQueryIterator<DatabaseUser>(getUserWithId))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var user in await feedIterator.ReadNextAsync())
                    {
                        usersWithId.Add(user);
                    }
                }
            }

            if (usersWithId.Count == 0 || usersWithId.Count > 1)
            {
                throw new Exception($"Several users found with id {userId}");
            }

            return usersWithId.Single();
        }

        private Task UpdateUserScore(DatabaseUser user, int newScore, ILogger log)
        {
            var toUpdate = new DatabaseUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Login = user.Login,
                Score = newScore,
            };

            _userContainer.CreateItemAsync(toUpdate);
            return Task.CompletedTask;
        }

        private Task InsertUserQuestionsLog(string userId, int questionId, int points, ILogger log)
        {
            var toInsert = new UserQuestionsLog
            {
                Id = Guid.NewGuid().ToString(),
                IdUser = userId,
                IdQuestion = questionId.ToString(),
                QuestionPoint = points,
                QuestionDone = points > 0
            };

            _userQuestionLogContainer.CreateItemAsync(toInsert);
            return Task.CompletedTask;
        }
    }
}
