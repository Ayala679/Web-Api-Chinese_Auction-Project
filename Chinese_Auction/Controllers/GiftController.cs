using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Services;
using ChineseAuction.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

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

        [HttpGet("byCategory/{categoryId}")]
        public async Task<IActionResult> GetGiftByCategoryIdAsync(int categoryId)
        {
            _logger.LogInformation("Getting gift by categoryID:" + categoryId);
            var gifts = await _giftService.GetGiftsByCategoryIdAsync(categoryId);
            _logger.LogInformation("Fetched gift by categoryID successfully.");
            return Ok(gifts);

        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateGiftAsync([FromForm] GiftDto gift,IFormFile? imageFile)
        {
            _logger.LogInformation("Creating a new gift.");
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/gifts", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    gift.Picture = fileName;
                }
                var newGift = await _giftService.CreateGiftAsync(gift);
                _logger.LogInformation("Created new gift successfully.");
                return CreatedAtAction(nameof(GetGiftByIdAsyncet), new { Id = newGift.Id }, newGift);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex,"Error occurred while creating a new gift.");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGiftAsync([FromForm] GiftDto gift,int id,IFormFile? imageFile)
        {
            _logger.LogInformation("Updating gift with ID:" + id);
            try
            {
                var existingGift = await _giftService.GetGiftByIdAsync(id);
                if (existingGift == null)
                {
                    return NotFound("gift with the given ID was not found");
                }
                
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingGift.Picture))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/gifts", existingGift.Picture);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/gifts", fileName);

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    gift.Picture = fileName;
                    
                }
                else
                {
                    gift.Picture = existingGift.Picture;
                }
                var updateGift = await _giftService.UpdateGiftAsync(id, gift);
                _logger.LogInformation("Updated gift successfully.");
                return Ok();
            }
            catch(InvalidOperationException)
            {
                return BadRequest("לא ניתן לערוך מתנה שכבר נבחרה להגרלה");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                _logger.LogError(ex,"Error occurred while updating gift with ID:" + id);
                return BadRequest(ex.Message);
            }
        }


        //remove
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
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGiftAsync(int id)
        {
            _logger.LogInformation("Deleting gift with ID:" + id);
            try
            {
                var existingGift = await _giftService.GetGiftByIdAsync(id);
                if (existingGift == null) return NotFound("gift with the given ID was not foundgift with the given ID was not found");
                var isDeleted = await _giftService.DeleteGiftAsync(id);
                if (!isDeleted)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/gifts", existingGift.Picture);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation("Deleted physical file: " + existingGift.Picture);
                    }
                }
                _logger.LogInformation("Deleted gift successfully.");
                return Ok("deleted succesfully");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("לא ניתן לערוך מתנה שכבר נבחרה להגרלה");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the gift.");
                return BadRequest(ex.Message);

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
