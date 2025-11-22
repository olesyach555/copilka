using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления категориями транзакций.
    /// </summary>
    public class CategoryService
    {
        private readonly KopilkaDbContext _context;

        public CategoryService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно получает список всех категорий для указанного пользователя.
        /// </summary>
        public async Task<List<Category>> GetCategoriesAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Асинхронно добавляет новую категорию.
        /// </summary>
        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно обновляет существующую категорию.
        /// </summary>
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно удаляет категорию.
        /// </summary>
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
