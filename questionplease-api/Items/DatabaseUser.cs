using Newtonsoft.Json;

namespace questionplease_api.Items
{
    public class ReturnedUser
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
    }

    public class DatabaseUser : ReturnedUser
    {
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
}
