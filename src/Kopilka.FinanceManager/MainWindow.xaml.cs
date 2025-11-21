using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.FinanceManager.Views;
using Kopilka.Shared;
using System.Linq;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly TransactionService _transactionService;
        private readonly KopilkaDbContext _dbContext;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            _dbContext = new KopilkaDbContext();
            _transactionService = new TransactionService(_dbContext);

            LoadUserData();
            NavigateToHome(); // Начальная навигация
        }

        private async void LoadUserData()
        {
            UsernameTextBlock.Text = _currentUser.Login;
            UserInitialTextBlock.Text = _currentUser.Login.FirstOrDefault().ToString().ToUpper();

            var balance = await _transactionService.GetTotalBalanceAsync(_currentUser.Id);
            BalanceTextBlock.Text = $"Остаток: {balance:C}";
        }

        private async void NavigateToHome()
        {
            var transactions = await _transactionService.GetRecentTransactionsAsync(_currentUser.Id, 15);
            var income = transactions.Where(t => t.Type == "Income").ToList();
            var expenses = transactions.Where(t => t.Type == "Expense").ToList();
            MainFrame.Navigate(new HomePage(income, expenses));
        }

        // --- Обработчики кнопок навигации ---
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToHome();
        }

        private void AccountsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AccountsPage());
        }

        private void ChartsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChartsPage());
        }

        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CategoriesPage());
        }

        private void RegularPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegularPaymentsPage());
        }

        private void RemindersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RemindersPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }
    }
}
