using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public static class DbModelExtensions
    {
        public static User ToUser(this DbUser dbUser)
        {
            return new User
            {
                Id = dbUser.Id,
                AvatarBig = dbUser.AvatarBig,
                FullName = dbUser.FullName,
                Token = dbUser.Token
            };
        }

        public static DbUser ToDbUser(this User user)
        {
            return new DbUser
            {
                Id = user.Id,
                AvatarBig = user.AvatarBig,
                FullName = user.FullName,
                Token = user.Token
            };
        }

        public static DbItem ToDbItem(this Item item)
        {
            return new DbItem
            {
                Id = item.Id,
                Checked = item.Checked,
                Content = item.Content
            };
        }

        public static Item ToItem(this DbItem dbItem)
        {
            return new Item
            {
                Id = dbItem.Id,
                Checked = dbItem.Checked,
                Content = dbItem.Content
            };
        }
    }
}
