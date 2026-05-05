using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет финансовую транзакцию.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Уникальный идентификатор транзакции.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Сумма транзакции.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата проведения транзакции.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Комментарий к транзакции.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор категории транзакции.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, совершившего операцию.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с категорией.
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство для связи с пользователем.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Идентификатор счета (сохраняем для обратной совместимости, если потребуется).
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи со счетом.
        /// </summary>
        public Account? Account { get; set; }
    }
}
