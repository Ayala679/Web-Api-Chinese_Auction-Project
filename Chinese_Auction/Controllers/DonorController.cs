using Chinese_Auction.Dto_s;
using Chinese_Auction.Services;
using ChineseAuction.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chinese_Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;
        private readonly ILogger<DonorController> _logger;


        public DonorController(IDonorService donorService, ILogger<DonorController> logger)
        {
            _donorService = donorService;
            _logger = logger;
        }


        [Authorize(Roles = "Manager,manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllDonors()
        {
            _logger.LogInformation("Getting all donors.");
            var donors = await _donorService.GetAllDonorsAsync();
            _logger.LogInformation("Fetched all donors successfully.");
            return Ok(donors);
        }

        [Authorize(Roles = "Manager,manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonorById(int id)
        {
            _logger.LogInformation("Getting donor by ID:" + id);
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (donor == null) return NotFound("donor with the given ID was not found");
            _logger.LogInformation("Fetched donor by ID:" + id + " successfully.");
            return Ok(donor);
        }

        [Authorize(Roles = "Manager,manager")]
        [HttpPost]
        public async Task<IActionResult> CreateDonor([FromForm] CreateDonorDto donor,IFormFile? imageFile)
        {
            _logger.LogInformation("Creating a new donor.");
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/companies", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    donor.Company_picture = fileName;
                }
                var newDonor = await _donorService.CreateDonorAsync(donor);
                _logger.LogInformation("Created new donor successfully.");
                return CreatedAtAction(nameof(GetDonorById), new { id = newDonor.Id }, newDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new donor.");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Manager,manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(int id, [FromForm] CreateDonorDto donor,IFormFile? imageFile)
        {
            _logger.LogInformation("Updating donor with ID:" + id);
            try
            {
                var existingDonor = await _donorService.GetDonorByIdAsync(id);
                if (existingDonor == null) return NotFound();
                if (!string.IsNullOrEmpty(existingDonor.Company_picture))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/companies", existingDonor.Company_picture);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/companies", fileName);

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    donor.Company_picture = fileName;
                }
                else
                {
                    donor.Company_picture = null;
                }
                var updatedDonor = await _donorService.UpdateDonorAsync(id, donor);
                _logger.LogInformation("Updated donor with ID:" + id + " successfully.");
                return Ok(updatedDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating donor with ID:" + id);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Manager,manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            _logger.LogInformation("Deleting donor with ID:" + id);
            try
            {
                var existingDonor = await _donorService.GetDonorByIdAsync(id);
                if (existingDonor == null) return NotFound("donor with the given ID was not found");
                var isDeleted = await _donorService.DeleteDonorAsync(id);
                if (!isDeleted)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/gifts", existingDonor.Company_picture);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation("Deleted physical file: " + existingDonor.Company_picture);
                    }
                }
                _logger.LogInformation("Deleted donor with ID:" + id + " successfully.");
                return Ok("deleted succesfully");
            }
            catch(InvalidOperationException)
            {
                return BadRequest("לא ניתן למחוק תורם שכבר תרם מתנות");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting donor with ID:" + id);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Manager,manager")]
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredDonors([FromQuery] string? name, [FromQuery] string? email, [FromQuery] string? giftName)
        {
            _logger.LogInformation("Starting to get filtered donors. Name: {Name}, Email: {Email}, Gift: {Gift}", name, email, giftName);
            try
            {
                var donors = await _donorService.GetFilteredDonorsAsync(name, email, giftName);
                _logger.LogInformation("Successfully retrieved filtered donors.");
                return Ok(donors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering donors.");
                return BadRequest("Internal server error occurred");
            }
        }
    }
}