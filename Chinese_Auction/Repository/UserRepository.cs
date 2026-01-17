using Chinese_Auction.Data;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Chinese_Auction.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChineseAuctionDbContext _context;

        public UserRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.Include(p => p.Purchases).FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<User?> UpdateUser(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> EmailExistsAsync(string email,int id)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.Id != id);
        }

        public async Task<IEnumerable<User>> GetFilteredUsersAsync(string? name, string? email)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.First_name.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(d => d.Email.Contains(email));

            return await query.ToListAsync();
        }
    }
}
 