using System.ComponentModel.DataAnnotations;

namespace TaskHubAPI.DTOs
{
    /// <summary>
    /// DTO for partial update - status only
    /// Used for PATCH endpoint to update task status
    /// </summary>
    public class UpdateTaskStatusDTO
    {
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;
    }
}