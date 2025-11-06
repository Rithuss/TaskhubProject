namespace TaskHubAPI.Core.Interfaces
{
    /// <summary>
    /// Validation strategy interface demonstrating STRATEGY PATTERN
    /// Allows different validation algorithms to be used interchangeably
    /// Demonstrates:
    /// - ABSTRACTION: Interface defines contract
    /// - POLYMORPHISM: Different implementations can be used
    /// - OPEN/CLOSED PRINCIPLE: Open for extension, closed for modification
    /// </summary>
    /// <typeparam name="T">Type to validate</typeparam>
    public interface IValidationStrategy<T>
    {
        /// <summary>
        /// Validate an object
        /// </summary>
        /// <param name="item">Object to validate</param>
        /// <returns>Validation result with success flag and error messages</returns>
        ValidationResult Validate(T item);
    }

    /// <summary>
    /// Validation result class demonstrating ENCAPSULATION
    /// Encapsulates validation outcome and error messages
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public List<string> Errors { get; private set; }

        public ValidationResult()
        {
            IsValid = true;
            Errors = new List<string>();
        }

        /// <summary>
        /// Add validation error - demonstrates ENCAPSULATION
        /// </summary>
        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                IsValid = false;
                Errors.Add(error);
            }
        }

        /// <summary>
        /// Create a successful validation result
        /// </summary>
        public static ValidationResult Success()
        {
            return new ValidationResult { IsValid = true };
        }

        /// <summary>
        /// Create a failed validation result with errors
        /// </summary>
        public static ValidationResult Failure(params string[] errors)
        {
            var result = new ValidationResult { IsValid = false };
            foreach (var error in errors)
            {
                result.Errors.Add(error);
            }
            return result;
        }
    }
}
