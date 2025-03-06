using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
        {
            var todos = await _context.TodoItems.ToListAsync();

            if (todos.Count == 0)
            {
                return NotFound(new { status = 404, message = "No To-Do items found" });
            }

            return Ok(new { status = 200, message = "Success", data = todos });
        }

        // GET: api/todo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound(new { status = 404, message = $"To-Do item with ID {id} not found" });
            }

            return Ok(new { status = 200, message = "Success", data = todoItem });
        }

        // POST: api/todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrWhiteSpace(todoItem.Title))
            {
                return BadRequest(new { status = 400, message = "Title is required" });
            }

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id },
                new { status = 201, message = "To-Do item created successfully", data = todoItem });
        }

        // PUT: api/todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest(new { status = 400, message = "ID mismatch" });
            }

            if (string.IsNullOrWhiteSpace(todoItem.Title))
            {
                return BadRequest(new { status = 400, message = "Title is required" });
            }

            var existingTodo = await _context.TodoItems.FindAsync(id);
            if (existingTodo == null)
            {
                return NotFound(new { status = 404, message = $"To-Do item with ID {id} not found" });
            }

            existingTodo.Title = todoItem.Title;
            existingTodo.IsCompleted = todoItem.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { status = 500, message = "Error updating data" });
            }

            return Ok(new { status = 200, message = "To-Do item updated successfully", data = existingTodo });
        }

        // DELETE: api/todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound(new { status = 404, message = $"To-Do item with ID {id} not found" });
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok(new { status = 200, message = "To-Do item deleted successfully" });
        }
    }
}
