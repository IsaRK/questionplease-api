﻿using Newtonsoft.Json;

namespace questionplease_api.Items
{
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