using Chinese_Auction.Data;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Chinese_Auction.Repository
{
    public class GiftRepository : IGiftRepository
    {
        private readonly ChineseAuctionDbContext _context;
        public GiftRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gift>> GetAllGiftsAsync()
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Purchases)
                //.Where(g => g.IsApproved)
                .ToListAsync();
        }

        public async Task<Gift?> GetGiftByIdAsync(int id)
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include (g => g.Purchases)
                .FirstOrDefaultAsync(g => g.Id == id);
        }


        public async Task<Gift> CreateGiftAsync(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<Gift?> UpdateGiftAsync(Gift gift)
        {
            var existing = await _context.Gifts.FindAsync(gift.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(gift);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteGiftAsync(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift != null)
            {
                _context.Gifts.Remove(gift);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Gift>> GetGiftsByCategoryIdAsync(int categoryId)
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Where(g => g.Category_Id == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gift>> GetGiftsByDonorIdAsync(int donorId)
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Where(g => g.Donor_Id == donorId)
                .ToListAsync();
        }


        public async Task<Gift?> UpdateGiftPurchasesQuantityAsync(int giftId)
        {
            int rowsAffected = await _context.Gifts
                .Where(g => g.Id == giftId)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    g => g.Purchase_quantity,
                    g => g.Purchase_quantity + 1));
            if (rowsAffected == 0) return null;
            return await _context.Gifts.FindAsync(giftId);
        }


        public async Task<IEnumerable<Gift>> GetFilteredGiftsAsync(string? giftName, string? donorName, int? minPurchases)
        {
            var query = _context.Gifts.Include(g => g.Donor).AsQueryable();

            if (!string.IsNullOrEmpty(giftName))
                query = query.Where(g => g.Name.Contains(giftName));

            if (!string.IsNullOrEmpty(donorName))
                query = query.Where(g => (g.Donor != null && (g.Donor.First_name.Contains(donorName) || g.Donor.Last_name.Contains(donorName))));

            if (minPurchases.HasValue)
                query = query.Where(g => g.Purchase_quantity >= minPurchases.Value);

            return await query.ToListAsync();
        }

        public async Task<Gift?> UpdateGiftLotteryAsync(int giftId)
        {
            var existing = await _context.Gifts.FindAsync(giftId);
            if (existing == null) return null;
            existing.IsLottery = true;
            await _context.SaveChangesAsync();
            return existing;
        }

    }
}