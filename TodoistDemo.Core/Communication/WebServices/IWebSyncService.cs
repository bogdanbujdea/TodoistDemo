using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Services
{
    public interface IWebSyncService
    {
        Task<SyncData> RetrieveAllItemsAsync(string token);
    }
}