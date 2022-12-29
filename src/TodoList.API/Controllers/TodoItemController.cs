using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.API.DataTransferObjects;
using TodoList.API.Infrastructure.Database;
using TodoList.API.Models;

namespace TodoList.API.Controllers
{
    [Route("api/todoitem")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            return await _context.TodoItems
                .Select(todoItem => ModelToDto(todoItem))
                .ToListAsync();
        }

        // GET: api/TodoItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ModelToDto(todoItem);
        }

        // PUT: api/TodoItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDto request)
        {
            var todoItemModel = DtoToModel(request);

            if (id != todoItemModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItemModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/TodoItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto request)
        {
            var todoItemModel = DtoToModel(request);

            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'TodoContext.TodoItems'  is null.");
            }
            _context.TodoItems.Add(todoItemModel);
            await _context.SaveChangesAsync();

            var response = ModelToDto(todoItemModel);

            return CreatedAtAction(nameof(GetTodoItem), new { id = response.Id }, response);
        }

        // DELETE: api/TodoItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private TodoItem DtoToModel(TodoItemDto request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new TodoItem()
            {
                Id = request.Id,
                Name = request.Name,
                IsCompleted = request.IsCompleted
            };
        }

        private TodoItemDto ModelToDto(TodoItem model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new TodoItemDto()
            {
                Id = model.Id,
                Name = model.Name,
                IsCompleted = model.IsCompleted
            };
        }
    }
}
