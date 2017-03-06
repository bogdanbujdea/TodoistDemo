using System.Collections.Generic;
using TodoistDemo.Core.Communication.ApiModels;

namespace TodoistDemo.Core.Storage.Database
{
    public class DbItem: IItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Content { get; set; }
        public string FormattedDate { get; set; }
        public string DateLanguage { get; set; }
        public int Indent { get; set; }
        public int Priority { get; set; }
        public int ItemOrder { get; set; }
        public int DayOrder { get; set; }
        public int Collapsed { get; set; }
        public int? AssignedBy { get; set; }
        public int Checked { get; set; }
        public int InHistory { get; set; }
        public int IsDeleted { get; set; }
        public int IsArchived { get; set; }
        public string DateAdded { get; set; }
    }
}