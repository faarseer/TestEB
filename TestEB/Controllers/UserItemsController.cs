using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestEB.Models;

namespace TestEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserItemsController: ControllerBase
    {
        private readonly TodoContext _todoContext;
        public UserItemsController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItem>>> GetUserItems()
        {
            if (_todoContext.UserItems== null)
            {
                return NotFound();
            }
            return await _todoContext.UserItems
                            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserItem>> GetUserItem(int id)
        {
            if (_todoContext.UserItems== null)
            {
                return NotFound();
            }
            var _userItem = await _todoContext.UserItems.FindAsync(id);
            if (_userItem == null)
            {
                return NotFound();
            }
            return _userItem;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserItem(int id, UserItem userItem)
        {
            if (id != userItem.Id)
            {
                return BadRequest();
            }
            if (_todoContext.UserItems== null)
            {
                return NotFound();
            }
            var _userItem = await _todoContext.UserItems.FindAsync(id);
            if (_userItem == null)
            {
                return NotFound();
            }
            _userItem.Name = userItem.Name;
            _userItem.Age = userItem.Age;

            _todoContext.Entry(_userItem).State = EntityState.Modified;

            try
            {
                await _todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(id))
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
        public async Task<ActionResult<UserItem>> PostUserItem(UserItem userItem)
        {
            if (_todoContext.UserItems== null)
            {
                return Problem("Entity set 'TodoContext.UserItemSet' is null.");
            }
            var _userItem = new UserItem
            {
                Name = userItem.Name,
                Age = userItem.Age
            };
            _todoContext.UserItems.Add(_userItem);
            await _todoContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserItem), new { id = _userItem.Id }, _userItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserItem(int id)
        {
            if (_todoContext.UserItems== null)
            {
                return NotFound();
            }
            var _userItem = await _todoContext.UserItems.FindAsync(id);
            if (_userItem == null)
            {
                return NotFound();
            }
            _todoContext.UserItems.Remove(_userItem);
            await _todoContext.SaveChangesAsync();
            return NoContent();
        }

        private bool UserItemExists(int id)
        {
            return (_todoContext.UserItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
