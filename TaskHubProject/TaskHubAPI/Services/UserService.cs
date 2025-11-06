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
    /// User service implementation demonstrating:
    /// - ABSTRACTION: Implements IUserService interface
    /// - ENCAPSULATION: Private repository field, public methods
    /// - SINGLE RESPONSIBILITY: Handles only user business logic
    /// - DEPENDENCY INVERSION: Depends on IRepository abstraction
    /// </summary>
    public class UserService : IUserService
    {
        // ENCAPSULATION: Private field with interface dependency
        private readonly IRepository<User> _userRepository;

        /// <summary>
        /// Constructor with DEPENDENCY INJECTION
        /// Demonstrates DEPENDENCY INVERSION PRINCIPLE
        /// </summary>
        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Get all users - demonstrates ABSTRACTION
        /// Business logic layer abstracts data access details
        /// </summary>
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDTO);
        }

        /// <summary>
        /// Get user by ID with null handling
        /// </summary>
        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDTO(user) : null;
        }

        /// <summary>
        /// Get user by email - demonstrates business logic in service layer
        /// </summary>
        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var users = await _userRepository.FindAsync(u => u.Email == email.ToLowerInvariant());
            var user = users.FirstOrDefault();
            return user != null ? MapToDTO(user) : null;
        }

        /// <summary>
        /// Create user with validation - demonstrates ENCAPSULATION of business rules
        /// </summary>
        public async Task<UserDTO> CreateUserAsync(CreateUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Business rule: Check if email already exists
            if (await EmailExistsAsync(dto.Email))
                throw new InvalidOperationException($"Email '{dto.Email}' is already registered.");

            // Create entity using constructor (ENCAPSULATION)
            var user = new User(dto.Name, dto.Email);

            // Validate entity
            if (!user.Validate())
                throw new InvalidOperationException("User validation failed.");

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToDTO(user);
        }

        /// <summary>
        /// Update user - demonstrates business logic and validation
        /// </summary>
        public async Task<bool> UpdateUserAsync(int id, CreateUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Business rule: Check if email is taken by another user
            var existingUser = await GetUserByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != id)
                throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

            // Use domain method (ENCAPSULATION)
            user.UpdateInfo(dto.Name, dto.Email);

            // Validate
            if (!user.Validate())
                throw new InvalidOperationException("User validation failed.");

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Delete user
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Check if user exists
        /// </summary>
        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        /// <summary>
        /// Check if email exists - business logic method
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var users = await _userRepository.FindAsync(u => u.Email == email.ToLowerInvariant());
            return users.Any();
        }

        /// <summary>
        /// Private mapping method - demonstrates ENCAPSULATION
        /// Centralizes DTO mapping logic
        /// </summary>
        private UserDTO MapToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
