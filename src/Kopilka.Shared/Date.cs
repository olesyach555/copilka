using System;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет запись о дате и операции в базе данных.
    /// </summary>
    public class Date
    {
        /// <summary>
        /// Уникальный идентификатор записи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полная дата и время операции.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Описание введенной операции.
        /// </summary>
        /// <remarks>
        /// Например: "расход", "доход", "перевод со счета X на счет Y".
        /// </remarks>
        public string OperationType { get; set; } = string.Empty;
    }
}
