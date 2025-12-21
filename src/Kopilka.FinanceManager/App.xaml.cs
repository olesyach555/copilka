using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private KopilkaDbContext _dbContext = null!;
        private AuthService _authService = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _dbContext = new KopilkaDbContext();
            // Применяем все ожидающие миграции при запуске
            _dbContext.Database.Migrate();

            _authService = new AuthService(_dbContext);

            var lastUserId = SettingsService.GetLastUserId();
            User? user = null;
            if (lastUserId > 0)
            {
                user = _dbContext.Users.Find(lastUserId);
            }

            var mainWindow = new MainWindow(user, _dbContext);
            mainWindow.Show();
        }
    }
}
