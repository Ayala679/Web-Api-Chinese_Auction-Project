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

        //private async Task SendNotificationEmail(GetPurchaseDto winner,int giftID)
        //{
        //    var user = await _userRepository.GetUserById(winner.User_Id);
        //    if(user == null)
        //    {
        //        _logger.LogWarning("User with ID {UserId} not found. Cannot send notification email.", winner.User_Id);
        //        return;
        //    }
        //    var recipientEmail = user.Email;
        //    if (!string.IsNullOrEmpty(recipientEmail))
        //    {
        //        string subject = "עדכון לגבי ההגרלה";
        //        string message = ":ברכותינו! עליית בגורל כזוכה עבור המתנה המבוקשת."+giftID;
        //        await _emailService.SendEmailAsync(recipientEmail, subject, message);
        //    }
        //}


        private async Task SendNotificationEmail(GetPurchaseDto winner, int giftID)
        {
            var user = await _userRepository.GetUserById(winner.User_Id);
            var gift = await _giftRepository.GetGiftByIdAsync(giftID);

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("User with ID {UserId} not found or has no email. Cannot send notification.", winner.User_Id);
                return;
            }

            string subject = "✨ מזל טוב! זכית בהגרלה של CREATIVETECH";

            // יצירת מבנה HTML בסגנון האתר (Dark Theme עם כתום)
            string htmlMessage = $@"
    <div dir='rtl' style='background-color: #0f172a; padding: 40px 20px; font-family: sans-serif; text-align: right;'>
        <div style='max-width: 600px; margin: 0 auto; background: #1a162e; border: 1px solid rgba(255,255,255,0.1); border-radius: 24px; overflow: hidden; box-shadow: 0 20px 50px rgba(0,0,0,0.5);'>
            
            <div style='padding: 30px; text-align: center; border-bottom: 1px solid rgba(255,255,255,0.1);'>
                <h1 style='font-size: 28px; margin: 0; letter-spacing: -1px;'>
                    <span style='color: white; font-weight: 900;'>CREATIVE</span><span style='color: #f97316; font-weight: 900;'>TECH</span>
                </h1>
            </div>

            <div style='padding: 40px 30px;'>
                <h2 style='color: #f97316; font-size: 32px; font-weight: 900; margin-bottom: 10px;'>מזל טוב, {user.First_name}!</h2>
                <p style='color: white; font-size: 18px; line-height: 1.6; margin-bottom: 30px;'>
                    אנחנו נרגשים לבשר לך שבתום הגרלה מותחת, עליית בגורל כזוכה הגדול!
                </p>

                <div style='background: rgba(255,255,255,0.05); border: 1px solid #f97316; border-radius: 16px; padding: 25px; margin-bottom: 30px; text-align: center;'>
                    <span style='color: #f97316; font-size: 14px; font-weight: bold; text-transform: uppercase;'>הזכייה שלך:</span>
                    <h3 style='color: white; font-size: 24px; font-weight: 800; margin: 10px 0;'>{gift?.Name}</h3>
                    <p style='color: rgba(255,255,255,0.7); font-size: 16px; margin-bottom: 0;'>{gift?.Description}</p>
                    <div style='margin-top: 15px; padding-top: 15px; border-top: 1px solid rgba(255,255,255,0.1);'>
                        <span style='color: white; font-size: 20px; font-weight: 900;'>שווי המתנה: {gift?.Value:C}</span>
                    </div>
                </div>

                <p style='color: rgba(255,255,255,0.6); font-size: 14px;'>
                    * נציג מטעמנו ייצור איתך קשר בכתובת המייל <strong>{user.Email}</strong> לתיאום קבלת הפרס.
                </p>

                <div style='margin-top: 40px; text-align: center;'>
                    <a href='https://localhost:4200/my-gifts' 
                       style='background-color: #f97316; color: white; padding: 16px 40px; text-decoration: none; border-radius: 50px; font-weight: bold; font-size: 16px; display: inline-block; box-shadow: 0 4px 15px rgba(249, 115, 22, 0.4);'>
                       צפייה בפרטי הזכייה באתר
                    </a>
                </div>
            </div>

            <div style='padding: 20px; background: rgba(0,0,0,0.2); text-align: center;'>
                <p style='color: rgba(255,255,255,0.3); font-size: 12px; margin: 0;'>
                    נשלח באהבה ע""י מערכת ההגרלות של CreativeTech &copy; 2026
                </p>
            </div>
        </div>
    </div>";

            await _emailService.SendEmailAsync(user.Email, subject, htmlMessage);
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
