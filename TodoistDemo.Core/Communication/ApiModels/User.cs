using Newtonsoft.Json;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class User : IUser
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "avatar_small")]
        public string AvatarSmall { get; set; }

        [JsonProperty(PropertyName = "sort_order")]
        public int SortOrder { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "avatar_s640")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "completed_count")]
        public int CompletedCount { get; set; }

        [JsonProperty(PropertyName = "avatar_medium")]
        public string AvatarMedium { get; set; }

        public string Email { get; set; }

        [JsonProperty(PropertyName = "tz_info")]
        public TzInfo TimezoneInfo { get; set; }

        [JsonProperty(PropertyName = "avatar_big")]
        public string AvatarBig { get; set; }

        public string Token { get; set; }
    }
}