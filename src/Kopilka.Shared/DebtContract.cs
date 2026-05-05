using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет договор займа (долг или кредит).
    /// </summary>
    public class DebtContract
    {
        public int Id { get; set; }

        /// <summary>
        /// Название договора.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Основная сумма долга.
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// Процент годовых.
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Пени за просрочку.
        /// </summary>
        public decimal PenaltyRate { get; set; }

        /// <summary>
        /// Дата начала договора.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания договора.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Идентификатор кредитора или контрагента.
        /// </summary>
        public string Counterparty { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор пользователя-владельца долга.
        /// </summary>
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
