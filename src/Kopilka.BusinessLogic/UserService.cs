using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Асинхронно находит пользователя по его логину.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        /// <summary>
        /// Асинхронно регистрирует нового пользователя.
        /// </summary>
        /// <param name="login">Логин нового пользователя.</param>
        /// <param name="password">Пароль нового пользователя.</param>
        /// <returns>Созданный пользователь.</returns>
        public async Task<User> RegisterUserAsync(string login, string password)
        {
            if (login == password)
            {
                throw new System.ArgumentException("Логин и пароль не должны совпадать.");
            }

            var user = new User
            {
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User" // По умолчанию все новые пользователи - обычные пользователи
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Асинхронно выполняет вход пользователя в систему.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Объект пользователя в случае успеха, иначе — null.</returns>
        public async Task<User?> LoginAsync(string login, string password)
        {
            var user = await GetUserByLoginAsync(login);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
    }
}
