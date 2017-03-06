using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Services;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private readonly IAccountManager _accountManager;
        private string _authToken;
        private ObservableCollection<Item> _items;
        private bool _tokenIsVisible;
        private string _avatarUri;
        private string _username;

        public ItemsViewModel(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            using (var db = new TodoistContext())
            {
                if (FirstTimeUse(db))
                {
                    return;
                }
                var user = await db.Users.FirstOrDefaultAsync();
                if (string.IsNullOrWhiteSpace(user?.Token))
                {
                    await Sync(true);
                    return;
                }
                AuthToken = user.Token;
                var dbItems = await db.Items.ToListAsync();
                var items = dbItems.Select(i => new Item
                {
                    Checked = i.Checked,
                    Content = i.Content
                }).ToList();
                Items = new ObservableCollection<Item>(items);
                await Sync();
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

        private bool FirstTimeUse(TodoistContext db)
        {
            return !db.Items.Any() || !db.Users.Any();
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

        public async Task Sync(bool clearData = false)
        {
            try
            {
                var syncData = await _accountManager.LoginAsync(AuthToken);
                TokenIsVisible = true;
                AvatarUri = syncData.User.AvatarBig;
                Username = syncData.User.FullName;
                Items = new ObservableCollection<Item>(syncData.Items);
                using (var db = new TodoistContext())
                {
                    var user = await db.Users.FirstOrDefaultAsync();
                    if (clearData || user == null)
                    {
                        var items = syncData.Items.Select(i => new DbItem
                        {
                            Checked = i.Checked,
                            Content = i.Content
                        }).ToList();
                        await db.Database.ExecuteSqlCommandAsync("delete from items");
                        await db.Database.ExecuteSqlCommandAsync("delete from users");
                        await db.Items.AddRangeAsync(items);
                        db.Users.Add(new DbUser
                        {
                            FullName = syncData.User.FullName,
                            AvatarBig = syncData.User.AvatarBig,
                            Token = syncData.User.Token
                        });
                    }
                    else
                    {
                        user.FullName = syncData.User.FullName;
                        user.AvatarBig = syncData.User.AvatarBig;
                        user.Token = syncData.User.Token;
                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (ApiException apiException)
            {
                TokenIsVisible = false;
                await new MessageDialog("Sync failed with error:" + apiException.ErrorMessage).ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}