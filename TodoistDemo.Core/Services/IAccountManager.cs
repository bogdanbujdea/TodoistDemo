using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Services
{
    public interface IAccountManager
    {
        Task<SyncData> LoginAsync(string token);
    }
}