using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Services
{
    public interface ITaskManager
    {
        Task<List<BindableItem>> RetrieveTasksAsync();
        Task AddTasksAsync(List<BindableItem> items);
        Task<List<BindableItem>> RetrieveTasksFromWebAsync();
        Task ToggleItem(BindableItem bindableItem);
    }
}