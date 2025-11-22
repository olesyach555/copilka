using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.FinanceManager.Views;
using Kopilka.FinanceManager.Views.Pages; // Добавляем using для страниц
using Kopilka.Shared;
using System.Linq;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly KopilkaDbContext _dbContext;
        private readonly TransactionService _transactionService;
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly AuthService _authService;


        public MainWindow(User user, KopilkaDbContext dbContext)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = dbContext;

            // Инициализируем все сервисы с единым DbContext
            _transactionService = new TransactionService(_dbContext);
            _accountService = new AccountService(_dbContext);
            _categoryService = new CategoryService(_dbContext);
            _userService = new UserService(_dbContext);
            _authService = new AuthService(_dbContext);


            LoadUserData();
            // Изменяем вызов на загрузку страницы счетов по умолчанию
            NavigateToAccounts();
        }

        private async void LoadUserData()
        {
            UsernameTextBlock.Text = _currentUser.Login;
            if (!string.IsNullOrEmpty(_currentUser.Login))
            {
                UserInitialTextBlock.Text = _currentUser.Login[0].ToString().ToUpper();
            }

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

        private void NavigateToAccounts()
        {
            // Указываем полный путь, чтобы избежать неоднозначности
            MainFrame.Navigate(new Kopilka.FinanceManager.Views.Pages.AccountsPage(_dbContext, _currentUser));
        }

        // --- Обработчики кнопок навигации ---
        private void HomeButton_Click(object sender, RoutedEventArgs e) => NavigateToHome();
        private void AccountsButton_Click(object sender, RoutedEventArgs e) => NavigateToAccounts(); // Исправляем вызов
        private void ChartsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ChartsPage());
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CategoriesPage(_currentUser, _categoryService));
        private void RegularPaymentsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegularPaymentsPage());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RemindersPage());
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new SettingsPage(_currentUser, _userService, _authService));
    }
}
