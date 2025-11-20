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

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => userAccountIds.Contains(t.AccountId) &&
                            t.Date >= startDate && t.Date <= endDate &&
                            t.Category.Type == "Income")
                .ToListAsync();
            return transactions.Sum(t => t.Amount);
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

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => userAccountIds.Contains(t.AccountId) &&
                            t.Date >= startDate && t.Date <= endDate &&
                            t.Category.Type == "Expense")
                .ToListAsync();
            return transactions.Sum(t => t.Amount);
        }

        /// <summary>
        /// Асинхронно добавляет новую транзакцию и связанную с ней запись о дате.
        /// </summary>
        /// <param name="transaction">Транзакция для добавления.</param>
        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);

            var date = new Date
            {
                DateTime = transaction.Date,
                OperationType = $"Транзакция: {transaction.Amount} {transaction.Category.Type}"
            };
            _context.Dates.Add(date);

            await _context.SaveChangesAsync();
        }
    }
}
