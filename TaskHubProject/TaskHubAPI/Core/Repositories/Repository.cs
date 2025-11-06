using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskHubAPI.Core.Data;
using TaskHubAPI.Core.Interfaces;
using TaskHubAPI.Models;

namespace TaskHubAPI.Core.Repositories
{
    /// <summary>
    /// Generic repository implementation demonstrating:
    /// - ABSTRACTION: Implements IRepository interface
    /// - ENCAPSULATION: Private DbContext and DbSet fields
    /// - GENERICS: Works with any entity type inheriting BaseEntity
    /// - SINGLE RESPONSIBILITY: Handles only data access logic
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        // ENCAPSULATION: Private fields with controlled access
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor with DEPENDENCY INJECTION
        /// </summary>
        public Repository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Get all entities - demonstrates async/await pattern
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Get entity by ID with null checking
        /// </summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Find with predicate - demonstrates LINQ and Expression Trees
        /// Allows flexible querying without exposing DbSet
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Add entity - marks timestamps
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Update entity - marks as updated
        /// </summary>
        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.MarkAsUpdated();
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Check existence efficiently
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Save all changes to database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
