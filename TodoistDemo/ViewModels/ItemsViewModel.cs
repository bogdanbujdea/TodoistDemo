﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool _userIsLoggedIn;
        private string _avatarUri;
        private string _username;
        private bool _completedItemsAreVisible;
        private IDisposable ItemsChangedDisposable;

        public ItemsViewModel(ITaskManager taskManager, IUserRepository userRepository, IAppSettings appSettings)
        {
            _taskManager = taskManager;
            _userRepository = userRepository;
            _appSettings = appSettings;
            Items = new ReactiveList<BindableItem>();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            AuthToken = _appSettings.GetData<string>(SettingsKey.UserToken);

            if (string.IsNullOrWhiteSpace(AuthToken) == false)
            {
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
        }

        private void BindToItemsChanged()
        {
            Items.ChangeTrackingEnabled = true;
            ItemsChangedDisposable = Items.ItemChanged
                .Buffer(TimeSpan.FromSeconds(3), 3)
                .DoWhile(() => string.IsNullOrWhiteSpace(AuthToken) == false)
                .ObserveOn(SynchronizationContext.Current)
                .Distinct()
                .Subscribe(async args =>
                {
                    if (string.IsNullOrWhiteSpace(AuthToken))
                    {
                        ItemsChangedDisposable.Dispose();
                        return;
                    }
                    await ToggleTasks(args.Select(s => s.Sender).ToList());
                });
        }

        private async Task ToggleTasks(List<BindableItem> items)
        {
            IsBusy = true;
            try
            {
                if (items?.Count == 0)
                {
                    await UpdateItems();
                    return;
                }
                var syncedItems = await _taskManager.ToggleItems(items);
                await UpdateItems(syncedItems);
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
                _appSettings.SetData(SettingsKey.UserToken, AuthToken);
                if (ItemsChangedDisposable == null || (ItemsChangedDisposable as CompositeDisposable)?.Count == 0)
                    BindToItemsChanged();
                await UpdateItems();
                await SetUserInfo();
                UserIsLoggedIn = true;
            }
            catch (ApiException apiException)
            {
                await HandleInvalidToken(apiException);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task HandleInvalidToken(ApiException apiException)
        {
            AuthToken = string.Empty;
            ItemsChangedDisposable?.Dispose();
            Items.Clear();
            await new MessageDialog("Sync failed with error:" + apiException.ErrorMessage).ShowAsync();
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

        public bool UserIsLoggedIn
        {
            get { return _userIsLoggedIn; }
            set
            {
                if (value == _userIsLoggedIn) return;
                _userIsLoggedIn = value;
                NotifyOfPropertyChange(() => UserIsLoggedIn);
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
            Expression<Func<Item, bool>> exp = item => item.Checked == CompletedItemsAreVisible;
            var allTasks = await _taskManager.RetrieveTasksFromDbAsync(exp);
            Items.Clear();
            Items.AddRange(allTasks);
        }

        private async Task UpdateItems(List<BindableItem> syncedItems = null)
        {
            try
            {
                if (Items.Count == 0)
                {
                    Expression<Func<Item, bool>> exp = item => TaskIsVisible(item.ToBindableItem());
                    var storedTasks = (await _taskManager.RetrieveTasksFromDbAsync(exp));
                    Items.AddRange(storedTasks.Where(TaskIsVisible).OrderBy(i => i.Content.ToLower()));
                }
                var items = syncedItems ?? await _taskManager.RetrieveTasksFromWebAsync();
                RemoveItems(items);
                AddItems(items);
            }
            catch (ApiException apiException)
            {
                await HandleInvalidToken(apiException);
            }
        }

        private void AddItems(List<BindableItem> items)
        {
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

        private void RemoveItems(List<BindableItem> items)
        {
            foreach (var item in GetItemsToRemove(items))
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                Items.Remove(existingItem);
            }
        }

        private IEnumerable<BindableItem> GetItemsToInsert(List<BindableItem> items)
        {
            return items.Where(TaskIsVisible);
        }

        private IEnumerable<BindableItem> GetItemsToRemove(List<BindableItem> items)
        {
            return items.Where(i => (i.Checked != CompletedItemsAreVisible) && !string.IsNullOrWhiteSpace(i.Content));
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
                if (value == _completedItemsAreVisible) return;
                _completedItemsAreVisible = value;
                NotifyOfPropertyChange(() => CompletedItemsAreVisible);
            }
        }

        private bool TaskIsVisible(BindableItem item)
        {
            return (item.Checked == CompletedItemsAreVisible) && !string.IsNullOrWhiteSpace(item.Content);
        }

        private void Insert(BindableItem bindableItem, ReactiveList<BindableItem> items)
        {
            for (int index = 0; index < items.Count; index++)
            {
                if (string.CompareOrdinal(items[index].Content.ToLower(), bindableItem.Content.ToLower()) > 0)
                {
                    items.Insert(index, bindableItem);
                    return;
                }
            }
            items.Add(bindableItem);
        }
    }
}