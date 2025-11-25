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
        /// Идентификатор счета, к которому относится транзакция.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Идентификатор категории транзакции.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Тип транзакции (например, "Доход" или "Расход").
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Сумма транзакции.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата и время проведения транзакции.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Комментарий к транзакции.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Навигационное свойство для связи с категорией.
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство для связи со счетом.
        /// </summary>
        public Account Account { get; set; } = null!;
    }
}
