using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public class UserRepository : IUserRepository
    {
        public async Task<User> GetUser()
        {
            using (var db = new TodoistContext())
            {
                var user = await db.Users.Select(u => u.ToUser()).FirstOrDefaultAsync();
                return user;
            }
        }

        public async Task SaveUser(User user)
        {
            if (user == null)
                return;
            using (var db = new TodoistContext())
            {
                await db.Database.ExecuteSqlCommandAsync("delete from users");
                db.Users.Add(user.ToDbUser());
                await db.SaveChangesAsync();
            }
        }
    }
}