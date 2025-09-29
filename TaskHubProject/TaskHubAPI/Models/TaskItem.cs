using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskHubAPI.Models
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;   

        public string? Description { get; set; }            

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }   
    }
}
