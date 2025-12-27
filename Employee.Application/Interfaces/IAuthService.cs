using Employee.Application.DTOs;

namespace Employee.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
}

