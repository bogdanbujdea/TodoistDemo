using Newtonsoft.Json;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class User
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "avatar_big")]
        public string AvatarBig { get; set; }

        public string Token { get; set; }
    }
}