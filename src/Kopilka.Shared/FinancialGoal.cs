using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет финансовую цель.
    /// </summary>
    public class FinancialGoal
    {
        public int Id { get; set; }

        /// <summary>
        /// Название цели.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Целевая сумма.
        /// </summary>
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Текущая накопленная сумма.
        /// </summary>
        public decimal CurrentAmount { get; set; }

        /// <summary>
        /// Целевая дата достижения.
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Идентификатор владельца цели.
        /// </summary>
        public int OwnerUserId { get; set; }

        public User OwnerUser { get; set; } = null!;

        /// <summary>
        /// Флаг общей семейной цели.
        /// </summary>
        public bool IsFamilyGoal { get; set; }
    }
}
