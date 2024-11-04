namespace TodoItems.Core;

public partial class TodoItemService
{
    public enum CreateOptionEnum
    {
        ManualOption = 0,
        NextAvailableInFiveDaysOption,
        MostAvailableInFiveDaysOption
    }
}

