using Chinese_Auction.Models;

namespace Chinese_Auction.Repository
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<User?> UpdateUser(User user);
        Task<bool> EmailExistsAsync(string email,int id);

        Task<IEnumerable<User>> GetFilteredUsersAsync(string? name, string? email);

    }
}