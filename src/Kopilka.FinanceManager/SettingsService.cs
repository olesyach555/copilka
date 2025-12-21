using System;
using System.IO;

namespace Kopilka.FinanceManager
{
    public static class SettingsService
    {
        private static readonly string SettingsFilePath = Path.Combine(AppContext.BaseDirectory, "last_user.txt");

        public static int GetLastUserId()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return 0;
            }

            var content = File.ReadAllText(SettingsFilePath);
            if (int.TryParse(content, out var userId))
            {
                return userId;
            }

            return 0;
        }

        public static void SetLastUserId(int userId)
        {
            File.WriteAllText(SettingsFilePath, userId.ToString());
        }
    }
}
