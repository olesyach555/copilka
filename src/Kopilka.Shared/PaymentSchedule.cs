using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет платеж в графике погашения долга.
    /// </summary>
    public class PaymentSchedule
    {
        public int Id { get; set; }

        public int DebtContractId { get; set; }

        public DebtContract DebtContract { get; set; } = null!;

        /// <summary>
        /// Плановая дата платежа.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Сумма к оплате.
        /// </summary>
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Флаг оплаты.
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Фактическая дата оплаты.
        /// </summary>
        public DateTime? PaidDate { get; set; }
    }
}
