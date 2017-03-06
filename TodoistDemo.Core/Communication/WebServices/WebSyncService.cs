﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Storage;

namespace TodoistDemo.Core.Communication.WebServices
{
    public class WebSyncService : IWebSyncService
    {
        private readonly IRestClient _restClient;
        private readonly IAppSettings _appSettings;

        public WebSyncService(IRestClient restClient, IAppSettings appSettings)
        {
            _restClient = restClient;
            _appSettings = appSettings;
        }

        public async Task<SyncData> RetrieveAllItemsAsync()
        {
            if (string.IsNullOrWhiteSpace(_appSettings.GetData<string>(SettingsKey.SyncToken)))
            {
                _appSettings.SetData(SettingsKey.SyncToken, "*");
            }
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("token", _appSettings.GetData<string>(SettingsKey.UserToken)),
                new KeyValuePair<string, string>("sync_token", _appSettings.GetData<string>(SettingsKey.SyncToken)),
                new KeyValuePair<string, string>("resource_types", "[\"items\", \"user\"]")
            };
            var basicWebReport = await _restClient.PostAsync(ApiEndpoints.SyncUrl, formData);
            if (basicWebReport.IsSuccessful)
            {
                var syncData = JsonConvert.DeserializeObject<SyncData>(basicWebReport.StringResponse);
                _appSettings.SetData(SettingsKey.SyncToken, syncData.SyncToken);
                return syncData;
            }
            throw new ApiException(basicWebReport.FailedRequestInfo.ErrorMessage);
        }
    }
}