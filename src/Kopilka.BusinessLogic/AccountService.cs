using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления счетами.
    /// </summary>
    public class AccountService
    {
        private readonly KopilkaDbContext _context;

        public AccountService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно вычисляет общий баланс по всем счетам пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Общий баланс.</returns>
        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.Balance);
        }

        /// <summary>
        /// Асинхронно получает список счетов пользователя.
        /// </summary>
        public async Task<System.Collections.Generic.List<Account>> GetUserAccountsAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}
