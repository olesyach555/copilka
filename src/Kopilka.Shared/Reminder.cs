using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет напоминание.
    /// </summary>
    public class Reminder
    {
        public int Id { get; set; }

        /// <summary>
        /// Заголовок напоминания.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время напоминания.
        /// </summary>
        public DateTime ReminderDate { get; set; }

        /// <summary>
        /// Флаг повторяющегося напоминания.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Идентификатор категории транзакции для автоподстановки.
        /// </summary>
        public int? TransactionCategoryId { get; set; }

        public Category? TransactionCategory { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
