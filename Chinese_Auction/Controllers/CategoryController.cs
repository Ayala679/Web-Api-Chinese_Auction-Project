using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chinese_Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("Getting all categories.");
            var categories = await _categoryService.GetAllCategoriesAsync();
            _logger.LogInformation("Fetched all categories successfully.");
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            _logger.LogInformation("Getting category by ID:"+id);
            var category = await _categoryService.GetCategoryByIdAsync(id);
            _logger.LogInformation("Fetched category by ID:"+id+" successfully.");
            if (category == null) 
                return NotFound("category with the given ID was not found");
            return Ok(category);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto createCategoryDto)
        {
            _logger.LogInformation("Creating a new category.");
            try
            {
                GetCategoryDto newCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);
                _logger.LogInformation("Created new category successfully.");
                return CreatedAtAction(nameof(GetCategoryById), new { Id = newCategory.Id }, newCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"An error occurred while creating a new category.");
                return BadRequest("Internal server error ocuured");
            }

        }


        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto updateCategoryDto)
        {
            _logger.LogInformation("Updating category with ID:"+id);
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                _logger.LogInformation("Updated category successfully.");
                if (updatedCategory == null) return NotFound();
                return Ok(updatedCategory);
            }
            catch (Exception ex) 
            {  
                _logger.LogError(ex,"An error occurred while updating the category.");
                return BadRequest("Internal server error ocuured");
            }

            
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("Deleting category with ID:" + id);
            try
            {
                var isDeleted = await _categoryService.DeleteCategoryAsync(id);
                _logger.LogInformation("Deleted category successfully.");
                if (!isDeleted) return NotFound("category with the given ID was not found");
                return Ok("deleted succesfully");

            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while deleting the category.");
                return BadRequest("Internal server error ocuured");
            }

        }



    }
}
