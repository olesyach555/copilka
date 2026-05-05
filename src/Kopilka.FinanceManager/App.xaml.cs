using System.Windows;
using Kopilka.DataAccess;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var context = new ApplicationDbContext();
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
    }
}
