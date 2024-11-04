namespace ToDoList.Api.Models
{
    public class ToDoItemCreateRequest
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; }
        //public DateTimeOffset CreatedTime { get; set; }
        public bool Favorite { get; set; }
    }
}