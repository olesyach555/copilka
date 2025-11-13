using Kopilka.DataAccess;
using Kopilka.Shared;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic
{
    /// <summary>
    /// Сервис для управления транзакциями.
    /// </summary>
    public class TransactionService
    {
        private readonly KopilkaDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TransactionService"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public TransactionService(KopilkaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Асинхронно добавляет новую транзакцию.
        /// </summary>
        /// <param name="transaction">Транзакция для добавления.</param>
        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
