using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId || c.UserId == 0) // 0 для системных категорий, если есть
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

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
