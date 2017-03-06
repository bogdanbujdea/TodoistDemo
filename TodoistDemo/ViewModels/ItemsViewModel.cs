using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
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
            await UpdateItems();
            AuthToken = _appSettings.GetData<string>(SettingsKey.UserToken);
            if (string.IsNullOrWhiteSpace(AuthToken) == false)
            {
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
                await SetUserInfo();
                await UpdateItems();
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
            var items = await _taskManager.RetrieveTasksFromWebAsync();
            var visibleItems = items
                .Where(i => i.Checked == false && string.IsNullOrWhiteSpace(i.Content) == false)
                .ToList();
            if (Items.Count == 0)
            {
                Items = new ObservableCollection<Item>(visibleItems.OrderBy(i => i.Content.ToLower()));
                return;
            }
            foreach (var item in items.Where(i => i.Checked))
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                    Items.Remove(existingItem);
            }
            foreach (var item in visibleItems)
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    if (ItemIsUpdated(existingItem, item))
                    {
                        Items.Remove(existingItem);
                        Insert(item);
                    }
                }
                else
                    Insert(item);
            }
        }

        private bool ItemIsUpdated(Item existingItem, Item updatedItem)
        {
            return existingItem.Checked != updatedItem.Checked || existingItem.Content != updatedItem.Content;
        }

        private void Insert(Item item)
        {
            for (int index = 0; index < Items.Count; index++)
            {
                if (string.CompareOrdinal(Items[index].Content.ToLower(), item.Content.ToLower()) > 0)
                {
                    Items.Insert(index, item);
                    return;
                }
            }
            Items.Add(item);
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
    }
}