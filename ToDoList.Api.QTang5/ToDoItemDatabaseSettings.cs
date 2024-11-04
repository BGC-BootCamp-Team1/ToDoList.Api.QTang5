namespace ToDoList.Api
{
    public class ToDoItemDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string TodoItemsCollectionName { get; set; } = null!;
    }
}
