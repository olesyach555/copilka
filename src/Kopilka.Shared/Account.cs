namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет финансовый счет пользователя.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Уникальный идентификатор счета.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя-владельца счета.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Тип счета (например, "Карта", "Наличные").
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Текущий баланс на счете.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Название счета.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Валюта счета (например, "RUB", "USD").
        /// </summary>
        public string Currency { get; set; } = string.Empty;
    }
}
