using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public class ItemsRepository : IItemsRepository
    {
        public async Task<List<BindableItem>> RetrieveItems()
        {
            using (var db = new TodoistContext())
            {
                var dbItems = await db.Items.ToListAsync();
                var items = dbItems.Select(item => item.ToItem()).ToList();
                return items;
            }
        }

        public async Task AddItems(List<BindableItem> items)
        {
            using (var db = new TodoistContext())
            {
                await db.Items.AddRangeAsync(items.Select(i => i.ToDbItem()));
                await db.SaveChangesAsync();                
            }
        }
    }
}
