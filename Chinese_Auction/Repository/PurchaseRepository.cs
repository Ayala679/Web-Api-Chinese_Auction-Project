using Chinese_Auction.Data;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinese_Auction.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ChineseAuctionDbContext _context;
        public PurchaseRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await _context.Purchases
                .Include(p => p.Gift).Include(p => p.User).ToListAsync();
        }

        public async Task<Purchase?> GetPurchaseByIdAsync(int id)
        {
            return await _context.Purchases.Include(p => p.Gift).Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Purchase>> AddPurchasesRangeAsync(IEnumerable<Purchase> purchases)
        {
            await _context.Purchases.AddRangeAsync(purchases);
            await _context.SaveChangesAsync();
            return purchases;
        }

        public async Task<Purchase?> UpdatePurchaseAsync(Purchase purchase)
        {
            var existing = await _context.Purchases.FindAsync(purchase.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(purchase);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(int userId)
        {
            return await _context.Purchases.Include(p => p.Gift).Where(p => p.User_Id == userId).ToListAsync();
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByGiftIdAsync(int giftId)
        {
            return await _context.Purchases.Where(p => p.Gift_Id == giftId).ToListAsync();
        }

        public async Task<Purchase?> GetWinnerByGiftIdAsync(int giftId)
        {
            return await _context.Purchases.FirstOrDefaultAsync(p => p.Gift_Id == giftId && p.Is_Won);
        }



        public async Task<int> GetTotalEarningsAsync()
        {
            var totalRevenue = await _context.Purchases
                .Select(p => new { p.Package_Id, p.Unique_Package_Id })
                .Distinct()
                .Join(_context.Packages,
                    purchase => purchase.Package_Id,
                    package => package.Id,
                    (purchase, package) => package.Price)
                .SumAsync();

            return totalRevenue;
        }

    }

}