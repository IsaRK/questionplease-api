﻿using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace questionplease_api.Items
{
    public class ReturnedQuestion
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "question")]
        public string FullQuestion { get; set; }

        [JsonProperty(PropertyName = "answers")]
        public string[] Answers { get; set; }

        public ReturnedQuestion(Question question)
        {
            this.Id = question.Id;
            this.FullQuestion = WebUtility.HtmlDecode(question.FullQuestion);

            var allAnswers = question.IncorrectAnswers.ToList();
            allAnswers.Add(question.CorrectAnswer);
            for(var i = 0; i < allAnswers.Count; i++)
            {
                allAnswers[i] = WebUtility.HtmlDecode(allAnswers[i]);
            }

            allAnswers.Shuffle();
            this.Answers = allAnswers.ToArray();
        }
    }

    public class Question
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty(PropertyName = "question")]
        public string FullQuestion { get; set; }

        [JsonProperty(PropertyName = "correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonProperty(PropertyName = "incorrect_answers")]
        public string[] IncorrectAnswers { get; set; }
    }
}
