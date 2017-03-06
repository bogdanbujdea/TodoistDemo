using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication.ApiModels;
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

        public async Task<List<Item>> RetrieveTasksFromWebAsync(string token)
        {
            var syncData = await _webSyncService.RetrieveAllItemsAsync(token);

            using (var db = new TodoistContext())
            {
                var user = await db.Users.FirstOrDefaultAsync();
                /*if (user == null)
                {
                    await db.Database.ExecuteSqlCommandAsync("delete from items");
                    user = new DbUser();
                }*/
                await db.Database.ExecuteSqlCommandAsync("delete from items");
                user = new DbUser();
                var items = syncData.Items.Select(i => i.ToDbItem()).ToList();
                user.FullName = syncData.User.FullName;
                user.AvatarBig = syncData.User.AvatarBig;
                user.Token = syncData.User.Token;
                await db.Items.AddRangeAsync(items);
                await _userRepository.SaveUser(syncData.User);
                await db.SaveChangesAsync();
            }
            return await _itemsRepository.RetrieveItems();
        }


        /* private bool FirstTimeUse(TodoistContext db)
        {
            return !db.Items.Any() || !db.Users.Any();
        }*/
    }
}
