using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public async Task<List<BindableItem>> RetrieveTasksAsync()
        {
            return await _itemsRepository.RetrieveItems();
        }

        public async Task AddTasksAsync(List<BindableItem> items)
        {
            await _itemsRepository.AddItems(items);
        }

        public async Task<List<BindableItem>> RetrieveTasksFromWebAsync()
        {
            var syncData = await _webSyncService.RetrieveAllItemsAsync();

            await UpdateDatabase(syncData);
            return syncData.Items.Select(item => item.ToItem()).ToList();
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

        public async Task ToggleItem(BindableItem bindableItem)
        {
            var command = new ApiCommand();
            if (bindableItem.Checked)
            {
                command.Type = "item_complete";
            }
            else command.Type = "item_uncomplete";
            command.Args = new CommandArguments {Ids = new List<int> {bindableItem.Id} };
            var syncData = await _webSyncService.RetrieveAllItemsAsync(command);
            await UpdateDatabase(syncData);
        }
    }

    public class ApiCommand
    {
        public ApiCommand()
        {
            Id = Guid.NewGuid().ToString();
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "args")]
        public CommandArguments Args{ get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string Id { get; set; }


    }

    public class CommandArguments
    {
        [JsonProperty(PropertyName = "ids")]
        public List<int> Ids { get; set; }
    }
}