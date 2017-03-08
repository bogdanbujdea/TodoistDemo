using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ReactiveUI;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.Core.Services
{
    public interface ITaskManager
    {
        Task<IEnumerable<BindableItem>> RetrieveTasksFromDbAsync(Expression<Func<Item, bool>> exp);
        Task AddTasksAsync(List<BindableItem> items);
        Task<List<BindableItem>> RetrieveTasksFromWebAsync();
        Task<List<BindableItem>> ToggleItems(List<BindableItem> items);
        Task UpdateItems(ReactiveList<BindableItem> displayedItems, List<BindableItem> syncedItems = null);
        bool CompletedItemsAreVisible { get; set; }
        ReactiveList<BindableItem> Items { get; set; }
    }
}