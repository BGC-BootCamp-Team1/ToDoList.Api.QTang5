using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.AppException;

namespace TodoItems.Core.Strategy
{
    public class MostAvailableInFiveDaysStrategy : ICreateTodoStrategy
    {
        public TodoItem Create(string description, DateOnly? dueDate, ITodosRepository todosRepository)
        {
            var mostAvailableInFiveDays = todosRepository
                .CountTodoItemsInFiveDays()
                .Select((count, index) => new { Count = count, Index = index })
                .OrderBy(x => x.Count)
                .First();
            if (mostAvailableInFiveDays.Count < Constant.MAX_TODOITEMS_PER_DUE_DATE)
            {
                return new TodoItem(description,
                    DateOnly.FromDateTime(DateTime.Today.AddDays(mostAvailableInFiveDays.Index)));
            }

            throw new ExceedMaxTodoItemsPerDueDateException();
        }
    }

}
