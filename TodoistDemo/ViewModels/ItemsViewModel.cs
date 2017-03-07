using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Caliburn.Micro;
using TodoistDemo.Core.Communication;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Services;
using TodoistDemo.Core.Storage;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private readonly ITaskManager _taskManager;
        private readonly IUserRepository _userRepository;
        private readonly IAppSettings _appSettings;
        private string _authToken;
        private ObservableCollection<Item> _items;
        private bool _tokenIsVisible;
        private string _avatarUri;
        private string _username;
        private bool _completedItemsAreVisible;

        public ItemsViewModel(ITaskManager taskManager, IUserRepository userRepository, IAppSettings appSettings)
        {
            _taskManager = taskManager;
            _userRepository = userRepository;
            _appSettings = appSettings;
            Items = new ObservableCollection<Item>();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            AuthToken = _appSettings.GetData<string>(SettingsKey.UserToken);

            if (string.IsNullOrWhiteSpace(AuthToken) == false)
            {
                await UpdateItems();
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
                _appSettings.SetData(SettingsKey.UserToken, AuthToken);
                await UpdateItems();
                await SetUserInfo();
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

        private async Task UpdateItems()
        {
            if (Items.Count == 0)
            {
                var storedTasks = (await _taskManager.RetrieveTasksAsync());
                Items = new ObservableCollection<Item>(storedTasks.Where(TaskIsVisible).OrderBy(i => i.Content.ToLower()));
            }
            var items = await _taskManager.RetrieveTasksFromWebAsync();
            foreach (var item in GetItemsToRemove(items))
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                Items.Remove(existingItem);
            }
            var visibleItems = GetItemsToInsert(items).Distinct();
            foreach (var item in visibleItems)
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    var index = Items.IndexOf(existingItem);
                    Items[index] = item;
                }
                else Insert(item, Items);
            }
        }

        private IEnumerable<Item> GetItemsToInsert(List<Item> items)
        {
            return items.Where(TaskIsVisible);
        }

        private IEnumerable<Item> GetItemsToRemove(List<Item> items)
        {
            return items.Where(i => (i.Checked && !CompletedItemsAreVisible) || string.IsNullOrWhiteSpace(i.Content));
        }

        private async Task SetUserInfo()
        {
            var user = await _userRepository.GetUser();
            AvatarUri = user?.AvatarBig;
            Username = user?.FullName;
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

        public async Task ToggleCompletedTasks()
        {
            var allTasks = await _taskManager.RetrieveTasksAsync();
            Items = new BindableCollection<Item>(allTasks.Where(t => t.Checked == CompletedItemsAreVisible));
        }

        public bool CompletedItemsAreVisible
        {
            get { return _completedItemsAreVisible; }
            set
            {
                if (value == _completedItemsAreVisible) return;
                _completedItemsAreVisible = value;
                if (value)
                {

                }
                NotifyOfPropertyChange(() => CompletedItemsAreVisible);
            }
        }

        private bool TaskIsVisible(Item i)
        {
            if (CompletedItemsAreVisible)
                return string.IsNullOrWhiteSpace(i.Content);
            return !i.Checked && !string.IsNullOrWhiteSpace(i.Content);
        }

        private void Insert(Item item, ObservableCollection<Item> items)
        {
            for (int index = 0; index < items.Count; index++)
            {
                if (string.CompareOrdinal(items[index].Content.ToLower(), item.Content.ToLower()) > 0)
                {
                    items.Insert(index, item);
                    return;
                }
            }
            items.Add(item);
        }
    }
}