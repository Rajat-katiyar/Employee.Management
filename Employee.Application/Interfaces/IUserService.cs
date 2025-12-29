using Employee.Application.DTOs;

namespace Employee.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
}
