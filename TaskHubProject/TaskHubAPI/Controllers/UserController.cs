using Microsoft.AspNetCore.Mvc;
using TaskHubAPI.DTOs;
using TaskHubAPI.Core.Interfaces;
using TaskHubAPI.Core.Strategies;

namespace TaskHubAPI.Controllers
{
    /// <summary>
    /// User controller demonstrating:
    /// - DEPENDENCY INVERSION: Depends on IUserService abstraction
    /// - SINGLE RESPONSIBILITY: Only handles HTTP concerns
    /// - SEPARATION OF CONCERNS: Business logic in service layer
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        // ENCAPSULATION: Private dependencies
        private readonly IUserService _userService;
        private readonly IValidationStrategy<CreateUserDTO> _validationStrategy;

        /// <summary>
        /// Constructor with DEPENDENCY INJECTION
        /// Demonstrates DEPENDENCY INVERSION PRINCIPLE - depends on abstractions
        /// </summary>
        public UserController(
            IUserService userService,
            IValidationStrategy<CreateUserDTO> validationStrategy)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _validationStrategy = validationStrategy ?? throw new ArgumentNullException(nameof(validationStrategy));
        }

        /// <summary>
        /// GET: api/users
        /// Get all users - demonstrates ABSTRACTION through service layer
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/users/{id}
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/users/email/{email}
        /// Get user by email - demonstrates business logic in service
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new { message = $"User with email '{email}' not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/users
        /// Create user - demonstrates STRATEGY PATTERN for validation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO dto)
        {
            try
            {
                // Use validation strategy (STRATEGY PATTERN)
                var validationResult = _validationStrategy.Validate(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { message = "Validation failed.", errors = validationResult.Errors });

                var user = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user.", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/users/{id}
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, CreateUserDTO dto)
        {
            try
            {
                // Validate using strategy
                var validationResult = _validationStrategy.Validate(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { message = "Validation failed.", errors = validationResult.Errors });

                var success = await _userService.UpdateUserAsync(id, dto);
                if (!success)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user.", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/users/{id}
        /// Delete user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }
    }
}
