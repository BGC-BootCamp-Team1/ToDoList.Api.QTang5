namespace ToDoList.Api.Models
{
    public class ToDoItemDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; }
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
        public bool Favorite { get; set; }
        public MongoDBToDoItem ConvertToDB()
        {
            return new MongoDBToDoItem()
            {
                Id = this.Id,
                CreatedTime = this.CreatedTime,
                Favorite = this.Favorite,
                Description = this.Description,
                Done = this.Done
            };
        }
    }
}