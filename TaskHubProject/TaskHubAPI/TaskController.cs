using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskHubAPI.Data;
using TaskHubAPI.DTOs;
using TaskHubAPI.Models;

namespace TaskHubAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasks()
        {
            var tasks = await _context.TaskItems.Select(t => new TaskItemDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,  
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                UserId = t.UserId
            }).ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDTO>> GetTask(int id)
        {
            var t = await _context.TaskItems.FindAsync(id);
            if (t == null) return NotFound();
            return Ok(new TaskItemDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,  
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                UserId = t.UserId
            });
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDTO>> CreateTask(CreateTaskItemDTO dto)
        {
            if (!Enum.TryParse<Models.TaskStatus>(dto.Status, out var status) || !Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                return BadRequest("Invalid status or priority");
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = status,
                Priority = priority,
                DueDate = dto.DueDate,
                UserId = dto.UserId
            };
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new TaskItemDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                UserId = task.UserId
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, CreateTaskItemDTO dto)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            if (!Enum.TryParse<Models.TaskStatus>(dto.Status, out var status) || !Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                return BadRequest("Invalid status or priority");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = status;
            task.Priority = priority;
            task.DueDate = dto.DueDate;
            task.UserId = dto.UserId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
