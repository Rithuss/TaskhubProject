using System;
using System.Text.RegularExpressions;
using TaskHubAPI.DTOs;
using TaskHubAPI.Core.Interfaces;

namespace TaskHubAPI.Core.Strategies
{
    /// <summary>
    /// User validation strategy demonstrating STRATEGY PATTERN
    /// Implements specific validation rules for user creation
    /// Demonstrates:
    /// - SINGLE RESPONSIBILITY: Only validates users
    /// - POLYMORPHISM: Can be swapped with other validation strategies
    /// </summary>
    public class UserValidationStrategy : IValidationStrategy<CreateUserDTO>
    {
        private readonly Regex _emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        /// <summary>
        /// Validate user DTO - demonstrates ENCAPSULATION of validation logic
        /// </summary>
        public ValidationResult Validate(CreateUserDTO user)
        {
            var result = new ValidationResult();

            if (user == null)
            {
                result.AddError("User cannot be null.");
                return result;
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                result.AddError("Name is required.");
            }
            else if (user.Name.Length < 2)
            {
                result.AddError("Name must be at least 2 characters long.");
            }
            else if (user.Name.Length > 100)
            {
                result.AddError("Name cannot exceed 100 characters.");
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                result.AddError("Email is required.");
            }
            else if (!_emailRegex.IsMatch(user.Email))
            {
                result.AddError("Email format is invalid.");
            }
            else if (user.Email.Length > 255)
            {
                result.AddError("Email cannot exceed 255 characters.");
            }

            return result;
        }
    }

    /// <summary>
    /// Task validation strategy demonstrating STRATEGY PATTERN
    /// Implements specific validation rules for task creation
    /// </summary>
    public class TaskValidationStrategy : IValidationStrategy<CreateTaskItemDTO>
    {
        /// <summary>
        /// Validate task DTO
        /// </summary>
        public ValidationResult Validate(CreateTaskItemDTO task)
        {
            var result = new ValidationResult();

            if (task == null)
            {
                result.AddError("Task cannot be null.");
                return result;
            }

            // Validate Title
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                result.AddError("Title is required.");
            }
            else if (task.Title.Length < 3)
            {
                result.AddError("Title must be at least 3 characters long.");
            }
            else if (task.Title.Length > 200)
            {
                result.AddError("Title cannot exceed 200 characters.");
            }

            // Validate Description
            if (!string.IsNullOrWhiteSpace(task.Description) && task.Description.Length > 1000)
            {
                result.AddError("Description cannot exceed 1000 characters.");
            }

            // Validate Status
            if (!Enum.TryParse<Models.TaskStatus>(task.Status, out _))
            {
                result.AddError($"Invalid status: {task.Status}. Must be Pending, InProgress, or Completed.");
            }

            // Validate Priority
            if (!Enum.TryParse<Models.TaskPriority>(task.Priority, out _))
            {
                result.AddError($"Invalid priority: {task.Priority}. Must be Low, Medium, or High.");
            }

            // Validate DueDate
            if (task.DueDate == default)
            {
                result.AddError("Due date is required.");
            }
            else if (task.DueDate < DateTime.UtcNow.Date)
            {
                result.AddError("Due date cannot be in the past.");
            }

            // Validate UserId
            if (task.UserId <= 0)
            {
                result.AddError("Valid user ID is required.");
            }

            return result;
        }
    }

    /// <summary>
    /// Composite validation strategy demonstrating COMPOSITE PATTERN
    /// Combines multiple validation strategies
    /// Demonstrates POLYMORPHISM and COMPOSITION
    /// </summary>
    /// <typeparam name="T">Type to validate</typeparam>
    public class CompositeValidationStrategy<T> : IValidationStrategy<T>
    {
        private readonly List<IValidationStrategy<T>> _strategies;

        public CompositeValidationStrategy()
        {
            _strategies = new List<IValidationStrategy<T>>();
        }

        /// <summary>
        /// Add a validation strategy - demonstrates COMPOSITION
        /// </summary>
        public void AddStrategy(IValidationStrategy<T> strategy)
        {
            if (strategy != null)
            {
                _strategies.Add(strategy);
            }
        }

        /// <summary>
        /// Validate using all strategies
        /// </summary>
        public ValidationResult Validate(T item)
        {
            var result = new ValidationResult();

            foreach (var strategy in _strategies)
            {
                var strategyResult = strategy.Validate(item);
                if (!strategyResult.IsValid)
                {
                    foreach (var error in strategyResult.Errors)
                    {
                        result.AddError(error);
                    }
                }
            }

            return result;
        }
    }
}
