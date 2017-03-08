using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public interface IItemsRepository
    {
        Task<IEnumerable<BindableItem>> RetrieveItems(Expression<Func<Item, bool>> query = null);

        Task AddItems(List<BindableItem> items);
    }
}