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
        /// Роль пользователя (Parent / Child).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор семьи.
        /// </summary>
        public int? FamilyId { get; set; }

        /// <summary>
        /// Семья, к которой принадлежит пользователь.
        /// </summary>
        public Family? Family { get; set; }

        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        public string? Email { get; set; }
    }
}
