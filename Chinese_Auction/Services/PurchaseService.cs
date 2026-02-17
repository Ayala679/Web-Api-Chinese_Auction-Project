using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace Chinese_Auction.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ILogger<PurchaseService> _logger;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IGiftRepository _giftRepository;

        public PurchaseService(ILogger<PurchaseService> logger, IPurchaseRepository purchaseRepository, IMapper mapper, IEmailService emailService, IUserRepository userRepository, IGiftRepository giftRepository)
        {
            _logger = logger;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _giftRepository = giftRepository;
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

        //public async Task<IEnumerable<GetPurchaseDto>> AddPurchasesAsync(List<CreatePurchaseDto> purchaseDtos)
        //{
        //    var uniqueGroupId = Guid.NewGuid().ToString();

        //    var purchases = purchaseDtos.Select(async dto =>
        //    {
        //        var purchase = _mapper.Map<Purchase>(dto);
        //        purchase.Unique_Package_Id = uniqueGroupId;
        //        purchase.Purchase_Date = DateTime.Now;
        //        purchase.Is_Won = false;
        //        await _giftRepository.UpdateGiftPurchasesQuantityAsync(dto.Gift_Id);
        //        return purchase;
        //    }).ToList();
        //    var finalPurchase = await Task.WhenAll(purchases);
        //    var savedPurchases = await _purchaseRepository.AddPurchasesRangeAsync(finalPurchase);
        //    return _mapper.Map<IEnumerable<GetPurchaseDto>>(savedPurchases);
        //}

        public async Task<IEnumerable<GetPurchaseDto>> AddPurchasesAsync(List<CreatePurchaseDto> purchaseDtos)
        {
            var uniqueGroupId = Guid.NewGuid().ToString();
            var finalPurchases = new List<Purchase>();

            foreach (var dto in purchaseDtos)
            {
                var purchase = _mapper.Map<Purchase>(dto);
                purchase.Unique_Package_Id = uniqueGroupId;
                purchase.Purchase_Date = DateTime.Now;
                purchase.Is_Won = false;

                await _giftRepository.UpdateGiftPurchasesQuantityAsync(dto.Gift_Id);

                finalPurchases.Add(purchase);
            }

            var savedPurchases = await _purchaseRepository.AddPurchasesRangeAsync(finalPurchases);
            return _mapper.Map<IEnumerable<GetPurchaseDto>>(savedPurchases);
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
            var gift = await _giftRepository.GetGiftByIdAsync(giftId);
            if (gift == null)
                throw new KeyNotFoundException("לא נמצאה מתנה להגרלה");
            if (gift.IsLottery)
                throw new InvalidOperationException("הגרלה כבר בוצעה עבור מתנה זו");
            if (allPurchases == null || !allPurchases.Any())
            {
                _logger.LogWarning("No purchases found for Gift ID {GiftId}. Cannot conduct lottery.", giftId);
                throw new Exception("לא נמצאו משתתפים להגרלה עבור מתנה זו");
            }
            var random = new Random();
            var allPurchasesList = allPurchases.ToList();
            var winner = allPurchasesList[random.Next(allPurchasesList.Count)];
            var winnerDto = _mapper.Map<GetPurchaseDto>(winner);
            winner.Is_Won = true;
            var updated=await _giftRepository.UpdateGiftLotteryAsync(giftId);
            await _purchaseRepository.UpdatePurchaseAsync(winner);
            await SendNotificationEmail(winnerDto,giftId);
            return _mapper.Map<GetPurchaseDto>(winner); 
        }

        private async Task SendNotificationEmail(GetPurchaseDto winner,int giftID)
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
                string message = ":ברכותינו! עליית בגורל כזוכה עבור המתנה המבוקשת."+giftID;
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

        public async Task<int> GetTotalEarningsAsync()
        {
            var totalRevenue = await _purchaseRepository.GetTotalEarningsAsync();
            _logger.LogInformation("Total revenue calculated successfully: {TotalRevenue}", totalRevenue);
            return totalRevenue;
        }

    }
}
