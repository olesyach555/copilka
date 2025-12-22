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
        /// Асинхронно добавляет новую транзакцию, обновляет баланс и создает запись о дате.
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

            if (transaction.Type == "Expense")
            {
                account.Balance -= transaction.Amount;
            }
            else if (transaction.Type == "Income")
            {
                account.Balance += transaction.Amount;
            }
            _context.Accounts.Update(account);

            await _context.Transactions.AddAsync(transaction);

            var date = new Date
            {
                DateTime = transaction.Date,
                OperationType = $"Добавлена транзакция: {transaction.Amount} {transaction.Type}"
            };
            _context.Dates.Add(date);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно обновляет существующую транзакцию, корректирует балансы и создает запись о дате.
        /// </summary>
        /// <param name="updatedTransaction">Обновленная транзакция.</param>
        public async Task UpdateTransactionAsync(Transaction updatedTransaction)
        {
            var originalTransaction = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == updatedTransaction.Id);

            if (originalTransaction == null)
            {
                throw new ArgumentException("Транзакция для обновления не найдена.");
            }

            var originalAccount = await _context.Accounts.FindAsync(originalTransaction.AccountId);
            if (originalAccount != null)
            {
                if (originalTransaction.Type == "Expense")
                {
                    originalAccount.Balance += originalTransaction.Amount;
                }
                else if (originalTransaction.Type == "Income")
                {
                    originalAccount.Balance -= originalTransaction.Amount;
                }
                _context.Accounts.Update(originalAccount);
            }

            var newAccount = await _context.Accounts.FindAsync(updatedTransaction.AccountId);
            if (newAccount != null)
            {
                updatedTransaction.UserId = newAccount.UserId;
                if (updatedTransaction.Type == "Expense")
                {
                    newAccount.Balance -= updatedTransaction.Amount;
                }
                else if (updatedTransaction.Type == "Income")
                {
                    newAccount.Balance += updatedTransaction.Amount;
                }
                _context.Accounts.Update(newAccount);
            }

            _context.Transactions.Update(updatedTransaction);

            var date = new Date
            {
                DateTime = updatedTransaction.Date,
                OperationType = $"Изменена транзакция: {updatedTransaction.Amount} {updatedTransaction.Type}"
            };
            _context.Dates.Add(date);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно удаляет транзакцию, корректирует баланс и создает запись о дате.
        /// </summary>
        /// <param name="transactionId">ID транзакции для удаления.</param>
        public async Task DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                throw new ArgumentException("Транзакция для удаления не найдена.");
            }

            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account != null)
            {
                if (transaction.Type == "Expense")
                {
                    account.Balance += transaction.Amount;
                }
                else if (transaction.Type == "Income")
                {
                    account.Balance -= transaction.Amount;
                }
                _context.Accounts.Update(account);
            }

            _context.Transactions.Remove(transaction);

            var date = new Date
            {
                DateTime = transaction.Date,
                OperationType = $"Удалена транзакция: {transaction.Amount} {transaction.Type}"
            };
            _context.Dates.Add(date);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно вычисляет общий баланс для пользователя на основе его счетов.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Общий баланс.</returns>
        public async Task<decimal> GetTotalBalanceAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.Balance);
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
