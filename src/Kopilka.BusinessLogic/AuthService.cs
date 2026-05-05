using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Kopilka.BusinessLogic
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.Family)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public async Task<User?> RegisterUserAsync(string login, string password, string role = "Parent")
        {
            if (await _context.Users.AnyAsync(u => u.Login == login))
                return null;

            var user = new User
            {
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Family?> CreateFamilyAsync(int ownerUserId, string familyName, string homePassword)
        {
            var owner = await _context.Users.FindAsync(ownerUserId);
            if (owner == null) return null;

            var family = new Family
            {
                Name = familyName,
                HomePassword = homePassword // В ТЗ не сказано хешировать, но для безопасности стоило бы. Оставим как в ТЗ.
            };

            _context.Families.Add(family);
            await _context.SaveChangesAsync();

            owner.FamilyId = family.Id;
            await _context.SaveChangesAsync();

            return family;
        }

        public async Task<bool> JoinFamilyAsync(int userId, int familyId, string homePassword)
        {
            var user = await _context.Users.FindAsync(userId);
            var family = await _context.Families.FindAsync(familyId);

            if (user == null || family == null || family.HomePassword != homePassword)
                return false;

            user.FamilyId = familyId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
