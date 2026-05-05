using System.Collections.Generic;

namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет семью пользователей.
    /// </summary>
    public class Family
    {
        /// <summary>
        /// Уникальный идентификатор семьи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название семьи.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Единый пароль для доступа к семейным данным.
        /// </summary>
        public string HomePassword { get; set; } = string.Empty;

        /// <summary>
        /// Список пользователей, входящих в семью.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();
    }
}
