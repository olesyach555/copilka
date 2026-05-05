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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var context = new ApplicationDbContext();

            // Применяем миграции и сидируем данные
            context.Database.Migrate();
            SeedData(context);

            var authService = new AuthService(context);
            var transactionService = new TransactionService(context);
            var debtService = new DebtService(context);
            var goalService = new GoalService(context);
            var reminderService = new ReminderService(context);

            var authViewModel = new AuthViewModel(authService);
            var loginWindow = new LoginWindow(authViewModel);

            if (loginWindow.ShowDialog() == true && loginWindow.Tag is User authenticatedUser)
            {
                var mainViewModel = new MainViewModel(transactionService, debtService, goalService, reminderService, authenticatedUser.Id);
                var mainWindow = new MainWindow(mainViewModel, authService, transactionService, debtService, goalService, reminderService);
                mainWindow.Show();
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
