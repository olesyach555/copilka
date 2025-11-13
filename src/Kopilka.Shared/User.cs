namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет пользователя системы.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя для входа в систему.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Хеш пароля пользователя.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя в системе (например, "Admin" или "User").
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
