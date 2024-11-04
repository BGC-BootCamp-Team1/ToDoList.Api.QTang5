using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    public interface ITodosRepository
    {
        public List<int> CountTodoItemsInFiveDays();
        public int CountTodoItemsByDueDate(DateOnly dueDate);
        public Task SaveAsync(TodoItem todoItem);
        public Task<TodoItem> FindByIdAsync(string? id);
    }
}
