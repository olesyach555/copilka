using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class GoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FinancialGoal>> GetUserGoalsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<FinancialGoal>();

            var query = _context.FinancialGoals.Where(g => g.OwnerUserId == userId);

            if (user.FamilyId.HasValue)
            {
                query = _context.FinancialGoals.Where(g => g.OwnerUserId == userId || (g.IsFamilyGoal && g.OwnerUser.FamilyId == user.FamilyId));
            }

            return await query.ToListAsync();
        }

        public async Task AddGoalAsync(FinancialGoal goal)
        {
            _context.FinancialGoals.Add(goal);
            await _context.SaveChangesAsync();
        }

        public async Task AddFundsAsync(int goalId, decimal amount)
        {
            var goal = await _context.FinancialGoals.FindAsync(goalId);
            if (goal == null) return;

            goal.CurrentAmount += amount;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Сценарий "что если" - расчет ежемесячного взноса для достижения цели к дате.
        /// </summary>
        public decimal CalculateWhatIfSavings(decimal targetAmount, decimal currentAmount, DateTime targetDate)
        {
            var remaining = targetAmount - currentAmount;
            if (remaining <= 0) return 0;

            var months = ((targetDate.Year - DateTime.Now.Year) * 12) + targetDate.Month - DateTime.Now.Month;
            if (months <= 0) return remaining;

            return remaining / months;
        }

        /// <summary>
        /// Сценарий "что если" - расчет даты достижения цели при фиксированном ежемесячном взносе.
        /// </summary>
        public DateTime CalculateWhatIfDate(decimal targetAmount, decimal currentAmount, decimal monthlyContribution)
        {
            if (monthlyContribution <= 0) return DateTime.MaxValue;
            var remaining = targetAmount - currentAmount;
            if (remaining <= 0) return DateTime.Now;

            int months = (int)Math.Ceiling(remaining / monthlyContribution);
            return DateTime.Now.AddMonths(months);
        }
    }
}
