using System;
using System.ComponentModel.DataAnnotations;

namespace TaskHubAPI.Models
{
    /// <summary>
    /// Abstract base class for all entities demonstrating INHERITANCE and ABSTRACTION
    /// Provides common properties and behavior for all domain entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Primary key - common to all entities
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Entity creation timestamp - demonstrates ENCAPSULATION with private setter
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Constructor - ensures CreatedAt is set when entity is created
        /// Demonstrates ENCAPSULATION - controlled initialization
        /// </summary>
        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Method to update the timestamp - demonstrates ENCAPSULATION
        /// Controlled access to UpdatedAt property
        /// </summary>
        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Virtual method for validation - demonstrates POLYMORPHISM
        /// Derived classes can override to provide specific validation
        /// </summary>
        /// <returns>True if entity is valid</returns>
        public virtual bool Validate()
        {
            return Id >= 0;
        }
    }
}
