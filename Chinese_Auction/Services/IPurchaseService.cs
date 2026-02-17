using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Services
{
    public interface IPurchaseService
    {
        Task<IEnumerable<GetPurchaseDto>> AddPurchasesAsync(List<CreatePurchaseDto> purchaseDtos);
        Task<IEnumerable<GetPurchaseDto>> GetAllPurchasesAsync();
        Task<GetPurchaseDto?> GetPurchaseByIdAsync(int purchaseId);
        Task<IEnumerable<GetPurchaseDto>> GetPurchasesByGiftIdAsync(int giftId);
        Task<IEnumerable<GetPurchaseDto>> GetPurchasesByUserIdAsync(int userId);
        Task<GetPurchaseDto?> GetWinnersByGiftIdAsync(int giftId);
        Task<GetPurchaseDto?> Lottery(int giftId);
        Task<IEnumerable<GetPurchaseDto>> GetSortedPurchasesAsync(string sortBy);
        Task<int> GetTotalEarningsAsync();
    }
}