using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using ReactiveUI;
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
        private ReactiveList<BindableItem> _items;
        private string _avatarUri;
        private string _username;
        private bool _completedItemsAreVisible;
        private ReactiveList<BindableItem> _changedItems;
        private IDisposable ItemsChangedDisposable;

        public ItemsViewModel(ITaskManager taskManager, IUserRepository userRepository, IAppSettings appSettings)
        {
            _taskManager = taskManager;
            _userRepository = userRepository;
            _appSettings = appSettings;
            Items = new ReactiveList<BindableItem>();
            _taskManager.Items = Items;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            AuthToken = _appSettings.GetData<string>(SettingsKey.UserToken);

            if (string.IsNullOrWhiteSpace(AuthToken)) return;
            IsBusy = true;
            try
            {
                await UpdateItems();
                await Sync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ListenToItemsChanged()
        {
            _changedItems = new ReactiveList<BindableItem> { ChangeTrackingEnabled = true };
            Items.ChangeTrackingEnabled = true;
            _changedItems.CountChanged
                .Buffer(TimeSpan.FromSeconds(10), 5)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(async list =>
                {
                    if (list.Count == 0)
                    {
                        await UpdateItems();
                        return;
                    }
                    await SynchronizeModifiedTasks();
                });
            ItemsChangedDisposable = Items.ItemChanged
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(args =>
               {
                   Items.Remove(args.Sender);
                   _changedItems.Add(args.Sender);
               });
        }

        private async Task SynchronizeModifiedTasks()
        {
            var changedItems = _changedItems.ToList();
            _changedItems.Clear();
            IsBusy = true;
            try
            {
                var syncedItems = await _taskManager.ToggleItems(changedItems);
                await UpdateItems(syncedItems);
            }
            catch (Exception exception)
            {
                await HandleInvalidToken(exception);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Sync()
        {
            IsBusy = true;
            try
            {
                if (string.IsNullOrWhiteSpace(AuthToken))
                    return;
                if (_changedItems?.Count > 0)
                {
                    await SynchronizeModifiedTasks();
                    return;
                }
                _appSettings.SetData(SettingsKey.UserToken, AuthToken);
                if (ItemsChangedDisposable == null || (ItemsChangedDisposable as CompositeDisposable)?.Count == 0)
                    ListenToItemsChanged();
                await UpdateItems();
                await SetUserInfo();
            }
            catch (Exception ex)
            {
                await HandleInvalidToken(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ReactiveList<BindableItem> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }
        
        public async Task ToggleCompletedTasks()
        {
            Expression<Func<Item, bool>> exp = item => item.Checked == CompletedItemsAreVisible;
            var allTasks = await _taskManager.RetrieveTasksFromDbAsync(exp);
            Items.Clear();
            Items.AddRange(allTasks);
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

        private async Task UpdateItems(List<BindableItem> syncedItems = null)
        {
            try
            {
                await _taskManager.UpdateItems(Items, syncedItems);
            }
            catch (Exception exception)
            {
                await HandleInvalidToken(exception);
            }
        }

        private async Task SetUserInfo()
        {
            var user = await _userRepository.GetUser();
            AvatarUri = user?.AvatarBig;
            Username = user?.FullName;
        }

        public bool CompletedItemsAreVisible
        {
            get { return _completedItemsAreVisible; }
            set
            {
                _completedItemsAreVisible = value;
                _taskManager.CompletedItemsAreVisible = value;
            }
        }

        private async Task HandleInvalidToken(Exception exception)
        {
            AuthToken = string.Empty;
            ItemsChangedDisposable?.Dispose();
            Items.Clear();
            var apiException = exception as ApiException;
            var errorMessage = apiException != null ? apiException.ErrorMessage : exception.Message;
            await new MessageDialog("Sync failed with error:" + errorMessage).ShowAsync();
        }
    }
}