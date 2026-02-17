using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Services;
using ChineseAuction.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chinese_Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IPurchaseService purchaseService, ILogger<PurchaseController> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllPurchases()
        {
            _logger.LogInformation("Getting all purchases.");
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            _logger.LogInformation("Fetched all purchases successfully.");
            return Ok(purchases);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(int id)
        {
            _logger.LogInformation("User " + User.GetUserId() + " is requesting purchase ID: " + id);
            if (!User.IsManager() && User.GetUserId() != id)
            {
                return Forbid();
            }
            _logger.LogInformation("Getting purchase by ID:" + id);
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            return purchase == null ? NotFound() : Ok(purchase);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPurchases([FromBody] List<CreatePurchaseDto> purchaseDtos)
        {
            _logger.LogInformation("User " + User.GetUserId() + " is adding purchases.");
            try
            {
                if (purchaseDtos == null || !purchaseDtos.Any())
                    return BadRequest("Purchase list is empty");
                var createdPurchases = await _purchaseService.AddPurchasesAsync(purchaseDtos);
                _logger.LogInformation("Purchases added successfully.");
                return CreatedAtAction(nameof(GetAllPurchases), createdPurchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding purchases: ");
                return BadRequest(ex.Message);
            }

        }



        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            _logger.LogInformation("User " + User.GetUserId() + " is requesting purchases for user ID: " + userId);
            if (!User.IsManager() && User.GetUserId() != userId)
            {
                return Forbid();
            }
            var purchases = await _purchaseService.GetPurchasesByUserIdAsync(userId);
            _logger.LogInformation("Fetched purchases for user ID: " + userId + " successfully.");
            return Ok(purchases);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("gift/{giftId}")]
        public async Task<IActionResult> GetByGiftId(int giftId)
        {
            _logger.LogInformation("Manager is requesting purchases for gift ID: " + giftId);
            var purchases = await _purchaseService.GetPurchasesByGiftIdAsync(giftId);
            _logger.LogInformation("Fetched purchases for gift ID: " + giftId + " successfully.");
            return Ok(purchases);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("lottery/{giftId}")]
        public async Task<IActionResult> RunLottery(int giftId)
        {
            try
            {
                _logger.LogInformation("Manager is running lottery for gift ID: " + giftId);
                var winner = await _purchaseService.Lottery(giftId);
                if (winner == null) return BadRequest("No participants for this gift or lottery failed.");
                _logger.LogInformation("Lottery for gift ID: " + giftId + " completed successfully. Winner ID: " + winner.Id);
                return Ok(winner);
            }
            
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "לא נמצאה במערכת מתנה" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "הגרלה כבר בוצעה עבור מתנה " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "לא נמצאו משתתפים להגרלה עבור מתנה " });
            }


        }

        [HttpGet("winner/{giftId}")]
        public async Task<IActionResult> GetWinner(int giftId)
        {
            _logger.LogInformation("User " + User.GetUserId() + " is requesting winner for gift ID: " + giftId);
            var winner = await _purchaseService.GetWinnersByGiftIdAsync(giftId);
            if (winner == null) return NotFound("No winner found for this gift yet.");
            _logger.LogInformation("Fetched winner for gift ID: " + giftId + " successfully. Winner ID: " + winner.Id);
            return Ok(winner);
        }



        [Authorize(Roles = "Manager")]
        [HttpGet("sorted")]
        public async Task<IActionResult> GetSortedPurchases([FromQuery] string sortBy = "popularity")
        {
            _logger.LogInformation("Starting to get sorted purchases by: {SortBy}", sortBy);
            try
            {
                var purchases = await _purchaseService.GetSortedPurchasesAsync(sortBy);
                _logger.LogInformation("Successfully retrieved sorted purchases.");
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sorted purchases.");
                return BadRequest("Internal server error occurred");
            }
        }
        //[Authorize(Roles = "manager,Manager")]
        [HttpGet("revenue/total")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            _logger.LogInformation("Starting to calculate total revenue from unique package transactions");
            try
            {
                var totalRevenue = await _purchaseService.GetTotalEarningsAsync();
                _logger.LogInformation("Total revenue retrieved successfully: {TotalRevenue}", totalRevenue);
                return Ok(new { totalRevenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total revenue.");
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}