using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления транзакциями.
    /// </summary>
    public class TransactionService
    {
        private readonly KopilkaDbContext _context;

        public TransactionService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно получает список транзакций для пользователя за указанный период.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="startDate">Начальная дата периода.</param>
        /// <param name="endDate">Конечная дата периода.</param>
        /// <returns>Список транзакций.</returns>
        public async Task<List<Transaction>> GetTransactionsForUserAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            return await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId))
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .Include(t => t.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Асинхронно вычисляет общую сумму доходов пользователя за период.
        /// </summary>
        public async Task<decimal> GetTotalIncomeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            return (decimal)await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId) &&
                            t.Date >= startDate && t.Date <= endDate &&
                            t.Type == "Income")
                .SumAsync(t => (double)t.Amount);
        }

        /// <summary>
        /// Асинхронно вычисляет общую сумму расходов пользователя за период.
        /// </summary>
        public async Task<decimal> GetTotalExpensesAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            return (decimal)await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId) &&
                            t.Date >= startDate && t.Date <= endDate &&
                            t.Type == "Expense")
                .SumAsync(t => (double)t.Amount);
        }

        /// <summary>
        /// Асинхронно добавляет новую транзакцию и связанную с ней запись о дате.
        /// </summary>
        /// <param name="transaction">Транзакция для добавления.</param>
        public async Task AddTransactionAsync(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account == null)
            {
                throw new ArgumentException("Указанный счет не существует.", nameof(transaction.AccountId));
            }

            transaction.UserId = account.UserId;
            _context.Transactions.Add(transaction);

            var date = new Date
            {
                DateTime = transaction.Date,
                OperationType = $"Транзакция: {transaction.Amount} {transaction.Category.Type}"
            };
            _context.Dates.Add(date);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно вычисляет общий баланс для пользователя на основе всех его транзакций.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Общий баланс.</returns>
        public async Task<decimal> GetTotalBalanceAsync(int userId)
        {
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            if (!userAccountIds.Any())
            {
                return 0;
            }

            var totalIncome = await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId) && t.Type == "Income")
                .SumAsync(t => (double)t.Amount);

            var totalExpenses = await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId) && t.Type == "Expense")
                .SumAsync(t => (double)t.Amount);

            return (decimal)(totalIncome - totalExpenses);
        }

        /// <summary>
        /// Асинхронно получает список последних транзакций пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="count">Количество транзакций для получения.</param>
        /// <returns>Список последних транзакций.</returns>
        public async Task<List<Transaction>> GetRecentTransactionsAsync(int userId, int count)
        {
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            return await _context.Transactions
                .Where(t => userAccountIds.Contains(t.AccountId))
                .OrderByDescending(t => t.Date)
                .Take(count)
                .Include(t => t.Category)
                .Include(t => t.Account)
                .ToListAsync();
        }
    }
}
