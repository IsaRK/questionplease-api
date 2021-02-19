using Newtonsoft.Json;

namespace questionplease_api.Items
{
    public class ValidateQuestionReply
    {
        [JsonProperty(PropertyName = "isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "newScore")]
        public int NewScore { get; set; }
    }
}
