using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Services;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private readonly ITaskManager _taskManager;
        private readonly IUserRepository _userRepository;
        private string _authToken;
        private ObservableCollection<Item> _items;
        private bool _tokenIsVisible;
        private string _avatarUri;
        private string _username;

        public ItemsViewModel(ITaskManager taskManager, IUserRepository userRepository)
        {
            _taskManager = taskManager;
            _userRepository = userRepository;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            Items = new ObservableCollection<Item>(await _taskManager.RetrieveTasksAsync());
            var user = await _userRepository.GetUser();
            if (user != null && string.IsNullOrWhiteSpace(user.Token) == false)
            {
                AuthToken = user.Token;
                await Sync();
            }
        }

        public async Task Sync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AuthToken))
                {
                    await new MessageDialog("Please type a valid token").ShowAsync();
                    return;
                }
                var items = await _taskManager.RetrieveTasksFromWebAsync(AuthToken);
                var user = await _userRepository.GetUser();
                AvatarUri = user.AvatarBig;
                Username = user.FullName;
                Items = new ObservableCollection<Item>(items);
                TokenIsVisible = true;
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

        public string AvatarUri
        {
            get { return _avatarUri; }
            set
            {
                if (value == _avatarUri) return;
                _avatarUri = value;
                NotifyOfPropertyChange(() => AvatarUri);
            }
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

        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public bool TokenIsVisible
        {
            get { return _tokenIsVisible; }
            set
            {
                if (value == _tokenIsVisible) return;
                _tokenIsVisible = value;
                NotifyOfPropertyChange(() => TokenIsVisible);
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }
    }
}