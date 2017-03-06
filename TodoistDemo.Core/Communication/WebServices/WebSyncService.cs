using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TodoistDemo.Core.Communication;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Services
{
    public class WebSyncService : IWebSyncService
    {
        private readonly IRestClient _restClient;

        public WebSyncService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<SyncData> RetrieveAllItemsAsync(string token)
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("sync_oken", "*"),
                new KeyValuePair<string, string>("resource_types", "[\"all\"]")
            };
            var basicWebReport = await _restClient.PostAsync(ApiEndpoints.SyncUrl, formData);
            if (basicWebReport.IsSuccessful)
            {
                var syncData = JsonConvert.DeserializeObject<SyncData>(basicWebReport.StringResponse);
                return syncData;
            }
            throw new ApiException(basicWebReport.FailedRequestInfo.ErrorMessage);
        }
    }
}
