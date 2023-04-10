using Microsoft.AspNetCore.Mvc;
using TestEB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace TestEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        public TodoItemsController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            if (_todoContext.TodoItems == null)
            {
                return NotFound();
            }
            return await _todoContext.TodoItems
                            .Select(x => ItemToDTO(x))
                            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(int id)
        {
            if (_todoContext.TodoItems == null)
            {
                return NotFound();
            }
            var _todoItem = await _todoContext.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }
            return ItemToDTO(_todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }
            if (_todoContext.TodoItems == null)
            {
                return NotFound();
            }
            var _todoItem = await _todoContext.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }
            _todoItem.Name = todoItemDTO.Name;
            _todoItem.IsComplete = todoItemDTO.IsComplete;

            _todoContext.Entry(_todoItem).State = EntityState.Modified;

            try
            {
                await _todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            if (_todoContext.TodoItems == null)
            {
                return Problem("Entity set 'TodoContext.TodoItems' is null.");
            }
            var _todoItem = new TodoItem
            {
                Name = todoItemDTO.Name,
                IsComplete = todoItemDTO.IsComplete
            };
            _todoContext.TodoItems.Add(_todoItem);
            await _todoContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = _todoItem.Id }, ItemToDTO(_todoItem));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            if (_todoContext.TodoItems == null)
            {
                return NotFound();
            }
            var _todoItem = await _todoContext.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }
            _todoContext.TodoItems.Remove(_todoItem);
            await _todoContext.SaveChangesAsync();
            return NoContent();
        }

        private bool TodoItemExists(int id)
        {
            return (_todoContext.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem)
        {
            return new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTodoItem(int id, [FromBody] JsonPatchDocument<TodoItemDTO> patchDoc)
        {
            var _todoItem = await _todoContext.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }

            var todoItemDTO = new TodoItemDTO
            {
                Id = _todoItem.Id,
                Name = _todoItem.Name,
                IsComplete = _todoItem.IsComplete
            };

            patchDoc.ApplyTo(todoItemDTO);

            _todoItem.Name = todoItemDTO.Name;
            _todoItem.IsComplete = todoItemDTO.IsComplete;

            _todoContext.Entry(_todoItem).State = EntityState.Modified;

            try
            {
                await _todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(todoItemDTO);
        }
    }
}
