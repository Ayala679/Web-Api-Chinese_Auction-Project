using Chinese_Auction.Dto_s;
using Chinese_Auction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chinese_Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IPackageService packageService, ILogger<PackageController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            _logger.LogInformation("Getting all packages.");
            var packages = await _packageService.GetAllPackagesAsync();
            _logger.LogInformation("Fetched all packages successfully.");
            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            _logger.LogInformation("Getting package by ID:" + id);
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null) return NotFound("Package with the given ID was not found");
            _logger.LogInformation("Fetched package successfully.");
            return Ok(package);
          
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto createPackageDto)
        {
            _logger.LogInformation("Creating a new package.");
            try
            {
                var newPackage = await _packageService.CreatePackageAsync(createPackageDto);
                _logger.LogInformation("Package created successfully with ID:" + newPackage.Id);
                return CreatedAtAction(nameof(GetPackageById), new { Id = newPackage.Id }, newPackage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while creating package: ");
                return BadRequest("Internal server error occurred");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] CreatePackageDto updatePackageDto)
        {
            _logger.LogInformation("Updating package with ID:" + id);
            try
            {
                var updatedPackage = await _packageService.UpdatePackageAsync(id, updatePackageDto);
                if (updatedPackage == null) return NotFound("Package with the given ID was not found");
                _logger.LogInformation("Package updated successfully.");
                return Ok(updatePackageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating package: ");
                return BadRequest("Internal server error occurred");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            _logger.LogInformation("Deleting package with ID:" + id);
            try
            {
                var isDeleted = await _packageService.DeletePackageAsync(id);
                if (!isDeleted) return NotFound("Package with the given ID was not found");
                _logger.LogInformation("Package deleted successfully.");
                return Ok("deleted succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting package: ");
                return BadRequest("Internal server error occurred");
            }
        }
        
    }
}
