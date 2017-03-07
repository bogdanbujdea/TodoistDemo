using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;
using TodoistDemo.Core.Services;

namespace TodoistDemo.Core.Communication.WebServices
{
    public interface IWebSyncService
    {
        Task<SyncData> RetrieveAllItemsAsync(ApiCommand command = null);
    }
}