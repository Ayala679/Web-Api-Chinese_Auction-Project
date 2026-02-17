using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Repository
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<Purchase>> AddPurchasesRangeAsync(IEnumerable<Purchase> purchases);
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<Purchase?> GetPurchaseByIdAsync(int id);
        Task<IEnumerable<Purchase>> GetPurchasesByGiftIdAsync(int giftId);
        Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(int userId);
        Task<Purchase?> UpdatePurchaseAsync(Purchase purchase);

        Task<Purchase?> GetWinnerByGiftIdAsync(int giftId);
        Task<int> GetTotalEarningsAsync();
    }
}