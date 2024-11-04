using TodoItems.Core.AppException;
using TodoItems.Core.Strategy;
namespace TodoItems.Core;

public partial class TodoItemService : ITodoItemsServiceV2
{
    private ITodosRepository _todosRepository;
    private Dictionary<CreateOptionEnum, ICreateTodoStrategy> _strategies;
    public TodoItemService(ITodosRepository todosRepository)
    {
        _todosRepository = todosRepository;
        _strategies = new Dictionary<CreateOptionEnum, ICreateTodoStrategy>
        {
            { CreateOptionEnum.ManualOption, new ManualStrategy() },
            { CreateOptionEnum.NextAvailableInFiveDaysOption, new NextAvailableInFiveDaysStrategy() },
            { CreateOptionEnum.MostAvailableInFiveDaysOption, new MostAvailableInFiveDaysStrategy() }
        };
    }
    public async Task<TodoItem> CreateAsync(string description, DateOnly? dueDate, CreateOptionEnum createOption)
    {
        if (dueDate != null)
            createOption = CreateOptionEnum.ManualOption;

        var strategy = _strategies[createOption];
        var newTodoItem = strategy.Create(description, dueDate, _todosRepository);

        /*        TodoItems.Add(newTodoItem);
        */
        await _todosRepository.SaveAsync(newTodoItem);
        return newTodoItem;
    }
    public async Task<TodoItem>? ModifyAsync(string id, string? description, DateOnly? dueDate)
    {
        if (description == null && dueDate == null)
            return null;
        var todoItem = await _todosRepository.FindByIdAsync(id);
        if (todoItem == null)
            return null;
        if (dueDate != null)
            todoItem.DueDate = dueDate.Value;
        if (description != null)
            todoItem.ModifyItem(description);
        await _todosRepository.SaveAsync(todoItem);
        return todoItem;

    }
}

