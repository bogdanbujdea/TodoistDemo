using Newtonsoft.Json;

namespace TodoistDemo.Core.Validation.Reports.Web
{
    public class FailedRequest
    {
        [JsonProperty(PropertyName = "error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_tag")]
        public string ErrorTag { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string ErrorMessage { get; set; }
    }
}