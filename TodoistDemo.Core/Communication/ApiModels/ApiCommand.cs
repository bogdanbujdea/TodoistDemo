using System;
using Newtonsoft.Json;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class ApiCommand
    {
        public ApiCommand()
        {
            Id = Guid.NewGuid().ToString();
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "args")]
        public CommandArguments Args{ get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string Id { get; set; }
    }
}