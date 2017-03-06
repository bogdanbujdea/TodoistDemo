using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public class DbUser: IUser
    {
        public int Id { get; set; }
        public string AvatarSmall { get; set; }
        public int SortOrder { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public int CompletedCount { get; set; }
        public string AvatarMedium { get; set; }
        public string Email { get; set; }
        public string AvatarBig { get; set; }
        public string Token { get; set; }
    }
}