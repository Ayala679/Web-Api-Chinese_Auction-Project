using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;
using System.Reflection.Metadata;

namespace Chinese_Auction.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ILogger<PurchaseService> _logger;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public PurchaseService(ILogger<PurchaseService> logger, IPurchaseRepository purchaseRepository, IMapper mapper, IEmailService emailService, IUserRepository userRepository)
        {
            _logger = logger;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<GetPurchaseDto>> GetAllPurchasesAsync()
        {
            var purchases = await _purchaseRepository.GetAllPurchasesAsync();
            return _mapper.Map<IEnumerable<GetPurchaseDto>>(purchases);
        }

        public async Task<GetPurchaseDto?> GetPurchaseByIdAsync(int purchaseId)
        {
            var purchase = await _purchaseRepository.GetPurchaseByIdAsync(purchaseId);
            if (purchase == null)
            {
                _logger.LogWarning("Purchase with ID {PurchaseId} not found.", purchaseId);
                return null;
            }
            return _mapper.Map<GetPurchaseDto>(purchase);
        }

        public async Task<IEnumerable<GetPurchaseDto>> AddPurchasesAsync(List<CreatePurchaseDto> purchaseDtos)
        {
            var uniqueGroupId = Guid.NewGuid().ToString();

            var purchases = purchaseDtos.Select(dto =>
            {
                var purchase = _mapper.Map<Purchase>(dto);
                purchase.Unique_Package_Id = uniqueGroupId;
                purchase.Purchase_Date = DateTime.Now;
                purchase.Is_Won = false;
                return purchase;
            }).ToList();

            var savedPurchases = await _purchaseRepository.AddPurchasesRangeAsync(purchases);
            return _mapper.Map<IEnumerable<GetPurchaseDto>>(savedPurchases);
        }

        public async Task<GetPurchaseDto?> UpdatePurchaseAsync(int purchaseId, UpdatePurchaseDto purchaseDto)
        {
            var existingPurchase = await _purchaseRepository.GetPurchaseByIdAsync(purchaseId);
            if (existingPurchase == null)
            {
                _logger.LogWarning("Purchase with ID {PurchaseId} not found for update.", purchaseId);
                return null;
            }
            _mapper.Map(purchaseDto, existingPurchase);
            existingPurchase.Id = purchaseId;
            var updatedPurchase = await _purchaseRepository.UpdatePurchaseAsync(existingPurchase);
            if(updatedPurchase == null)
            {
                _logger.LogError("Failed to update Purchase with ID {PurchaseId}.", purchaseId);
            }
            return _mapper.Map<GetPurchaseDto>(updatedPurchase);
        }



        public async Task<IEnumerable<GetPurchaseDto>> GetPurchasesByUserIdAsync(int userId)
        {
            var purchases = await _purchaseRepository.GetPurchasesByUserIdAsync(userId);
            if(purchases == null)
            {
                _logger.LogWarning("No purchases found for User ID {UserId}.", userId);
                return Enumerable.Empty<GetPurchaseDto>();
            }
            return _mapper.Map<IEnumerable<GetPurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<GetPurchaseDto>> GetPurchasesByGiftIdAsync(int giftId)
        {
            var purchases = await _purchaseRepository.GetPurchasesByGiftIdAsync(giftId);
            if(purchases == null)
            {
                _logger.LogWarning("No purchases found for Gift ID {GiftId}.", giftId);
                return Enumerable.Empty<GetPurchaseDto>();
            }
            return _mapper.Map<IEnumerable<GetPurchaseDto>>(purchases);
        }


        public async Task<GetPurchaseDto?> Lottery(int giftId)
        {
            IEnumerable<Purchase> allPurchases = await _purchaseRepository.GetPurchasesByGiftIdAsync(giftId);
            if (allPurchases == null || !allPurchases.Any())
            {
                _logger.LogWarning("No purchases found for Gift ID {GiftId}. Cannot conduct lottery.", giftId);
                return null;
            }
            var random = new Random();
            var allPurchasesList = allPurchases.ToList();
            var winner = allPurchasesList[random.Next(allPurchasesList.Count)];
            var winnerDto = _mapper.Map<GetPurchaseDto>(winner);
            winner.Is_Won = true;
            await _purchaseRepository.UpdatePurchaseAsync(winner);
            await SendNotificationEmail(winnerDto);
            return _mapper.Map<GetPurchaseDto>(winner); 
        }

        private async Task SendNotificationEmail(GetPurchaseDto winner)
        {
            var user = await _userRepository.GetUserById(winner.User_Id);
            if(user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found. Cannot send notification email.", winner.User_Id);
                return;
            }
            var recipientEmail = user.Email;
            if (!string.IsNullOrEmpty(recipientEmail))
            {
                string subject = "עדכון לגבי ההגרלה";
                string message = "ברכותינו! עליית בגורל כזוכה עבור המתנה המבוקשת.";
                await _emailService.SendEmailAsync(recipientEmail, subject, message);
            }
        }

        public async Task<GetPurchaseDto?> GetWinnersByGiftIdAsync(int giftId)
        {
            var winner = await _purchaseRepository.GetWinnerByGiftIdAsync(giftId);
            if (winner == null)
            {
                _logger.LogWarning("No winning purchase found for Gift ID {GiftId}.", giftId);
                return null;
            }
            return _mapper.Map<GetPurchaseDto>(winner);
        }

        public async Task<IEnumerable<GetPurchaseDto>> GetSortedPurchasesAsync(string sortBy)
        {
            var purchases = await _purchaseRepository.GetAllPurchasesAsync();
            if (sortBy == "value")
                purchases = purchases
                    .Where(p => p.Gift != null)
                    .OrderByDescending(p => p.Gift!.Value);
            else if (sortBy == "popularity")
                purchases = purchases
                    .Where(p => p.Gift != null)
                    .OrderByDescending(p => p.Gift!.Purchase_quantity);

            return _mapper.Map<IEnumerable<GetPurchaseDto>>(purchases);
        }
    }
}
