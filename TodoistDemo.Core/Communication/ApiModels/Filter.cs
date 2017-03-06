using Newtonsoft.Json;

public class Filter
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Color { get; set; }

    [JsonProperty(PropertyName = "item_order")]
    public int ItemOrder { get; set; }

    public string Query { get; set; }

    [JsonProperty(PropertyName = "is_deleted")]
    public int IsDeleted { get; set; }
}