using System;
using System.ComponentModel.DataAnnotations;

namespace TaskHubAPI.DTOs
{
    public class CreateTaskItemDTO
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
