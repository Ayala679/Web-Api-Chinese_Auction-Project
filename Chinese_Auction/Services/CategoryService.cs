using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Repository;
using Chinese_Auction.Models;

namespace Chinese_Auction.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GetCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<GetCategoryDto>>(categories);
        }

        public async Task<GetCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with id {CategoryId} not found.", id);
            }
            return _mapper.Map<GetCategoryDto>(category);
        }

        public async Task<GetCategoryDto> CreateCategoryAsync(CategoryDto createCategoryDto)
        {
            if (await CategoryNameExistsAsync(createCategoryDto.Name, -1))
            {
                _logger.LogWarning("Attempt to create duplicate category with name {CategoryName}.", createCategoryDto.Name);
                throw new Exception("Category with the same name already exists.");
            }
            var category = _mapper.Map<Category>(createCategoryDto);
            await _categoryRepository.CreateCategoryAsync(category);
            return _mapper.Map<GetCategoryDto>(category);
        }

        public async Task<GetCategoryDto?> UpdateCategoryAsync(int id, CategoryDto updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null) 
            {
                _logger.LogWarning("Attempt to update non-existing category with id {CategoryId}.", id);
                return null;
            }
            if (await CategoryNameExistsAsync(updateCategoryDto.Name, -1))
            {
                _logger.LogWarning("Attempt to update category to duplicate name {CategoryName}.", updateCategoryDto.Name);
                throw new Exception("Category with the same name already exists.");
            }
            _mapper.Map(updateCategoryDto, existingCategory); 
            existingCategory.Id = id;
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(existingCategory);
            if (updatedCategory == null)
            { 
                _logger.LogError("Failed to update category with id {CategoryId}.", id);
                return null; 
            }
             return _mapper.Map<GetCategoryDto>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.LogWarning("Attempt to delete non-existing category with id {CategoryId}.", id);
                return false;
            }
            await _categoryRepository.DeleteCategoryAsync(id);
            return true;
        }

        public async Task<bool> CategoryNameExistsAsync(string name,int id)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Any(c => c.Name.Equals(name) && c.Id.Equals(id));
        }

    }
}
