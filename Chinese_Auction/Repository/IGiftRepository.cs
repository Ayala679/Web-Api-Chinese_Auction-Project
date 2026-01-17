using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Repository
{
    public interface IGiftRepository
    {
        Task<Gift> CreateGiftAsync(Gift gift);
        Task<bool> DeleteGiftAsync(int id);
        Task<IEnumerable<Gift>> GetAllGiftsAsync();
        Task<Gift?> GetGiftByIdAsync(int id);
        Task<IEnumerable<Gift>> GetGiftsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Gift>> GetGiftsByDonorIdAsync(int donorId);
        Task<IEnumerable<Gift>> GetUnApprovedGiftsAsync();
        Task<Gift?> UpdateGiftAsync(Gift gift);
        Task<Gift?> UpdateGiftPurchasesQuantityAsync(int giftId);
        Task<bool> ApproveGiftAsync(int giftId);
        Task<IEnumerable<Gift>> GetFilteredGiftsAsync(string? giftName, string? donorName, int? minPurchases);
    }
}