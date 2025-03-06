// InterviewSystem/Application/Interfaces/IUserService.cs
using InterviewSystem.Application.DTOs;

namespace InterviewSystem.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(UserDto userDto);
    Task UpdateUserAsync(UserDto userDto);
    Task DeleteUserAsync(string id);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
}