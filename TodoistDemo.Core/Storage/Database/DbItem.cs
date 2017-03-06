namespace TodoistDemo.Core.Storage.Database
{
    public class DbItem
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool Checked { get; set; }
    }
}