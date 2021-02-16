using Newtonsoft.Json;

namespace questionplease_api.Items
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "homeAccountId")]
        public string HomeAccountId { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
    }
}
