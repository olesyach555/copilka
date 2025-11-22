using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private KopilkaDbContext _dbContext;
        private AuthService _authService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _dbContext = new KopilkaDbContext();
            // Применяем все ожидающие миграции при запуске
            _dbContext.Database.Migrate();

            _authService = new AuthService(_dbContext);

            var loginWindow = new LoginWindow(_authService);
            loginWindow.Show();
        }
    }
}
