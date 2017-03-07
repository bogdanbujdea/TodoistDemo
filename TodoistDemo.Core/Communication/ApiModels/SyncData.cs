using System.Collections.Generic;
using Newtonsoft.Json;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class SyncData
    {
        [JsonProperty(PropertyName = "full_sync")]
        public bool FullSync { get; set; }

        public User User { get; set; }

        [JsonProperty(PropertyName = "sync_token")]
        public string SyncToken { get; set; }

        public List<Project> Projects { get; set; }

        public List<Item> Items { get; set; }
    }
}
