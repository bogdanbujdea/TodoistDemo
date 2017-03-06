using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Services
{
    public interface ITaskManager
    {
        Task<List<Item>> RetrieveTasksAsync();
        Task AddTasksAsync(List<Item> items);
        Task<List<Item>> RetrieveTasksFromWebAsync();
    }
}