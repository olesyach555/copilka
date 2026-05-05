using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет историю погашения или реструктуризации долга.
    /// </summary>
    public class PaymentHistory
    {
        public int Id { get; set; }
        public int DebtContractId { get; set; }
        public DebtContract DebtContract { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
