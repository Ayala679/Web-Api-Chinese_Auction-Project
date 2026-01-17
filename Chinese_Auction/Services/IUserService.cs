using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Services
{
    public interface IUserService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto loginRequest);
        Task<GetUserDto?> CreateUser(CreateUserDto user);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<IEnumerable<User>> GetFilteredUsersAsync(string? name, string? email);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<GetUserDto?> UpdateUser(int id, CreateUserDto user);
    }
}