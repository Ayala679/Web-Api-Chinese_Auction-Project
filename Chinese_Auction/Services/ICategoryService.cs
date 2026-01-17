using Chinese_Auction.Dto_s;

namespace Chinese_Auction.Services
{
    public interface ICategoryService
    {
        Task<bool> CategoryNameExistsAsync(string name,int id);
        Task<GetCategoryDto> CreateCategoryAsync(CategoryDto createCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<GetCategoryDto>> GetAllCategoriesAsync();
        Task<GetCategoryDto?> GetCategoryByIdAsync(int id);
        Task<GetCategoryDto?> UpdateCategoryAsync(int id, CategoryDto updateCategoryDto);
    }
}