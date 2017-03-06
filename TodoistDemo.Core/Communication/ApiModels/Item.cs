using Newtonsoft.Json;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class Item : IItem
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "project_id")]
        public int ProjectId { get; set; }

        public string Content { get; set; }

        [JsonProperty(PropertyName = "date_string")]
        public string FormattedDate { get; set; }

        [JsonProperty(PropertyName = "date_lang")]
        public string DateLanguage { get; set; }

        [JsonProperty(PropertyName = "due_date_utc")]
        public object DueDate { get; set; }

        public int Indent { get; set; }

        public int Priority { get; set; }

        [JsonProperty(PropertyName = "item_order")]
        public int ItemOrder { get; set; }

        [JsonProperty(PropertyName = "day_order")]
        public int DayOrder { get; set; }

        public int Collapsed { get; set; }

        public object Children { get; set; }

        public int[] Labels { get; set; }

        [JsonProperty(PropertyName = "assigned_by_uid")]
        public int? AssignedBy { get; set; }

        public int Checked { get; set; }

        [JsonProperty(PropertyName = "in_history")]
        public int InHistory { get; set; }

        [JsonProperty(PropertyName = "is_deleted")]
        public int IsDeleted { get; set; }

        [JsonProperty(PropertyName = "is_archived")]
        public int IsArchived { get; set; }

        [JsonProperty(PropertyName = "sync_id")]
        public object SyncId { get; set; }

        [JsonProperty(PropertyName = "date_added")]
        public string DateAdded { get; set; }
    }
}