using Employee.Application.DTOs;
using Employee.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Employee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(new { message = "User registered successfully.", data = response });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering user.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                return Ok(new { message = "Login successful.", data = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while logging in.", error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(new { message = "Users retrieved successfully.", data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new { message = "User retrieved successfully.", data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user.", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
            try
            {
                // Get current user ID from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new { message = "Current user retrieved successfully.", data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving current user.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // Get current user ID from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                // Users can only update their own profile (unless you want admin role)
                if (id != currentUserId)
                {
                    return Forbid("You can only update your own profile.");
                }

                var updatedUser = await _authService.UpdateUserAsync(id, request);
                return Ok(new { message = "User updated successfully.", data = updatedUser });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                // Get current user ID from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                // Users can only delete their own account (unless you want admin role)
                if (id != currentUserId)
                {
                    return Forbid("You can only delete your own account.");
                }

                var deleted = await _authService.DeleteUserAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting user.", error = ex.Message });
            }
        }
    }
}

