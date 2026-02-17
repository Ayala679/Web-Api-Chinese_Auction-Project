using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;
using Microsoft.EntityFrameworkCore;

namespace Chinese_Auction.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GiftRepository> _logger;

        public GiftService(IGiftRepository giftRepository, IMapper mapper, ILogger<GiftRepository> logger)
        {
            _giftRepository = giftRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GetGiftDto>> GetAllGiftsAsync()
        {
            var gifts = await _giftRepository.GetAllGiftsAsync();
            return _mapper.Map<IEnumerable<GetGiftDto>>(gifts);
        }

        public async Task<GetGiftDto?> GetGiftByIdAsync(int id)
        {
            var gift = await _giftRepository.GetGiftByIdAsync(id);
            if (gift == null)
            {
                _logger.LogWarning($"Gift with ID {id} not found.");
                return null; 
            }
            return _mapper.Map<GetGiftDto?>(gift);
        }

        public async Task<IEnumerable<GetGiftDto>> GetGiftsByCategoryIdAsync(int categoryId)
        {
            var gifts = await _giftRepository.GetGiftsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<GetGiftDto>>(gifts);
        }


        public async Task<GetGiftDto> CreateGiftAsync(GiftDto gift)
        {
            var createGift = _mapper.Map<Gift>(gift);
            var addedGift = await _giftRepository.CreateGiftAsync(createGift);
            return _mapper.Map<GetGiftDto>(addedGift);
        }

        public async Task<GetGiftDto?> UpdateGiftAsync(int id, GiftDto gift)
        {
            var existingGift = await _giftRepository.GetGiftByIdAsync(id);
            if (existingGift == null)
            {
                _logger.LogWarning($"Gift with ID {id} not found for update.");
                return null;
            }
            if (existingGift.Purchases.Any() || existingGift.Purchases.Count() > 0)
                throw new InvalidOperationException("לא ניתן למחוק מתנה שכבר נבחרה להגרלה");
            _mapper.Map(gift,existingGift);
            existingGift.Id = id;
            var updatedGift = await _giftRepository.UpdateGiftAsync(existingGift);
            if (updatedGift == null)
            {
                _logger.LogError($"Failed to update Gift with ID {id}.");
                return null;
            }
            return _mapper.Map<GetGiftDto>(updatedGift);
        }


        public async Task<UpdateGiftDto?> UpdateGiftPurchasesQuantityAsync(int id)
        {
            var existingGift = await _giftRepository.GetGiftByIdAsync(id);
            if (existingGift == null)
            {
                _logger.LogWarning($"Gift with ID {id} not found for updating purchase quantity.");
                return null;
            }
            var updatedGift = await _giftRepository.UpdateGiftPurchasesQuantityAsync(id);
            if (updatedGift == null)
            {                
                _logger.LogError($"Failed to update purchase quantity for Gift with ID {id}.");
                return null;
            }
            return _mapper.Map<UpdateGiftDto>(updatedGift);
        }


        public async Task<bool> DeleteGiftAsync(int id)
        {
            var existingGift = await _giftRepository.GetGiftByIdAsync(id);
            if (existingGift == null)
            {
               _logger.LogWarning($"Gift with ID {id} not found for deletion.");
                return false;
            }
            if (existingGift.Purchases.Any() || existingGift.Purchases.Count() > 0)
                throw new InvalidOperationException("לא ניתן למחוק מתנה שכבר נבחרה להגרלה");
            await _giftRepository.DeleteGiftAsync(id);
            return true;
        }

        public async Task<IEnumerable<GetGiftDto>> GetFilteredGiftsAsync(string? giftName, string? donorName, int? minPurchases)
        {
            var gifts = await _giftRepository.GetFilteredGiftsAsync(giftName, donorName, minPurchases);
            return _mapper.Map<IEnumerable<GetGiftDto>>(gifts);
        }

        public async Task<IEnumerable<GetGiftDto>> GetSortedGiftsAsync(string sortBy)
        {
            var gifts = await _giftRepository.GetAllGiftsAsync();
            if (sortBy == "value")
                gifts = gifts.OrderByDescending(g => g.Value);
            else if (sortBy == "category")
                gifts = gifts
                    .Where(g => g.Category != null)
                    .OrderByDescending(g => g.Category!.Name);
            return _mapper.Map<IEnumerable<GetGiftDto>>(gifts);
        }





    }
}
