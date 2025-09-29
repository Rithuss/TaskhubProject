using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskHubAPI.Data;
using TaskHubAPI.DTOs;
using TaskHubAPI.Models;

namespace TaskHubAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            }).ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, CreateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Name = dto.Name;
            user.Email = dto.Email;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
