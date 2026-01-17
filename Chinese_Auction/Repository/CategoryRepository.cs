using Chinese_Auction.Data;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinese_Auction.Repository
{


    public class CategoryRepository : ICategoryRepository
    {
        private readonly ChineseAuctionDbContext _context;

        public CategoryRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();

        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        //manager only
        public async Task CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        //manager only
        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null)
            {
                return null;
            }
            existing.Name = category.Name;
            _context.Categories.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        //manager only
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
