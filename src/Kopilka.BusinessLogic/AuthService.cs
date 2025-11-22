using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для аутентификации пользователей.
    /// </summary>
    public class AuthService
    {
        private readonly KopilkaDbContext _context;

        public AuthService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно регистрирует нового пользователя с валидацией.
        /// </summary>
        public async Task<User> RegisterUserAsync(string login, string password)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            {
                throw new ArgumentException("Логин должен быть не менее 3 символов.");
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Пароль должен быть не менее 6 символов.");
            }
            if (!password.Any(char.IsDigit))
            {
                throw new ArgumentException("Пароль должен содержать хотя бы одну цифру.");
            }
            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Login = login, PasswordHash = hashedPassword };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Асинхронно выполняет вход пользователя.
        /// </summary>
        public async Task<User?> LoginAsync(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }
}
