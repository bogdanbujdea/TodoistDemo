using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
    }
}