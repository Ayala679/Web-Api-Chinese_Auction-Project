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


        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllDonors()
        {
            _logger.LogInformation("Getting all donors.");
            var donors = await _donorService.GetAllDonorsAsync();
            _logger.LogInformation("Fetched all donors successfully.");
            return Ok(donors);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonorById(int id)
        {
            _logger.LogInformation("Getting donor by ID:" + id);
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (donor == null) return NotFound("donor with the given ID was not found");
            _logger.LogInformation("Fetched donor by ID:" + id + " successfully.");
            return Ok(donor);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateDonor([FromBody] CreateDonorDto donor)
        {
            _logger.LogInformation("Creating a new donor.");
            try
            {
                _logger.LogInformation("Created new donor successfully.");
                var newDonor = await _donorService.CreateDonorAsync(donor);
                return CreatedAtAction(nameof(GetDonorById), new { id = newDonor.Id }, newDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new donor.");
                return BadRequest("Internal server error ocuured");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(int id, [FromBody] CreateDonorDto donor)
        {
            _logger.LogInformation("Updating donor with ID:" + id);
            try
            {
                var updatedDonor = await _donorService.UpdateDonorAsync(id, donor);
                if (updatedDonor == null) return NotFound("donor with the given ID was not found");
                _logger.LogInformation("Updated donor with ID:" + id + " successfully.");
                return Ok(updatedDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating donor with ID:" + id);
                return BadRequest("Internal server error ocuured");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            _logger.LogInformation("Deleting donor with ID:" + id);
            try {
                var isDeleted = await _donorService.DeleteDonorAsync(id);
                if (!isDeleted) return NotFound("donor with the given ID was not found");
                _logger.LogInformation("Deleted donor with ID:" + id + " successfully.");
                return Ok("deleted succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting donor with ID:" + id);
                return BadRequest("Internal server error ocuured");
            }
        }

        [Authorize(Roles = "Manager")]
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