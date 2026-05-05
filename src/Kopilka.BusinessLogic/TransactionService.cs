using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int userId, DateTime? start = null, DateTime? end = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (start.HasValue)
                query = query.Where(t => t.Date >= start.Value);
            if (end.HasValue)
                query = query.Where(t => t.Date <= end.Value);

            return await query.OrderByDescending(t => t.Date).ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalBalanceAsync(int userId)
        {
            var income = await _context.Transactions
                .Where(t => t.UserId == userId && t.Category.IsIncome)
                .SumAsync(t => (double)t.Amount);

            var expense = await _context.Transactions
                .Where(t => t.UserId == userId && !t.Category.IsIncome)
                .SumAsync(t => (double)t.Amount);

            return (decimal)(income - expense);
        }

        public async Task<List<ChartPoint>> GetChartDataAsync(int userId, DateTime start, DateTime end)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= start && t.Date <= end)
                .ToListAsync();

            return transactions
                .GroupBy(t => t.Date.Date)
                .Select(g => new ChartPoint
                {
                    Date = g.Key,
                    Amount = (decimal)g.Sum(t => t.Category.IsIncome ? (double)t.Amount : -(double)t.Amount)
                })
                .OrderBy(p => p.Date)
                .ToList();
        }
    }

    public class ChartPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
