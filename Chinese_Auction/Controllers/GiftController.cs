using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Services;
using ChineseAuction.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chinese_Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly ILogger<GiftController> _logger;
        public GiftController(IGiftService giftService, ILogger<GiftController> logger)
        {
            _giftService = giftService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            _logger.LogInformation("Getting all gifts.");
            var gifts = await _giftService.GetAllGiftsAsync();
            _logger.LogInformation("Fetched all gifts successfully.");
            return Ok(gifts);
        }


        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("un-approved")]
        public async Task<IActionResult> GetUnApprovedGiftsAsync()
        {
            _logger.LogInformation("Getting all unapproved gifts.");
            var gifts = await _giftService.GetUnApprovedGiftsAsync();
            _logger.LogInformation("Fetched all unapproved gifts successfully.");
            return Ok(gifts);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftByIdAsyncet(int id)
        {
            _logger.LogInformation("Getting gift by ID:" + id);
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
            {
                return NotFound("gift with the given ID was not found");
            }
            _logger.LogInformation("Fetched gift by ID successfully.");
            return Ok(gift);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateGiftAsync([FromBody] GiftDto gift)
        {
            _logger.LogInformation("Creating a new gift.");
            try
            {
                var newGift = await _giftService.CreateGiftAsync(gift);
                _logger.LogInformation("Created new gift successfully.");
                return CreatedAtAction(nameof(GetGiftByIdAsyncet), new { Id = newGift.Id }, newGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while creating a new gift.");
                return BadRequest("Internal server error ocuured");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGiftAsync([FromBody] GiftDto gift,int id)
        {
            _logger.LogInformation("Updating gift with ID:" + id);
            try
            {
                var updatedGift = await _giftService.UpdateGiftAsync(id,gift);
                if (updatedGift == null)
                {
                    return NotFound("gift with the given ID was not found");
                }
                _logger.LogInformation("Updated gift successfully.");
                return Ok(updatedGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while updating gift with ID:" + id);
                return BadRequest("Internal server error ocuured");
            }

        }

        [Authorize]
        [HttpPut]
        [Route("purchase-quantity/{id}")]
        public async Task<IActionResult> UpdateGiftPurchasesQuantityAsync([FromBody] UpdateGiftDto giftPurchase,int id)
        {
            _logger.LogInformation("Updating gift purchase quantity with ID:" + id);
            try
            {

                var updatedGift = await _giftService.UpdateGiftPurchasesQuantityAsync(id);
                if (updatedGift == null)
                {
                    return NotFound("gift with the given ID was not found");
                }
                _logger.LogInformation("Updated gift purchase quantity successfully.");
                return Ok(updatedGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while updating gift purchase quantity with ID:" + id);
                return BadRequest("Internal server error ocuured");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGiftAsync(int id)
        {
            _logger.LogInformation("Deleting gift with ID:" + id);
            var deleted = await _giftService.DeleteGiftAsync(id);
            if (!deleted)
            {
                return NotFound("gift with the given ID was not foundgift with the given ID was not found");
            }
            _logger.LogInformation("Deleted gift successfully.");
            return Ok("deleted succesfully");
        }

        [Authorize(Roles = "Manager")]
        [HttpPut]
        [Route("approve")]
        public async Task<IActionResult> ApproveGiftAsync(ApproveGiftDto gift)
        {
            _logger.LogInformation("Approving gift with ID:" + gift.Id);
            try
            {
                var approved = await _giftService.ApproveGiftAsync(gift.Id);
                if (!approved)
                {
                    return NotFound("gift with the given ID was not found");
                }
                _logger.LogInformation("Approved gift successfully.");
                return Ok("gift approved succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while approving gift with ID:" + gift.Id);
                return BadRequest("Internal server error ocuured" );
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetFilteredGifts([FromQuery] string? giftName, [FromQuery] string? donorName, [FromQuery] int? minPurchases)
        {
            _logger.LogInformation("Starting to search gifts. GiftName: {GiftName}, Donor: {DonorName}", giftName, donorName);
            try
            {
                var gifts = await _giftService.GetFilteredGiftsAsync(giftName, donorName, minPurchases);
                _logger.LogInformation("Successfully retrieved filtered gifts.");
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching gifts.");
                return BadRequest("Internal server error occurred");
            }
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetSortedGifts([FromQuery] string sortBy = "popularity")
        {
            _logger.LogInformation("Starting to get sorted purchases by: {SortBy}", sortBy);
            try
            {
                var purchases = await _giftService.GetSortedGiftsAsync(sortBy);
                _logger.LogInformation("Successfully retrieved sorted purchases.");
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sorted purchases.");
                return BadRequest("Internal server error occurred");
            }
        }
    } 
}
