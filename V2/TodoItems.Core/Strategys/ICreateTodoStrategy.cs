using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Strategy
{
    public interface ICreateTodoStrategy
    {
        TodoItem Create(string description, DateOnly? dueDate, ITodosRepository todosRepository);
    }
}
