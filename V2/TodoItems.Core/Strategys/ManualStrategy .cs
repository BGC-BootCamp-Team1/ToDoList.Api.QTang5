using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.AppException;

namespace TodoItems.Core.Strategy
{
    public class ManualStrategy : ICreateTodoStrategy
    {
        public TodoItem Create(string description, DateOnly? dueDate, ITodosRepository todosRepository)
        {
            var todoItemCount = todosRepository.CountTodoItemsByDueDate(dueDate.Value);
            var isValidDueDate = dueDate >= DateOnly.FromDateTime(DateTime.Today);
            if (!isValidDueDate)
                throw new InvalidDueDateException();
            if (todoItemCount < Constant.MAX_TODOITEMS_PER_DUE_DATE)
            {
                var newTodoItem = new TodoItem(description, dueDate.Value);
                return newTodoItem;
            }
            else
            {
                throw new ExceedMaxTodoItemsPerDueDateException();
            }
        }
    }
}
