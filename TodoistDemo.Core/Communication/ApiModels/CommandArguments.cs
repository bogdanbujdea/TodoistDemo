using System.Collections.Generic;
using Newtonsoft.Json;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class CommandArguments
    {
        [JsonProperty(PropertyName = "ids")]
        public List<int> Ids { get; set; }
    }
}