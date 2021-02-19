using Newtonsoft.Json;
using System;

namespace questionplease_api.Items
{
    public class UserQuestionsLog
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "idUser")]
        public string IdUser { get; set; }

        [JsonProperty(PropertyName = "idQuestion")]
        public string IdQuestion { get; set; }

        [JsonProperty(PropertyName = "questionDone")]
        public Boolean QuestionDone { get; set; }

        [JsonProperty(PropertyName = "questionPoint")]
        public int QuestionPoint { get; set; }
    }
}
