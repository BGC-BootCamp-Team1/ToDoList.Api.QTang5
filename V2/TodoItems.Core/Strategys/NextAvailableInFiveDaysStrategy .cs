using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.AppException;

namespace TodoItems.Core.Strategy
{
    public class NextAvailableInFiveDaysStrategy : ICreateTodoStrategy
    {
        public TodoItem Create(string description, DateOnly? dueDate, ITodosRepository todosRepository)
        {
            var availableDate = todosRepository
                .CountTodoItemsInFiveDays()
                .Select((count, index) => new { Count = count, Index = index })
                .FirstOrDefault(pair => pair.Count < Constant.MAX_TODOITEMS_PER_DUE_DATE);
            if (availableDate != default)
            {
                return new TodoItem(description, DateOnly.FromDateTime(DateTime.Today.AddDays(availableDate.Index)));
            }

            throw new ExceedMaxTodoItemsPerDueDateException();
        }
    }

}
