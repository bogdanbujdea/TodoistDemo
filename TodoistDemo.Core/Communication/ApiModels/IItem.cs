namespace TodoistDemo.Core.Communication.ApiModels
{
    public interface IItem
    {
        int Id { get; set; }
        int UserId { get; set; }
        int ProjectId { get; set; }
        string Content { get; set; }
        string FormattedDate { get; set; }
        string DateLanguage { get; set; }
        int Indent { get; set; }
        int Priority { get; set; }
        int ItemOrder { get; set; }
        int DayOrder { get; set; }
        int Collapsed { get; set; }
        int? AssignedBy { get; set; }
        int Checked { get; set; }
        int InHistory { get; set; }
        int IsDeleted { get; set; }
        int IsArchived { get; set; }
        string DateAdded { get; set; }
    }
}