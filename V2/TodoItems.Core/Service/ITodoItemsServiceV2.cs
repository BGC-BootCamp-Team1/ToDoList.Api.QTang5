
namespace TodoItems.Core
{
    public interface ITodoItemsServiceV2
    {
        Task<TodoItem> CreateAsync(string description, DateOnly? dueDate, TodoItemService.CreateOptionEnum createOption);
        Task<TodoItem>? ModifyAsync(string id, string? description, DateOnly? dueDate);
    }
}