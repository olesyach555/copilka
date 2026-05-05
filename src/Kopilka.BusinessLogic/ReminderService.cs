using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class ReminderService
    {
        private readonly ApplicationDbContext _context;

        public ReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reminder>> GetActiveRemindersAsync(int userId)
        {
            return await _context.Reminders
                .Include(r => r.TransactionCategory)
                .Where(r => r.UserId == userId && r.ReminderDate >= DateTime.Now)
                .OrderBy(r => r.ReminderDate)
                .ToListAsync();
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder != null)
            {
                _context.Reminders.Remove(reminder);
                await _context.SaveChangesAsync();
            }
        }
    }
}
