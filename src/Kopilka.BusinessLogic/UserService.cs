using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления данными пользователей.
    /// </summary>
    public class UserService
    {
        private readonly KopilkaDbContext _context;

        public UserService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно получает пользователя по его ID.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        /// <summary>
        /// Асинхронно обновляет данные пользователя.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно меняет пароль пользователя.
        /// </summary>
        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await UpdateUserAsync(user);
            }
        }
    }
}
