using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Services;

namespace TodoistDemo.ViewModels
{
    public class TasksViewModel: ViewModelBase
    {
        private readonly IAccountManager _accountManager;
        private string _authToken;
        private ObservableCollection<Item> _tasks;

        public TasksViewModel(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public string AuthToken
        {
            get { return _authToken; }
            set
            {
                if (value == _authToken) return;
                _authToken = value;
                NotifyOfPropertyChange(() => AuthToken);
            }
        }

        public ObservableCollection<Item> Tasks
        {
            get { return _tasks; }
            set
            {
                if (Equals(value, _tasks)) return;
                _tasks = value;
                NotifyOfPropertyChange(() => Tasks);
            }
        }

        public async Task Sync()
        {
            try
            {
                var syncData = await _accountManager.LoginAsync(AuthToken);
                //await new MessageDialog("Hello " + syncData.User.FullName).ShowAsync();
                Tasks = new ObservableCollection<Item>(syncData.Items);
            }
            catch (ApiException apiException)
            {
                await new MessageDialog("Sync failed with error:" + apiException.ErrorMessage).ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}