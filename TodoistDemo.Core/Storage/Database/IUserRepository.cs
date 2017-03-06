using System.Threading.Tasks;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public interface IUserRepository
    {
        Task<User> GetUser();

        Task SaveUser(User user);
    }
}
