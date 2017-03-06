using System.Collections.Generic;
using System.Diagnostics;
using TodoistDemo.Core.Communication;

namespace TodoistDemo.ViewModels
{
    public class TasksViewModel: ViewModelBase
    {
        private string _authToken;

        public TasksViewModel()
        {

        }

        public string AuthToken
        {
            get { return _authToken; }
            set
            {
                if (value == _authToken) return;
                _authToken = value;
                SyncUser();                
                NotifyOfPropertyChange(() => AuthToken);
            }
        }

        private async void SyncUser()
        {
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("token", AuthToken));
            formData.Add(new KeyValuePair<string, string>("sync_oken", "*"));
            formData.Add(new KeyValuePair<string, string>("resource_types", "[\"all\"]"));
            var basicWebReport = await new RestClient().PostAsync(ApiEndpoints.SyncUrl, formData);
            Debug.WriteLine(basicWebReport.StringResponse);
        }
    }
}