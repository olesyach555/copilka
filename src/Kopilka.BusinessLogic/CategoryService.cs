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
                .Where(c => c.UserId == userId || c.UserId == 0) // 0 для системных категорий
                .ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task SeedDefaultCategoriesAsync(int userId)
        {
            var defaults = new List<Category>
            {
                new Category { Name = "Продукты", IsIncome = false, UserId = userId },
                new Category { Name = "Зарплата", IsIncome = true, UserId = userId },
                new Category { Name = "Карманные расходы", IsIncome = false, UserId = userId },
                new Category { Name = "ЖКУ", IsIncome = false, UserId = userId },
                new Category { Name = "Транспорт", IsIncome = false, UserId = userId }
            };

            foreach (var cat in defaults)
            {
                if (!await _context.Categories.AnyAsync(c => c.UserId == userId && c.Name == cat.Name))
                {
                    _context.Categories.Add(cat);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
