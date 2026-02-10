using Chinese_Auction.Data;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinese_Auction.Repository
{
    //manager only
    public class DonorRepository : IDonorRepository
    {
        

        private readonly ChineseAuctionDbContext _context;
        public DonorRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }


        //only manager can get all donors
        public async Task<IEnumerable<Donor>> GetAllDonorsAsync()
        {
            return await _context.Donors.Include(d => d.Gifts).ToListAsync();
        }


        //donor
        public async Task<Donor?> GetDonorByIdAsync(int id)
        {
            return await _context.Donors.Include(d => d.Gifts).FirstOrDefaultAsync(d => d.Id == id);
        }

        //donor
        public async Task CreateDonorAsync(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
        }

        //donor
        public async Task<Donor?> UpdateDonorAsync(Donor donor)
        {
            var existing = await _context.Donors.FindAsync(donor.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(donor);
            await _context.SaveChangesAsync();
            return existing;
        }

        //donor
        public async Task DeleteDonorAsync(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor != null)
            {
                _context.Donors.Remove(donor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DonorEmailExistsAsync(string email, int id)
        {
            var emailExist = await _context.Donors.AnyAsync(d => d.Email.Equals(email) && d.Id != id);
            if(emailExist)
                return true;
            return false;
        }

        public async Task<Donor?> GetDonorByEmailAsync(string email)
        {
            return await _context.Donors.FirstOrDefaultAsync(d => d.Email == email);
        }

        //filter
        //public async Task<IEnumerable<Donor>> GetFilteredDonorsAsync(string? name, string? email, string? giftName)
        //{
        //    var query = _context.Donors.Include(d => d.Gifts).AsQueryable();

        //    if (!string.IsNullOrEmpty(name))
        //        query = query.Where(d => d.First_name.Contains(name));

        //    if (!string.IsNullOrEmpty(email))
        //        query = query.Where(d => d.Email.Contains(email));

        //    if (!string.IsNullOrEmpty(giftName))
        //        query = query.Where(d => d.Gifts.Any(g => g.Name.Contains(giftName)));

        //    return await query.ToListAsync();
        //}

        public async Task<IEnumerable<Donor>> GetFilteredDonorsAsync(string? name, string? email, string? giftName)
        {
            var query = _context.Donors.Include(d => d.Gifts).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.First_name.Contains(name) || d.Last_name.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(d => d.Email.Contains(email));

            if (!string.IsNullOrEmpty(giftName))
                query = query.Where(d => d.Gifts.Any(g => g.Name.Contains(giftName)));

            return await query.ToListAsync();
        }
    }
}
