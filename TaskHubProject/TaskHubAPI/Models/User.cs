using System.ComponentModel.DataAnnotations;

namespace TaskHubAPI.Models
{
    /// <summary>
    /// User entity demonstrating INHERITANCE from BaseEntity
    /// Inherits common properties (Id, CreatedAt, UpdatedAt) and methods (Validate, MarkAsUpdated)
    /// Demonstrates ENCAPSULATION with data validation attributes
    /// </summary>
    public class User : BaseEntity
    {
        // ENCAPSULATION: Private backing field with public property
        private string _name = string.Empty;
        private string _email = string.Empty;

        [Required]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? string.Empty; // Automatic trimming
        }

        [Required]
        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty; // Normalize email
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public User() : base()
        {
        }

        /// <summary>
        /// Parameterized constructor demonstrating ENCAPSULATION
        /// </summary>
        public User(string name, string email) : base()
        {
            Name = name;
            Email = email;
        }

        /// <summary>
        /// Override Validate method - demonstrates POLYMORPHISM
        /// Provides User-specific validation logic
        /// </summary>
        public override bool Validate()
        {
            return base.Validate() && 
                   !string.IsNullOrWhiteSpace(Name) && 
                   !string.IsNullOrWhiteSpace(Email) &&
                   Email.Contains("@");
        }

        /// <summary>
        /// Business logic method - demonstrates ENCAPSULATION
        /// Updates user information
        /// </summary>
        public void UpdateInfo(string name, string email)
        {
            Name = name;
            Email = email;
            MarkAsUpdated();
        }
    }
}
