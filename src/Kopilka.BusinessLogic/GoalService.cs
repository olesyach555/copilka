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

            return await _context.FinancialGoals
                .Where(g => g.OwnerUserId == userId || (g.IsFamilyGoal && g.OwnerUser.FamilyId == user.FamilyId))
                .ToListAsync();
        }

        public async Task AddGoalAsync(FinancialGoal goal)
        {
            _context.FinancialGoals.Add(goal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGoalAsync(FinancialGoal goal)
        {
            _context.Entry(goal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGoalAsync(int id)
        {
            var goal = await _context.FinancialGoals.FindAsync(id);
            if (goal != null)
            {
                _context.FinancialGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFundsAsync(int goalId, decimal amount, int userId)
        {
            var goal = await _context.FinancialGoals.FindAsync(goalId);
            if (goal != null)
            {
                goal.CurrentAmount += amount;
                await _context.SaveChangesAsync();

                // Создаем транзакцию расхода (перевод в копилку)
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Копилка" && c.UserId == userId);
                if (category == null)
                {
                    category = new Category { Name = "Копилка", IsIncome = false, UserId = userId };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                }

                var transaction = new Transaction
                {
                    UserId = userId,
                    Amount = amount,
                    Date = DateTime.Now,
                    Comment = $"Пополнение цели: {goal.Name}",
                    CategoryId = category.Id
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
