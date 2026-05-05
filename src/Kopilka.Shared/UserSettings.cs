namespace Kopilka.Shared
{
    /// <summary>
    /// Представляет настройки пользователя.
    /// </summary>
    public class UserSettings
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        /// <summary>
        /// Язык интерфейса (по умолчанию "ru-RU").
        /// </summary>
        public string Language { get; set; } = "ru-RU";
    }
}
