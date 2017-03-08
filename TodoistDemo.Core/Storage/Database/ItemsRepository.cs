using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public class ItemsRepository : IItemsRepository
    {
        public async Task<IEnumerable<BindableItem>> RetrieveItems(Expression<Func<Item, bool>> query)
        {
            using (var db = new TodoistContext())
            {
                var dbItems = await db.Items.Where(item => string.IsNullOrWhiteSpace(item.Content) == false).Where(query).ToListAsync();
                var items = dbItems.Select(item => item
                            .ToBindableItem())
                            .OrderBy(item => item.Content.ToLower());
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