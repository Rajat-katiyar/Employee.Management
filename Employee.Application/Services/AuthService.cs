using Employee.Application.Interfaces;
using Employee.Application.DTOs;
using Employee.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using System.Linq;

namespace Employee.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            // Check if email already exists
            existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user, out var expiresAt);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = expiresAt
            };
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Find user by username
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return null; // User not found
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null; // Invalid password
            }

            // Generate JWT token
            var token = GenerateJwtToken(user, out var expiresAt);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = expiresAt
            };
        }

        private string GenerateJwtToken(User user, out DateTime expiresAt)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "EmployeeManagementAPI";
            var audience = jwtSettings["Audience"] ?? "EmployeeManagementClient";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440"); // Default 24 hours
            
            expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return MapToUserResponse(user);
        }

        public async Task<UserResponse?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            return MapToUserResponse(user);
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponse);
        }

        public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                // Check if email already exists for another user
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    throw new InvalidOperationException("Email already exists.");
                }
                user.Email = request.Email;
            }

            // Update full name if provided
            if (request.FullName != null)
            {
                user.FullName = request.FullName;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _userRepository.UpdateAsync(user);
            return MapToUserResponse(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }

        private UserResponse MapToUserResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}

