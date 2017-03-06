using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Communication.WebServices
{
    public interface IWebSyncService
    {
        Task<SyncData> RetrieveAllItemsAsync();
    }
}