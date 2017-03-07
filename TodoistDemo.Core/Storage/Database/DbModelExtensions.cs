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

        public static Item ToDbItem(this BindableItem bindableItem)
        {
            return new Item
            {
                Id = bindableItem.Id,
                Checked = bindableItem.Checked,
                Content = bindableItem.Content
            };
        }

        public static BindableItem ToItem(this Item item)
        {
            return new BindableItem
            {
                Id = item.Id,
                Checked = item.Checked,
                Content = item.Content
            };
        }
    }
}
