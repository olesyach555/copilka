using Kopilka.DataAccess;
using Kopilka.Shared;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления пользователями.
    /// </summary>
    public class UserService
    {
        private readonly KopilkaDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserService"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public UserService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно находит пользователя по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
