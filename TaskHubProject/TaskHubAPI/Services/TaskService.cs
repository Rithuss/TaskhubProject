using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHubAPI.DTOs;
using TaskHubAPI.Core.Interfaces;
using TaskHubAPI.Models;

namespace TaskHubAPI.Services
{
    /// <summary>
    /// Task service implementation demonstrating:
    /// - ABSTRACTION: Implements ITaskService interface
    /// - ENCAPSULATION: Private repositories, public business methods
    /// - SINGLE RESPONSIBILITY: Handles only task business logic
    /// - DEPENDENCY INVERSION: Depends on abstractions
    /// </summary>
    public class TaskService : ITaskService
    {
        // ENCAPSULATION: Private dependencies
        private readonly IRepository<TaskItem> _taskRepository;
        private readonly IRepository<User> _userRepository;

        /// <summary>
        /// Constructor with DEPENDENCY INJECTION
        /// Multiple dependencies demonstrate COMPOSITION
        /// </summary>
        public TaskService(IRepository<TaskItem> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        public async Task<IEnumerable<TaskItemDTO>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(MapToDTO);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        public async Task<TaskItemDTO?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            return task != null ? MapToDTO(task) : null;
        }

        /// <summary>
        /// Get tasks by user ID - demonstrates business query logic
        /// </summary>
        public async Task<IEnumerable<TaskItemDTO>> GetTasksByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.FindAsync(t => t.UserId == userId);
            return tasks.Select(MapToDTO);
        }

        /// <summary>
        /// Get tasks by status - demonstrates ENCAPSULATION of query logic
        /// </summary>
        public async Task<IEnumerable<TaskItemDTO>> GetTasksByStatusAsync(Models.TaskStatus status)
        {
            var tasks = await _taskRepository.FindAsync(t => t.Status == status);
            return tasks.Select(MapToDTO);
        }

        /// <summary>
        /// Get tasks by priority
        /// </summary>
        public async Task<IEnumerable<TaskItemDTO>> GetTasksByPriorityAsync(TaskPriority priority)
        {
            var tasks = await _taskRepository.FindAsync(t => t.Priority == priority);
            return tasks.Select(MapToDTO);
        }

        /// <summary>
        /// Get overdue tasks - demonstrates business logic
        /// Uses domain method IsOverdue() (ENCAPSULATION)
        /// </summary>
        public async Task<IEnumerable<TaskItemDTO>> GetOverdueTasksAsync()
        {
            var allTasks = await _taskRepository.GetAllAsync();
            var overdueTasks = allTasks.Where(t => t.IsOverdue());
            return overdueTasks.Select(MapToDTO);
        }

        /// <summary>
        /// Create task with validation - demonstrates business rules
        /// </summary>
        public async Task<TaskItemDTO> CreateTaskAsync(CreateTaskItemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Business rule: Validate user exists
            if (!await _userRepository.ExistsAsync(dto.UserId))
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            // Parse and validate enums
            if (!Enum.TryParse<Models.TaskStatus>(dto.Status, out var status))
                throw new ArgumentException($"Invalid status: {dto.Status}");

            if (!Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                throw new ArgumentException($"Invalid priority: {dto.Priority}");

            // Business rule: Due date should be in the future
            if (dto.DueDate < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Due date cannot be in the past.");

            // Create entity using constructor (ENCAPSULATION)
            var task = new TaskItem(dto.Title, dto.Description, dto.DueDate, dto.UserId)
            {
                Status = status,
                Priority = priority
            };

            // Validate entity
            if (!task.Validate())
                throw new InvalidOperationException("Task validation failed.");

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return MapToDTO(task);
        }

        /// <summary>
        /// Update task - demonstrates business logic and validation
        /// </summary>
        public async Task<bool> UpdateTaskAsync(int id, CreateTaskItemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return false;

            // Validate user exists
            if (!await _userRepository.ExistsAsync(dto.UserId))
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            // Parse enums
            if (!Enum.TryParse<Models.TaskStatus>(dto.Status, out var status))
                throw new ArgumentException($"Invalid status: {dto.Status}");

            if (!Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
                throw new ArgumentException($"Invalid priority: {dto.Priority}");

            // Use domain method to update (ENCAPSULATION)
            task.UpdateDetails(dto.Title, dto.Description, status, priority, dto.DueDate);
            task.UserId = dto.UserId;

            // Validate
            if (!task.Validate())
                throw new InvalidOperationException("Task validation failed.");

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Delete task
        /// </summary>
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return false;

            await _taskRepository.DeleteAsync(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Check if task exists
        /// </summary>
        public async Task<bool> TaskExistsAsync(int id)
        {
            return await _taskRepository.ExistsAsync(id);
        }

        /// <summary>
        /// Private mapping method - demonstrates ENCAPSULATION
        /// Centralizes DTO mapping logic
        /// </summary>
        private TaskItemDTO MapToDTO(TaskItem task)
        {
            return new TaskItemDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description ?? string.Empty,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                UserId = task.UserId
            };
        }
    }
}
