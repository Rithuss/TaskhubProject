using Microsoft.AspNetCore.Mvc;
using TaskHubAPI.DTOs;
using TaskHubAPI.Core.Interfaces;
using TaskHubAPI.Models;

namespace TaskHubAPI.Controllers
{
    /// <summary>
    /// Task controller demonstrating:
    /// - DEPENDENCY INVERSION: Depends on ITaskService abstraction
    /// - SINGLE RESPONSIBILITY: Only handles HTTP concerns
    /// - SEPARATION OF CONCERNS: Business logic in service layer
    /// </summary>
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        // ENCAPSULATION: Private dependencies
        private readonly ITaskService _taskService;
        private readonly IValidationStrategy<CreateTaskItemDTO> _validationStrategy;

        /// <summary>
        /// Constructor with DEPENDENCY INJECTION
        /// Demonstrates DEPENDENCY INVERSION PRINCIPLE
        /// </summary>
        public TaskController(
            ITaskService taskService,
            IValidationStrategy<CreateTaskItemDTO> validationStrategy)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _validationStrategy = validationStrategy ?? throw new ArgumentNullException(nameof(validationStrategy));
        }

        /// <summary>
        /// GET: api/tasks
        /// Get all tasks - demonstrates service layer abstraction
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/tasks/{id}
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDTO>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound(new { message = $"Task with ID {id} not found." });

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the task.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/tasks/user/{userId}
        /// Get tasks by user - demonstrates business query in service layer
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasksByUser(int userId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByUserIdAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/tasks/status/{status}
        /// Get tasks by status - demonstrates enum parameter binding
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasksByStatus(Models.TaskStatus status)
        {
            try
            {
                var tasks = await _taskService.GetTasksByStatusAsync(status);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/tasks/priority/{priority}
        /// Get tasks by priority
        /// </summary>
        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasksByPriority(TaskPriority priority)
        {
            try
            {
                var tasks = await _taskService.GetTasksByPriorityAsync(priority);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/tasks/overdue
        /// Get overdue tasks - demonstrates business logic in service
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetOverdueTasks()
        {
            try
            {
                var tasks = await _taskService.GetOverdueTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving overdue tasks.", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/tasks
        /// Create task - demonstrates STRATEGY PATTERN for validation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskItemDTO>> CreateTask(CreateTaskItemDTO dto)
        {
            try
            {
                // Use validation strategy (STRATEGY PATTERN)
                var validationResult = _validationStrategy.Validate(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { message = "Validation failed.", errors = validationResult.Errors });

                var task = await _taskService.CreateTaskAsync(dto);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the task.", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/tasks/{id}
        /// Update task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, CreateTaskItemDTO dto)
        {
            try
            {
                // Validate using strategy
                var validationResult = _validationStrategy.Validate(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { message = "Validation failed.", errors = validationResult.Errors });

                var success = await _taskService.UpdateTaskAsync(id, dto);
                if (!success)
                    return NotFound(new { message = $"Task with ID {id} not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task.", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/tasks/{id}
        /// Delete task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var success = await _taskService.DeleteTaskAsync(id);
                if (!success)
                    return NotFound(new { message = $"Task with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task.", error = ex.Message });
            }
        }
    }
}
