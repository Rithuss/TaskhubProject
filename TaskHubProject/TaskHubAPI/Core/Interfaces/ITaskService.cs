using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskHubAPI.DTOs;
using TaskHubAPI.Models;

namespace TaskHubAPI.Core.Interfaces
{
    /// <summary>
    /// Task service interface demonstrating INTERFACE SEGREGATION PRINCIPLE
    /// Defines business logic contract for task operations
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Get all tasks
        /// </summary>
        Task<IEnumerable<TaskItemDTO>> GetAllTasksAsync();

        /// <summary>
        /// Get task by ID
        /// </summary>
        Task<TaskItemDTO?> GetTaskByIdAsync(int id);

        /// <summary>
        /// Get tasks by user ID
        /// </summary>
        Task<IEnumerable<TaskItemDTO>> GetTasksByUserIdAsync(int userId);

        /// <summary>
        /// Get tasks by status
        /// </summary>
        Task<IEnumerable<TaskItemDTO>> GetTasksByStatusAsync(Models.TaskStatus status);

        /// <summary>
        /// Get tasks by priority
        /// </summary>
        Task<IEnumerable<TaskItemDTO>> GetTasksByPriorityAsync(TaskPriority priority);

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        Task<IEnumerable<TaskItemDTO>> GetOverdueTasksAsync();

        /// <summary>
        /// Create new task with validation
        /// </summary>
        Task<TaskItemDTO> CreateTaskAsync(CreateTaskItemDTO dto);

        /// <summary>
        /// Update existing task
        /// </summary>
        Task<bool> UpdateTaskAsync(int id, CreateTaskItemDTO dto);

        /// <summary>
        /// Delete task
        /// </summary>
        Task<bool> DeleteTaskAsync(int id);

        /// <summary>
        /// Check if task exists
        /// </summary>
        Task<bool> TaskExistsAsync(int id);
    }
}
