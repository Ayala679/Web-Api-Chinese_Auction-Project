using Chinese_Auction.Dto_s;

namespace Chinese_Auction.Services
{
    public interface IGiftService
    {
        Task<GetGiftDto> CreateGiftAsync(GiftDto gift);
        Task<bool> DeleteGiftAsync(int id);
        Task<IEnumerable<GetGiftDto>> GetAllGiftsAsync();
        Task<GetGiftDto?> GetGiftByIdAsync(int id);
        Task<GetGiftDto?> UpdateGiftAsync(int id, GiftDto gift);
        Task<UpdateGiftDto?> UpdateGiftPurchasesQuantityAsync(int id);
        Task<IEnumerable<GetGiftDto>> GetFilteredGiftsAsync(string? giftName, string? donorName, int? minPurchases);
        Task<IEnumerable<GetGiftDto>> GetSortedGiftsAsync(string sortBy);
        Task<IEnumerable<GetGiftDto>> GetGiftsByCategoryIdAsync(int categoryId);

    }
}