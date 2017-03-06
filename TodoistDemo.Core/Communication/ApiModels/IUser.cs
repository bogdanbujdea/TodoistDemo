namespace TodoistDemo.Core.Communication.ApiModels
{
    public interface IUser
    {
        int Id { get; set; }
        string AvatarSmall { get; set; }
        int SortOrder { get; set; }
        string FullName { get; set; }
        string Avatar { get; set; }
        int CompletedCount { get; set; }
        string AvatarMedium { get; set; }
        string Email { get; set; }
        string AvatarBig { get; set; }
        string Token { get; set; }
    }
}