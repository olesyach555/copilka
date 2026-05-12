using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetFamilyMembersAsync(int familyId)
        {
            return await _context.Users
                .Where(u => u.FamilyId == familyId)
                .ToListAsync();
        }

        public async Task<Family?> GetFamilyByIdAsync(int familyId)
        {
            return await _context.Families.FindAsync(familyId);
        }

        public async Task UpdateFamilyAsync(Family family)
        {
            _context.Entry(family).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFamilyAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.FamilyId = null;
                await _context.SaveChangesAsync();
            }
        }
    }
}
