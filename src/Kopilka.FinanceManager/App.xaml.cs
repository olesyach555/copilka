using System.Windows;
using Kopilka.DataAccess;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private ApplicationDbContext? _context;
        private AuthService? _authService;
        private TransactionService? _transactionService;
        private CategoryService? _categoryService;
        private DebtService? _debtService;
        private GoalService? _goalService;
        private ReminderService? _reminderService;
        private UserService? _userService;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Предотвращаем закрытие приложения при закрытии промежуточных окон
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _context = new ApplicationDbContext();
            _context.Database.Migrate();
            SeedData(_context);

            _authService = new AuthService(_context);
            _transactionService = new TransactionService(_context);
            _categoryService = new CategoryService(_context);
            _debtService = new DebtService(_context);
            _goalService = new GoalService(_context);
            _reminderService = new ReminderService(_context);
            _userService = new UserService(_context);

            ShowLoginWindow();
        }

        public void ShowLoginWindow()
        {
            if (_authService == null) return;

            var authViewModel = new AuthViewModel(_authService);
            var loginWindow = new LoginWindow(authViewModel);

            if (loginWindow.ShowDialog() == true && loginWindow.Tag is User authenticatedUser)
            {
                var mainViewModel = new MainViewModel(
                    _transactionService!,
                    _categoryService!,
                    _debtService!,
                    _goalService!,
                    _reminderService!,
                    _authService!,
                    _userService!,
                    authenticatedUser.Id);

                var mainWindow = new MainWindow(mainViewModel);
                this.MainWindow = mainWindow;
                mainWindow.Show();

                // Теперь можно вернуть режим закрытия по умолчанию
                this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            else
            {
                Shutdown();
            }
        }

        private void SeedData(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            var user = new User
            {
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Parent"
            };
            context.Users.Add(user);
            context.SaveChanges();

            var category = new Category { Name = "Продукты", IsIncome = false, UserId = user.Id };
            context.Categories.Add(category);
            context.SaveChanges();

            context.Transactions.Add(new Transaction
            {
                Amount = 1500,
                Date = DateTime.Now.AddDays(-1),
                Comment = "Покупка в магазине",
                CategoryId = category.Id,
                UserId = user.Id
            });
            context.SaveChanges();
        }
    }
}
