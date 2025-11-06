using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskHubAPI.Models
{
    /// <summary>
    /// Task status enumeration - demonstrates ENCAPSULATION
    /// Restricts status values to valid options
    /// </summary>
    public enum TaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2
    }

    /// <summary>
    /// Task priority enumeration - demonstrates ENCAPSULATION
    /// </summary>
    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    /// <summary>
    /// TaskItem entity demonstrating INHERITANCE from BaseEntity
    /// Demonstrates multiple OOP concepts:
    /// - INHERITANCE: Extends BaseEntity
    /// - ENCAPSULATION: Private fields, public properties, validation
    /// - POLYMORPHISM: Overrides Validate method
    /// - ABSTRACTION: Hides complexity of task management
    /// </summary>
    public class TaskItem : BaseEntity
    {
        // ENCAPSULATION: Private backing fields
        private string _title = string.Empty;
        private string? _description;
        private TaskStatus _status;
        private TaskPriority _priority;

        [Required]
        public string Title
        {
            get => _title;
            set => _title = value?.Trim() ?? string.Empty;
        }

        public string? Description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        [Required]
        public TaskStatus Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    MarkAsUpdated();
                }
            }
        }

        [Required]
        public TaskPriority Priority
        {
            get => _priority;
            set
            {
                if (value != _priority)
                {
                    _priority = value;
                    MarkAsUpdated();
                }
            }
        }

        [Required]
        public DateTime DueDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property - demonstrates COMPOSITION
        public User? User { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskItem() : base()
        {
            Status = TaskStatus.Pending;
            Priority = TaskPriority.Medium;
        }

        /// <summary>
        /// Parameterized constructor demonstrating ENCAPSULATION
        /// </summary>
        public TaskItem(string title, string? description, DateTime dueDate, int userId) : base()
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            UserId = userId;
            Status = TaskStatus.Pending;
            Priority = TaskPriority.Medium;
        }

        /// <summary>
        /// Override Validate method - demonstrates POLYMORPHISM
        /// Provides TaskItem-specific validation
        /// </summary>
        public override bool Validate()
        {
            return base.Validate() &&
                   !string.IsNullOrWhiteSpace(Title) &&
                   UserId > 0 &&
                   DueDate > DateTime.MinValue;
        }

        /// <summary>
        /// Business logic method - check if task is overdue
        /// Demonstrates ENCAPSULATION of business logic
        /// </summary>
        public bool IsOverdue()
        {
            return Status != TaskStatus.Completed && DueDate < DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic method - mark task as completed
        /// Demonstrates ENCAPSULATION
        /// </summary>
        public void MarkAsCompleted()
        {
            Status = TaskStatus.Completed;
            MarkAsUpdated();
        }

        /// <summary>
        /// Business logic method - update task details
        /// Demonstrates ENCAPSULATION
        /// </summary>
        public void UpdateDetails(string title, string? description, TaskStatus status, TaskPriority priority, DateTime dueDate)
        {
            Title = title;
            Description = description;
            Status = status;
            Priority = priority;
            DueDate = dueDate;
            MarkAsUpdated();
        }
    }
}
