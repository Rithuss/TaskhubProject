using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskHubAPI.Models;

namespace TaskHubAPI.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface demonstrating ABSTRACTION and INTERFACE SEGREGATION
    /// Defines contract for data access operations
    /// Supports DEPENDENCY INVERSION PRINCIPLE - depend on abstraction, not concrete implementation
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get all entities asynchronously
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Find entities matching a predicate - demonstrates LINQ and functional programming
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Update existing entity
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Check if entity exists
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Save all changes to database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
