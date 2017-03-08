using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ReactiveUI;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Communication.WebServices;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.Core.Services
{
    public class TaskManager : ITaskManager
    {
        private readonly IItemsRepository _itemsRepository;
        private readonly IWebSyncService _webSyncService;
        private readonly IUserRepository _userRepository;

        public TaskManager(IItemsRepository itemsRepository, IWebSyncService webSyncService, IUserRepository userRepository)
        {
            _itemsRepository = itemsRepository;
            _webSyncService = webSyncService;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BindableItem>> RetrieveTasksFromDbAsync(Expression<Func<Item, bool>> exp)
        {
            return await _itemsRepository.RetrieveItems(exp);
        }

        public async Task AddTasksAsync(List<BindableItem> items)
        {
            await _itemsRepository.AddItems(items);
        }

        public async Task<List<BindableItem>> RetrieveTasksFromWebAsync()
        {
            var syncData = await _webSyncService.RetrieveAllItemsAsync();

            await UpdateDatabase(syncData);
            return syncData.Items.Select(item => item.ToBindableItem()).ToList();
        }

        private async Task UpdateDatabase(SyncData syncData)
        {
            using (var db = new TodoistContext())
            {
                await _userRepository.SaveUser(syncData.User);
                var items = syncData.Items;

                foreach (var dbItem in items)
                {
                    if (db.Items.Any(i => i.Id == dbItem.Id))
                    {
                        if (string.IsNullOrWhiteSpace(dbItem.Content))
                        {
                            db.Items.Remove(dbItem);
                        }
                        else
                            db.Items.Update(dbItem);
                    }
                    else
                    {
                        db.Items.Add(dbItem);
                    }
                }
                await db.SaveChangesAsync();
            }
        }
        
        public async Task<List<BindableItem>> ToggleItems(List<BindableItem> items)
        {
            var commands = new List<ApiCommand>();
            foreach (var bindableItem in items)
            {
                var command = new ApiCommand();
                if (bindableItem.Checked)
                {
                    command.Type = "item_complete";
                }
                else command.Type = "item_uncomplete";
                command.Args = new CommandArguments { Ids = new List<int> { bindableItem.Id } };
                commands.Add(command);
            }
            
            var syncData = await _webSyncService.RetrieveAllItemsAsync(commands);
            await UpdateDatabase(syncData);
            return syncData.Items.Select(i => i.ToBindableItem()).ToList();
        }

        public async Task UpdateItems(ReactiveList<BindableItem> displayedItems, List<BindableItem> syncedItems = null)
        {
            if (displayedItems.Count == 0)
            {
                Expression<Func<Item, bool>> exp = item => TaskIsVisible(item.ToBindableItem());
                var storedTasks = (await RetrieveTasksFromDbAsync(exp));
                displayedItems.AddRange(storedTasks.Where(TaskIsVisible).OrderBy(i => i.Content.ToLower()));
            }
            var items = syncedItems ?? await RetrieveTasksFromWebAsync();
            RemoveItems(displayedItems, items);
            AddItems(displayedItems, items);
        }

        public bool CompletedItemsAreVisible { get; set; }
        public ReactiveList<BindableItem> Items { get; set; }

        private void AddItems(ReactiveList<BindableItem> displayedItems, List<BindableItem> itemsToAdd)
        {
            var visibleItems = GetItemsToInsert(itemsToAdd).Distinct();
            foreach (var item in visibleItems)
            {
                var existingItem = displayedItems.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    var index = displayedItems.IndexOf(existingItem);
                    displayedItems[index] = item;
                }
                else Insert(item, displayedItems);
            }
        }

        private void RemoveItems(ReactiveList<BindableItem> displayedItems, List<BindableItem> items)
        {
            foreach (var item in GetItemsToRemove(items))
            {
                var existingItem = displayedItems.FirstOrDefault(i => i.Id == item.Id);
                displayedItems.Remove(existingItem);
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

        private bool TaskIsVisible(BindableItem item)
        {
            return (item.Checked == CompletedItemsAreVisible) && !string.IsNullOrWhiteSpace(item.Content);
        }
    }
}