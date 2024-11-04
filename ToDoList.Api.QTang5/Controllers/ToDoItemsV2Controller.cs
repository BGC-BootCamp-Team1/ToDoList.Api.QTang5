using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using ToDoList.Api.Models;
using ToDoList.Api.Services;
using TodoItems.Core;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class ToDoListV2Controller : ControllerBase
    {
        private readonly ILogger<ToDoListController> _logger;
        //private readonly InMemoryToDoItemsService _toDoItemsService;
        private readonly TodoItems.Core.ITodoItemsServiceV2 _toDoItemsService;
        public ToDoListV2Controller(ILogger<ToDoListController> logger, ITodoItemsServiceV2 toDoItemsService)
        {
            _logger = logger;
            //_toDoItemsService = new InMemoryToDoItemsService();
            _toDoItemsService = toDoItemsService;
        }

        [HttpPost()]
        public async Task<ActionResult> PostAsync(ToDoItemCreateRequest toDoItem)
        {
            var newTodoItem = await _toDoItemsService.CreateAsync(description: toDoItem.Description,dueDate:null,createOption: TodoItemService.CreateOptionEnum.MostAvailableInFiveDaysOption);
            return Created("", newTodoItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(string id, ToDoItemDto toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return BadRequest("ToDo Item ID in url must be equal to request body");
            }
            var UpdatedTodoItem = await _toDoItemsService.ModifyAsync(id, toDoItem.Description, null);
            return Ok(UpdatedTodoItem);
            
        }

    }
}
