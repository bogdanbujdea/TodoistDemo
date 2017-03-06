using Newtonsoft.Json;

public class Project
{
    public int Id { get; set; }

    public int Color { get; set; }

    public int Collapsed { get; set; }

    [JsonProperty(PropertyName = "inbox_project")]
    public bool InboxProject { get; set; }

    public int Indent { get; set; }

    [JsonProperty(PropertyName = "is_deleted")]
    public int IsDeleted { get; set; }

    public string Name { get; set; }

    [JsonProperty(PropertyName = "item_order")]
    public int ItemOrder { get; set; }
}