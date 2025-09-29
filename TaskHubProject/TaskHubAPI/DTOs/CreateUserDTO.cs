using System.ComponentModel.DataAnnotations;

namespace TaskHubAPI.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
