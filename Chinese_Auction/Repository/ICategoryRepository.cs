using Chinese_Auction.Models;

namespace Chinese_Auction.Repository
{
    public interface ICategoryRepository
    {
        Task CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> UpdateCategoryAsync(Category category);
    }
}