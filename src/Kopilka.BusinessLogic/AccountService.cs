using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления счетами пользователей.
    /// </summary>
    public class AccountService
    {
        private readonly KopilkaDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса AccountService.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public AccountService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно получает список всех счетов для указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список счетов пользователя.</returns>
        public async Task<List<Account>> GetAccountsAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Асинхронно добавляет новый счет.
        /// </summary>
        /// <param name="account">Счет для добавления.</param>
        public async Task AddAccountAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно обновляет существующий счет.
        /// </summary>
        /// <param name="account">Счет с обновленными данными.</param>
        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Асинхронно удаляет счет.
        /// </summary>
        /// <param name="accountId">Идентификатор счета для удаления.</param>
        public async Task DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
