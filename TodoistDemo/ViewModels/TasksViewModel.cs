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
    public class TasksViewModel: ViewModelBase
    {
        private readonly IAccountManager _accountManager;
        private string _authToken;
        private ObservableCollection<Item> _tasks;

        public TasksViewModel(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            using (var db = new TodoistContext())
            {
                var dbItems = await db.Items.ToListAsync();
                var items = dbItems.Select(i => new Item
                {
                    Checked = i.Checked,
                    Content = i.Content                   
                }).ToList();
                Tasks = new ObservableCollection<Item>(items);
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
                using (var db = new TodoistContext())
                {
                    var items = syncData.Items.Select(i => new DbItem
                    {
                        Checked = i.Checked,
                        Content = i.Content
                    }).ToList();
                    await db.Database.ExecuteSqlCommandAsync("delete from items");
                    await db.Items.AddRangeAsync(items);
                    await db.SaveChangesAsync();
                }
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