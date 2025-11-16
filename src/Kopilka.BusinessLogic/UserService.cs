using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    public class UserService
    {
        private readonly KopilkaDbContext _context;

        public UserService(KopilkaDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> RegisterUserAsync(string login, string password, string passwordConfirm)
        {
            // 1. Проверка на совпадение паролей
            if (password != passwordConfirm)
            {
                throw new ArgumentException("Пароли не совпадают.");
            }

            // 2. Проверка на совпадение логина и пароля
            if (login == password)
            {
                throw new ArgumentException("Логин и пароль не должны совпадать.");
            }

            // 3. Проверка формата логина (латиница, цифры, _)
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException("Логин может содержать только латинские буквы, цифры и символ подчёркивания.");
            }

            // 4. Проверка сложности пароля (минимум 8 символов, 1 заглавная, 1 цифра)
            if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsDigit))
            {
                throw new ArgumentException("Пароль должен быть не менее 8 символов и содержать хотя бы одну заглавную букву и одну цифру.");
            }

            // 5. Проверка на существование пользователя (уже была, но оставляем на всякий случай)
            var existingUser = await GetUserByLoginAsync(login);
            if (existingUser != null)
            {
                throw new ArgumentException("Пользователь с таким логином уже существует.");
            }

            var user = new User
            {
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

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
