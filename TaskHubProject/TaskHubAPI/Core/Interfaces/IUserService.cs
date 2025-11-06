using System.Collections.Generic;
using System.Threading.Tasks;
using TaskHubAPI.DTOs;
using TaskHubAPI.Models;

namespace TaskHubAPI.Core.Interfaces
{
    /// <summary>
    /// User service interface demonstrating INTERFACE SEGREGATION PRINCIPLE
    /// Defines business logic contract for user operations
    /// Supports DEPENDENCY INVERSION - controllers depend on this abstraction
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();

        /// <summary>
        /// Get user by ID
        /// </summary>
        Task<UserDTO?> GetUserByIdAsync(int id);

        /// <summary>
        /// Get user by email - business logic method
        /// </summary>
        Task<UserDTO?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Create new user with validation
        /// </summary>
        Task<UserDTO> CreateUserAsync(CreateUserDTO dto);

        /// <summary>
        /// Update existing user
        /// </summary>
        Task<bool> UpdateUserAsync(int id, CreateUserDTO dto);

        /// <summary>
        /// Delete user
        /// </summary>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Check if user exists
        /// </summary>
        Task<bool> UserExistsAsync(int id);

        /// <summary>
        /// Check if email is already taken
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
}
