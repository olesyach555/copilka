namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет категорию для доходов или расходов.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Уникальный идентификатор категории.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Тип категории ("Expense" или "Income").
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Описание категории.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор пользователя, которому принадлежит категория.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Пользователь, которому принадлежит категория.
        /// </summary>
        public User User { get; set; } = null!;
    }
}
