using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ToDoListController : ControllerBase
    {
        private readonly ILogger<ToDoListController> _logger;
        //private readonly InMemoryToDoItemsService _toDoItemsService;
        private readonly IToDoItemsService _toDoItemsService;
        public ToDoListController(ILogger<ToDoListController> logger, IToDoItemsService toDoItemsService)
        {
            _logger = logger;
            //_toDoItemsService = new InMemoryToDoItemsService();
            _toDoItemsService = toDoItemsService;
        }

        [HttpPost()]
        public async Task<ActionResult> PostAsync(ToDoItemCreateRequest toDoItem)
        {
            ToDoItemDto result = new ToDoItemDto
            {
                Description = toDoItem.Description,
                Done = toDoItem.Done,
                Favorite = toDoItem.Favorite
            };
            await _toDoItemsService.CreateAsync(result);
            return Created("",result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(string id, ToDoItemDto toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return BadRequest("ToDo Item ID in url must be equal to request body");
            }
            var isCreated = await _toDoItemsService.GetAsync(id);
            if(isCreated == null)
            {
                ToDoItemDto result = new ToDoItemDto
                {
                    Description = toDoItem.Description,
                    Done = toDoItem.Done,
                    Favorite = toDoItem.Favorite
                };
                await _toDoItemsService.CreateAsync(result);
                return Created("", result);
            }
            else
            {
                await _toDoItemsService.ReplaceAsync(id, toDoItem);
                return Ok(toDoItem);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var result = await _toDoItemsService.GetAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet()]
        public async Task<ActionResult<List<ToDoItemDto>>> GetAsync()
        {
            var resluts = await _toDoItemsService.GetAllAsync();
            return Ok(resluts);
        }
        

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var isDelete = await _toDoItemsService.RemoveAsync(id);
            if (isDelete == false)
            {
                return NotFound();
            }
            else
                return NoContent();
        }


    }
}
