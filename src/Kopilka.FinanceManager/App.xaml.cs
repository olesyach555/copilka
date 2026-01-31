using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private KopilkaDbContext? _dbContext;
        private AuthService? _authService;
        private TransactionService? _transactionService;
        private AccountService? _accountService;
        private CategoryService? _categoryService;
        private UserService? _userService;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _dbContext = new KopilkaDbContext();
            // Применяем все ожидающие миграции при запуске
            await _dbContext.Database.MigrateAsync();

            _transactionService = new TransactionService(_dbContext);
            _accountService = new AccountService(_dbContext);
            _categoryService = new CategoryService(_dbContext);
            _userService = new UserService(_dbContext);
            _authService = new AuthService(_dbContext);

            var lastUserId = SettingsService.GetLastUserId();
            User? user = null;
            if (lastUserId > 0)
            {
                user = await _dbContext!.Users.FindAsync(lastUserId);
            }

            var mainWindow = new MainWindow(user, _dbContext, _transactionService, _accountService, _categoryService, _userService, _authService);
            mainWindow.Show();
        }
    }
}
