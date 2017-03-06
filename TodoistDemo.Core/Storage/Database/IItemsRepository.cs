using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public interface IItemsRepository
    {
        Task<List<Item>> RetrieveItems();
        Task AddItems(List<Item> items);
    }
}