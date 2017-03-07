using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<Item>> RetrieveTasksAsync()
        {
            return await _itemsRepository.RetrieveItems();
        }

        public async Task AddTasksAsync(List<Item> items)
        {
            await _itemsRepository.AddItems(items);
        }

        public async Task<List<Item>> RetrieveTasksFromWebAsync()
        {
            var syncData = await _webSyncService.RetrieveAllItemsAsync();

            using (var db = new TodoistContext())
            {
                await _userRepository.SaveUser(syncData.User);
                var items = syncData.Items.Select(i => i.ToDbItem()).ToList();

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
            return syncData.Items;
        }
    }
}